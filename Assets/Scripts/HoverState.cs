using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverState : MonoBehaviour, ISelectHandler, IDeselectHandler  {

    [SerializeField] private GameObject go_backing;
    [SerializeField] private Text txt_button;

    private Color col_original;

    void Awake() {
        col_original = txt_button.color;
    }

    public void OnSelect(BaseEventData eventData) {
        go_backing.SetActive(true);
        txt_button.color = new Color(255,231,0);
    }
    
    public void OnDeselect(BaseEventData eventData) {
        go_backing.SetActive(false);
        txt_button.color = col_original;
    }

    public void OnDisable() {
        go_backing.SetActive(false);
        txt_button.color = col_original;
    }

}
