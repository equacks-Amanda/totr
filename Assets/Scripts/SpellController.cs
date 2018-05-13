/*  Spell Controller - Zak Olyarnik
 * 
 *  Desc:   Parent class of all in-game spells
 * 
 */

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public abstract class SpellController : MonoBehaviour {
#region Variables and Declarations
    [SerializeField] protected Constants.SpellStats.SpellType e_spellType;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected GameObject go_onDestroyParticles;
    protected Constants.Global.Color e_color;
	protected float f_damage;
    protected float f_charge = 1;         // charging multiplier
    protected PlayerController pc_owner;      // owner of the spell
    protected RiftController riftController;    // reference to Rift singleton
#endregion
    #region Getters and Setters
    public Constants.Global.Color Color{
        get { return e_color; }
    }

    public PlayerController PC_Owner {
        get { return pc_owner; }
    }
    #endregion

    #region SpellController Shared Methods
    protected abstract void Charge(float f_chargeTime);
    protected abstract void BuffSpell();

    public void Init(PlayerController owner, Constants.Global.Color color, float chargeTime) {
        pc_owner = owner;
        e_color = color;
        Physics.IgnoreCollision(pc_owner.RigidBody.GetComponent<Collider>(), GetComponent<Collider>());

        Charge(chargeTime);
        if(e_color == Constants.Global.Color.RED) {
            gameObject.layer = LayerMask.NameToLayer("RedShot");
        }
        else {
            gameObject.layer = LayerMask.NameToLayer("BlueShot");
        }

    }

    void InvokeDestroy() {
		Destroy(gameObject);
	}
#endregion

#region Unity Overrides
    protected virtual void Start() {
        riftController = RiftController.Instance;
        Invoke("InvokeDestroy", Constants.SpellStats.C_SpellLiveTime);
	}

    protected virtual void OnCollisionEnter(Collision collision)
    {
        SpellTarget target;
        if (target = collision.gameObject.GetComponent<SpellTarget>()) {
            target.ApplySpellEffect(e_spellType, e_color, f_damage, transform.forward.normalized);
        }
        Destroy(gameObject);
	}

	protected virtual void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Rift")) {	    // Rift reacts to spells by trigger rather than collision
			CancelInvoke();     // cancels and restarts spell's live timer
			BuffSpell();
			Invoke("InvokeDestroy", Constants.SpellStats.C_SpellLiveTime);
        }

        if (other.CompareTag("ParryShield")) {

            CancelInvoke();     // cancels and restarts spell's live timer
            Invoke("InvokeDestroy", Constants.SpellStats.C_SpellLiveTime);

            // deflect spell back from whence it came
            // this sends it backwards from where it came, not to where the player was directing it toward
            //Vector3 v3_direction = -transform.forward.normalized;
            //transform.forward = v3_direction;
            //rb.velocity = v3_direction * rb.velocity.magnitude;


            // deflect spell in player's facing direction
            Vector3 v3_direction = other.gameObject.transform.forward.normalized;
            transform.forward = v3_direction;
            rb.velocity = v3_direction * rb.velocity.magnitude;

            //Allow any collision with the original owner of the spell, reassign the pc_owner, and now ignore the new playercontroller
            Physics.IgnoreCollision(pc_owner.RigidBody.GetComponent<Collider>(), GetComponent<Collider>(), false);
            pc_owner = other.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
            Physics.IgnoreCollision(pc_owner.RigidBody.GetComponent<Collider>(), GetComponent<Collider>());
            e_color = other.gameObject.transform.parent.gameObject.GetComponent<PlayerController>().Color;
        }
    }
    #endregion
}

