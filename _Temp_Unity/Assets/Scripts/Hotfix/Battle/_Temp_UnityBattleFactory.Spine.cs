using DeepMetaGame.Unity;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hotfix.Battle
{

    public partial class SkinSlotAttachmentData
    {
        public string SlotName;
        public string AttachmentName;
        public string AttachmentSprite;
    }

    public partial class SkeletonSkinUnit : ISpine
    {
        private SkeletonAnimation animator;
        private SkeletonDataAsset skeletonDataAsset;
        private SkeletonData skeletonData;
        private Material sourceMaterial;
        //private Skin mixAndMatchSkin;
        //---------------------------------------------------------------------------------------------------------------
        #region 战斗编辑器调用
        private SkeletonAnimation anim => animator;
        bool ISpine.playing { get => anim.enabled; set => anim.enabled = value; }
        bool ISpine.loop { get => anim.loop; set => anim.loop = value; }
        float ISpine.speed { get => anim.timeScale; set => anim.timeScale = value; }
        string ISpine.initialSkinName
        {
            get => anim.initialSkinName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    anim.initialSkinName = value;
                    anim.skeleton.SetSkin(value);
                }
                else
                {
                    anim.initialSkinName = "default";
                    anim.skeleton.SetSkin("default");
                }
            }
        }
        string ISpine.AnimationName
        {
            get => anim.AnimationName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {

                    anim.AnimationName = value;
                    anim.AnimationState.SetAnimation(0, value, ((ISpine)this).loop);
                }
            }
        }
        float ISpine.TotalDuration => anim.Skeleton.Data.Animations.Max(a => a.Duration);
        bool ISpine.HasAnimation(string name) => anim.Skeleton.Data.FindAnimation(name) != null;
        IEnumerable<string> ISpine.Animations => anim.Skeleton.Data.Animations.ConvertAll(t=>t.Name);
        bool ISpine.HasSkin(string name) => anim.skeleton.Data.FindSkin(name) != null;
        IEnumerable<string> ISpine.Skins => anim.Skeleton.Data.Skins.ConvertAll(t => t.Name);

        void ISpine.SetAvatar(params string[] skins)
        {
//             if (skins != null)
//             {
//                 mixAndMatchSkin.Clear();
//                 var skeleton = animator.Skeleton;
//                 if (!string.IsNullOrEmpty(anim.initialSkinName))
//                 {
//                     var skin = skeletonData.FindSkin(anim.initialSkinName);
//                     if (skin != null)
//                     {
//                         mixAndMatchSkin.AddSkin(skin);
//                     }
//                 }
//                 foreach (var skinName in skins)
//                 {
//                     if (!string.IsNullOrEmpty(skinName))
//                     {
//                         var skin = skeletonData.FindSkin(skinName);
//                         if (skin == null)
//                         {
//                             Debug.LogError($"错误的Spine皮肤名 ：{skinName} ");
//                             continue;
//                         }
//                         mixAndMatchSkin.AddSkin(skin);
//                     }
//                 }
//                 skeleton.SetSkin(mixAndMatchSkin);
//                 animator.Skeleton.SetSlotsToSetupPose();
//                 animator.AnimationState.Apply(animator.Skeleton); //skeletonAnimation.Update(0);
//             }
        }
        #endregion
        //---------------------------------------------------------------------------------------------------------------
        public void Init(SkeletonAnimation animation)
        {
            animator = animation;
            skeletonDataAsset = animator.SkeletonDataAsset;
            sourceMaterial = skeletonDataAsset.atlasAssets[0].PrimaryMaterial;
            skeletonData = animator.Skeleton.Data;

            //mixAndMatchSkin = new Skin("Result");
        }

#if false
        public void RefreshSkin(List<string> skinNames, List<SkinSlotAttachmentData> attachmentDatas)
        {
            var skeleton = animator.Skeleton;
            var skeletonData = skeleton.Data;

            mixAndMatchSkin.Clear();

            if (skinNames != null)
            {
                foreach (var skinName in skinNames)
                {
                    var skin = skeletonData.FindSkin(skinName);
                    if (skin == null)
                    {
                        Debug.LogError($"错误的Spine皮肤名 ：{skinName} ");
                        continue;
                    }

                    mixAndMatchSkin.AddSkin(skin);
                }
            }

            if (attachmentDatas != null)
            {
                foreach (var attachment in attachmentDatas)
                {
                    AssetHandle rawFile = AssetManager.Instance.LoadAsset<Sprite>(attachment.AttachmentSprite);
                    var sprite = rawFile.GetAssetObject<Sprite>();

                    ChangeImage(sprite, attachment.SlotName, attachment.AttachmentName);
                }
            }

            skeleton.SetSkin(mixAndMatchSkin);
            RefreshSkeletonAttachments();
        }


        public void ChangeImage(Sprite sprite, string slotName, string attachmentName)
        {
            var slot = skeletonData.FindSlot(slotName);
            if (slot == null)
            {
                Debug.LogError($"错误的插槽名：{slotName}");
                return;
            }

            int slotIndex = slot.Index;//根据骨骼槽点 找到槽点索引
            Attachment attachment = GenerateAttachment(slotIndex, mixAndMatchSkin, attachmentName, sprite);
            if (attachment == null) return;

            mixAndMatchSkin.SetAttachment(slotIndex, attachmentName, attachment);
        }


        private Attachment GenerateAttachment(int slotIndex, Skin skin, string attachmentName, Sprite sprite)
        {
            Attachment tAttachment = skin.GetAttachment(slotIndex, attachmentName);
            if (tAttachment == null)
            {
                Debug.LogError($"skin {skin.Name} slotIndex {slotIndex} attachmentName {attachmentName} ==》attachment=null");
                return null;
            }
            var attachment = tAttachment?.GetRemappedClone(sprite, sourceMaterial, premultiplyAlpha: true);
            return attachment;
        }
        /// <summary>
        /// 刷新皮肤
        /// </summary>
        public void RefreshSkeletonAttachments()
        {
            animator.Skeleton.SetSlotsToSetupPose();
            animator.AnimationState.Apply(animator.Skeleton); //skeletonAnimation.Update(0);
        }

#endif



        public void Destroy()
        {
            animator = null;
            skeletonDataAsset = null;
            sourceMaterial = null;
            skeletonData = null;
            //mixAndMatchSkin = null;
        }
    }

    /// <summary>
    /// GPU 动画版 ISpine 实现。
    /// 用于以 GPUAnimationController 驱动的 body（VAT 方案）。
    /// Skin 相关接口 GPU 方案不支持，调用会抛 NotImplementedException。
    /// </summary>
//     public partial class GPUAnimSkinUnit : ISpine
//     {
//         private GPUAnimationController controller;
//         private string currentAnim;
// 
//         public void Init(GPUAnimationController ctrl)
//         {
//             controller = ctrl;
//             currentAnim = ctrl != null ? ctrl.CurrentAnimName : null;
//         }
// 
//         public void Destroy()
//         {
//             controller = null;
//             currentAnim = null;
//         }
// 
//         #region ISpine
//         bool ISpine.playing
//         {
//             get => controller != null && controller.IsPlaying;
//             set
//             {
//                 if (controller == null) return;
//                 if (value) controller.Resume();
//                 else controller.Pause();
//             }
//         }
// 
//         bool ISpine.loop
//         {
//             get => controller != null && controller.IsLoop;
//             set { if (controller != null) controller.IsLoop = value; }
//         }
// 
//         float ISpine.speed
//         {
//             get => controller != null ? controller.TimeScale : 1f;
//             set { if (controller != null) controller.TimeScale = value; }
//         }
// 
//         string ISpine.initialSkinName
//         {
//             get => "Idle";
//             set
//             {
//                 
//             }
//         }
// 
//         string ISpine.AnimationName
//         {
//             get => currentAnim;
//             set
//             {
//                 if (controller == null || string.IsNullOrEmpty(value)) return;
//                 if (value == currentAnim && controller.IsPlaying) return;
//                 currentAnim = value;
//                 controller.Play(value, ((ISpine)this).loop);
//             }
//         }
// 
//         float ISpine.TotalDuration => controller != null ? controller.TotalDuration : 0f;
// 
//         bool ISpine.HasAnimation(string name) => controller != null && controller.HasAnim(name);
// 
//         IEnumerable<string> ISpine.Animations => controller != null ? controller.GetAnimNames() : System.Linq.Enumerable.Empty<string>();
// 
//         bool ISpine.HasSkin(string name) => true;
// 
//         IEnumerable<string> ISpine.Skins
//         {
//             get => null;
//         }
// 
//         void ISpine.SetAvatar(params string[] skins)
//         {
//         }
// 
//         #endregion
//     }
    //     partial class SkinSlotAttachmentData
    //     {
    //
    //     }
    //
    //     partial class SkeletonSkinUnit
    //     {
    //     }
}