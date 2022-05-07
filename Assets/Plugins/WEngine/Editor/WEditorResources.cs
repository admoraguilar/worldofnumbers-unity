using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WEditor {
    [CreateAssetMenu(fileName = "WEditorResources", menuName = "WEngine/Settings/WEditor Resource")]
    public class WEditorResources : ScriptableObject {
        public static WEditorResources Global {
            get {
                if(global == null) {
                    global = Resources.Load<WEditorResources>("WEditorResources");
                }
                return global;
            }
        }

        private static WEditorResources global;

        public SettingsWrapper Settings;
        public EditorWrapper Editor;

        #region Settings

        public static SettingsEditor GetSettingsEditor() { return Global.Settings.Editor; }

        #endregion

        #region Editor


        #endregion


        [Serializable]
        public class SettingsWrapper {
            public SettingsEditor Editor;
        }

        [Serializable]
        public class EditorWrapper {

        }
    }
}
