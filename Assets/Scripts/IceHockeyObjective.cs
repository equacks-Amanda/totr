/*  Ice Hockey Objective - Dana Thompson
 * 
 *  Desc:   Facilitates Ice Hockey Objective
 * 
 */
 using UnityEngine;

public class IceHockeyObjective : Objective {
#region Variables and Declarations
    [SerializeField] private GoalController gc_owned;
#endregion

#region IceHockeyObjective Methods
    override protected void SetUI() {
        calligrapher.IceHockeyInit(e_color);
    }

    override protected void ResetUI() {
        calligrapher.GoalScoreReset(e_color);
    }

    // Update UI and check for completion
    public void UpdatePuckScore() {
        i_score++;
		maestro.PlayScore();
        calligrapher.UpdateGoalScoreUI(e_color, i_score);
        gc_owned.FlashOn();
        if (i_score >= Constants.ObjectiveStats.C_HockeyMaxScore) {
            b_isComplete = true;
        }
    }
#endregion

#region Unity Overrides
    void OnEnable() {
        maestro.PlayBeginHockey();
    }
#endregion
}
