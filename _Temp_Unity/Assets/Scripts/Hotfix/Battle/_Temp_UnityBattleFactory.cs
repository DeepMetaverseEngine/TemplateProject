using Cysharp.Threading.Tasks;
using DeepCore;
using DeepCore.Unity;
using DeepCore.Unity3D;
using DeepGame3D.Unity.BattleView;
using DeepMetaGame.Data;
using DeepMetaGame.Unity;
using DeepMetaGame.Unity.BattleView;
using DeepMetaGame.Unity.BattleView.UI;
using Spine.Unity;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Yoo;
using YooAsset;
using static DeepMetaGame.Unity.SimpleResourceComponent;

namespace Hotfix.Battle
{
    public partial class _Temp_UnityBattleFactory : _Temp_.Client.Unity.Battle._Temp_BaseUnityBattleFactory
    {
        public _Temp_AssetLoader Loader { get; }

        public _Temp_UnityBattleFactory(string root) : base(root)
        {
            Loader = new _Temp_AssetLoader();
            new _Temp_ResourceComponent();
        }

        public override UnityZone CreateBattle()
        {
            return new _Temp_UnityZone();
        }


        //         private void ClearWrapPoolChildren()
        //         {
        // //             var poolNode = WrapPoolNode;
        // //             if (!poolNode) return;
        // //             for (var i = poolNode.childCount - 1; i >= 0; i--)
        // //             {
        // //                 Object.Destroy(poolNode.GetChild(i).gameObject);
        // //             }
        //         }

        public override HUDUnitHPBar CreateHUDUnitHPBar(UnityLayerObject obj)
        {
            //             if (obj is UnityZoneUnit unit)
            //             {
            //                 var bar = unit.layer.ObjectPool.AllocAutoRelease<HotfixZumaUnitHPBar>();
            //                 var ret = bar.Init(unit);
            //                 return ret;
            //             }
            return null;
        }

        public class _Temp_AssetLoader : AssetsLoaderComponent
        {

            public const string YOO_START = "/res/yoo/UnityPackage/";

            public override void LoadAssets<ST>(in string file, ResourceType resType, ST st, LoadAssetsHandler<ST> cb)
            {
                // Debug.Log($"loadAsset : {file}   {resType}");
                try
                {
                    var path = file;
                    _loadAssetsAsync(path, resType, st, cb).Forget();
                }
                catch (Exception err)
                {
                    err.PrintStackTrace();
                    cb.Invoke(st, null);
                }
            }
            private async UniTaskVoid _loadAssetsAsync<ST>(string file, ResourceType resType, ST st, LoadAssetsHandler<ST> cb)
            {
                try
                {
                    var path = file;
                    //                     if (path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                    //                     {
                    //                         var handle = await AssetManager.Instance.LoadSceneAsync(path);
                    //                         if (handle != null)
                    //                         {
                    //                             cb.Invoke(st, new _Temp_BaseAssetsTuple(file, resType, handle));
                    //                             return;
                    //                         }
                    //                     }
                    //                     else
                    {
                        var handle = await AssetManager.Instance.LoadAssetAsync(path);
                        if (handle != null)
                        {
                            cb.Invoke(st, new _Temp_BaseAssetsTuple(file, resType, handle));
                            return;
                        }
                    }
                    cb.Invoke(st, null);

                }
                catch (Exception err)
                {
                    err.PrintStackTrace();
                    cb.Invoke(st, null);
                }
            }

            public override IAssetsTemplate LoadAssets(in string file, DeepMetaGame.Data.ResourceType resType)
            {
                // Debug.Log($"loadAsset : {file}   {resType}");
                try
                {
                    var path = file;
                    var handle = AssetManager.Instance.LoadAsset<GameObject>(path);
                    if (handle != null)
                    {
                        return new _Temp_BaseAssetsTuple(file, resType, handle);
                    }
                }
                catch (Exception err)
                {
                    err.PrintStackTrace();
                }

                return (null);
            }
        }
        public class _Temp_BaseAssetsTuple : DefaultAssetsTuple
        {
            private AssetHandle handle;
            private GameObject prefab;
            public override object handler => handle;
            public override UnityEngine.Object template => prefab;
            public _Temp_BaseAssetsTuple(string resName, ResourceType resType, AssetHandle handle)
            {
                this.resName = resName;
                this.resType = resType;
                this.handle = handle;
                // PC / 微信统一：template 始终为 InstantiateSync 副本，避免 OfflinePlayMode 下
                // GetAssetObject 原始资产被挂入 tempPoolNode 后只 Release 不 Destroy 导致引用不一致。
                this.prefab = handle.InstantiateSync();
                this.InitRes(prefab);
            }
            protected override void Disposing()
            {
                if (prefab != null)
                {
                    UnityEngine.Object.Destroy(prefab);
                    prefab = null;
                }
                if (handle != null)
                {
                    AssetManager.Instance.UnloadAssetHandle(handle);
                    handle = null;
                }
            }
            protected virtual void InitRes(GameObject prefab)
            {
                //                 if (res != null)
                //                 {
                //                     if (prefab && prefab is GameObject go)
                //                     {
                //                         if (go.TryGetComponentsInChildren<Renderer>(out var renders))
                //                         {
                //                             foreach (var r in renders)
                //                             {
                //                                 //r.rendererPriority += res.RenderQueueOffset;
                //                                 r.sortingOrder += res.RenderQueueOffset;
                //                             }
                //                         }
                //                         //                     if (go.TryGetComponentsInChildren<ParticleSystem>(out var pss))
                //                         //                     {
                //                         //                         foreach (var ps in pss)
                //                         //                         {
                //                         //                             ps.
                //                         //                         }
                //                         //                     }
                //                     }
                //                 }
            }
        }

        class _Temp_ResourceComponent : SimpleResourceComponent
        {
            public override bool TryGetSpine(GameObject go, out ISpine spine)
            {
                if (go)
                {
                    // 优先识别 GPU 动画组件（VAT 方案）
                    //                       if (go.TryGetComponentInChildren<GPUAnimationController>(out var gpuCtrl))
                    //                       {
                    //                           var gpuSkin = new GPUAnimSkinUnit();
                    //                           gpuSkin.Init(gpuCtrl);
                    //                           spine = gpuSkin;
                    //                           return true;
                    //                       }
                    if (go.TryGetComponentInChildren<SkeletonAnimation>(out var _spine))
                    {
                        var skin = new SkeletonSkinUnit();
                        skin.Init(_spine);
                        spine = skin;
                        //                         if (wrap.SrcAssets is IRogueAssetsTuple rwrap && rwrap.ResData != null)
                        //                         {
                        //                             spine.initialSkinName = rwrap.ResData.SkinName;
                        //                             if (rwrap.ResData.AppendSkin.IsNotEmpty())
                        //                             {
                        //                                 spine.AppendSkin(rwrap.ResData.AppendSkin);
                        //                             }
                        //                         }
                        return true;
                    }
                }

                spine = null;
                return false;
            }

            public override IAssetLoadingTask LoadSceneResource(UnityZone zone, BattleResourceLoaderHandler<UnityZone, IZoneResource> cb)
            {
                //                 if (zone.layer.Data.FileName.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                //                 {
                //                     var handle = await AssetManager.Instance.LoadSceneAsync(path);
                //                     if (handle != null)
                //                     {
                //                         cb.Invoke(st, new _Temp_BaseAssetsTuple(file, resType, handle));
                //                         return;
                //                     }
                //                 }
                return base.LoadSceneResource(zone, cb);
            }
        }

        class _Temp_UnitResource : SimpleUnitRes
        {
            protected override bool TryPlayAnimator(UnityZoneUnit.UnityActionStatus status, string StateName, float speed, bool loop, float NormalizeTime)
            {
                if (base.TryPlayAnimator(status, StateName, speed, loop, NormalizeTime))
                {
                    return true;
                }
                return false;
            }

        }
        //         class _Temp_SceneResource : Disposable, IZoneResource
        //         {
        //             private SceneHandle wrap;
        //             public UnityZone zone { get; private set; }
        //             public virtual bool Active
        //             {
        //                 get => wrap.gameObject.activeSelf;
        //                 set => wrap.gameObject.SetActive(value);
        //             }
        //             public virtual _Temp_SceneResource Init(UnityZone zone, SceneHandle wrap)
        //             {
        //                 this.wrap = wrap;
        //                 this.zone = zone;
        //                 var layer = zone.layer;
        //                 this.gameObject = wrap?.gameObject;
        //                 this.transform = wrap?.gameObject?.transform;
        //                 if (gameObject != null)
        //                 {
        //                     if (!string.IsNullOrEmpty(zone.config.RayCastTerrainLayerName))
        //                     {
        //                         gameObject.SetLayer(zone.config.RayCastTerrainLayerName);
        //                     }
        //                     //                     if (wrap.transform.TryGetComponentInChildren<Light>(out var sceneLight))
        //                     //                     {
        //                     //                         if (zone.DefaultLight != null)
        //                     //                         {
        //                     //                             zone.DefaultLight.enabled = false;
        //                     //                         }
        //                     //                         zone.DefaultLight = sceneLight;
        //                     //                     }
        //                 }
        //                 return this;
        //             }
        //             protected override void Disposing()
        //             {
        //                 wrap?.Dispose();
        //                 wrap = null;
        //                 zone = null;
        //             }
        //             public virtual void UpdateResource()
        //             {
        // 
        //             }
        //         }


    }
}
