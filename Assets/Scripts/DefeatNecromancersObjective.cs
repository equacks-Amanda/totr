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
        calligrapher.ScoreReset(e_color);
    }

    // Update UI and check for completion
    public void UpdateNecroScore() {
        i_score++;
		maestro.PlayScore();
        calligrapher.UpdateScoreUI(e_color, i_score);
        if (i_score >= Constants.ObjectiveStats.C_NecromancersMaxScore) {
            b_isComplete = true;
        }
    }
#endregion

#region Unity Overrides	
    void OnEnable() {
        //maestro.PlayBeginDefeatNecromancers();
    }
#endregion
}