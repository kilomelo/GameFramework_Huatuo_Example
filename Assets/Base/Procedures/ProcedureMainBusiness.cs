using System;
using Global;
using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using UGFHelper;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Tommy;
using Tommy.Extensions;

namespace Base
{
    /// <summary>
    /// App main business logic procedure
    /// </summary>
    public class ProcedureMainBusiness : ProcedureBase
    {
        private LoadAssetCallbacks _callbacks;

        private enum EAsmLoadState
        {
            Invlaid,
            Ready,
            Loading,
            WaitingResult,
            Finish,
        }

        private EAsmLoadState _state = EAsmLoadState.Invlaid;
        private int _loadingCnt;
        private int _failureCnt;

        private ProcedureOwner _procedureOwner;
        private Assembly _mainBusinessLogicAsm;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            _procedureOwner = procedureOwner;
            Log.Info("ProcedureMainBusiness OnEnter");

            _state = EAsmLoadState.Ready;
            _loadingCnt = 0;
            _failureCnt = 0;
            UGFIF.Resource.InitResources(InitResFinishCallback);
        }

        private void InitResFinishCallback()
        {
            Log.Debug("ProcedureMainBusiness.InitResFinishCallback");
            Utils.LogAssemblyInfo();

            var loadAssemblies = false;
            
#if ENABLE_HUATUO
            loadAssemblies = true;
#endif
#if UNITY_EDITOR
            var huatuoEditorConfigPath = "Assets/Settings/GFConfigs/HuatuoSettings.txt";
            var configKey = "UnityGameFramework.HuatuoSettings.HuatuoEditorMode";
            if (File.Exists(huatuoEditorConfigPath))
            {
                // Debug.Log($"Huatuo config file [{HuatuoDefine.EditorConfigPath}] found.");
                using (TOMLParser parser = new TOMLParser(File.OpenText(huatuoEditorConfigPath)))
                {
                    IEnumerable<TomlSyntaxException> errors;
                    if (parser.TryParse(out TomlNode rootNode, out errors))
                    {
                        var node = rootNode.FindNode(configKey);
                        UnityEngine.Debug.Log($"{configKey}: [ {node} ]");
                        if (null != node && null != node.AsBoolean) loadAssemblies &= !node.AsBoolean.Value;
                    }
                }
            }
#endif
            if (!loadAssemblies)
            {
                Log.Info("Skip load assemblies.");
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Compare(HotfixDefine.LogicEntranceDllName, $"{asm.GetName().Name}.dll",
                            StringComparison.Ordinal) == 0)
                    {
                        _mainBusinessLogicAsm = asm;
                        break;
                    }
                }
                AllAsmLoadComplete();
                return;
            }
            _state = EAsmLoadState.Loading;
            _callbacks ??= new LoadAssetCallbacks(LoadAssetSuccess, LoadAssetFailure);
            foreach (var hotUpdateDllName in HotfixDefine.AllHotUpdateDllNames)
            {
                var assetPath = Utility.Path.GetRegularPath(Path.Combine(HotfixDefine.AssemblyTextAssetResPath, $"{hotUpdateDllName}.{HotfixDefine.AssemblyTextAssetExtension}"));
                Log.Debug($"LoadAsset: [ {assetPath} ]");
                _loadingCnt++;
                UGFIF.Resource.LoadAsset(assetPath, _callbacks, hotUpdateDllName);
            }

            _state = EAsmLoadState.WaitingResult;
            if (0 == _loadingCnt) AllAsmLoadComplete();
        }

        private void LoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            _loadingCnt--;
            Log.Debug($"LoadAssetSuccess, assetName: [ {assetName} ], duration: [ {duration} ], userData: [ {userData} ]");
            var textAsset = asset as TextAsset;
            if (null == textAsset)
            {
                Log.Debug($"Load text asset [ {assetName} ] failed.");
                return;
            }

            try
            {
                var asm = System.Reflection.Assembly.Load(textAsset.bytes);
                if (string.Compare(HotfixDefine.LogicEntranceDllName, userData as string, StringComparison.Ordinal) ==
                    0)
                    _mainBusinessLogicAsm = asm;
                Log.Debug($"Assembly [ {asm.GetName().Name} ] loaded");
                Utils.LogAssemblyInfo();
            }
            catch (Exception e)
            {
                _failureCnt++;
                Log.Fatal(e);
                throw;
            }
            finally
            {
                Log.Debug($"_state: [ {_state} ], _loadingCnt: [ {_loadingCnt} ]");
                if (EAsmLoadState.WaitingResult == _state && 0 == _loadingCnt)
                {
                    AllAsmLoadComplete();
                }
            }
        }

        private void LoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Warning($"LoadAssetFailure, assetName: [ {assetName} ], status: [ {status} ], errorMessage: [ {errorMessage} ], userData: [ {userData} ]");
            _loadingCnt--;
            _failureCnt++;
        }

        private void AllAsmLoadComplete()
        {
            _state = EAsmLoadState.Finish;
            Log.Debug($"All assemblies load complete. failure cnt: [ {_failureCnt} ], loading duration: [ {UGFIF.Procedure.CurrentProcedureTime} ]");
            Utils.LogAssemblyInfo();
            RunMain();
        }
        
        public void RunMain()
        {
            if (null == _mainBusinessLogicAsm)
            {
                Log.Fatal("Main business logic assembly missing.");
                return;
            }
            var appType = _mainBusinessLogicAsm.GetType("HotfixMain.App");
            if (null == appType)
            {
                Log.Fatal("Main business logic type 'App' missing.");
                return;
            }
            var entryMethod = appType.GetMethod("Entry");
            if (null == entryMethod)
            {
                Log.Fatal("Main business logic entry method 'Entry' missing.");
                return;
            }
            entryMethod.Invoke(null, null);
        }
    }
}