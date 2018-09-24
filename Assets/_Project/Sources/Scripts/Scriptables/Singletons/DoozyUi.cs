using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using DoozyUI;
using RSG.Scene.Query;




[CreateAssetMenu(menuName = "Scriptables/Singleton/DoozyUi")]
public class DoozyUi : AUiScriptable {
    private SceneTraversal sceneTraversal = new SceneTraversal();


    public override List<T> GetUi<T>(string uiName, string uiCategory = "Uncategorized") {
        return UIManager.GetUiElements(uiName, uiCategory).Cast<T>().ToList();
    }

    public override void SetUi(string uiName, string uiCategory = "Uncategorized", bool instantAction = false) {
        ClearUi();
        PushUi(uiName, uiCategory, instantAction);
    }

    public override void SetUiHierarchy(string uiName, string uiCategory = "Uncategorized", bool instantAction = false) {
        List<UIElement> uis = UIManager.GetUiElements(uiName, uiCategory).ToList();
        List<UIElement> uisToShow = new List<UIElement>();
        for(int i = 0; i < uis.Count; ++i) {
            IEnumerable<GameObject> ancestors = sceneTraversal.Ancestors(uis[i].gameObject);
            foreach(GameObject go in ancestors) {
                UIElement element = go.GetComponent<UIElement>();
                if(element) uisToShow.Add(element);
            }
            uisToShow.Add(uis[i]);
        }
        IEnumerable<UIElement> uisToHide = UIManager.GetVisibleUIElements().Except(uisToShow);

        foreach(UIElement ui in uisToHide) {
            PopUi(ui.elementName, ui.elementCategory, instantAction);
        }
        foreach(UIElement ui in uisToShow) {
            PushUi(ui.elementName, ui.elementCategory, instantAction);
        }
    }

    public override void PushUi(string uiName, string uiCategory = "Uncategorized", bool instantAction = false) {
        // Don't push already visible ui elements
        var uiElements = UIManager.GetUiElements(uiName, uiCategory).ToArray();
        foreach(var uiElement in uiElements) {
            if(uiElement.isVisible) continue;
            uiElement.Show(instantAction);
            _OnPushUi(uiName, uiCategory);
        }
    }

    public override void PushUiHierarchy(string uiName, string uiCategory = "Uncategorized", bool instantAction = false) {
        List<UIElement> uis = UIManager.GetUiElements(uiName, uiCategory).ToList();
        List<UIElement> uisToPush = new List<UIElement>();
        for(int i = 0; i < uis.Count; ++i) {
            IEnumerable<GameObject> ancestors = sceneTraversal.Ancestors(uis[i].gameObject);
            foreach(GameObject go in ancestors) {
                UIElement element = go.GetComponent<UIElement>();
                if(element) uisToPush.Add(element);
            }
            uisToPush.Add(uis[i]);
        }

        foreach(UIElement ui in uisToPush) {
            PushUi(ui.elementName, ui.elementCategory, instantAction);
        }
    }

    public override void PopUi(string uiName, string uiCategory = "Uncategorized", bool instantAction = false) {
        // Don't pop already hidden ui elements
        var uiElements = UIManager.GetUiElements(uiName, uiCategory).ToArray();
        foreach(var uiElement in uiElements) {
            if(!uiElement.isVisible || uiElement.CompareTag("ManualControlUi")) continue;
            uiElement.Hide(instantAction);
            _OnPopUi(uiName, uiCategory);
        }
    }

    public override void PopUiHierarchy(string uiName, string uiCategory = "Uncategorized", bool instantAction = false) {
        List<UIElement> uis = UIManager.GetUiElements(uiName, uiCategory).ToList();
        List<UIElement> uisToHide = new List<UIElement>();
        for(int i = 0; i < uis.Count; ++i) {
            IEnumerable<GameObject> descendants = sceneTraversal.Descendents(uis[i].gameObject);
            foreach(GameObject go in descendants) {
                UIElement element = go.GetComponent<UIElement>();
                if(element) uisToHide.Add(element);
            }
            uisToHide.Add(uis[i]);
        }

        foreach(UIElement ui in uisToHide) PopUi(ui.elementName, ui.elementCategory, instantAction);
    }

    public override void PushLastUi() {
        UIManager.BackButtonEvent();
    }

    public override void ClearUi(bool instantAction = false) {
        List<UIElement> uiElements = UIManager.GetVisibleUIElements();
        foreach(UIElement uiElement in uiElements) {
            if(!uiElement.isVisible || uiElement.CompareTag("ManualControlUi")) continue;
            uiElement.Hide(instantAction);
        }
    }
}


public abstract class AUiScriptable : CustomScriptableObject {
    public event Action<string, string> OnPushUi = delegate { };
    public event Action<string, string> OnPopUi = delegate { };

    protected void _OnPushUi(string uiName, string uiCategory) { OnPushUi(uiName, uiCategory); }
    protected void _OnPopUi(string uiName, string uiCategory) { OnPopUi(uiName, uiCategory); }

    abstract public List<T> GetUi<T>(string uiName, string uiCategory = "Uncategorized");
    abstract public void SetUi(string uiName, string uiCategory = "Uncategorized", bool instantAction = false);

    /// <summary>
    /// Same as SetUi but also activates parent UiElements on top of the indicated UiElements.
    /// </summary>
    /// <param name="uiName"></param>
    /// <param name="uiCategory"></param>
    /// <param name="instantAction"></param>
    abstract public void SetUiHierarchy(string uiName, string uiCategory = "Uncategorized", bool instantAction = false);
    abstract public void PushUi(string uiName, string uiCategory = "Uncategorized", bool instantAction = false);

    /// <summary>
    /// Same as PushUi but also activates parent UiElements on top of the indicated UiElements.
    /// </summary>
    /// <param name="uiName"></param>
    /// <param name="uiCategory"></param>
    /// <param name="instantAction"></param>
    abstract public void PushUiHierarchy(string uiName, string uiCategory = "Uncategorized", bool instantAction = false);
    abstract public void PopUi(string uiName, string uiCategory = "Uncategorized", bool instantAction = false);

    /// <summary>
    /// Same as PopUi but also deactivates parent UiElements on top of the indicated UiElements.
    /// </summary>
    /// <param name="uiName"></param>
    /// <param name="uiCategory"></param>
    /// <param name="instantAction"></param>
    abstract public void PopUiHierarchy(string uiName, string uiCategory = "Uncategorized", bool instantAction = false);

    /// <summary>
    /// Pushes the last activated Ui.
    /// </summary>
    abstract public void PushLastUi();
    abstract public void ClearUi(bool instantAction = false);
}
