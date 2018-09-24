using UnityEngine;
using Sirenix.OdinInspector;


public class UiRepositioner : MonoBehaviour {
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RepositionAt repositionAt;
    [SerializeField] private Vector3 position;

    [Button("Get Current Position")]
    private void GetCurrentPosition() {
        position = rectTransform.localPosition;
    }

    private void RepositionUi(Vector3 position) {
        rectTransform.localPosition = position;
    }

    private void Awake() {
        if(repositionAt == RepositionAt.Awake) RepositionUi(position);
    }

    private void OnEnable() {
        if(repositionAt == RepositionAt.OnEnable) RepositionUi(position);
    }

    private void Start() {
        if(repositionAt == RepositionAt.Start) RepositionUi(position);
    }


    public enum RepositionAt {
        Awake,
        OnEnable,
        Start
    }
}
