using UnityEngine;
using System;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName = "Scriptables/ObjectSet")]
public class ObjectSetData : ScriptableObject {
    public UnityEngine.Object[] Objects;
}
