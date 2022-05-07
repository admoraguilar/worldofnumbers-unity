using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;


public static class EditorUtils {
    public static IEnumerable<FieldInfo> GetPublicConstants(Type type) {
        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Concat(type.GetNestedTypes(BindingFlags.Public).SelectMany(GetPublicConstants));
    }

    public static void SetScriptExecutionOrder(Type type, int order) {
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
    }
}
