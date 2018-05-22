/*  Register Players - Zak Olyarnik
 * 
 *  Desc:   Connects controllers and allows player and team selection.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

public class RegisterPlayers : SceneLoader {

    [SerializeField] private Image go_backing1;
    [SerializeField] private Image go_backing2;
    [SerializeField] private Image go_backing3;
    [SerializeField] private Image go_backing4;
    [SerializeField] private Image go_hat1;
    [SerializeField] private Image go_hat2;
    [SerializeField] private Image go_hat3;
    [SerializeField] private Image go_hat4;
    [SerializeField] private Sprite img_red;
    [SerializeField] private Sprite img_blue;
    [SerializeField] private MeshRenderer[] mr_hats; //16 units
    [SerializeField] private Material[] mat_hatRed;
    [SerializeField] private Material[] mat_hatBlue;
    [SerializeField] private SkinnedMeshRenderer[] smr_outfits; //4 units
    [SerializeField] private Material mat_outfitRed;
    [SerializeField] private Material mat_outfitBlue;
    [SerializeField] private Text txt_p1Message;
    [SerializeField] private Text txt_p2Message;
    [SerializeField] private Text txt_p3Message;
    [SerializeField] private Text txt_p4Message;
    [SerializeField] private GameObject go_go;
    [SerializeField] private GameObject go_paramMenu;


    private Player p_player1, p_player2, p_player3, p_player4;
    private bool b_p1Connected = false, b_p2Connected = false, b_p3Connected = false, b_p4Connected = false;	// set when 4 controllers are detected
    private bool b_p1Ready = false, b_p2Ready = false, b_p3Ready = false, b_p4Ready = false;
    private Constants.Global.Color e_p1Color, e_p2Color, e_p3Color, e_p4Color;
    private int i_numRed = 0, i_numBlue = 0;
    private int i_p1Hat, i_p2Hat, i_p3Hat, i_p4Hat;

	/*/////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

    void Awake()
    {
        p_player1 = ReInput.players.GetPlayer(0);
        p_player2 = ReInput.players.GetPlayer(1);
        p_player3 = ReInput.players.GetPlayer(2);
        p_player4 = ReInput.players.GetPlayer(3);
    }

    void Start()
    {
        e_p1Color = Constants.Global.Color.RED;
        e_p2Color = Constants.Global.Color.RED;
        e_p3Color = Constants.Global.Color.BLUE;
        e_p4Color = Constants.Global.Color.BLUE;
        i_p1Hat = 0;
        i_p2Hat = 1;
        i_p3Hat = 2;
        i_p4Hat = 3;
    }

    void Update()
    {
        if (Input.GetKeyDown("space")) {
            LoadNextScene("WarmUp");
        }

        if (!go_paramMenu.activeSelf) {
            // test connection
            int connectedControllers = ReInput.controllers.GetControllerCount(ControllerType.Joystick);
            if (connectedControllers >= 1 && !b_p1Connected) {
                txt_p1Message.text = "CONNECTED";
                b_p1Connected = true;
                go_backing1.sprite = img_red;
                mr_hats[(4 * 0) + i_p1Hat].gameObject.SetActive(true);
                mr_hats[(4 * 0) + i_p1Hat].materials = new Material[] { mat_hatRed[i_p1Hat] };
                smr_outfits[0].materials = new Material[] { mat_outfitRed, mat_outfitRed };
            }
            if (connectedControllers >= 2 && !b_p2Connected) {
                txt_p2Message.text = "CONNECTED";
                b_p2Connected = true;
                go_backing2.sprite = img_red;
                mr_hats[(4 * 1) + i_p2Hat].gameObject.SetActive(true);
                mr_hats[(4 * 1) + i_p2Hat].materials = new Material[] { mat_hatRed[i_p2Hat] };
                smr_outfits[1].materials = new Material[] { mat_outfitRed, mat_outfitRed };
            }
            if (connectedControllers >= 3 && !b_p3Connected) {
                txt_p3Message.text = "CONNECTED";
                b_p3Connected = true;
                go_backing3.sprite = img_blue;
                mr_hats[(4 * 2) + i_p3Hat].gameObject.SetActive(true);
                mr_hats[(4 * 2) + i_p3Hat].materials = new Material[] { mat_hatBlue[i_p3Hat] };
                smr_outfits[2].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
            }
            if (connectedControllers >= 4 && !b_p4Connected) {
                txt_p4Message.text = "CONNECTED";
                b_p4Connected = true;
                go_backing4.sprite = img_blue;
                mr_hats[(4 * 3) + i_p4Hat].gameObject.SetActive(true);
                mr_hats[(4 * 3) + i_p4Hat].materials = new Material[] { mat_hatBlue[i_p4Hat] };
                smr_outfits[3].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
            }

            // switch colors
            if ((p_player1.GetButtonDown("UIPageLeft") || p_player1.GetButtonDown("UIPageRight")) && !b_p1Ready)    // AND NOT READY!!!!
            {
                if (go_backing1.sprite == img_red) {
                    go_backing1.sprite = img_blue;
                    e_p1Color = Constants.Global.Color.BLUE;
                    mr_hats[(4 * 0) + i_p1Hat].materials = new Material[] { mat_hatBlue[i_p1Hat] };
                    smr_outfits[0].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
                }
                else {
                    go_backing1.sprite = img_red;
                    e_p1Color = Constants.Global.Color.RED;
                    mr_hats[(4 * 0) + i_p1Hat].materials = new Material[] { mat_hatRed[i_p1Hat] };
                    smr_outfits[0].materials = new Material[] { mat_outfitRed, mat_outfitRed };
                }
            }
            if ((p_player2.GetButtonDown("UIPageLeft") || p_player2.GetButtonDown("UIPageRight")) && !b_p2Ready) {
                if (go_backing2.sprite == img_red) {
                    go_backing2.sprite = img_blue;
                    e_p2Color = Constants.Global.Color.BLUE;
                    mr_hats[(4 * 1) + i_p2Hat].materials = new Material[] { mat_hatBlue[i_p2Hat] };
                    smr_outfits[1].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
                }
                else {
                    go_backing2.sprite = img_red;
                    e_p2Color = Constants.Global.Color.RED;
                    mr_hats[(4 * 1) + i_p2Hat].materials = new Material[] { mat_hatRed[i_p2Hat] };
                    smr_outfits[1].materials = new Material[] { mat_outfitRed, mat_outfitRed };
                }
            }
            if ((p_player3.GetButtonDown("UIPageLeft") || p_player3.GetButtonDown("UIPageRight")) && !b_p3Ready) {
                if (go_backing3.sprite == img_red) {
                    go_backing3.sprite = img_blue;
                    e_p3Color = Constants.Global.Color.BLUE;
                    mr_hats[(4 * 2) + i_p3Hat].materials = new Material[] { mat_hatBlue[i_p3Hat] };
                    smr_outfits[2].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
                }
                else {
                    go_backing3.sprite = img_red;
                    e_p3Color = Constants.Global.Color.RED;
                    mr_hats[(4 * 2) + i_p3Hat].materials = new Material[] { mat_hatRed[i_p3Hat] };
                    smr_outfits[2].materials = new Material[] { mat_outfitRed, mat_outfitRed };
                }
            }
            if ((p_player4.GetButtonDown("UIPageLeft") || p_player4.GetButtonDown("UIPageRight")) && !b_p4Ready) {
                if (go_backing4.sprite == img_red) {
                    go_backing4.sprite = img_blue;
                    e_p4Color = Constants.Global.Color.BLUE;
                    mr_hats[(4 * 3) + i_p4Hat].materials = new Material[] { mat_hatBlue[i_p4Hat] };
                    smr_outfits[3].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
                }
                else {
                    go_backing4.sprite = img_red;
                    e_p4Color = Constants.Global.Color.RED;
                    mr_hats[(4 * 3) + i_p4Hat].materials = new Material[] { mat_hatRed[i_p4Hat] };
                    smr_outfits[3].materials = new Material[] { mat_outfitRed, mat_outfitRed };
                }
            }


            // switch hats               // AND NOT READY!!!!
            if ((p_player1.GetNegativeButtonDown("UIHorizontal") && !b_p1Ready)) {
                mr_hats[(4 * 0) + i_p1Hat].gameObject.SetActive(false);
                i_p1Hat--;
                if (i_p1Hat < 0) {
                    i_p1Hat = 3;
                }
                if (e_p1Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 0) + i_p1Hat].materials = new Material[] { mat_hatRed[i_p1Hat] };
                }
                else {
                    mr_hats[(4 * 0) + i_p1Hat].materials = new Material[] { mat_hatBlue[i_p1Hat] };
                }
                mr_hats[(4 * 0) + i_p1Hat].gameObject.SetActive(true);

            }
            if ((p_player1.GetButtonDown("UIHorizontal") && !b_p1Ready)) {
                mr_hats[(4 * 0) + i_p1Hat].gameObject.SetActive(false);
                i_p1Hat++;
                if (i_p1Hat > 3) {
                    i_p1Hat = 0;
                }
                if (e_p1Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 0) + i_p1Hat].materials = new Material[] { mat_hatRed[i_p1Hat] };
                }
                else {
                    mr_hats[(4 * 0) + i_p1Hat].materials = new Material[] { mat_hatBlue[i_p1Hat] };
                }
                mr_hats[(4 * 0) + i_p1Hat].gameObject.SetActive(true);
            }
            if ((p_player2.GetNegativeButtonDown("UIHorizontal") && !b_p2Ready)) {
                mr_hats[(4 * 1) + i_p2Hat].gameObject.SetActive(false);
                i_p2Hat--;
                if (i_p2Hat < 0) {
                    i_p2Hat = 3;
                }
                if (e_p2Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 1) + i_p2Hat].materials = new Material[] { mat_hatRed[i_p2Hat] };
                }
                else {
                    mr_hats[(4 * 1) + i_p2Hat].materials = new Material[] { mat_hatBlue[i_p2Hat] };
                }
                mr_hats[(4 * 1) + i_p2Hat].gameObject.SetActive(true);
            }
            if ((p_player2.GetButtonDown("UIHorizontal") && !b_p2Ready)) {
                mr_hats[(4 * 1) + i_p2Hat].gameObject.SetActive(false);
                i_p2Hat++;
                if (i_p2Hat > 3) {
                    i_p2Hat = 0;
                }
                if (e_p2Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 1) + i_p2Hat].materials = new Material[] { mat_hatRed[i_p2Hat] };
                }
                else {
                    mr_hats[(4 * 1) + i_p2Hat].materials = new Material[] { mat_hatBlue[i_p2Hat] };
                }
                mr_hats[(4 * 1) + i_p2Hat].gameObject.SetActive(true);
            }
            if ((p_player3.GetNegativeButtonDown("UIHorizontal") && !b_p3Ready)) {
                mr_hats[(4 * 2) + i_p3Hat].gameObject.SetActive(false);
                i_p3Hat--;
                if (i_p3Hat < 0) {
                    i_p3Hat = 3;
                }
                if (e_p3Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 2) + i_p3Hat].materials = new Material[] { mat_hatRed[i_p3Hat] };
                }
                else {
                    mr_hats[(4 * 2) + i_p3Hat].materials = new Material[] { mat_hatBlue[i_p3Hat] };
                }
                mr_hats[(4 * 2) + i_p3Hat].gameObject.SetActive(true);
            }
            if ((p_player3.GetButtonDown("UIHorizontal") && !b_p3Ready)) {
                mr_hats[(4 * 2) + i_p3Hat].gameObject.SetActive(false);
                i_p3Hat++;
                if (i_p3Hat > 3) {
                    i_p3Hat = 0;
                }
                if (e_p3Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 2) + i_p3Hat].materials = new Material[] { mat_hatRed[i_p3Hat] };
                }
                else {
                    mr_hats[(4 * 2) + i_p3Hat].materials = new Material[] { mat_hatBlue[i_p3Hat] };
                }
                mr_hats[(4 * 2) + i_p3Hat].gameObject.SetActive(true);
            }
            if ((p_player4.GetNegativeButtonDown("UIHorizontal") && !b_p4Ready)) {
                mr_hats[(4 * 3) + i_p4Hat].gameObject.SetActive(false);
                i_p4Hat--;
                if (i_p4Hat < 0) {
                    i_p4Hat = 3;
                }
                if (e_p4Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 3) + i_p4Hat].materials = new Material[] { mat_hatRed[i_p4Hat] };
                }
                else {
                    mr_hats[(4 * 3) + i_p4Hat].materials = new Material[] { mat_hatBlue[i_p4Hat] };
                }
                mr_hats[(4 * 3) + i_p4Hat].gameObject.SetActive(true);
            }
            if ((p_player4.GetButtonDown("UIHorizontal") && !b_p4Ready)) {
                mr_hats[(4 * 3) + i_p4Hat].gameObject.SetActive(false);
                i_p4Hat++;
                if (i_p4Hat > 3) {
                    i_p4Hat = 0;
                }
                if (e_p4Color == Constants.Global.Color.RED) {
                    mr_hats[(4 * 3) + i_p4Hat].materials = new Material[] { mat_hatRed[i_p4Hat] };
                }
                else {
                    mr_hats[(4 * 3) + i_p4Hat].materials = new Material[] { mat_hatBlue[i_p4Hat] };
                }
                mr_hats[(4 * 3) + i_p4Hat].gameObject.SetActive(true);
            }


            // confirm selection
            if (p_player1.GetButtonDown("UISubmit") && !b_p1Ready) {
                if (e_p1Color == Constants.Global.Color.RED && i_numRed < 2) {
                    i_numRed++;
                    txt_p1Message.text = "OK!";
                    b_p1Ready = true;
                }
                else if (e_p1Color == Constants.Global.Color.BLUE && i_numBlue < 2) {
                    i_numBlue++;
                    txt_p1Message.text = "OK!";
                    b_p1Ready = true;
                }
            }
            if (p_player2.GetButtonDown("UISubmit") && !b_p2Ready) {
                if (e_p2Color == Constants.Global.Color.RED && i_numRed < 2) {
                    i_numRed++;
                    txt_p2Message.text = "OK!";
                    b_p2Ready = true;
                }
                else if (e_p2Color == Constants.Global.Color.BLUE && i_numBlue < 2) {
                    i_numBlue++;
                    txt_p2Message.text = "OK!";
                    b_p2Ready = true;
                }
            }
            if (p_player3.GetButtonDown("UISubmit") && !b_p3Ready) {
                if (e_p3Color == Constants.Global.Color.RED && i_numRed < 2) {
                    i_numRed++;
                    txt_p3Message.text = "OK!";
                    b_p3Ready = true;
                }
                else if (e_p3Color == Constants.Global.Color.BLUE && i_numBlue < 2) {
                    i_numBlue++;
                    txt_p3Message.text = "OK!";
                    b_p3Ready = true;
                }
            }
            if (p_player4.GetButtonDown("UISubmit") && !b_p4Ready) {
                if (e_p4Color == Constants.Global.Color.RED && i_numRed < 2) {
                    i_numRed++;
                    txt_p4Message.text = "OK!";
                    b_p4Ready = true;
                }
                else if (e_p4Color == Constants.Global.Color.BLUE && i_numBlue < 2) {
                    i_numBlue++;
                    txt_p4Message.text = "OK!";
                    b_p4Ready = true;
                }
            }


            // reset selection
            if (p_player1.GetButtonDown("UICancel") && b_p1Ready) {
                // reduce color number
                if (e_p1Color == Constants.Global.Color.RED) {
                    i_numRed--;
                }
                else if (e_p1Color == Constants.Global.Color.BLUE) {
                    i_numBlue--;
                }
                txt_p1Message.text = "CONNECTED!";
                b_p1Ready = false;
            }
            if (p_player2.GetButtonDown("UICancel") && b_p2Ready) {
                // reduce color number
                if (e_p2Color == Constants.Global.Color.RED) {
                    i_numRed--;
                }
                else if (e_p2Color == Constants.Global.Color.BLUE) {
                    i_numBlue--;
                }
                txt_p2Message.text = "CONNECTED!";
                b_p2Ready = false;
            }
            if (p_player3.GetButtonDown("UICancel") && b_p3Ready) {
                // reduce color number
                if (e_p3Color == Constants.Global.Color.RED) {
                    i_numRed--;
                }
                else if (e_p3Color == Constants.Global.Color.BLUE) {
                    i_numBlue--;
                }
                txt_p3Message.text = "CONNECTED!";
                b_p3Ready = false;
            }
            if (p_player4.GetButtonDown("UICancel") && b_p4Ready) {
                // reduce color number
                if (e_p4Color == Constants.Global.Color.RED) {
                    i_numRed--;
                }
                else if (e_p4Color == Constants.Global.Color.BLUE) {
                    i_numBlue--;
                }
                txt_p4Message.text = "CONNECTED!";
                b_p4Ready = false;
            }


            // load next scene
            if (b_p1Ready && b_p2Ready && b_p3Ready && b_p4Ready) {
                go_go.SetActive(true);
                if (p_player1.GetButtonDown("MenuStart")) {
                    // set constants color, hat for all 4 players
                    Constants.PlayerStats.C_p1Color = e_p1Color;
                    Constants.PlayerStats.C_p2Color = e_p2Color;
                    Constants.PlayerStats.C_p3Color = e_p3Color;
                    Constants.PlayerStats.C_p4Color = e_p4Color;
                    Constants.PlayerStats.C_p1Hat = i_p1Hat;
                    Constants.PlayerStats.C_p2Hat = i_p2Hat;
                    Constants.PlayerStats.C_p3Hat = i_p3Hat;
                    Constants.PlayerStats.C_p4Hat = i_p4Hat;

                    LoadNextScene("WarmUp");
                }
            }
            else {
                go_go.SetActive(false);
            }
        }
    }
}
