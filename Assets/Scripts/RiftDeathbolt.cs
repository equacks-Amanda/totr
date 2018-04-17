
using UnityEngine;

public class RiftDeathbolt : MonoBehaviour {

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Wall"))
            Destroy(gameObject);
        else {      // players are the only other thing deathbolts can collide with
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(Constants.RiftStats.C_VolatilityDeathboltDamage, Constants.Global.DamageType.DEATHBOLT);
            Destroy(gameObject);
        }
    }
}
