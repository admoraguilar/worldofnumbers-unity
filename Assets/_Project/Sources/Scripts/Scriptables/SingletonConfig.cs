using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;


[CreateAssetMenu(menuName = "Configs/Singleton")]
public class SingletonConfig : SerializedScriptableObject {
    public Dictionary<Type, ScriptableObject> Singletons = new Dictionary<Type, ScriptableObject>();
}


public class Singleton {
    private static Dictionary<Type, ScriptableObject> singletons = new Dictionary<Type, ScriptableObject>();


    public static T Get<T>() where T : ScriptableObject {
        return singletons.Count == 0 ? Init<T>() : (T)singletons[typeof(T)];
    }

    private static T Init<T>() where T : ScriptableObject {
        SingletonConfig config = Resources.LoadAll<SingletonConfig>("")[0];

        foreach(var singleton in config.Singletons) {
            singletons[singleton.Key] = singleton.Value;
        }

        return (T)singletons[typeof(T)];
    }
}