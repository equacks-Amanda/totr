using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class HelpMenuController : MonoBehaviour {

    [SerializeField] GameObject[] go_menuArray = new GameObject[3];
    [SerializeField] Button[] butt_selectArray = new Button[3];
    [SerializeField] GameObject go_helpMenu;
    [SerializeField] MenuController mc_main;

    Player p_uiPlayer;
    int currentMenu;

    private void Start() {
        p_uiPlayer = ReInput.players.GetPlayer(0);
        currentMenu = 0;
        MenuSwitch(currentMenu);
    }

    private void FixedUpdate() {
        if (go_helpMenu.activeSelf) {
            if (p_uiPlayer.GetButtonDown("UIPageRight")) {
                MenuSwitch(++currentMenu);
            } else if (p_uiPlayer.GetButtonDown("UIPageLeft")) {
                MenuSwitch(--currentMenu);
            } else if (p_uiPlayer.GetButtonDown("UICancel")) {
                mc_main.CloseHelp();
            }
        }
    }

    // Show the proper menu on click
    public void MenuSwitch(int which) {
        //Make Menus Loop.
        if (which > 2) {
            which = 0;
            currentMenu = 0;
        } else if (which < 0) {
            which = 2;
            currentMenu = 2;
        }

        for (int i = 0; i < 3; i++) {
            if (i == which) {
                go_menuArray[i].SetActive(true);
                if (go_helpMenu.activeSelf) {
                    butt_selectArray[i].Select();
                    butt_selectArray[i].OnSelect(null);
                }
            } else {
                go_menuArray[i].SetActive(false);
            }
        }
    }
}
