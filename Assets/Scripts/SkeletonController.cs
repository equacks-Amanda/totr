/*  Skeleton Controller - Noah Nam and Jeff Brown
 * 
 *  Desc:   Extends chase, attack, and wander of the enemycontroller
 * 
 */
 
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class SkeletonController : EnemyController {
	
	private GameObject go_closestTarget;


	public override void Init(Constants.Global.Side side) {
		base.Init(side);
		nma_agent.speed = riftController.EnemySpeed;
        f_health = Constants.EnemyStats.C_EnemyHealth;
		maestro.PlaySkeletonSpawn();
	}

    protected override void UpdateChase() {

		base.UpdateChase();
		go_closestTarget = null;
		float f_minDistance = 9999f;
		float f_currentDistance = 0;
		for(int i = 0; i < riftController.go_playerReferences.Length; i++){
			if(riftController.go_playerReferences[i])
				f_currentDistance = Vector3.Distance(riftController.go_playerReferences[i].transform.position,transform.position);
			if(riftController.go_playerReferences[i].GetComponent<PlayerController>().Side == e_startSide && f_currentDistance < f_minDistance && riftController.go_playerReferences[i].GetComponent<PlayerController>().Wisp == false){
				go_closestTarget = riftController.go_playerReferences[i];
				f_minDistance = f_currentDistance;
			}
		}
		
		
		if(go_closestTarget){
			nma_agent.isStopped = false;
			nma_agent.SetDestination(go_closestTarget.transform.position);
			if(Vector3.Distance(transform.position,go_closestTarget.transform.position) < Constants.EnemyStats.C_EnemyAttackRange)
				EnterStateAttack();
		}
		else{
			EnterStateWander();
			//nma_agent.isStopped = true;
		}
			
    }

	private void Wander() {
		f_timer += Time.deltaTime;

        if (f_timer >= f_timeLimit || Vector3.Distance(transform.position, v3_destination) <= 1.0f ) {

			v3_destination = GetWanderPos(transform.position, f_wanderingRadius);
			CheckOutOfBounds();

            nma_agent.SetDestination(v3_destination);

            f_timer = 0;
        }
	}

    private static Vector3 GetWanderPos(Vector3 transform, float f_wanderingRadius) {
 
		float angle = Random.Range(0, 2 * Mathf.PI);

		float deltaZ = Mathf.Sin(angle)*f_wanderingRadius;
		float deltaX = Mathf.Cos(angle)*f_wanderingRadius;

		//TODO: While loop to keep checking if deltaX and deltaZ are within bounds

		Vector3 position = new Vector3(transform.x + deltaX, 0, transform.z + deltaZ);
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (position, out navHit, f_wanderingRadius, -1);

        return navHit.position;
    }

	protected override void UpdateWander() {

		base.UpdateWander();
		bool b_playersAvailable = false;
        if( riftController != null )
        {
            if(riftController.go_playerReferences != null )
            {
                for (int i = 0; i < riftController.go_playerReferences.Length; i++)
                {
                    if (riftController.go_playerReferences[i].GetComponent<PlayerController>().Side == e_startSide && riftController.go_playerReferences[i].GetComponent<PlayerController>().Wisp == false)
                    {
                        b_playersAvailable = true;
                        break;
                    }
                }
            }
        }
		

		if (b_playersAvailable) {
			EnterStateChase();
			anim.SetBool("isRunning", true);

		}
		else {
			 Wander();
			anim.SetBool("isRunning", false);
		}
	}
	
	protected override void EnterStateDie(Constants.Global.Color color) {
		riftController.DecreaseEnemies(e_startSide);									  
		base.EnterStateDie(color);
		maestro.PlaySkeletonDie();
    }

	protected override void UpdateDie() {
		base.UpdateDie();
	}

	protected override void EnterStateWander() {
		base.EnterStateWander();
		//Reset f_timer
		f_timer = f_timeLimit;
	}

    protected override void EnterStateAttack() {
        rb.velocity = Vector3.zero;
		base.EnterStateAttack();
        DoAttack();
    }

    protected override void DoAttack() {
		base.DoAttack();
        rb.velocity = Vector3.zero;
        if (go_closestTarget) {
            anim.SetBool("isAttacking", true);
            go_closestTarget.GetComponent<PlayerController>().TakeDamage(Constants.EnemyStats.C_EnemyDamage,Constants.Global.DamageType.ENEMY);
		}
    }
	
	protected override void UpdateSlowed() {
		base.UpdateSlowed();

		//There are some instances where go_closestTarget is null, this check prevents a null reference exception
		if (go_closestTarget) {
			if(Vector3.Distance(transform.position,go_closestTarget.transform.position) < Constants.EnemyStats.C_EnemyAttackRange) {
				EnterStateAttack();
			}
			else {
				EnterStateChase();
			}
		}
		else {
			EnterStateWander();
		}
	}

    void OnEnable() {
        // do nothing; need to override the InvokeRepeating in SpellTarget
    }
}