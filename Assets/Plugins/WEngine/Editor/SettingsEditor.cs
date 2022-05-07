using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WEditor {
    [CreateAssetMenu(fileName = "NewSettingsEditor", menuName = "WEngine/Settings/Editor")]
    public class SettingsEditor : ScriptableObject {
        public bool IsObjUniqueNameOnDuplicate { get { return isObjUniqueNameOnDuplicate; } }

        [SerializeField] private bool isObjUniqueNameOnDuplicate;
    }
}