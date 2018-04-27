/*  Capture The Flag Objective - Zak Olyarnik
 * 
 *  Desc:   Facilitates Capture The Flag Objective
 * 
 */
 using UnityEngine;

public class CaptureTheFlagObjective : Objective {
    [SerializeField] private GoalController gc_owned;
    [SerializeField] private FlagController fc_owned;

    #region CaptureTheFlagObjective Methods
    override protected void SetUI() {
        calligrapher.CTFInit(e_color);
    }

    override protected void ResetUI() {
        calligrapher.GoalScoreReset(e_color);
    }

    // Update UI and check for completion
    public void UpdateFlagScore() {
        i_score++;
        maestro.PlayScore();
        calligrapher.UpdateGoalScoreUI(e_color, i_score);
        gc_owned.FlashOn();
        if (i_score >= Constants.ObjectiveStats.C_CTFMaxScore) {
            b_isComplete = true;
        }
    }


#endregion

#region Unity Overrides	
    void OnEnable() {
        maestro.PlayBeginCTF();
    }

    void OnDestroy() {
        // add reference to FlagController
        if (fc_owned.IsPickedUp()) {
            fc_owned.gameObject.transform.parent.parent.GetComponent<PlayerController>().DropFlag();
            Destroy(fc_owned.gameObject);
        }
    }

#endregion
}
