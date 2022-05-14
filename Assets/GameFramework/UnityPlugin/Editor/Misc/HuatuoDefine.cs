using System;
using System.Collections.Generic;
using System.Linq;
using UnityGameFramework.Editor.ResourceTools;
using UnityEditor;

namespace UnityGameFramework.Editor
{
    public static class HuatuoDefine
    {
        public static readonly string EditorConfigPath = "Assets/Settings/GFConfigs/HuatuoSettings.txt";
        public static readonly string[] HuatuoSymbols =
        {
            "ENABLE_HUATUO"
        };
        
        /// <summary>
        /// Convert UGF platform to Unity platform define
        /// </summary>
        public static readonly Dictionary<Platform, BuildTarget> Platform2BuildTargetDic =
            new Dictionary<Platform, BuildTarget>()
            {
                {Platform.Undefined, BuildTarget.NoTarget},
                {Platform.Windows, BuildTarget.StandaloneWindows},
                {Platform.Windows64, BuildTarget.StandaloneWindows64},
                {Platform.MacOS, BuildTarget.StandaloneOSX},
                {Platform.Linux, BuildTarget.StandaloneLinux64},
                {Platform.IOS, BuildTarget.iOS},
                {Platform.Android, BuildTarget.Android},
                {Platform.WindowsStore, BuildTarget.WSAPlayer},
                {Platform.WebGL, BuildTarget.WebGL}
            };
        
        private static List<BuildTargetGroup> _buildTargetGroupsUGFCache;
        public static List<BuildTargetGroup> BuildTargetGroupsOfUGFOption
        {
            get
            {
                if (null == _buildTargetGroupsUGFCache)
                {
                    _buildTargetGroupsUGFCache = new List<BuildTargetGroup>();
                    foreach (var platform in Enum.GetValues(typeof(Platform)).Cast<Platform>())
                    {
                        if (Platform.Undefined == platform) continue;
                        var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(Platform2BuildTargetDic[platform]);
                        if (!_buildTargetGroupsUGFCache.Contains(buildTargetGroup)) _buildTargetGroupsUGFCache.Add(buildTargetGroup);
                    }
                }

                return _buildTargetGroupsUGFCache;
            }         
        }
    }
}