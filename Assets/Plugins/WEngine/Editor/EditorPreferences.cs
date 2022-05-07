using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using WEngine;


namespace WEditor {
    public static class EditorPreferences {
        private static SettingsEditor settingsEditor;

        [InitializeMethodOnEditorStartup(100)]
        private static void HookPreferences() {
            CallbackEditor.OnHierarchyWindowChanged += OnHierarchyWindowChanged;
        }

        private static void OnHierarchyWindowChanged() {
            if(!LoadEditorResources()) return;

            // Disable unique object name on hierarchy gameObject duplicate
            if(settingsEditor.IsObjUniqueNameOnDuplicate) return;

            GameObject[] gos = Selection.gameObjects;
            if(gos != null) {
                for(int i = 0; i < gos.Length; ++i) {
                    GameObject go = gos[i];
                    if(go.scene.name != "Null") {
                        int index = go.name.IndexOf('(');
                        if(index > 0) {
                            go.name = go.name.Remove(index);
                        }
                    }
                }
            }
        }

        private static bool LoadEditorResources() {
            if(!WEditorResources.Global) {
                Debug.Log("no editor resources");
                return false;
            }

            settingsEditor = WEditorResources.GetSettingsEditor();
            return settingsEditor != null;
        }
    }
}