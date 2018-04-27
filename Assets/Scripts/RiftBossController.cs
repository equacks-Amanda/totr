/*  Rift Boss Controller - Dana Thompson
 * 
 *  Desc:   Controls how the Rift Boss works
 * 
 */

using UnityEngine;

public class RiftBossController : SpellTarget {
#region Variables and Declarations
    [SerializeField] private RiftBossObjective rbo_owner;  // identifies objective rift boss is a part of
    [SerializeField] private GameObject go_ForceField;
    [SerializeField] private GameObject[] go_runes;
    [SerializeField] private GameObject[] go_skeletons;
	private bool b_firstShield, b_secondShield = false;
#endregion

#region RiftBossController Methods
    public void TakeDamage(float damage) {
        if (!go_ForceField.activeSelf) {
            f_health -= damage;
            rbo_owner.UpdateRiftBossHealth(f_health);
            CancelInvoke("Notify");
            InvokeRepeating("Notify", Constants.ObjectiveStats.C_NotificationTimer, Constants.ObjectiveStats.C_NotificationTimer);
        }
    }

    override public void ApplySpellEffect(Constants.SpellStats.SpellType spell, Constants.Global.Color color, float damage, Vector3 direction) {
        if (color == e_color) {
            TakeDamage(damage);
        }
    }
    
    private void SpawnRunes() {
        foreach (GameObject runes in go_runes) {
            if (!runes.activeSelf) {
                runes.SetActive(true);
            }
        }
        anim.SetTrigger("runeTrigger");
    }

    private void FireDeathBolts() {
        riftController.FireDeathBolts(gameObject.transform, e_color);
        //go_ForceField.SetActive(false);
        //Invoke("TurnOnForceField", Constants.ObjectiveStats.C_ForceFieldCooldown);
        anim.SetTrigger("deathboltTrigger");
    }

    private void TurnOnForceField() {
        go_ForceField.SetActive(true);

        if (e_color == riftController.GetRiftBossWinningTeamColor())
        {
            ActivateEnemies(Constants.ObjectiveStats.C_RiftBossEnemies);
        }
        else
        {
            ActivateEnemies(Constants.ObjectiveStats.C_RiftBossEnemies-1);
        }
    }
	
	private void ActivateEnemies(float skeletonSpawnCount){
        for (int i = 0; i < skeletonSpawnCount; i++) {
            // move skeleton into position - must happen before Init() is called
            go_skeletons[i].transform.position = go_runes[i].transform.position;
            go_skeletons[i].GetComponent<SkeletonController>().Init(e_startSide);
            go_skeletons[i].SetActive(true);
        }
    }
#endregion

#region Unity Overrides
    protected override void Start() {
        riftController = RiftController.Instance;
        if (e_color == Constants.Global.Color.RED) {
            f_health = (Constants.ObjectiveStats.C_RiftBossMaxHealth - (Constants.ObjectiveStats.C_RiftBossHealthReductionMultiplier * Constants.TeamStats.C_RedTeamScore));     // cannot read from Constants.cs in initialization at top
            rbo_owner.UpdateRiftBossHealth(f_health);
        }
        else if (e_color == Constants.Global.Color.BLUE) {
            f_health = (Constants.ObjectiveStats.C_RiftBossMaxHealth - (Constants.ObjectiveStats.C_RiftBossHealthReductionMultiplier * Constants.TeamStats.C_BlueTeamScore));     // cannot read from Constants.cs in initialization at top
            rbo_owner.UpdateRiftBossHealth(f_health);
        }

        InvokeRepeating("FireDeathBolts", Constants.ObjectiveStats.C_DeathBoltCooldown, Constants.ObjectiveStats.C_DeathBoltCooldown + Constants.ObjectiveStats.C_ForceFieldCooldown);
        InvokeRepeating("SpawnRunes", Constants.ObjectiveStats.C_RuneSpawnInterval, Constants.ObjectiveStats.C_RuneSpawnInterval);
		TurnOnForceField();
    }

    private void Update() {
        if ((f_health <= (Constants.ObjectiveStats.C_RiftBossMaxHealth * 0.6667)) && !b_firstShield) {
			b_firstShield = true;
            TurnOnForceField();
        }
		else if ((f_health <= (Constants.ObjectiveStats.C_RiftBossMaxHealth * 0.3334)) && !b_secondShield) {
			b_secondShield = true;
            TurnOnForceField();
        }

        if (go_ForceField.activeSelf) {
            int inactiveSkeletonCount = 0;

            for (int i = 0; i < 3; i++) {
                if (!go_skeletons[i].activeSelf) {
                    inactiveSkeletonCount++;
                }
            }

            if (inactiveSkeletonCount == 3) {
                go_ForceField.SetActive(false);
            }
        }
    }
    #endregion
}
