/*  Rune Explosion Controller - Jeff Brown
 * 
 *  Desc:   Damages Players on contact
 * 
 */

using UnityEngine;
using System.Collections;

public class RuneExplosionController : MonoBehaviour {
	bool startupFinished;
	bool activeOver;
#region Unity Overrides
    void Start() {
		Destroy(gameObject, Constants.EnemyStats.C_RuneExplosionLiveTime);
		startupFinished = false;
		activeOver = false;
		Invoke("ExplosionReady",1f);
	}

    void OnTriggerStay(Collider other) {
        if(other.CompareTag("Player") && startupFinished && !activeOver) {
            other.GetComponent<PlayerController>().TakeDamage(Constants.EnemyStats.C_RuneDamage, Constants.Global.DamageType.RUNE);
			activeOver = true;
        }
    }
	
	void ExplosionReady(){
		startupFinished = true;
	}
#endregion
}