/*  Necromancer Controller - Jeff Brown
 * 
 *  Desc:   Extends wander state from enemy controller, implements riftController for summoning and rune dropping
 * 
 */
 
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NecromancerController : EnemyController {
#region Variables and Declarations
    [SerializeField] private DefeatNecromancersObjective dno_owner;    // identifies objective necromancer is a part of
    private bool b_teleported = false;
    private Constants.Global.Side e_currentSide;
#endregion

	public override void Init(Constants.Global.Side side) {
        base.Init(side);
        e_currentSide = side;
        nma_agent.enabled = true;
		nma_agent.speed = Constants.EnemyStats.C_NecromancerBaseSpeed;
		f_health = Constants.EnemyStats.C_NecromancerHealth;
        b_teleported = false;
		InvokeRepeating("DropRune", 10.0f, Constants.EnemyStats.C_RuneTimer);
		InvokeRepeating("Summon", 16.0f, Constants.EnemyStats.C_SummonTimer);
		maestro.PlayNecromancerSpawn();
	}
	
	override public void ApplySpellEffect(Constants.SpellStats.SpellType spell, Constants.Global.Color color, float damage, Vector3 direction) {
        switch(spell) {
			case Constants.SpellStats.SpellType.MAGICMISSILE:
				maestro.PlayEnemyHit();
				break;
            case Constants.SpellStats.SpellType.WIND:
                StartCoroutine(WindPush(Constants.EnemyStats.C_NecromancerWindPushMultiplier,direction,true));
				maestro.PlayEnemyHit();
                break;
            case Constants.SpellStats.SpellType.ICE:
                Freeze();
				maestro.PlayEnemyHit();
                break;
            case Constants.SpellStats.SpellType.ELECTRICITYAOE:
                Slow();
				if(b_electricDamageSoundOk){
					maestro.PlayEnemyHit();
					b_electricDamageSoundOk = false;
					StartCoroutine("AdmitElectricDamageSound");
				}
                break;
        }
        TakeDamage(damage, color);
    }

	protected override void Update() {
		base.Update();
	}

    protected override void EnterStateChase() {
        EnterStateWander(); // necromancer has no chase state, wander instead
    }

    protected override void UpdateWander() {
		base.UpdateWander();

		if (IsCornered(2.0f) == true) {
			f_timer = 0;
			EnterStateBreakout();
		}
		else {
			bool b_playersAvailable = false;
			for(int i = 0; i < riftController.go_playerReferences.Length; i++){	
				if(riftController.go_playerReferences[i].GetComponent<PlayerController>().Side == e_startSide && riftController.go_playerReferences[i].GetComponent<PlayerController>().Wisp == false){
					if (Vector3.Distance(riftController.go_playerReferences[i].transform.position, transform.position) < Constants.EnemyStats.C_NecromancerAvoidDistance) {
						b_playersAvailable = true;
						break;
					}
				}
			}

			if (b_playersAvailable) {
				EnterStateFlee();
			}
			else {
				 Wander();
			}
		}
    }

	//TODO: delete dropping and summoning states, need to be discussed first

	private void Wander() {
		f_timer += Time.deltaTime;

        if (f_timer >= f_timeLimit || Vector3.Distance(transform.position, v3_destination) <= 1.0f ) {

			//bool b_isDestinationValid = false;

			//while(b_isDestinationValid == false) {
				v3_destination = GetWanderPos(transform.position, f_wanderingRadius);
				CheckOutOfBounds();
				//b_isDestinationValid = IsWithinBounds(transform.position, e_side);
			//}

            nma_agent.SetDestination(v3_destination);

            f_timer = 0;
        }
	}

    private static Vector3 GetWanderPos(Vector3 transform, float f_wanderingRadius) {
 
		float angle = Random.Range(0, 2 * Mathf.PI);
		float deltaZ = Mathf.Sin(angle)*f_wanderingRadius;
		float deltaX = Mathf.Cos(angle)*f_wanderingRadius;
		Vector3 position = new Vector3(transform.x + deltaX, 0, transform.z + deltaZ);
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (position, out navHit, f_wanderingRadius, -1);
 
        return navHit.position;
    }

	protected override void EnterStateBreakout() {
		base.EnterStateBreakout();
		float z = Random.Range(-1.0f*(Constants.EnemyStats.C_MapBoundryZAxis-4.0f), (Constants.EnemyStats.C_MapBoundryZAxis-4.0f));
		float x = Random.Range(5.0f, (Constants.EnemyStats.C_MapBoundryXAxis-5.0f));
		if (transform.position.x < 0) {
			x = -1 * x;
		}
		v3_destination = new Vector3(x,0,z);
		nma_agent.SetDestination(v3_destination);
	}

	protected override void UpdateBreakout() {
		f_timer += Time.deltaTime;

		if (f_timer >= f_timeLimit || Vector3.Distance(transform.position, v3_destination) <= 2.0f ) {
			f_timer = 0;
			EnterStateWander();
		}
	}

	private void FindFleeDestination() {
		int count = 0;

		float angle = 0.0f;
		float sumAngle = 0.0f;

		Vector3 dir;


		for(int i = 0; i < riftController.go_playerReferences.Length; i++){
			if(riftController.go_playerReferences[i].GetComponent<PlayerController>().Side == e_startSide && riftController.go_playerReferences[i].GetComponent<PlayerController>().Wisp == false) {
				if (Vector3.Distance(riftController.go_playerReferences[i].transform.position, transform.position) < Constants.EnemyStats.C_NecromancerAvoidDistance) {

					count = count + 1;

					dir = riftController.go_playerReferences[i].transform.InverseTransformDirection((riftController.go_playerReferences[i].transform.position - transform.position));

					angle = (Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg)+180.0f;

					if (angle >= 360)
						angle = angle-360.0f;

					sumAngle = sumAngle + angle;
				}

			}
		}

		if (count > 0) {
			angle = sumAngle/count;
			float deltaZ = Mathf.Sin((angle * Mathf.PI)/180)*Constants.EnemyStats.C_NecromancerAvoidDistance;
			float deltaX = Mathf.Cos((angle * Mathf.PI)/180)*Constants.EnemyStats.C_NecromancerAvoidDistance;
			v3_destination = new Vector3(transform.position.x + deltaX, 0, transform.position.z + deltaZ);
			CheckOutOfBounds();
			nma_agent.SetDestination(v3_destination);
		}
		else {
			f_timer = f_timeLimit;
			EnterStateWander();
		}
	}

	protected override void UpdateFlee() {

		base.UpdateFlee();

		if (IsCornered(3.0f) == true) {
			f_timer = 0;
			EnterStateBreakout();
		}
		
		else {
			FindFleeDestination();
		}

	}

	protected override void EnterStateWander() {
		base.EnterStateWander();
		//Reset f_timer
		f_timer = f_timeLimit;
	}
	
	protected override void EnterStateDie(Constants.Global.Color color) {
		CancelInvoke();
        maestro.PlayNecromancerDie();
        base.EnterStateDie(color);
        if(color != Constants.Global.Color.NULL && e_color == color) {
            dno_owner.UpdateNecroScore();
        }
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, -1000.0f, gameObject.transform.localPosition.z);
        //gameObject.SetActive(false);    // nav mesh must be turned off before moving
        Invoke("ResetNecroPosition", Constants.ObjectiveStats.C_NecromancerSpawnTime);
    }

	protected override void UpdateDie() {
		base.UpdateDie();
	}

    public override void TakeDamage(float damage, Constants.Global.Color color) {
        CancelInvoke("Notify");
        InvokeRepeating("Notify", Constants.ObjectiveStats.C_NotificationTimer, Constants.ObjectiveStats.C_NotificationTimer);

        base.TakeDamage(damage, color);
        if(f_health < (Constants.ObjectiveStats.C_NecromancerTeleportHealthThreshold * Constants.EnemyStats.C_NecromancerHealth) && !b_teleported) {
            b_teleported = true;
            e_currentSide = (Constants.Global.Side) (-1 * (int)e_startSide);
            NegateSpellEffect(Constants.SpellStats.SpellType.ELECTRICITYAOE);    // manually stop the coroutine if it's running
            gameObject.SetActive(false);    // nav mesh must be turned off before moving
            gameObject.transform.localPosition = new Vector3(-gameObject.transform.localPosition.x, 0.5f, gameObject.transform.localPosition.z);
            gameObject.SetActive(true);
        }
    }

    private void DropRune() {
		riftController.ActivateRune(transform.position);
	}

	private void Summon() {
		for (int i = 0; i < 2; i++) {
			riftController.CircularEnemySpawn(transform.position, e_currentSide);
		}
	}

    private void ResetNecroPosition() {
        if (e_color == Constants.Global.Color.RED) {
            transform.localPosition = Constants.ObjectiveStats.C_RedNecromancerSpawn;
        }
        else {
            transform.localPosition = Constants.ObjectiveStats.C_BlueNecromancerSpawn;
        }
        b_teleported = false;
        gameObject.SetActive(true);
        Init(e_startSide);
    }

	private bool IsCornered(float length) {
		if (transform.position.x <= length || transform.position.x <= -1*Constants.EnemyStats.C_MapBoundryXAxis+length) {
			if ((transform.position.z >= Constants.EnemyStats.C_MapBoundryZAxis-length)) {
				return true;
			}
			else if ((transform.position.z <= -1*Constants.EnemyStats.C_MapBoundryZAxis+length)) {
				return true;
			}
		}
		else if (transform.position.x >= -length || transform.position.x >= Constants.EnemyStats.C_MapBoundryXAxis-length) {
			if ((transform.position.z >= Constants.EnemyStats.C_MapBoundryZAxis-length)) {
				return true;
			}
			else if ((transform.position.z <= -1*Constants.EnemyStats.C_MapBoundryZAxis+length)) {
				return true;
			}
		}
		return false;
	}

#region Unity Overrides	
    protected override void Start() {
        if (e_color == Constants.Global.Color.RED)
            Init(Constants.Global.Side.LEFT);
        else
            Init(Constants.Global.Side.RIGHT);
    }
#endregion
}