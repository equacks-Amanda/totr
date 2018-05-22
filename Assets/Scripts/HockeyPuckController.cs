/*  Hockey Puck Controller - Dana Thompson
 * 
 *  Desc:   Controls changes to Ice Hockey Objective's Puck movement and speed
 * 
 */
using System.Collections;
using UnityEngine;

public class HockeyPuckController : SpellTarget {
#region Variables and Declarations
    [SerializeField] private IceHockeyObjective iho_owner;    // identifies objective puck is a part of
    [SerializeField] private GameObject go_scoreParticle;
    #endregion

    #region HockeyPuckController Methods
    override public void ApplySpellEffect(Constants.SpellStats.SpellType spell, Constants.Global.Color color, float damage, Vector3 direction) {

        CancelInvoke("DecreaseSpeed");     // reset slowdown invoke
        InvokeRepeating("DecreaseSpeed", Constants.ObjectiveStats.C_PuckSpeedDecayDelay, Constants.ObjectiveStats.C_PuckSpeedDecayRate);

        switch (spell) {
            case Constants.SpellStats.SpellType.WIND:
                f_speed += Constants.ObjectiveStats.C_PuckSpeedHitIncrease;
                StartCoroutine(WindPush(Constants.ObjectiveStats.C_PuckWindPushMultiplier,direction, false));
                transform.Rotate(direction);
				maestro.PlayPuckBounce();
                break;
            case Constants.SpellStats.SpellType.ICE:
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                f_speed = Constants.ObjectiveStats.C_PuckBaseSpeed;
                break;
            case Constants.SpellStats.SpellType.ELECTRICITYAOE:
                f_speed = 0.5f * Constants.ObjectiveStats.C_PuckBaseSpeed;
                break;
        }
    }

    public void ResetPuckPosition() {
        if (e_color == Constants.Global.Color.RED) {
            transform.localPosition = Constants.ObjectiveStats.C_RedPuckSpawn;
        }
        else {
            transform.localPosition = Constants.ObjectiveStats.C_BluePuckSpawn;
        }

        //stop its movement entirely
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        f_speed = Constants.ObjectiveStats.C_PuckBaseSpeed;
    }

    private void DecreaseSpeed() {
        f_speed -= Constants.ObjectiveStats.C_PuckSpeedDecreaseAmount;
    }

    private void DisableParticle()
    {
        go_scoreParticle.SetActive(false);
    }

    ////if the puck gets stuck in the portal, move it over from it and reset its speed
    //private void PuckIsStuckInPortal() {

    //    Vector3 v3_rightPortal = new Vector3(37.25f, 0.5f, -15.25f);
    //    Vector3 v3_leftPortal = new Vector3(-37.25f, 0.5f, 15.25f);
    //    if (transform.position == v3_rightPortal) {
    //        transform.position = new Vector3(32.25f, 0.5f, -15.25f);
    //    }
    //    if (transform.position == v3_leftPortal) {
    //        transform.position = new Vector3(-32.25f, 0.5f, 15.25f);
    //    }

    //    isPuckStuck = false;
    //}

    #endregion

    #region Unity Overrides
    protected override void Start() {
		base.Start();
        f_speed = Constants.ObjectiveStats.C_PuckBaseSpeed;     // cannot read from Constants.cs in initialization at top
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    void Update() {
        // resets speed if it goes over threshold
        if (f_speed > Constants.ObjectiveStats.C_PuckMaxSpeed) {
            f_speed = Constants.ObjectiveStats.C_PuckMaxSpeed;
        } else if (f_speed < Constants.ObjectiveStats.C_PuckBaseSpeed && f_speed != (0.5f * Constants.ObjectiveStats.C_PuckBaseSpeed)) {
            CancelInvoke("DecreaseSpeed");
            f_speed = Constants.ObjectiveStats.C_PuckBaseSpeed;
        }

        Vector3 v3_dir = rb.velocity.normalized;
        rb.velocity = v3_dir * f_speed;
    }

    void OnCollisionEnter(Collision collision) {
         if (!collision.gameObject.CompareTag("Rift") && !collision.gameObject.CompareTag("Portal") && !collision.gameObject.CompareTag("Spell")) {
            // Reflect puck on collision
            // https://youtube.com/watch?v=u_p50wENBY
            Vector3 v = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
            float rot = 90 - Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(90, rot, 0);
            transform.Rotate(v);
			maestro.PlayPuckBounce();
            rb.velocity = transform.forward * f_speed;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("HockeyGoal")) {   // player scoring with puck
            if (other.GetComponent<GoalController>().Color != e_color) {
                Vector3 pos = transform.position;
                go_scoreParticle.transform.position = pos;
                go_scoreParticle.SetActive(true);
                Invoke("DisableParticle", Constants.ObjectiveStats.C_ScoringParticleLiveTime);

                ResetPuckPosition();
                iho_owner.UpdatePuckScore();
                iho_owner.StopCoroutine("Notify");
                iho_owner.StartCoroutine("Notify");
            }
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("Player")) {
            StartCoroutine("ApplyDamage", other.gameObject);
        } else if (other.CompareTag("OutofBounds")) {
            ResetPuckPosition();
        }
    }

    void OnTriggerExit(Collider other) {
        StopCoroutine("ApplyDamage");
    }

    public IEnumerator ApplyDamage(GameObject go_target) {
        if (go_target.GetComponent<PlayerController>()) {
            go_target.GetComponent<PlayerController>().TakeDamage(Constants.ObjectiveStats.C_PuckDamage, Constants.Global.DamageType.PUCK);
        } else if (go_target.GetComponent<EnemyController>()) { //TODO: this can never happen now, right?  right?
            go_target.GetComponent<EnemyController>().TakeDamage(Constants.ObjectiveStats.C_PuckDamage, e_color);
        }

        yield return new WaitForSeconds(1);
        StartCoroutine("ApplyDamage", go_target);
    }
    #endregion
}
