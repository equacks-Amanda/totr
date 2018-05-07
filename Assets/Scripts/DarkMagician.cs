/*  Dark Magician - Zak Olyarnik
 * 
 *  Desc:   GameController functionality - Facilitates Objective switching and checking for win condition
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public sealed class DarkMagician : MonoBehaviour {
#region Variables and Declarations
    [SerializeField] private Objective[] objv_redObjectiveList;
    [SerializeField] private Objective[] objv_blueObjectiveList;
    [SerializeField] private Text txt_winMsg;
    [SerializeField] private Button butt_winSelect;
    private Objective objv_currentRedObjective, objv_currentBlueObjective;
	private Maestro maestro;                  // reference to audio controller singleton
    private bool b_gameOver;
    private RiftController riftController;
#endregion

#region Dark Magician Methods
    // Shuffles the order of both red and blue objective lists in parallel
    private void ShuffleObjectives() {
        for (int i = 0; i < objv_redObjectiveList.Length - 1; i++) {
            Objective tmp1 = objv_redObjectiveList[i];
            Objective tmp2 = objv_blueObjectiveList[i];
            int j = Random.Range(i, objv_redObjectiveList.Length - 1);
            objv_redObjectiveList[i] = objv_redObjectiveList[j];
            objv_blueObjectiveList[i] = objv_blueObjectiveList[j];
            objv_redObjectiveList[j] = tmp1;
            objv_blueObjectiveList[j] = tmp2;
        }
    }

    private void GetNextObjective(Constants.Global.Color c, int objectiveNumber) {
        // check for game end
        if (objectiveNumber == objv_redObjectiveList.Length) {
			b_gameOver = true;
			txt_winMsg.text = c + " team won!";
            Constants.Global.C_WinningTeam = c;
			return;
		}
        objectiveNumber++;
        maestro.PlayObjectiveStart();
        objv_currentRedObjective.Complete();
        objv_currentRedObjective = objv_redObjectiveList[objectiveNumber-1].Activate(objectiveNumber);  // objectiveNumber starts with 1 but array is 0-based
        objv_currentBlueObjective.Complete();
        objv_currentBlueObjective = objv_blueObjectiveList[objectiveNumber - 1].Activate(objectiveNumber);  // objectiveNumber starts with 1 but array is 0-based
        riftController.ActivateLights(objectiveNumber - 1);
    }

    private IEnumerator SwitchToEndGame() {
        yield return new WaitForSecondsRealtime(4f);
        Time.timeScale = 1;
        SceneManager.LoadScene("EndGame");
    }
#endregion

#region Unity Overrides
    void Start() {
        txt_winMsg.enabled = false;
        b_gameOver = false;
        ShuffleObjectives();
		maestro = Maestro.Instance;
        objv_currentRedObjective = objv_redObjectiveList[0].Activate(1);
        objv_currentBlueObjective = objv_blueObjectiveList[0].Activate(1);
        riftController = RiftController.Instance;
    }

    void Update() {
        // Dev shortcuts TODO: remove in release
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            GetNextObjective(objv_currentRedObjective.Color, objv_currentRedObjective.NumberInList);
        }

        // check for completion of objectives
        if (b_gameOver && txt_winMsg.enabled == false) {
			txt_winMsg.enabled = true;
            StartCoroutine(SwitchToEndGame());	
		}
		else {
			if (objv_currentRedObjective.IsComplete) {
                Constants.TeamStats.C_RedTeamScore++;
                GetNextObjective(objv_currentRedObjective.Color, objv_currentRedObjective.NumberInList);
			}
			if(objv_currentBlueObjective.IsComplete) {
                Constants.TeamStats.C_BlueTeamScore++;
                GetNextObjective(objv_currentBlueObjective.Color, objv_currentBlueObjective.NumberInList);
            }
		}
	}
#endregion
}
