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
		UpdateScore(Constants.ObjectiveStats.C_CTFMaxScore);
		gc_owned.FlashOn();
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
