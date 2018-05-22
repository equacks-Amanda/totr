/*  Objective - Zak Olyarnik
 * 
 *  Desc:   Parent class of all in-game Objectives.
 * 
 */

using System.Collections;
using UnityEngine;

public abstract class Objective : MonoBehaviour {
#region Variables and Declarations
    [SerializeField] protected Constants.Global.Color e_color;  // identifies owning team
    [SerializeField] protected GameObject[] go_roomPool;        // rooms allowed for this objective to be played in

    [SerializeField] protected GameObject go_objectIndicator;
    [SerializeField] protected GameObject go_goalIndicator;
    [SerializeField] protected GameObject go_hologram;
    [SerializeField] protected ScoreOrbController soc_scoreOrb;

    protected GameObject go_activeRoom;     // active room specific to this objective instance 
    protected int i_numberInList;           // this is the i'th objective faced by this team (1-based)
    protected bool b_isComplete = false;    // set when criteria to beat the objective is met
    protected int i_score;                  // current progress towards i_maxScore (currently used by CTF and Hockey)

    protected Calligrapher calligrapher;        // reference to UI controller singleton
	protected Maestro maestro;                  // reference to audio controller singleton
    protected RiftController riftController;    // reference to rift singleton

    #region Getters and Setters
    public Constants.Global.Color Color {
        get { return e_color; }
    }
    public int Score {
        get { return i_score; }
    }
    public int NumberInList {
        get { return i_numberInList; }
    }
    public bool IsComplete {
        get { return b_isComplete; }
    }
    #endregion
#endregion

#region Objective Shared Methods
    protected abstract void SetUI();
    protected abstract void ResetUI();
    public abstract int GetMax();

    // Activates all aspects of this objective
    public Objective Activate(int i) {
        calligrapher = Calligrapher.Instance;       // set singletons
        maestro = Maestro.Instance;
        riftController = RiftController.Instance;
        b_isComplete = false;                       // initialize variables
        i_numberInList = i;
        SetUI();                                    // set UI
        go_activeRoom = SelectRoom();               // set room
        go_activeRoom.SetActive(true);
        gameObject.SetActive(true);                 // finally, turn on objective
        riftController.ResetPlayers();
        if (soc_scoreOrb != null) {
            soc_scoreOrb.Objective = this;
        }
        StartCoroutine("Notify");
		InvokeRepeating("AnnounceIdle",40f,40f);
		go_hologram.SetActive(true);
        return this;
    }

    // Destroy this objective once it is complete
    public void Complete() {
        maestro.PlayAnnouncementTrialTransition();
        if (e_color == Constants.Global.Color.RED) {
            Constants.TeamStats.C_RedTeamScore += i_score;
        }
        else if (e_color == Constants.Global.Color.BLUE) {
            Constants.TeamStats.C_BlueTeamScore += i_score;
        }
        riftController.IncrementObjectiveCount(e_color);
        riftController.IncreaseVolatility(Constants.RiftStats.C_VolatilityIncrease_RoomAdvance);
        ResetUI();                              // turn off UI
        go_activeRoom.SetActive(false);         // turn off room
		Destroy(go_hologram);
        Destroy(gameObject);                    // each objective is only played once, so destroy after use
    }

    // Randomly selects a room from the list of approved rooms for this objective
    private GameObject SelectRoom() {
        int i = Random.Range(0, go_roomPool.Length);
        return go_roomPool[i];
    }

    public IEnumerator Notify() {
        if (!go_objectIndicator)
            yield break;

        go_objectIndicator.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        go_goalIndicator.SetActive(true);
        yield return new WaitForSeconds(Constants.ObjectiveStats.C_NotificationTimer);

        StartCoroutine("Notify");
    }

    public void DeNotify() {
        StopCoroutine("Notify");
    }
	
	protected Constants.Global.Color GetLeadColor(){
		if(Constants.TeamStats.C_RedTeamScore > Constants.TeamStats.C_BlueTeamScore) return Constants.Global.Color.RED;
		else if(Constants.TeamStats.C_RedTeamScore < Constants.TeamStats.C_BlueTeamScore) return Constants.Global.Color.BLUE;
		else return Constants.Global.Color.NULL;
	}
	
	protected void UpdateScore(int max){
		Constants.Global.Color oldLead = GetLeadColor();
        i_score++;
		Constants.Global.Color newLead = GetLeadColor();
		
		//If this is the first point of the game, play the first point announcement.
		if(oldLead == Constants.Global.Color.NULL && i_score == 1) maestro.PlayAnnouncementFirstScore();
		
		//If the lead changed, play the lead changed announcement.
		if(oldLead != newLead && newLead != Constants.Global.Color.NULL){
			maestro.PlayAnnouncementScoreComeback();
			maestro.PlayAnnouncementScoreLoser();
		}
		
		maestro.PlayAnnouncementScore();
		
		maestro.PlayScore();
        if (soc_scoreOrb != null) {
            Debug.Log("Help?");
            if (go_goalIndicator != null)
            {
                soc_scoreOrb.StartPosition = go_goalIndicator.transform.position;
            }
            soc_scoreOrb.gameObject.SetActive(true);
        }
		if (i_score == max - 1) {
            maestro.PlayTeamEncouragement();
        }
        else 
		CancelInvoke("AnnounceIdle");
		InvokeRepeating("AnnounceIdle",40f,40f);
	}
	
	protected void AnnounceIdle(){
		maestro.PlayAnnouncementIdle();
	}

    public void ScoreOrbHit(int max) {
        calligrapher.UpdateGoalScoreUI(e_color, i_score);
        if (i_score >= max) {
            b_isComplete = true;
        }
    }
#endregion
}
