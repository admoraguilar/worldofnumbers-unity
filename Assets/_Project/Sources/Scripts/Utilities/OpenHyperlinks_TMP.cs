using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


// somewhat based upon the TextMesh Pro example script: TMP_TextSelector_B
public class OpenHyperlinks_TMP : MonoBehaviour, IPointerClickHandler {
    private TextMeshProUGUI pTextMeshPro;
    private Canvas pCanvas;
    private Camera pCamera;


    protected virtual void Awake() {
        pTextMeshPro = GetComponent<TextMeshProUGUI>();
        pCanvas = GetComponentInParent<Canvas>();

        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if(pCanvas.renderMode == RenderMode.ScreenSpaceOverlay) pCamera = null;
        else pCamera = pCanvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData) {
        // Debug.Log("Click at POS: " + eventData.position + "  World POS: " + eventData.worldPosition);

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, pCamera);
        if(linkIndex != -1) { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            // Debug.Log(string.Format("id: {0}, text: {1}", linkInfo.GetLinkID(), linkInfo.GetLinkText()));
            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
