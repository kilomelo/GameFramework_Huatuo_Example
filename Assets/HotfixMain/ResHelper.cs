using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using UnityEngine;

namespace HotfixMain
{
    public static class ResHelper
    {
        /// <summary>
        /// Root path of hotfix resources
        /// </summary>
        public static readonly string HotfixResRootPath = "Assets/Res/ResHotfix/";

        /// <summary>
        /// Get hotfix scene asset path
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static string GetScenePath(string sceneName)
        {
            return Utility.Path.GetRegularPath(Path.Combine(HotfixResRootPath, $"Scenes/{sceneName}.unity"));
        }
    }
}