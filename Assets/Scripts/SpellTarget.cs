/*  Spell Target - Zak Olyarnik
 * 
 *  Desc:   Parent class of Player, Enemy, and Objective interactables - anything that can be hit by a Spell
 * 
 */

using System.Collections;
using UnityEngine;

public abstract class SpellTarget : MonoBehaviour {
#region Variables and Declarations
    [SerializeField] protected Constants.Global.Color e_color;  // identifies owning team
    [SerializeField] protected Constants.Global.Side e_startSide;
    [SerializeField] protected GameObject go_indicator;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator anim;
	[SerializeField] protected ShaderEffect se_dissolve;
    [SerializeField] protected ShaderEffect se_fader;
    [SerializeField] protected ShaderEffect se_clothesDissolve;
    protected float f_health;
    protected float f_speed;
    protected Coroutine cor_AOECoroutine;
    protected Maestro maestro;                  // reference to audio controller singleton
    protected RiftController riftController;    // reference to Rift singleton
    protected bool b_electricDamageSoundOk = true;
    [SerializeField] protected float f_electricDamageSoundRate = 0.5f;
    [SerializeField] protected Collider col_attachedCollider;
    #region Getters and Setters
    public Constants.Global.Color Color {
        get { return e_color; }
    }

    public Coroutine AOECoroutine {
        set { cor_AOECoroutine = value; }
    }
	
	public float Health{
		get { return f_health; }
	}

    public Rigidbody RigidBody {
        get { return rb; }
    }
    #endregion
    #endregion

    #region SpellTarget Shared Methods
    public abstract void ApplySpellEffect(Constants.SpellStats.SpellType spell, Constants.Global.Color color, float damage, Vector3 direction);

    public virtual void NegateSpellEffect(Constants.SpellStats.SpellType spell) { }

    public virtual void Notify() {
        go_indicator.SetActive(true);
    }
	
	public IEnumerator WindPush(float multiplier, Vector3 direction, bool velocityReset){
		maestro.PlayWindHit();
		float startTime = Time.time;
		float elapsedTime = 0;
		while(elapsedTime < .99f){
            if (velocityReset) {
                rb.velocity = Vector3.zero;
            }
			elapsedTime = (Time.time - startTime)/Constants.SpellStats.C_WindPushTime;
			rb.AddForce(direction * Mathf.Lerp(Constants.SpellStats.C_WindForce*multiplier,0f,elapsedTime));
			yield return 0;
		}
	}
	
	protected IEnumerator AdmitElectricDamageSound(){
		yield return new WaitForSeconds(f_electricDamageSoundRate);
		b_electricDamageSoundOk = true;
	}
#endregion

#region Unity Overrides
	protected virtual void Start(){
		maestro = Maestro.Instance;
        col_attachedCollider = GetComponent<Collider>();
	}
	
    void OnEnable() {
        if (go_indicator) {
            InvokeRepeating("Notify", 0, Constants.ObjectiveStats.C_NotificationTimer);
        }
    }
#endregion
}
