/*  WarmUp Portal Controller - Noah Nam
 * 
 *  Desc:   Facilitates moving from WarmUp scene to BuildSetUp scene
 * 
 */

using System.Collections;
using UnityEngine;

public class WarmUpPortalController : SceneLoader {
#region Variables and Declarations
    [SerializeField] private GameObject go_pulseEffect;
    [SerializeField] private GameObject go_whatshisface;
    [SerializeField] private GameObject go_watshisface2;
    private static int i_players = 4;           // number of players in the game, should be 4.
    private int i_remainingPlayers = i_players; // decreases as players enter the portal
#endregion

    // this should absolutely not be being done here
    private IEnumerator WaitForCameraAnimation() {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(12f);
        Time.timeScale = 1;
        go_whatshisface.SetActive(true);
        go_watshisface2.SetActive(false);
    }


#region Unity Overrides
void Start() {
        StartCoroutine(WaitForCameraAnimation());
    }


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
