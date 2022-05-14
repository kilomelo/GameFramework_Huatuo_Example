using System.Collections.Generic;
using System.Linq;
namespace Global
{
    /// <summary>
    /// 热更相关定义
    /// </summary>
    public static class HotfixDefine
    {
        /// <summary>
        /// 需要在Prefab上挂脚本的热更dll名称列表，不需要挂到Prefab上的脚本可以不放在这里
        /// 但放在这里的dll即使勾选了 AnyPlatform 也会在打包过程中被排除
        /// 
        /// 另外请务必注意！： 需要挂脚本的dll的名字最好别改，因为这个列表无法热更（上线后删除或添加某些非挂脚本dll没问题）
        /// </summary>
        public static readonly List<string> MonoHotUpdateDllNames = new List<string>()
        {
            "HotfixMono.dll",
        };

        /// <summary>
        /// 所有热更新dll列表
        /// </summary>
        public static readonly List<string> AllHotUpdateDllNames = MonoHotUpdateDllNames.Concat(new List<string>
        {
            // 这里放除了s_monoHotUpdateDllNames以外的脚本不需要挂到资源上的dll列表
            "HotfixMain.dll",
        }).ToList();

        /// <summary>
        /// Dll of main business logic assembly
        /// </summary>
        public static readonly string LogicEntranceDllName = "HotfixMain.dll";
        
        /// <summary>
        /// 程序集文本资产打包Asset后缀名
        /// </summary>
        public static readonly string AssemblyTextAssetExtension = "txt";

        /// <summary>
        /// 程序集文本资产资源目录
        /// </summary>
        public static readonly string AssemblyTextAssetResPath = "Assets/Res/ResHotfix/Assembly";
        
        // #if UNITY_EDITOR
        public static readonly string DllBuildOutputDir = "{0}/../Temp/HuaTuo/build";
        // #endif
    }
}