/*  Enemy Controller - Noah Nam & Jeff Brown
 * 
 *  Desc:   Defines base functionality of enemy bots
 * 
 */
 
using UnityEngine;
using System.Linq;
using System;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]

public abstract class EnemyController : SpellTarget {

    [SerializeField] protected UnityEngine.AI.NavMeshAgent nma_agent;

    //Added WANDER and FLEE states
    protected enum State {CHASE, ATTACK, FROZEN, SLOWED, DIE, WANDER, FLEE, SUMMONING, DROPPING, BREAKOUT};
	protected float f_damage;
	protected State e_state;
	protected State e_previousState; //Used for returning to the state previous to entering the AttackState.
	protected State[] e_statusPriorityList = new State[] {State.FROZEN,State.SLOWED};
	protected float f_canMove = 1f;

	//The random destination the bot chooses when wandering
	protected Vector3 v3_destination;

	//The radius of which the bot will pick it's random destination
	protected float f_wanderingRadius = Constants.EnemyStats.C_WanderingRadius;

	//The a f_timer that keeps track of how long a bot has been wandering
	protected float f_timer;

	//The time limit for the bot to wander
	protected float f_timeLimit = 4.0f;


    override public void ApplySpellEffect(Constants.SpellStats.SpellType spell, Constants.Global.Color color, float damage, Vector3 direction) {
        if( e_state != State.DIE )
        {
            switch (spell)
            {
                case Constants.SpellStats.SpellType.MAGICMISSILE:
                    maestro.PlayEnemyHit();
                    break;
                case Constants.SpellStats.SpellType.WIND:
                    StartCoroutine(WindPush(Constants.EnemyStats.C_SkeletonWindPushMultiplier, direction, false));
                    maestro.PlayEnemyHit();
                    break;
                case Constants.SpellStats.SpellType.ICE:
                    Freeze();
                    maestro.PlayEnemyHit();
                    break;
                case Constants.SpellStats.SpellType.ELECTRICITYAOE:
                    Slow();
                    if (b_electricDamageSoundOk)
                    {
                        maestro.PlayEnemyHit();
                        b_electricDamageSoundOk = false;
                        StartCoroutine("AdmitElectricDamageSound");
                    }
                    break;
            }
            TakeDamage(damage, color);
        }
       
        
    }

    override public void NegateSpellEffect(Constants.SpellStats.SpellType spell) {
        if (spell == Constants.SpellStats.SpellType.ELECTRICITYAOE) {
            Unslow();
        }
    }

	protected virtual void EnterStateChase() {
		e_state = State.CHASE;
    }

    protected virtual void UpdateChase() {}

	protected virtual void EnterStateFlee() {
		e_state = State.FLEE;
    }

    protected virtual void UpdateFlee() {}

	protected virtual void EnterStateWander() {
		e_state = State.WANDER;
    }

    protected virtual void UpdateWander() {
    }

	protected virtual void EnterStateSummoning() {
		e_state = State.SUMMONING;
    }

    protected virtual void UpdateSummoning() {
    }

	protected virtual void EnterStateDropping() {
		e_state = State.DROPPING;
    }

    protected virtual void UpdateDropping() {
    }

    protected virtual void EnterStateAttack() {
		if(e_state != State.ATTACK)
			e_previousState = e_state;
        e_state = State.ATTACK;
    }

    protected virtual void UpdateAttack() {
        rb.velocity = Vector3.zero;
    }

    protected virtual void DoAttack() {
    }

    protected void AttackOver() {
        anim.SetBool("isAttacking",false);
		switch (e_previousState) {
		case State.SLOWED:
			UpdateSlowed();
			break;
		default:
			EnterStateChase ();
			break;
		}
    }

    protected virtual void EnterStateDie(Constants.Global.Color color) {
		e_state = State.DIE;
		this.enabled = false;
        rb.freezeRotation = true;
        //gameObject.SetActive(false);	
        nma_agent.enabled = false;
        if (col_attachedCollider != null)
            col_attachedCollider.enabled = false;
        anim.SetTrigger("doDying");
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation( )
    {
      
        yield return new WaitForSeconds(5f);
        
        gameObject.SetActive(false);
    }

    protected virtual void UpdateDie() {
        //riftController.DecreaseEnemies(e_side);
		//Destroy(gameObject);
		//gameObject.SetActive(false);
    }
	
	public virtual void TakeDamage(float damage, Constants.Global.Color color){
		//If for some reason this enemy is dead but it's still taking damage
		//This if statement will prevent it
		if (gameObject.activeSelf) {
			f_health -= damage;
			//Debug.Log(i_health);
			if(f_health <= 0f){
				//Debug.Log("death");
                if( e_state != State.DIE)
				    EnterStateDie(color);
			}
		}
	}
	
	protected virtual void EnterStateFrozen() {
		e_state = State.FROZEN;
		f_canMove = 0;
		UpdateSpeed();
		//nma_agent.isStopped = true;
		Invoke("Unfreeze", Constants.SpellStats.C_IceFreezeTime);
    }

    protected virtual void UpdateFrozen() {}
	
	public void Freeze(){
		EnterStateFrozen();
	}

	private void Unfreeze(){
		f_canMove = 1f;
		UpdateSpeed();
		EnterStateChase();
	}
	
	protected virtual void EnterStateSlowed() {
		if(e_statusPriorityList.Contains(e_state) && Array.IndexOf(e_statusPriorityList,State.SLOWED) > Array.IndexOf(e_statusPriorityList,e_state))
			return;
		e_state = State.SLOWED;
		f_canMove = Constants.SpellStats.C_ElectricAOESlowDownMultiplier;
		UpdateSpeed();
    }

    protected virtual void UpdateSlowed() {
    }
	
	public void Slow(){
		EnterStateSlowed();
	}

	public void Unslow(){
        f_canMove = 1f;
		UpdateSpeed();
		EnterStateChase();
	}
	
	private void UpdateSpeed(){
        nma_agent.speed = riftController.EnemySpeed * f_canMove;
		//nma_agent.speed = riftController.f_enemySpeed * f_canMove;
		//nma_agent.acceleration = nma_agent.acceleration* (Constants.EnviroStats.C_EnemySpeed / 3.5f) * f_canMove;
	}

	protected virtual void EnterStateBreakout() {
		e_state = State.BREAKOUT;
    }

    protected virtual void UpdateBreakout() {}

	//If the bot tries to move to a destination that's out of bounds
	//This will reset the destination with in bounds
	protected void CheckOutOfBounds() {
		if (e_startSide == Constants.Global.Side.LEFT) {
			if (v3_destination.x < -1*Constants.EnemyStats.C_MapBoundryXAxis+1.5) {
				v3_destination.x = -1*Constants.EnemyStats.C_MapBoundryXAxis+1.5f;
			}
			else if (v3_destination.x > -1.5f) {
				v3_destination.x = -1.5f;
			}
		}
		else {
			if (v3_destination.x > Constants.EnemyStats.C_MapBoundryXAxis-1.5) {
				v3_destination.x = Constants.EnemyStats.C_MapBoundryXAxis-1.5f;
			}
			else if (v3_destination.x < 1.5f) {
				v3_destination.x = 1.5f;
			}
		}

		if (v3_destination.z > Constants.EnemyStats.C_MapBoundryZAxis-1.5) {
			v3_destination.z = Constants.EnemyStats.C_MapBoundryZAxis-1.5f;
		}
		else if (v3_destination.z < -1*Constants.EnemyStats.C_MapBoundryZAxis+1.5) {
			v3_destination.z = -1*Constants.EnemyStats.C_MapBoundryZAxis+1.5f;
		}
	}

	public virtual void Init(Constants.Global.Side side) {
        //GameObject enemyIndi = Instantiate(go_enemyIndiPrefab, transform.position, Quaternion.identity);
        //CameraFacingBillboard cfb_this = enemyIndi.GetComponent<CameraFacingBillboard>();
        //cfb_this.Init(cam_camera, gameObject);

        //go_enemyIndiPrefab.ena
        this.enabled = true;
        rb.freezeRotation = false;
        nma_agent.enabled = true;

        riftController = RiftController.Instance;   // Init() is called before Start(), these must be set here (repeatedly...)
        maestro = Maestro.Instance;
        e_startSide = side;
	EnterStateWander();
	}

    protected override void Start() {
		f_damage = Constants.EnemyStats.C_EnemyDamage;

		//nma_agent.speed = Constants.EnemyStats.C_EnemyBaseSpeed;

		nma_agent.acceleration = nma_agent.acceleration* (Constants.EnemyStats.C_EnemyBaseSpeed / 3.5f);

		//Initializing the f_timer, destination, and f_wanderingRadius
		f_timer = 0.0f;
		v3_destination = transform.position;

		EnterStateWander ();
    }

    void OnDisable() {
        // @Jeff, this is no longer necessary because of new necro behavior, right?
		//if (this.enabled) {
		//	Debug.Log("OnDisable");
		//	EnterStateDie(Constants.Global.Color.Null);
		//}
	}				 
	// Update is called once per frame
	protected virtual void Update () {
		/*
		if(f_health <= 0f){
			Debug.Log("death");
			EnterStateDie();
		}
		*/

		switch (e_state) {
		case State.CHASE:
			UpdateChase ();
			break;
		case State.WANDER:
			UpdateWander ();
			break;
		case State.FLEE:
			UpdateFlee ();
			break;
		case State.SUMMONING:
			UpdateSummoning ();
			break;
		case State.DROPPING:
			UpdateDropping ();
			break;
		case State.ATTACK:
			UpdateAttack();
			break;
		case State.FROZEN:
			UpdateFrozen();
			break;
		case State.SLOWED:
			UpdateSlowed();
			break;
		case State.DIE:
			UpdateDie ();
			break;
		case State.BREAKOUT:
			UpdateBreakout();
			break;
		}
    }
}
