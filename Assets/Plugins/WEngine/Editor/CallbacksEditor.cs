using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


namespace WEditor {
    // TODO:
    // Reset callback hooks on delegate invocation add.
    public static class CallbackEditor {
        public static event EditorApplication.CallbackFunction StartupEditor = delegate { };

        public static event EditorApplication.CallbackFunction OnPlayModeChanged = delegate { };
        public static event Action<PlayModeStateChange> OnPlayModeStateChangedParams = delegate { };

        public static event EditorApplication.CallbackFunction OnEditorUpdate = delegate { };

        public static event Action OnHierarchyWindowChanged = delegate { };
        public static event EditorApplication.HierarchyWindowItemCallback OnHierarchyWindowItemOnGUI = delegate { };

        public static event Action OnSelectionChanged = delegate { };


        // Called on editor startup
        [InitializeOnLoadMethod()]
        private static void OnEditorStartup() {
            // Do some cool stuff on editor startup

            // Go thru methods with [InitializeMethodOnEditorStartupAttribute]
            List<MethodInfo> editorStartupMethods = new List<MethodInfo>();

            //// Get all the methods with the attribute in the assembly
            Assembly assembly = Assembly.GetAssembly(typeof(CallbackEditor));
            Type[] assemblyTypes = assembly.GetTypes();
            foreach(Type type in assemblyTypes) {
                editorStartupMethods.AddRange(type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(
                    m => Attribute.GetCustomAttribute(m, typeof(InitializeMethodOnEditorStartupAttribute)) != null
                ));
            }

            editorStartupMethods.Sort((MethodInfo lhs, MethodInfo rhs) => {
                InitializeMethodOnEditorStartupAttribute lhsAttr = (InitializeMethodOnEditorStartupAttribute)Attribute.GetCustomAttribute(lhs, typeof(InitializeMethodOnEditorStartupAttribute));
                InitializeMethodOnEditorStartupAttribute rhsAttr = (InitializeMethodOnEditorStartupAttribute)Attribute.GetCustomAttribute(rhs, typeof(InitializeMethodOnEditorStartupAttribute));

                return lhsAttr.Order < rhsAttr.Order ? -1 : lhsAttr.Order > rhsAttr.Order ? 1 : 0;
            });

            foreach(MethodInfo method in editorStartupMethods) {
                method.Invoke(null, null);
            }

            // Hook callbacks
            EditorApplication.playModeStateChanged += (PlayModeStateChange state) => {
                OnPlayModeChanged();
                OnPlayModeStateChangedParams(state);
            };

            EditorApplication.update += OnEditorUpdate;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            Selection.selectionChanged += OnSelectionChanged;

            StartupEditor();

            Debug.Log("Editor callbacks successfully hooked");
        }
    } 
}