using System;
using System.Xml;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFrameWork.Editor
{
    [InitializeOnLoad]
    public static class XMLConfigHelper
    {
        private const string CONFIG_DIR = "Assets/Settings/GFConfigs";
        private const string ROOT_NODE_NAME = "UnityGameFramework";
        [BuildSettingsConfigPath]
        public static string BuildSettingsConfigPath => $"{CONFIG_DIR}/BuildSettings.xml";
        
        [ResourceBuilderConfigPath]
        public static string ResourceBuilderConfigPath => $"{CONFIG_DIR}/ResourceBuilder.xml";
        
        [ResourceCollectionConfigPath]
        public static string ResourceCollectionConfigPath => $"{CONFIG_DIR}/ResourceCollection.xml";
        
        [ResourceEditorConfigPath]
        public static string ResourceEditorConfigPath => $"{CONFIG_DIR}/ResourceEditor.xml";
        
        // xml template:
        // <ROOT_NODE_NAME>
        //     <ConfigName>
        //         <SettingName>SettingValue</SettingName>
        //     </ConfigName>
        // </ROOT_NODE_NAME>
            
        /// <summary>
        /// read xml config in simple format.
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>true if read success</returns>
        public static bool TryGetXMLSetting<T>(string configName, string settingName, out T value) where T : struct
        {
            return InternalTryGetXMLSetting(configName, settingName, out value);
        }
        
        /// <summary>
        /// read xml config in simple format.
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <returns>true if read success</returns>
        public static bool TryGetXMLSetting(string configName, string settingName, out string value)
        {
            return InternalTryGetXMLSetting(configName, settingName, out value);
        }

        private static bool InternalTryGetXMLSetting<T>(string configName, string settingName, out T value)
        {
            try
            {
                value = default;
                var xmlFilePath = $"{CONFIG_DIR}/{configName}.xml";
                if (!File.Exists(xmlFilePath))
                {
                    return false;
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlFilePath);
                var xmlRoot = xmlDocument.SelectSingleNode(ROOT_NODE_NAME);
                if (null == xmlRoot) return false;
                var xmlConfig = xmlRoot.SelectSingleNode(configName);
                if (null == xmlConfig) return false;
                var xmlSetting = xmlConfig.SelectSingleNode(settingName);
                if (null == xmlSetting) return false;
                value = (T) Convert.ChangeType(xmlSetting.InnerText, typeof(T));
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        /// <summary>
        /// write xml config in simple format.
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>true if write success</returns>
        public static bool TrySetXMLSetting<T>(string configName, string settingName, T value) where T : struct
        {
            return InternalTrySetXMLSetting(configName, settingName, value);
        }
        
        /// <summary>
        /// write xml config in simple format.
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>true if write success</returns>
        public static bool TrySetXMLSetting(string configName, string settingName, string value)
        {
            return InternalTrySetXMLSetting(configName, settingName, value);
        }
        
        private static bool InternalTrySetXMLSetting<T>(string configName, string settingName, T value)
        {
            try
            {
                var xmlFilePath = $"{CONFIG_DIR}/{configName}.xml";
                if (!File.Exists(xmlFilePath))
                {
                    return false;
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlFilePath);
                var xmlRoot = xmlDocument.SelectSingleNode(ROOT_NODE_NAME);
                if (null == xmlRoot)
                {
                    xmlRoot = xmlDocument.CreateElement(ROOT_NODE_NAME);
                    xmlDocument.AppendChild(xmlRoot);
                }

                var xmlConfig = xmlRoot.SelectSingleNode(configName);
                if (null == xmlConfig)
                {
                    xmlConfig = xmlDocument.CreateElement(configName);
                    xmlRoot.AppendChild(xmlConfig);
                }

                var xmlSetting = xmlConfig.SelectSingleNode(settingName);
                if (null == xmlSetting)
                {
                    xmlSetting = xmlDocument.CreateElement(settingName);
                    xmlSetting.InnerText = value.ToString();
                    xmlConfig.AppendChild(xmlSetting);
                }
                else
                {
                    xmlSetting.InnerText = value.ToString();
                }

                xmlDocument.Save(xmlFilePath);
                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
}