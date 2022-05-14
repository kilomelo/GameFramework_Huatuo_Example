using Global;
using System.Collections;
using System.Collections.Generic;
using HuaTuo;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Web.Configuration;
using GameFramework;

namespace Editor.Base
{
    public static class Shortcuts
    {
        /// <summary>
        /// Build Hot Update Assembly Assets and copy to assembly text asset folder
        /// </summary>
        [MenuItem("Shortcuts/Build Hot Update Assembly Assets", false, 1)]
        public static void BuildHotUpdateAssemblyAssets()
        {
            var dllTempDir = HuaTuoEditorHelper.GetDllBuildOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget);
            HuaTuoEditorHelper.CompileDll(dllTempDir, EditorUserBuildSettings.activeBuildTarget);
            FileTools.ClearFolder(HotfixDefine.AssemblyTextAssetResPath);
            foreach (var hotUpdateDllName in HotfixDefine.AllHotUpdateDllNames)
            {
                var dllTempPath = Utility.Path.GetRegularPath(Path.Combine(dllTempDir, hotUpdateDllName));
                if (File.Exists(dllTempPath))
                {
                    var textAssetPath = Utility.Path.GetRegularPath(Path.Combine(HotfixDefine.AssemblyTextAssetResPath, $"{hotUpdateDllName}.{HotfixDefine.AssemblyTextAssetExtension}"));
                    File.Copy(dllTempPath, textAssetPath);
                    Debug.Log($"Copy hot update assembly asset from [ {dllTempPath} ] success.");
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
