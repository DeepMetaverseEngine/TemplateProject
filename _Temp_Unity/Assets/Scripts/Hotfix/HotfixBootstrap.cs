using _Temp_.Battle.Data;
using _Temp_.Battle.Host;
using _Temp_.Battle.Slave;
using _Temp_.Client;
using _Temp_.Codec;
using Cysharp.Threading.Tasks;
using DeepCore;
using DeepCore.Concurrent;
using DeepCore.IO;
using DeepCore.Net.WS;
using DeepCore.NetClient;
using DeepCore.PomeloClient;
using DeepMetaGame.Data.Template;
using DeepMetaGame.Unity.Preview;
using Gate.Client;
using Hotfix.Battle;
using System;
using System.IO;
using UnityEngine;
using Yoo;

namespace Hotfix
{
    public class HotfixBootstrap : BattleBootstrap<
        _Temp_BattleCodec,
        _Temp_ZoneDataFactory,
        _Temp_ZoneHostFactory,
        _Temp_ZoneSlaveFactory,
        _Temp_ClientManager,
        _Temp_UnityBattleFactory
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        , _Temp_.Client.UnityPreview._Temp_UnityPreviewFactory
#endif
        >
    {
        static HotfixBootstrap()
        {
            new _Temp_Types();
        }
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        //----------------------------------------------------------------------------------------------------
        protected override async UniTask OnInitBegin()
        { 
            if (Directory.Exists(EditorRootPath))
            {
            }
            else if (new DirectoryInfo(Application.dataPath).TryFindParentDirectory(Path.Combine("GameEditor"), out var editorRoot))
            {
                EditorRootPath = editorRoot.FullName;
            }
            // 微信 WebGL + Android 统一：注册 YooAssetResourceLoader（接管资源加载，走 MPQFiles 包）。
            // 两平台共用同一套加载链：dataDir 资源（data.gz.bytes / scenes）从 MPQFiles 包按 location
            // "Assets/MPQFiles/..." 异步加载。loader 的 IsStartWith 永远 true 接管所有，从路径里的
            // "/data/" 自切 location，不依赖 EditorRootPath / Resource.PathRoot（保持 null，dataDir="/data"）。
            //
            // ⚠️ 必须显式白名单 WebGL/Android，不能写成 !UNITY_EDITOR：
            // GameEditor 预览拉起的 standalone .exe(-Preview)也满足 !UNITY_EDITOR && YOOASSET，
            // 一旦注册此 loader 会接管全部资源、把磁盘 Data/GameEditor/data 改道到 MPQFiles 包，
            // 而预览进程从不初始化 YooAsset → GetPackage 抛 "YooAssets not initialize"。
            // 预览/PC standalone 走磁盘直读，不应被此 loader 接管。
#if (UNITY_WEBGL || UNITY_ANDROID) && !UNITY_EDITOR && YOOASSET
            new Assets.Hotfix.YooAssetResourceLoader(
                cdnRoot: GameEntry.Instance?.hostURL,
                localRoot: GameEntry.Instance?.fallbackURL,
                mpqPackageName: "MPQFiles");
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN         
            new DeepCore.Template.NewtonJson.NewtonJsonTemplateLoader(true);
            TemplateDataCenter.ENABLE_LOAD_FROM_BIN = false;
            TemplateDataCenter.ENABLE_BATCH_LOAD = false;
            TypeAllocRecorder.ENABLE_STATISTICS = true;
#endif
        }
        protected override async UniTask OnInitFinish()
        {
            // YooAssetManager.DefaultUnityPackage = "DefaultPackage";         
            {
                YooAssetManager.DefaultUnityPackage = "DefaultPackage";
                //YooAssetParam.YooVersion = YooAssetClientManager.DefaultYooVersion;
                YooAssetManager.YooAssetRoot = $"{EditorRootPath}/res/yoo";
                YooAssetManager.DefaultUnityPackage = "DefaultPackage";
                //YooAssetManager.DefaultYooVersion = YooAssetClientManager.DefaultYooVersion;
#if UNITY_WEBGL
                WSWebSocketAdapter.ENABLE_SENDING_POOL = false;
                var data = new YooAssetData()
                {
                    HostURL = GameEntry.Instance.hostURL,
                    FallbackURL = GameEntry.Instance.fallbackURL,
                    PlayMode = GameEntry.Instance.EPlayMode,
                    DownloadingMaxCount = 20,
                    ReTryTimes = 3,
                    DefaultPackageName = "DefaultPackage",
                };
                await YooAssetManager.InitYooAsset(data);
#else
                //                 await YooAssetManager.InitYooAsset(new YooAssetData()
                //                 {
                // //                     HostURL = GameEntry.Instance.hostURL,
                // //                     FallbackURL = GameEntry.Instance.fallbackURL,
                //                     PlayMode = YooAssetManager.DefaultPlayMode,
                //                     DownloadingMaxCount = 20,
                //                     ReTryTimes = 3,
                //                     DefaultPackageName = "DefaultPackage",
                //                 });
                await YooAssetManager.InitYooAsset(null);
#endif
                await YooAssetManager.InitPackage();
            }
            await base.OnInitFinish();
        }

        protected override async UniTask OnLoadTemplatesAsync(IRangeValue p)
        {
            TemplateManager.IsEditor = RuntimeGameMode != EGameMode.GameClient;
            try
            {
                await base.OnLoadTemplatesAsync(p);
            }
            finally
            {
                // Keep the preloaded MPQ bundle resident through template parsing so
                // the real asset request can reuse both the cached file and bundle.
            }
        }
        protected override void OnError(Exception err)
        {
        }
        //----------------------------------------------------------------------------------------------------
        //         protected override NewtonJsonTemplateLoader CreateTemplateLoader(string root)
        //         {
        //             return new NewtonJsonTemplateLoader(true);
        //         }
        protected override _Temp_UnityBattleFactory CreateUnityFactory(string root)
        {
            return new _Temp_UnityBattleFactory(root);
        }
        private ETemplateLangKey GetGameLanguage()
        {
            //             if (Enum.TryParse(GameEntry.Instance.Lang.ToString(), out ETemplateLangKey key))
            //                 return key;
            return ETemplateLangKey.zh_CN;
        }
        protected override _Temp_ClientManager CreateClientManager()
        {
            UnityEngine.Debug.Log("EditorRootPath : " + HotfixBootstrap.EditorRootPath);
            _Temp_ClientManager.LangKey = GetGameLanguage();
            return new _Temp_UnityGateClientManager();
        }
        //----------------------------------------------------------------------------------------------------
    }
    class _Temp_UnityGateClientManager : _Temp_ClientManager
    {
        public _Temp_UnityGateClientManager()
        {
#if UNITY_WEBGL
            new WSNetClientFactory();
#else
            new DeepCore.PomeloClient.PomeloClientFactory();
#endif
        }
        public override IClientAdapter CreateNetClientAdapter(string address, GateNetClient client)
        {
            Debug.Log("CreateNetClientAdapter Called!");
#if UNITY_WEBGL
            // 微信 WebGL 只支持 WebSocket，PomeloClient（KCP/TCP）不可用，强制走 WS
            Debug.Log($"CreateNetClientAdapter WebGL addr={address}");
            return WSNetClientFactory.WSInstance.CreateAdapter(client);
#else
            Debug.Log($"CreateNetClientAdapter Non-WebGL addr={address}");
            if (address.StringStartWithIgnoreCase("ws://") || address.StringStartWithIgnoreCase("wss://"))
            {
                return WSNetClientFactory.WSInstance.CreateAdapter(client);
            }
            else
            {
                return PomeloClientFactory.IOInstance.CreateAdapter(client);
            }
#endif
        }
    }
}
