using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

public class EndGameController : MonoBehaviour {

    [SerializeField] private GameObject go_menu;
    [SerializeField] private Button butt_select;

    [SerializeField] Rewired.Integration.UnityUI.RewiredStandaloneInputModule rsim;


    private int[] i_hatNums = new int[4];

    private Constants.Global.Color[] col_playerColors = new Constants.Global.Color[4];

    [SerializeField] private Image[] img_flags = new Image[4];
    [SerializeField] private Sprite img_redFlag;
    [SerializeField] private Sprite img_blueFlag;

    [SerializeField] private SkinnedMeshRenderer[] smr_winnerOutfit = new SkinnedMeshRenderer[2];
    [SerializeField] private SkinnedMeshRenderer[] smr_loserOutfit = new SkinnedMeshRenderer[2];

    [SerializeField] private Material[] mat_outfitBlue;
    [SerializeField] private Material[] mat_outfitRed;

    [SerializeField] private Material[] mat_hatBlue = new Material[4];
    [SerializeField] private Material[] mat_hatRed = new Material[4];

  

    private Player p_player;
    private bool b_open = false;

    //ART ADDITIONS
    [SerializeField] private Image[] img_icons;
    [SerializeField] private Sprite sp_sunTeamIcon;
    [SerializeField] private Sprite sp_moonTeamIcon;
    [SerializeField] private GameObject[] go_loserPlayerModels = new GameObject[8];
    [SerializeField] private GameObject[] go_winnerPlayerModels = new GameObject[8];
    [SerializeField] private Transform[] tran_winnerPlayerHeads = new Transform[8];
    [SerializeField] private Transform[] tran_loserPlayerHeads = new Transform[8];
    [SerializeField] private GameObject[] go_hatPrefabs = new GameObject[4];

    private GameObject[] go_activePlayerModels = new GameObject[4];
    

    // Use this for initialization
    void Start () {
        //Ensure timeScale is correct.
        Time.timeScale = 1;
        //Only player 1 does things on this menu.
        p_player = ReInput.players.GetPlayer(0);
        rsim.RewiredPlayerIds = new int[] { 0 };

        
        //Grab Constants
        col_playerColors[0] = Constants.PlayerStats.C_p1Color;
        col_playerColors[1] = Constants.PlayerStats.C_p2Color;
        col_playerColors[2] = Constants.PlayerStats.C_p3Color;
        col_playerColors[3] = Constants.PlayerStats.C_p4Color;
        i_hatNums[0] = Constants.PlayerStats.C_p1Hat;
        i_hatNums[1] = Constants.PlayerStats.C_p2Hat;
        i_hatNums[2] = Constants.PlayerStats.C_p3Hat;
        i_hatNums[3] = Constants.PlayerStats.C_p4Hat;

        DeterminePlayerModels();
        SetFlagsAppropriately();

    }
	
	// Update is called once per frame
	void Update () {
        //Listen for P1 hitting A.
        //Then do menu things.
        if (p_player.GetButton("UISubmit") && b_open == false) {
            OpenEndMenu();
        }
	}

    public void OpenEndMenu() {
        go_menu.SetActive(true);
        butt_select.Select();
        b_open = true;
    }

    public void MainMenu() {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Rematch() {
        SceneManager.LoadSceneAsync("BuildSetUp");
    }

    public void PlayerSelect() {
        SceneManager.LoadSceneAsync("RegisterPlayers");
    }

    public void Quit() {
        Application.Quit();
    }

    /// <summary>
    /// ART ADDED functionality for multiple player outfits.
    /// Finds what teams players were on. Organizes them into winner and loser.
    /// Assigns appropriate team colors
    /// </summary>
    void DeterminePlayerModels( )
    {
        int playerCounter = 0;
       
        for (int i = 0; i < col_playerColors.Length; i++)
        {
            if(col_playerColors[i] == Constants.Global.C_WinningTeam )
            {
                if( playerCounter%2 == 0 )
                {
                    go_winnerPlayerModels[i].SetActive(true);
                    go_activePlayerModels[i] = go_winnerPlayerModels[i];
                    playerCounter += 1;

                    if(col_playerColors[i] == Constants.Global.Color.BLUE )
                    {
                        smr_winnerOutfit[i].material = mat_outfitBlue[i];
                    }
                    else
                    {
                        smr_winnerOutfit[i].material = mat_outfitRed[i];
                    }
                    SetPlayerHat(i, i, col_playerColors[i], true);

                }
                else
                {
                    go_winnerPlayerModels[i+4].SetActive(true);
                    go_activePlayerModels[i] = go_winnerPlayerModels[i + 4];
                    playerCounter += 1;

                    if (col_playerColors[i] == Constants.Global.Color.BLUE)
                    {
                        smr_winnerOutfit[i+4].material = mat_outfitBlue[i];
                    }
                    else
                    {
                        smr_winnerOutfit[i+4].material = mat_outfitRed[i];
                    }
                    SetPlayerHat(i, i+4, col_playerColors[i], true);

                }

            }
            else
            {
                if (playerCounter % 2 == 0)
                {
                    go_loserPlayerModels[i].SetActive(true);
                    go_activePlayerModels[i] = go_loserPlayerModels[i];
                    playerCounter += 1;

                    if (Constants.Global.C_WinningTeam == Constants.Global.Color.BLUE)
                    {
                        smr_loserOutfit[i].material = mat_outfitBlue[i];
                    }
                    else
                    {
                        smr_loserOutfit[i].material = mat_outfitRed[i];
                    }
                    SetPlayerHat(i, i, col_playerColors[i], false);
                }
                else
                {
                    go_loserPlayerModels[i + 4].SetActive(true);
                    go_activePlayerModels[i] = go_loserPlayerModels[i + 4];
                    playerCounter += 1;

                    if (Constants.Global.C_WinningTeam == Constants.Global.Color.BLUE)
                    {
                        smr_loserOutfit[i + 4].material = mat_outfitBlue[i];
                    }
                    else
                    {
                        smr_loserOutfit[i + 4].material = mat_outfitRed[i];
                    }
                    SetPlayerHat(i, i+4, col_playerColors[i], false);
                }
            }
        }
    }

    /// <summary>
    /// Instantiates proper hat prefab with player model associated with player.
    /// </summary>
    /// <param name="player">P1 - 4</param>
    /// <param name="modelIndex"> Model Index 0 - 7</param>
    /// <param name="team">team color, RED or BLUE</param>
    /// <param name="winner">Pull from winner models or loser models</param>
    void SetPlayerHat(int player, int modelIndex, Constants.Global.Color team, bool winner )
    {
        GameObject hat = new GameObject();
        if (winner)
        {
            switch (player)
            {
                case 0:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p1Hat], tran_winnerPlayerHeads[modelIndex]);
                    break;
                case 1:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p2Hat], tran_winnerPlayerHeads[modelIndex]);
                    break;
                case 2:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p3Hat], tran_winnerPlayerHeads[modelIndex]);
                    break;
                case 3:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p4Hat], tran_winnerPlayerHeads[modelIndex]);
                    break;
                default:
                    break;
            }
            
        }
        else
        {
            switch (player)
            {
                case 0:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p1Hat], tran_loserPlayerHeads[modelIndex]);
                    break;
                case 1:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p2Hat], tran_loserPlayerHeads[modelIndex]);
                    break;
                case 2:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p3Hat], tran_loserPlayerHeads[modelIndex]);
                    break;
                case 3:
                    hat = GameObject.Instantiate(go_hatPrefabs[Constants.PlayerStats.C_p4Hat], tran_loserPlayerHeads[modelIndex]);
                    break;
                default:
                    break;
            }
        }


        if (team == Constants.Global.Color.BLUE)
        {
            hat.GetComponentInChildren<MeshRenderer>().material = mat_hatBlue[player];
        }
        else
        {
            hat.GetComponentInChildren<MeshRenderer>().material = mat_hatRed[player];
        }

    }

    /// <summary>
    /// Set the flags the proper color to which team won and what team lost.
    /// </summary>
    void SetFlagsAppropriately( )
    {
        //Make flags correct color.
        if (Constants.Global.C_WinningTeam == Constants.Global.Color.BLUE)
        {
            img_flags[0].sprite = img_flags[1].sprite = img_blueFlag;
            img_icons[0].sprite = img_icons[1].sprite = sp_moonTeamIcon;

            img_flags[2].sprite = img_flags[3].sprite = img_redFlag;
            img_icons[2].sprite = img_icons[3].sprite = sp_sunTeamIcon;
        }
        else
        {
            img_flags[0].sprite = img_flags[1].sprite = img_redFlag;
            img_icons[0].sprite = img_icons[1].sprite = sp_sunTeamIcon;

            img_flags[2].sprite = img_flags[3].sprite = img_blueFlag;
            img_icons[2].sprite = img_icons[3].sprite = sp_moonTeamIcon;
        }
    }

    /*
    void OldHatFunctions()
    {
        //Activate the correct hat and materials on models.
        int winnerCounter = 0;
        int loserCounter = 0;
        for (int i = 0; i < 4; i++)
        {
            if (col_playerColors[i] == Constants.Global.C_WinningTeam)
            {
                if (col_playerColors[i] == Constants.Global.Color.BLUE)
                {
                    //smr_winnerOutfit[winnerCounter].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
                    mr_winnerHat[(4 * winnerCounter) + i_hatNums[i]].materials = new Material[] { mat_hatBlue[i_hatNums[i]] };
                    mr_winnerHat[(4 * winnerCounter) + i_hatNums[i]].gameObject.SetActive(true);
                }
                else
                {
                    //smr_winnerOutfit[winnerCounter].materials = new Material[] { mat_outfitRed, mat_outfitRed };
                    mr_winnerHat[(4 * winnerCounter) + i_hatNums[i]].materials = new Material[] { mat_hatRed[i_hatNums[i]] };
                    mr_winnerHat[(4 * winnerCounter) + i_hatNums[i]].gameObject.SetActive(true);
                }
                winnerCounter++;
            }
            else
            {
                if (col_playerColors[i] == Constants.Global.Color.BLUE)
                {
                    //smr_loserOutfit[loserCounter].materials = new Material[] { mat_outfitBlue, mat_outfitBlue };
                    mr_loserHat[(4 * loserCounter) + i_hatNums[i]].materials = new Material[] { mat_hatBlue[i_hatNums[i]] };
                    mr_loserHat[(4 * loserCounter) + i_hatNums[i]].gameObject.SetActive(true);
                }
                else
                {
                    //smr_loserOutfit[loserCounter].materials = new Material[] { mat_outfitRed, mat_outfitRed };
                    mr_loserHat[(4 * loserCounter) + i_hatNums[i]].materials = new Material[] { mat_hatRed[i_hatNums[i]] };
                    mr_loserHat[(4 * loserCounter) + i_hatNums[i]].gameObject.SetActive(true);
                }
                loserCounter++;
            }
        }
    }*/
}
