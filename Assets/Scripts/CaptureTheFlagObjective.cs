/*  Capture The Flag Objective - Zak Olyarnik
 * 
 *  Desc:   Facilitates Capture The Flag Objective
 * 
 */
 using UnityEngine;

public class CaptureTheFlagObjective : Objective {
    [SerializeField] private GoalController gc_owned;

#region CaptureTheFlagObjective Methods
    override protected void SetUI() {
        calligrapher.CTFInit(e_color);
    }

    override protected void ResetUI() {
        calligrapher.ScoreReset(e_color);
    }

    // Update UI and check for completion
    public void UpdateFlagScore() {
        i_score++;
		maestro.PlayScore();
        calligrapher.UpdateScoreUI(e_color, i_score);
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
#endregion
}
