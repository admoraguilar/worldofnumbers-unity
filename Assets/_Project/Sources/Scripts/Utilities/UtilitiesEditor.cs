using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class UtilitiesEditor {
    public static void SetExecutionOrder(Type type, int order) {
#if UNITY_EDITOR
        // Get the name of the script we want to change it's execution order
        string scriptName = type.Name;

        // Iterate through all scripts (Might be a better way to do this?)
        foreach(MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
            // If found our script
            if(monoScript.name == scriptName) {
                // And it's not at the execution time we want already
                // (Without this we will get stuck in an infinite loop)
                if(MonoImporter.GetExecutionOrder(monoScript) != order) {
                    MonoImporter.SetExecutionOrder(monoScript, order);
                }
                break;
            }
        }
#endif
    }
}
