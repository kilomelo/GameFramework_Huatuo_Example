using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using UGFHelper;

namespace HotfixMain
{
    public class App
    {
        private static App _instance;
        // private LoadAssetCallbacks _callbacks;

        public static void Entry()
        {
            Log.Info("App.Entry");
            _instance = (App)Activator.CreateInstance(typeof(App));
            if (null == _instance)
            {
                Log.Fatal("Create App instance failed.");
                return;
            }
            _instance.Launch();
        }

        private void Launch()
        {
            Log.Info("App.Launch");
            // _callbacks ??= new LoadAssetCallbacks(LoadAssetSuccess, LoadAssetFailure);
            UGFIF.Scene.LoadScene(ResHelper.GetScenePath("App"));
        }
        
        private void LoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            Log.Debug($"LoadAssetSuccess, assetName: [ {assetName} ], duration: [ {duration} ], userData: [ {userData} ]");
            // UGFIF.Scene.LoadScene(ResHelper.GetScenePath("App"));
        }

        private void LoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Warning($"LoadAssetFailure, assetName: [ {assetName} ], status: [ {status} ], errorMessage: [ {errorMessage} ], userData: [ {userData} ]");
        }
    }
}
