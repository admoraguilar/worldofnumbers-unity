using UnityEditor;
using UnityEngine;

namespace FLGCoreEditor.Utilities {
    public class FindMissingScriptsRecursivelyAndRemove : EditorWindow {
        private static int _goCount;
        private static int _componentsCount;
        private static int _missingCount;

        private static bool _bHaveRun;

        [MenuItem("FLGCore/Editor/Utility/FindMissingScriptsRecursivelyAndRemove")]
        public static void ShowWindow() {
            GetWindow(typeof(FindMissingScriptsRecursivelyAndRemove));
        }

        public void OnGUI() {
            if(GUILayout.Button("Find Missing Scripts in selected GameObjects")) {
                FindInSelected();
            }

            if(!_bHaveRun)
                return;

            EditorGUILayout.TextField(string.Format("{0} GameObjects Selected", _goCount));
            if(_goCount > 0)
                EditorGUILayout.TextField(string.Format("{0} components", _componentsCount));
            if(_goCount > 0)
                EditorGUILayout.TextField(string.Format("{0} deleted", _missingCount));
        }

        private static void FindInSelected() {
            var go = Selection.gameObjects;
            _goCount = 0;
            _componentsCount = 0;
            _missingCount = 0;
            foreach(var g in go) {
                FindInGo(g);
            }

            _bHaveRun = true;
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", _goCount, _componentsCount, _missingCount));

            AssetDatabase.SaveAssets();
        }

        private static void FindInGo(GameObject g) {
            _goCount++;
            var components = g.GetComponents<Component>();

            var r = 0;

            for(var i = 0; i < components.Length; i++) {
                _componentsCount++;
                if(components[i] != null)
                    continue;
                _missingCount++;
                var s = g.name;
                var t = g.transform;
                while(t.parent != null) {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }

                Debug.Log(string.Format("{0} has a missing at script {1}", s, i), g);

                var serializedObject = new SerializedObject(g);

                var prop = serializedObject.FindProperty("m_Component");

                prop.DeleteArrayElementAtIndex(i - r);
                r++;

                serializedObject.ApplyModifiedProperties();
            }

            foreach(Transform childT in g.transform) {
                FindInGo(childT.gameObject);
            }
        }
    }
}