using Global;
using GameFramework;
using UnityGameFramework.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Base
{
    public static class Utils
    {
        public static void LogAssemblyInfo()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            int unityAsmCnt = 0;
            int systemAsmCnt = 0;
            int otherAsmCnt = 0;
            foreach (var asm in asms)
            {
                if (asm.ToString().StartsWith("Unity")) unityAsmCnt++;
                else if (asm.ToString().StartsWith("System")) systemAsmCnt++;
                else
                {
                    var types = asm.GetTypes();
                    otherAsmCnt++;
                    if (HotfixDefine.AllHotUpdateDllNames.Contains($"{asm.GetName().Name}.dll"))
                    {
                        Log.Debug($"asm: [ {asm.GetName().Name} ]");
                        Log.Debug($"typesCnt: [ {types.Length} ]");
                        foreach (var type in types)
                        {
                            Log.Debug($"    type: [ {type} ]");
                        }
                        if (types.Length == 0)
                            Log.Warning("no type in assembly.");
                    }
                }
            }
            Log.Debug($"Unity asm cnt: [ {unityAsmCnt} ]");
            Log.Debug($"System asm cnt: [ {systemAsmCnt} ]");
            Log.Debug($"Other asm cnt: [ {otherAsmCnt} ]");
        }

        public static string[] GetTypeNames(System.Type typeBase, string assemblyName)
        {
            List<string> typeNames = new List<string>();
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch
            {
                return null;
            }

            if (assembly == null)
            {
                return null;
            }

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                {
                    typeNames.Add(type.FullName);
                }
            }
            typeNames.Sort();
            return typeNames.ToArray();
        }
    }
}