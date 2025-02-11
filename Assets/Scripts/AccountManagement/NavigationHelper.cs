﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NavigationHelper : MonoBehaviour {
    private EventSystem system;

    private void Start() {
        system = EventSystem.current;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)
            && system.currentSelectedGameObject != null
            && system.currentSelectedGameObject.GetComponent<Selectable>() != null) {
            Selectable next = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ?
                system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() :
                system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null) {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null) {
                    inputfield.OnPointerClick(new PointerEventData(system));
                }

                system.SetSelectedGameObject(next.gameObject);
            }

            //Here is the navigating back part:
            else {
                next = Selectable.allSelectables[0];
                system.SetSelectedGameObject(next.gameObject);
            }

        } 
    }
}