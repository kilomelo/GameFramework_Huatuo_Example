using System.Collections.Generic;
using Global;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityGameFrameWork.Editor;
using UnityGameFramework.Editor.ResourceTools;
using System;
using System.Diagnostics;
using System.Linq;
using HuaTuo;
using GameFramework;
using UnityGameFramework.Editor;
using Debug = UnityEngine.Debug;

namespace Base.Editor.GameFrameWorkExtension
{
    public class VRBaseBuildEventHandler : IBuildEventHandler
    {
        private long _tempDynamicAssemblyPlatforms =0;

        /// <summary>
        /// 获取当某个平台生成失败时，是否继续生成下一个平台。
        /// </summary>
        public bool ContinueOnFailure => false;

        private string _lastCopiedAssemblyDllSourceFolder = string.Empty;

        /// <summary>
        /// 所有平台生成开始前的预处理事件。
        /// </summary>
        /// <param name="productName">产品名称。</param>
        /// <param name="companyName">公司名称。</param>
        /// <param name="gameIdentifier">游戏识别号。</param>
        /// <param name="gameFrameworkVersion">游戏框架版本。</param>
        /// <param name="unityVersion">Unity 版本。</param>
        /// <param name="applicableGameVersion">适用游戏版本。</param>
        /// <param name="internalResourceVersion">内部资源版本。</param>
        /// <param name="platforms">生成的目标平台。</param>
        /// <param name="dynamicAssemblyPlatforms">开启热更程序集的平台。</param>
        /// <param name="assetBundleCompression">AssetBundle 压缩类型。</param>
        /// <param name="compressionHelperTypeName">压缩解压缩辅助器类型名称。</param>
        /// <param name="additionalCompressionSelected">是否进行再压缩以降低传输开销。</param>
        /// <param name="forceRebuildAssetBundleSelected">是否强制重新构建 AssetBundle。</param>
        /// <param name="buildEventHandlerTypeName">生成资源事件处理函数名称。</param>
        /// <param name="outputDirectory">生成目录。</param>
        /// <param name="buildAssetBundleOptions">AssetBundle 生成选项。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="buildReportPath">生成报告路径。</param>
        public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion,
            Platform platforms, long dynamicAssemblyPlatforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
            bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName,
            string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions,
            string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected,
            string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            Debug.Log($"Start build resources, platform: [ {platforms} ]");
            _lastCopiedAssemblyDllSourceFolder = string.Empty;
            _tempDynamicAssemblyPlatforms = dynamicAssemblyPlatforms;
        }

        /// <summary>
        /// 某个平台生成开始前的预处理事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected,
            string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected,
            string outputPackedPath)
        {
            // compile user assemblies manually and copy to assembly text asset folder
            var buildTarget = HuatuoDefine.Platform2BuildTargetDic[platform];
            var dllTempDir = HuaTuoEditorHelper.GetDllBuildOutputDirByTarget(buildTarget);
            if (((int) platform & (int) _tempDynamicAssemblyPlatforms) == 0)
            {
                HuaTuoEditorHelper.CreateDirIfNotExists(dllTempDir);
                FileTools.ClearFolder(dllTempDir);
                foreach (var hotUpdateDllName in HotfixDefine.AllHotUpdateDllNames)
                {
                    File.Create($"{dllTempDir}/{hotUpdateDllName}").Close();
                }
            }
            else
            {
                HuaTuoEditorHelper.CompileDll(dllTempDir, buildTarget);
            }
            CopyAssemblyTextAssets(dllTempDir);
        }

        /// <summary>
        /// 某个平台生成 AssetBundle 完成事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="assetBundleManifest">AssetBundle 的描述文件。</param>
        public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected,
            string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected,
            string outputPackedPath, AssetBundleManifest assetBundleManifest) {}

        /// <summary>
        /// 某个平台可更新模式版本列表文件的输出事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="versionListPath">可更新模式版本列表文件的路径。</param>
        /// <param name="versionListLength">可更新模式版本列表文件的长度。</param>
        /// <param name="versionListHashCode">可更新模式版本列表文件的校验值。</param>
        /// <param name="versionListCompressedLength">可更新模式版本列表文件压缩后的长度。</param>
        /// <param name="versionListCompressedHashCode">可更新模式版本列表文件压缩后的校验值。</param>
        public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength,
            int versionListHashCode, int versionListCompressedLength, int versionListCompressedHashCode) {}

        /// <summary>
        /// 某个平台生成结束后的后处理事件。
        /// </summary>
        /// <param name="platform">生成平台。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="isSuccess">是否生成成功。</param>
        public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected,
            string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected,
            string outputPackedPath, bool isSuccess) {}

        /// <summary>
        /// 所有平台生成结束后的后处理事件。
        /// </summary>
        /// <param name="productName">产品名称。</param>
        /// <param name="companyName">公司名称。</param>
        /// <param name="gameIdentifier">游戏识别号。</param>
        /// <param name="gameFrameworkVersion">游戏框架版本。</param>
        /// <param name="unityVersion">Unity 版本。</param>
        /// <param name="applicableGameVersion">适用游戏版本。</param>
        /// <param name="internalResourceVersion">内部资源版本。</param>
        /// <param name="platforms">生成的目标平台。</param>
        /// <param name="dynamicAssemblyPlatforms">开启热更程序集的平台。</param>
        /// <param name="assetBundleCompression">AssetBundle 压缩类型。</param>
        /// <param name="compressionHelperTypeName">压缩解压缩辅助器类型名称。</param>
        /// <param name="additionalCompressionSelected">是否进行再压缩以降低传输开销。</param>
        /// <param name="forceRebuildAssetBundleSelected">是否强制重新构建 AssetBundle。</param>
        /// <param name="buildEventHandlerTypeName">生成资源事件处理函数名称。</param>
        /// <param name="outputDirectory">生成目录。</param>
        /// <param name="buildAssetBundleOptions">AssetBundle 生成选项。</param>
        /// <param name="workingPath">生成时的工作路径。</param>
        /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
        /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
        /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
        /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
        /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
        /// <param name="buildReportPath">生成报告路径。</param>
        public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion,
            Platform platforms, long dynamicAssemblyPlatforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName,
            bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName,
            string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions,
            string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected,
            string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            // copy hot update assembly dll of active build target to assembly asset folder if active build target support assembly hot update.
            var dllTempDir = HuaTuoEditorHelper.GetDllBuildOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget);
            // if already copied, skip
            if (string.Compare(_lastCopiedAssemblyDllSourceFolder, dllTempDir, StringComparison.Ordinal) != 0)
            {
                CopyAssemblyTextAssets(dllTempDir);
            }
            
            // copy asset bundles of active build target to streaming assets folder
            var activeTarget = EditorUserBuildSettings.activeBuildTarget;
            var activePlatform = Platform.Undefined;
            foreach (var pair in HuatuoDefine.Platform2BuildTargetDic.Where(pair => pair.Value == activeTarget))
            {
                activePlatform = pair.Key;
            }
            // return if active platform is not included in platforms
            if (((int) platforms & (int) activePlatform) == 0)
            {
                Debug.LogWarning($"Active platform [ {activePlatform} ] is not included in platforms, copy asset bundle skipped.");
                return;
            }
            var sourceDirectory = Utility.Path.GetRegularPath(Path.Combine(outputPackagePath, activePlatform.ToString()));
            Debug.Log($"Copy asset bundles from [ {sourceDirectory} ] to streaming asset folder.");
            FileTools.ClearFolder(Application.streamingAssetsPath);
            foreach (var file in Directory.EnumerateFiles(sourceDirectory))
            {
                File.Copy(file, Utility.Path.GetRegularPath(Path.Combine(Application.streamingAssetsPath, Path.GetFileName(file))));
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            Debug.Log($"Copy success.");

            _lastCopiedAssemblyDllSourceFolder = string.Empty;
            _tempDynamicAssemblyPlatforms = 0;
        }
        
        
        // copy assembly file to hotfix res folder as text asset
        private void CopyAssemblyTextAssets(string dllTempDir)
        {
            FileTools.ClearFolder(HotfixDefine.AssemblyTextAssetResPath);
            foreach (var hotUpdateDllName in HotfixDefine.AllHotUpdateDllNames)
            {
                var dllTempPath = Utility.Path.GetRegularPath(Path.Combine(dllTempDir, hotUpdateDllName));
                if (File.Exists(dllTempPath))
                {
                    var textAssetPath = Utility.Path.GetRegularPath(Path.Combine(HotfixDefine.AssemblyTextAssetResPath,
                        $"{hotUpdateDllName}.{HotfixDefine.AssemblyTextAssetExtension}"));
                    File.Copy(dllTempPath, textAssetPath);
                    Debug.Log($"Copy hot update assembly asset from [ {dllTempPath} ] success.");
                    _lastCopiedAssemblyDllSourceFolder = dllTempDir;
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}