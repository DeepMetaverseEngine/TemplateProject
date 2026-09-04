

#if YOOASSET_EDITOR

using DeepCore;
using DeepCore.IO;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

public class BuildTools
{

    [MenuItem("Build/Bundle/Windows")]
    public static void BuildWindowsBundle()
    {
        EditorWindowUtility.ClearUnityConsole();

        CheckPlatform("Windows");

        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_Standard);
        PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 1);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

        BuildInternal(BuildTarget.StandaloneWindows64);
    }
    public static void CheckPlatform(string param)
    {

        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        switch (param)
        {
            case "Android":
                buildTarget = BuildTarget.Android;
                break;
            case "Windows":
                buildTarget = BuildTarget.StandaloneWindows64;
                break;
            default:
                break;
        }

        // ï¿½ï¿½éµ±Ç°Æ½Ì?
        if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
        {
            bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget == BuildTarget.Android ? BuildTargetGroup.Android : BuildTargetGroup.Standalone, buildTarget);
            if (success)
            {

            }
            else
            {

            }
        }

    }
    private static void BuildInternal(BuildTarget buildTarget)
    {
        var buildoutputRoot = BundleBuilderHelper.GetDefaultBuildOutputRoot();
        var streamingAssetsRoot = BundleBuilderHelper.GetStreamingAssetsRoot();

        // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        var buildParameters = new ScriptableBuildParameters();
        buildParameters.BuildOutputRoot = buildoutputRoot;
        buildParameters.BundledFileRoot = streamingAssetsRoot;
        buildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString();
        buildParameters.BuildBundleType = (int)EBundleType.AssetBundle; //ï¿½ï¿½ï¿½ï¿½Ö¸ï¿½ï¿½ï¿½ï¿½Ô´ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        buildParameters.BuildTarget = buildTarget;
        buildParameters.PackageName = "DefaultPackage";
        buildParameters.PackageVersion = CUtils.FormatTime(DateTime.Now);
        buildParameters.VerifyBuildingResult = true;
        buildParameters.EnableSharePackRule = true; //ï¿½ï¿½ï¿½Ã¹ï¿½ï¿½ï¿½ï¿½ï¿½Ô´ï¿½ï¿½ï¿½ï¿½Ä£Ê½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½1.5xï¿½æ±¾
        buildParameters.FileNameStyle = EFileNameStyle.HashName;
        buildParameters.BundledCopyOption = EBundledCopyOption.ClearAndCopyAll;
        buildParameters.BundledCopyParams = string.Empty;
        buildParameters.BundleEncryptor = new EncryptionNone();
        buildParameters.CompressOption = ECompressOption.LZ4;
        buildParameters.ClearBuildCacheFiles = false; //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½æ£¬ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ß´ï¿½ï¿½ï¿½Ù¶È£ï¿½
        buildParameters.UseAssetDependencyDB = true; //Ê¹ï¿½ï¿½ï¿½ï¿½Ô´ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ïµï¿½ï¿½ï¿½İ¿â£¬ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ß´ï¿½ï¿½ï¿½Ù¶È£ï¿½
        buildParameters.BuiltinShadersBundleName = GetBuiltinShaderBundleName(buildParameters.PackageName);

        var pipeline = new ScriptableBuildPipeline();
        BuildResult buildResult;
        //using (YooCollectPlatform.Begin(buildTarget))
        {
            buildResult = pipeline.Run(buildParameters, true);
        }
        if (buildResult.Success)
        {
            if (CFiles.TryFindParentDirectory(buildResult.OutputPackageDirectory, Path.Combine("GameEditor", "res"), out var res_yoo))
            {
                CFiles.DeleteAll(Path.Combine(res_yoo, "yoo", buildParameters.PackageName));
                CFiles.DirectoryCopy($"{buildResult.OutputPackageDirectory}", Path.Combine(res_yoo, "yoo", buildParameters.PackageName));
                CFiles.DirectoryCopy($"{Application.streamingAssetsPath}/yoo", Path.Combine(res_yoo, "yoo"), new CFiles.FileFilter(f => f.Extension != ".meta"));
            }
        }
        else
        {
            Debug.LogError($": {buildResult.ErrorInfo}");
        }
    }


    /// <summary>
    /// ÄÚÖÃ×ÅÉ«Æ÷×ÊÔ´°üÃû³Æ
    /// ×¢Òâ£ººÍ×Ô¶¯ÊÕ¼¯µÄ×ÅÉ«Æ÷×ÊÔ´°üÃû±£³ÖÒ»ÖÂ£¡
    /// </summary>
    private static string GetBuiltinShaderBundleName(string packageName)
    {
        var uniqueBundleName = BundleCollectorSettingData.Setting.UniqueBundleName;
        var packRuleResult = DefaultBundlePackRule.CreateShadersPackRuleResult();
        return packRuleResult.GetBundleName(packageName, uniqueBundleName);
    }
}
#endif