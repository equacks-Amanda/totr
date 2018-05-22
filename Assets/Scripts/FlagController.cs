/*  Flag Controller - Zak Olyarnik
 * 
 *  Desc:   Facilitates picking up and scoring with Capture the Flag Objective's Flag object
 * 
 */

using UnityEngine;
using UnityEngine.UI;

public class FlagController : MonoBehaviour {
#region Variables and Declarations
    [SerializeField] private CaptureTheFlagObjective ctfo_owner;    // identifies Objective flag is a part of
    [SerializeField] private Constants.Global.Color e_color;        // identifies owning team
    [SerializeField] private GameObject go_buttonPrompt;            // indicator to pickup
    [SerializeField] private GameObject go_scoreParticle;
    [SerializeField] private Text go_timer;
    private int i_flagTimer = 0;
    #endregion

    #region FlagController Methods
    public void DropFlag() {
        transform.SetParent(ctfo_owner.gameObject.transform);   // resets flag parent so Objective can be deactivated correctly
        transform.localPosition = new Vector3(transform.localPosition.x, Constants.ObjectiveStats.C_RedFlagSpawn.y, transform.localPosition.z);
        if(ctfo_owner.gameObject.activeSelf)    // needed for safety of auto flag drop at Objective's end
            ctfo_owner.StartCoroutine("Notify");
        InvokeRepeating("FlagResetTimer", 1.0f, 1.0f);
    }

    private void FlagResetTimer() {
        i_flagTimer++;
        go_timer.text = (Constants.ObjectiveStats.C_FlagResetTimer - i_flagTimer).ToString();

        if (i_flagTimer >= Constants.ObjectiveStats.C_FlagResetTimer) {
            ResetTimer();
            ResetFlagPosition();
        }
    }

    private void ResetTimer() {
        i_flagTimer = 0;
        go_timer.text = "";// Constants.ObjectiveStats.C_FlagResetTimer.ToString();
    }

    public void ResetFlagPosition() {
        CancelInvoke("FlagResetTimer");
        if (e_color == Constants.Global.Color.RED) {
            transform.localPosition = Constants.ObjectiveStats.C_RedFlagSpawn;
        }
        else {
            transform.localPosition = Constants.ObjectiveStats.C_BlueFlagSpawn;
        }
    }

    public bool IsPickedUp() {
        return !transform.parent.parent ? false : transform.parent.parent.gameObject.CompareTag("Player");
    }

    private void DisableParticle() {
        go_scoreParticle.SetActive(false);
    }
    #endregion

    #region Unity Overrides

    private void OnDestroy()
    {
        CancelInvoke();
    }

    void OnTriggerEnter(Collider other) {
        // Player trying to pick up flag (and flag not already picked up)
        if (other.CompareTag("InteractCollider") && !IsPickedUp() && other.GetComponentInParent<PlayerController>().Color != e_color) {
			other.GetComponentInParent<PlayerController>().Pickup(gameObject);
			other.gameObject.SetActive(false);
            ctfo_owner.StopCoroutine("Notify");
            go_buttonPrompt.SetActive(false);

            ResetTimer();
            CancelInvoke("FlagResetTimer");
        }

        if (other.CompareTag("OutofBounds")) {
            DropFlag();
            ResetFlagPosition();
        }

        // Player scoring with flag
        if (other.CompareTag("Goal")) {
			if (IsPickedUp() &&
                transform.parent.parent.GetComponent<PlayerController>().Color == other.GetComponent<GoalController>().Color &&
                other.GetComponent<GoalController>().Color != e_color) {        // check for correct color of player/flag/goal
                Vector3 pos = transform.position;
                go_scoreParticle.transform.position = pos;
                go_scoreParticle.SetActive(true);
                Invoke("DisableParticle", Constants.ObjectiveStats.C_ScoringParticleLiveTime);

                ctfo_owner.UpdateFlagScore();                                   // increase score and update UI      
				transform.parent.parent.GetComponent<PlayerController>().DropFlag();     // make carrying player drop flag (sets player's flag reference to null and calls FlagController.DropFlag)
                ResetFlagPosition();   // reset flag to original spawn position
            }
		}
	}

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && !IsPickedUp() && other.GetComponent<PlayerController>().Color != e_color) {
            go_buttonPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && !IsPickedUp()) {
            go_buttonPrompt.SetActive(false);
        }
    }
#endregion
}
