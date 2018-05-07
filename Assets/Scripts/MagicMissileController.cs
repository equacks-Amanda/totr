﻿/*  Magic Missile Controller - Joe Chew
 * 
 *  Desc:   Facilitates Magic Missile interactions
 * 
 */

using UnityEngine;

public class MagicMissileController : SpellController {
#region Variables and Declarations
    //private bool i_reflect = false;
#endregion

#region Magic Missile Methods
    override protected void Charge(float f_chargeTime) {
        f_damage = Constants.SpellStats.C_MagicMissileDamage;
    }

    override protected void BuffSpell() {
        // Magic Missile doesn't cross the rift. Destroy it
        //Destroy(gameObject);
    }
#endregion

#region Unity Overrides
    override protected void OnCollisionEnter(Collision collision) {
        SpellTarget target;
        if (target = collision.gameObject.GetComponent<SpellTarget>()) {
            target.ApplySpellEffect(e_spellType, e_color, f_damage, transform.forward.normalized);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Spell")) {
            Constants.Global.Color spellColor = collision.gameObject.GetComponent<SpellController>().Color;
            //if (spellColor != e_color) {    // opposing spells destroy each other
                Destroy(gameObject);
            //}
            //else {              // ignore collisions between spells of the same color -THIS DOES NOT WORK!
                //Debug.Log("oof");
            //    Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
            //}
        }
        else {    // magic missile can reflect off of surfaces
            Vector3 v = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
            float rot = 90 - Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            rb.velocity = transform.forward * Constants.SpellStats.C_MagicMissileSpeed;
        }
    }

    void OnDestroy() {
        Instantiate(go_onDestroyParticles, transform.position, Quaternion.identity);
    }
#endregion
}
