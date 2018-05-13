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
		UpdateScore(Constants.ObjectiveStats.C_HockeyMaxScore);
		gc_owned.FlashOn();
    }
#endregion

#region Unity Overrides
    void OnEnable() {
        maestro.PlayBeginHockey();
    }
#endregion
}
