/*  Defeat Necromancers Objective - Zak Olyarnik
 * 
 *  Desc:   Facilitates Defeat Necromancers Objective
 * 
 */

public class DefeatNecromancersObjective : Objective {
#region DefeatNecromancersObjective Methods
    override protected void SetUI() {
        calligrapher.DefeatNecromancersInit(e_color);
    }

    override protected void ResetUI() {
        calligrapher.GoalScoreReset(e_color);
    }

    override public int GetMax() {
        return Constants.ObjectiveStats.C_NecromancersMaxScore;
    }

    // Update UI and check for completion
    public void UpdateNecroScore() {
		UpdateScore(Constants.ObjectiveStats.C_NecromancersMaxScore);
    }
#endregion

#region Unity Overrides	
    void OnEnable() {
        maestro.PlayBeginNecromancer();
    }
#endregion
}