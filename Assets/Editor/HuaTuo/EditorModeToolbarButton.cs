using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Tommy;
using Tommy.Extensions;
using UnityToolbarExtender;

[InitializeOnLoad]
public static class EditorModeToolbarButton
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
                fixedHeight = 20
            };
        }
    }

    private static readonly string HuatuoEditorConfigPath = "Assets/Settings/GFConfigs/HuatuoSettings.txt"; 
    static bool m_enabled;

    static bool Enabled
    {
        get { return m_enabled; }
        set
        {
            if (m_enabled == value) return;
            m_enabled = value;
            SaveHuatuoSettings();
        }
    }
    
    static EditorModeToolbarButton()
    {
        m_enabled = GetHuatuoSetting();
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
    }
    static void OnToolbarGUI()
    {
        GUI.changed = false;
        var info = string.Format("Huatuo editor mode {0}", Enabled ? "On" : "Off");
        GUILayout.Toggle(m_enabled,
            new GUIContent(null, EditorGUIUtility.IconContent(@"tree_icon_leaf").image, info),
            ToolbarStyles.commandButtonStyle);
        if (GUI.changed)
        {
            Enabled = !Enabled;
            Debug.Log(info);
        }
    }
    private static bool GetHuatuoSetting()
    {
        if (File.Exists(HuatuoEditorConfigPath))
        {
            // Debug.Log($"Huatuo config file [{HuatuoDefine.EditorConfigPath}] found.");
            using (TOMLParser parser = new TOMLParser(File.OpenText(HuatuoEditorConfigPath)))
            {
                IEnumerable<TomlSyntaxException> errors;
                if (parser.TryParse(out TomlNode rootNode, out errors))
                {
                    var node = rootNode.FindNode("UnityGameFramework.HuatuoSettings.HuatuoEditorMode");
                    if (null != node && null != node.AsBoolean) return node.AsBoolean.Value;
                    return false;
                }
            }
        }
        else
        {
            Debug.LogWarning($"Huatuo config file [{HuatuoEditorConfigPath}] not found.");
        }
    
        return false;
    }
    private static void SaveHuatuoSettings()
    {
        TomlNode rootNode = null;
        bool initialized = false;
        if (File.Exists(HuatuoEditorConfigPath))
        {
            using (TOMLParser parser = new TOMLParser(File.OpenText(HuatuoEditorConfigPath)))
            {
                IEnumerable<TomlSyntaxException> errors;
                if (parser.TryParse(out rootNode, out errors))
                {
                    initialized = true;
                }
                else
                {
                    UnityEngine.Debug.LogError("TryParse HuatuoSettings failed, errors:");
                    foreach (var error in errors)
                    {
                        UnityEngine.Debug.LogError(error);
                    }
                }
            }
        }
        if (!initialized)
        {
            rootNode = new TomlTable()
            {
                            
                ["UnityGameFramework"] =
                {
                    ["HuatuoSettings"] =
                    {
                        ["DynamicAssemblyPlatforms"] = new TomlInteger
                        {
                            Value = 0,
                            Comment = "BuildTargetGroups use Huatuo hotfix feature."
                        },
                        ["HuatuoEditorMode"] = new TomlBoolean
                        {
                            Value = false,
                            Comment = "Run as mono in editor mode if huatuo enabled on current build target."
                        }
                    }
                }
            };
        }
        rootNode["UnityGameFramework"]["HuatuoSettings"]["HuatuoEditorMode"].MergeWith(
            new TomlBoolean{Value = m_enabled});
        
        TOML.ForceASCII = true;
        using (StreamWriter writer = File.CreateText(HuatuoEditorConfigPath))
        {
            rootNode.WriteTo(writer);
            // Remember to flush the data if needed!
            writer.Flush();
            AssetDatabase.Refresh();
        }
    }
}
