/*  WarmUp Portal Controller - Noah Nam
 * 
 *  Desc:   Facilitates moving from WarmUp scene to BuildSetUp scene
 * 
 */

using UnityEngine;

public class WarmUpPortalController : SceneLoader {
#region Variables and Declarations
    [SerializeField] private GameObject go_pulseEffect;
    private static int i_players = 4;           // number of players in the game, should be 4.
    private int i_remainingPlayers = i_players; // decreases as players enter the portal
#endregion

#region Unity Overrides
    void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
            go_pulseEffect.SetActive(true);
            Invoke("TurnOffParticleSystem", 1f);
            other.gameObject.SetActive(false);
			i_remainingPlayers--;
			if(i_remainingPlayers <= 0){
                LoadNextScene("BuildSetUp");
            }
		}
	}

    private void TurnOffParticleSystem() {
        go_pulseEffect.SetActive(false);
    }

    void Update() {     // TODO: dev hotkey, remove for release
        if (Input.GetKeyDown("space")) {
            LoadNextScene("BuildSetUp");
        }
    }
#endregion
}
