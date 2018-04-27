/*  Debug Parameters Controller - Sam Caulker
 * 
 *  Desc:   Facilitates pausing the game and limiting it to only one user
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Rewired;

public class PauseController : MonoBehaviour {

    static PlayerController pc_owner;
    public GameObject img_pauseBacking;
    public Text txt_pauseIndicator;
    [SerializeField] GameObject go_options;
    [SerializeField] GameObject go_pause;

    [SerializeField]Text txt_buildLabel;
    [SerializeField]Text txt_redScoreLabel;
    [SerializeField]Text txt_blueScoreLabel;
    [SerializeField]Button butt_select;
    [SerializeField]Button butt_optSelect;
    [SerializeField]Button butt_options;
    //private Player p_player;
    private float f_unPause;
    [SerializeField] Rewired.Integration.UnityUI.RewiredStandaloneInputModule rsim;
    [SerializeField] EventSystem es_master;


    public void Pause(PlayerController pc_in) {
        if (pc_owner == null) {
            pc_owner = pc_in;
            txt_pauseIndicator.text = "P" + (pc_owner.Num + 1) + " Pause";
            if(txt_redScoreLabel != null) {
                txt_redScoreLabel.text = "Total Score: " + Constants.TeamStats.C_RedTeamScore;
                txt_blueScoreLabel.text = "Total Score: " + Constants.TeamStats.C_BlueTeamScore;
            }
            img_pauseBacking.SetActive(true);

            rsim.RewiredPlayerIds = new int[] { pc_owner.Num };

            //Properly highlight the button.
            butt_select.Select();

            Time.timeScale = 0;
            
        }  
    }

    public void Unpause() {
        pc_owner = null;
        img_pauseBacking.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenOptions() {
        go_options.SetActive(true);
        go_pause.SetActive(false);
        butt_optSelect.Select();
        butt_optSelect.OnSelect(null);
    }

    public void CloseOptions() {
        go_options.SetActive(false);
        go_pause.SetActive(true);
        butt_options.Select();
    }

    public void GameReset() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void MatchRestart() {
		Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start() {
        txt_buildLabel.text = "Build: v" + Constants.Global.C_BuildNumber;
    }
}
