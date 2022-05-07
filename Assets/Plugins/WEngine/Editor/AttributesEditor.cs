using UnityEngine;
using System;


namespace WEditor {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InitializeMethodOnEditorStartupAttribute : Attribute {
        public InitializeMethodOnEditorStartupAttribute(int order = 1000) {
            Order = order;
        }

        public readonly int Order;
    }
}