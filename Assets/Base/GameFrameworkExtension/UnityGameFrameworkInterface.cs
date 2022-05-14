using UnityGameFramework.Runtime;

namespace UGFHelper
{
    public class UGFIF
    {
        /// <summary>
        /// 基础组件。
        /// </summary>
        public static BaseComponent Base => _base ??= GameEntry.GetComponent<BaseComponent>();
        private static BaseComponent _base;
        
        /// <summary>
        /// 配置组件。
        /// </summary>
        public static ConfigComponent Config => _config ??= GameEntry.GetComponent<ConfigComponent>();
        private static ConfigComponent _config;
        
        /// <summary>
        /// 数据结点组件。
        /// </summary>
        public static DataNodeComponent DataNode => _dataNode ??= GameEntry.GetComponent<DataNodeComponent>();
        private static DataNodeComponent _dataNode;
        
        /// <summary>
        /// 数据表组件。
        /// </summary>
        public static DataTableComponent DataTable => _dataTable ??= GameEntry.GetComponent<DataTableComponent>();
        private static DataTableComponent _dataTable;
        
        /// <summary>
        /// 获取调试组件。
        /// </summary>
        public static DebuggerComponent Debugger => _debugger ??= GameEntry.GetComponent<DebuggerComponent>();
        private static DebuggerComponent _debugger;

        /// <summary>
        /// 获取下载组件。
        /// </summary>
        public static DownloadComponent Download => _download ??= GameEntry.GetComponent<DownloadComponent>();
        private static DownloadComponent _download;

        /// <summary>
        /// 获取实体组件。
        /// </summary>
        public static EntityComponent Entity => _entity ??= GameEntry.GetComponent<EntityComponent>();
        private static EntityComponent _entity;

        /// <summary>
        /// 获取事件组件。
        /// </summary>
        public static EventComponent Event => _event ??= GameEntry.GetComponent<EventComponent>();
        private static EventComponent _event;

        /// <summary>
        /// 获取文件系统组件。
        /// </summary>
        public static FileSystemComponent FileSystem => _fileSystem ??= GameEntry.GetComponent<FileSystemComponent>();
        private static FileSystemComponent _fileSystem;

        /// <summary>
        /// 获取有限状态机组件。
        /// </summary>
        public static FsmComponent Fsm => _fsm ??= GameEntry.GetComponent<FsmComponent>();
        private static FsmComponent _fsm;

        /// <summary>
        /// 获取本地化组件。
        /// </summary>
        public static LocalizationComponent Localization => _localization ??= GameEntry.GetComponent<LocalizationComponent>();
        private static LocalizationComponent _localization;

        /// <summary>
        /// 获取网络组件。
        /// </summary>
        public static NetworkComponent Network => _network ??= GameEntry.GetComponent<NetworkComponent>();
        private static NetworkComponent _network;

        /// <summary>
        /// 获取对象池组件。
        /// </summary>
        public static ObjectPoolComponent ObjectPool => _objectPool ??= GameEntry.GetComponent<ObjectPoolComponent>();
        private static ObjectPoolComponent _objectPool;

        /// <summary>
        /// 获取流程组件。
        /// </summary>
        public static ProcedureComponent Procedure => _procedure ??= GameEntry.GetComponent<ProcedureComponent>();
        private static ProcedureComponent _procedure;

        /// <summary>
        /// 资源组件。
        /// </summary>
        public static ResourceComponent Resource => _resource ??= GameEntry.GetComponent<ResourceComponent>();
        private static ResourceComponent _resource;

        /// <summary>
        /// 获取场景组件。
        /// </summary>
        public static SceneComponent Scene => _scene ??= GameEntry.GetComponent<SceneComponent>();
        private static SceneComponent _scene;

        /// <summary>
        /// 获取配置组件。
        /// </summary>
        public static SettingComponent Setting => _setting ??= GameEntry.GetComponent<SettingComponent>();
        private static SettingComponent _setting;

        /// <summary>
        /// 获取声音组件。
        /// </summary>
        public static SoundComponent Sound => _sound ??= GameEntry.GetComponent<SoundComponent>();
        private static SoundComponent _sound;

        /// <summary>
        /// 获取界面组件。
        /// </summary>
        public static UIComponent UI => _ui ??= GameEntry.GetComponent<UIComponent>();
        private static UIComponent _ui;

        /// <summary>
        /// 获取网络组件。
        /// </summary>
        public static WebRequestComponent WebRequest => _webRequest ??= GameEntry.GetComponent<WebRequestComponent>();
        private static WebRequestComponent _webRequest;
    }
}