using Cysharp.Threading.Tasks;
using DeepMetaGame.Unity;
using DeepMetaGame.Unity.Preview;
using Hotfix;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yoo;
using YooAsset;

public class Launcher : MonoBehaviour
{
    public static Launcher Instance { get; private set; }
    [SerializeField] private AssetMode AssetMode = AssetMode.Bundle;

    private async void Awake()
    {
        Instance = this;
        if (!Application.isEditor)
        {
            Application.quitting += static () => { System.Diagnostics.Process.GetCurrentProcess().Kill(); };
        }

#if UNITY_WEBGL && WEIXINMINIGAME
        this.AssetMode = AssetMode.Bundle;
        YooAssetManager.DefaultPlayMode = EPlayMode.WebPlayMode;
#else
        if (!Application.isEditor)
        {
            this.AssetMode = AssetMode.Bundle;
            YooAssetManager.DefaultPlayMode = EPlayMode.OfflinePlayMode;
        }
        else
        {
            YooAssetManager.DefaultPlayMode = AssetMode == AssetMode.Bundle ? EPlayMode.OfflinePlayMode : EPlayMode.EditorSimulateMode;
        }
        //         if (GameEntry.Instance)
        //         {
        //             YooAssetManager.DefaultPlayMode = GameEntry.Instance.EPlayMode;
        //         }
#endif
        BattleBootstrap.OnFinish(Bootstrap_OnFinish);
    }




    private async UniTask Bootstrap_OnFinish()
    {
        var GameMode = BattleBootstrap.RuntimeGameMode;
        switch (GameMode)
        {
            case EGameMode.Editor_Preview:
                // 进入编辑器 预览
                {
                    UnityBattleFactory.CommandLineArgs.Put("-Preview", "true");
                    //                     var sceneHandle = AssetManager.Instance.LoadSceneAsync("Assets/Scenes/EditorMain.unity", LoadSceneMode.Additive);
                    //                     await sceneHandle;
                    SceneManager.LoadScene("EditorMain", LoadSceneMode.Additive);
                }

                break;
            case EGameMode.Editor_Resource:
                // 进入编辑器 资源
                {
                    UnityBattleFactory.CommandLineArgs.Put("-Resource", "true");
                    //                     var sceneHandle = AssetManager.Instance.LoadSceneAsync("Assets/Scenes/EditorMain.unity", LoadSceneMode.Additive);
                    //                     await sceneHandle;
                    SceneManager.LoadScene("EditorMain", LoadSceneMode.Additive);
                }
                break;
            case EGameMode.Editor_Scene:
                // 进入编辑器 场景
                {
                    UnityBattleFactory.CommandLineArgs.Put("-SceneEditor", "true");
                    //                     var sceneHandle = AssetManager.Instance.LoadSceneAsync("Assets/Scenes/EditorMain.unity", LoadSceneMode.Additive);
                    //                     await sceneHandle;
                    SceneManager.LoadScene("EditorMain", LoadSceneMode.Additive);
                }
                break;
            case EGameMode.BattleTest:
                // 进入编辑器 战斗测试 BattleTest 场景
                {
                    //var sceneHandle = AssetManager.Instance.LoadSceneAsync("Assets/Scenes/EditorGame.unity", LoadSceneMode.Additive);
                    //await sceneHandle;
                    SceneManager.LoadScene("EditorGame", LoadSceneMode.Additive);
                }
                break;
            case EGameMode.GameClient:
                Screen.SetResolution(1280, 720, false);
                {
                    //                     var sceneHandle = AssetManager.Instance.LoadSceneAsync("Assets/Scenes/GameLogin.unity", LoadSceneMode.Additive);
                    //                     await sceneHandle;
                    // 进入游戏
                    // SceneManager.LoadScene("GameRoot", LoadSceneMode.Additive);
                    // TODO 默认进入游戏
                    // 正式登录游戏场景
                    //SceneManager.LoadScene("Launcher_Game", LoadSceneMode.Single);
                }
                {
                    //这里初始化管理器
                    //HotfixMain当个管理器吧
                    // #if !UNITY_EDITOR
                    //                     var handle = await AssetManager.Instance.LoadAssetAsync<TextAsset>("Assets/HotfixDlls/Hotfix.dll.bytes");
                    //                     Assembly.Load(handle.GetAssetObject<TextAsset>().bytes);
                    // #else
                    // #endif
                    //                     // Editor下无需加载，直接查找获得HotUpdate程序集
                    //                     var hotfixAssembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Hotfix");
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}

public enum AssetMode
{
    Prefab,
    Bundle,
}