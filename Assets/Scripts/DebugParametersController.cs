/*  Debug Parameters Controller - Sam Caulker
 * 
 *  Desc:   Allows players to change the values stored in Constants.cs via interactable sliders
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Rewired;

public class DebugParametersController : MonoBehaviour {
    // Public Vars
    [SerializeField] PlayerSelectController psc_master;

    // Menu buttons
    [SerializeField] private Button butt_playerSelect;
    [SerializeField] private Button butt_spellSelect;
    [SerializeField] private Button butt_enemySelect;
    [SerializeField] private Button butt_objectiveSelect;
    private Button[] butt_buttonArray = new Button[4];
    
    //Other button references (for navigation)
    [SerializeField]private Button butt_go;

    // Menu organization
    [SerializeField] private GameObject go_topMenu;
    [SerializeField] private GameObject go_select;
    [SerializeField] private GameObject go_regController;

    [SerializeField] private GameObject go_playerMenu;
    [SerializeField] private GameObject go_spellMenu;
    [SerializeField] private GameObject go_enemyMenu;
    [SerializeField] private GameObject go_objectiveMenu;
    private GameObject[] go_menuArray = new GameObject[4];
    private int currentMenu = 0;
    private Player p_uiPlayer;

    // UI sliders (set in editor)
    [SerializeField] private Slider slider_playerMoveSpeed;
    [SerializeField] private Slider slider_windSpeed;
    [SerializeField] private Slider slider_iceSpeed;
    [SerializeField] private Slider slider_electricSpeed;
    [SerializeField] private Slider slider_windCooldown;
    [SerializeField] private Slider slider_iceCooldown;
    [SerializeField] private Slider slider_electricCooldown;
    [SerializeField] private Slider slider_magicMissileSpeed;
    [SerializeField] private Slider slider_magicMissileHeal;
    [SerializeField] private Slider slider_magicMissileFireRate;
    [SerializeField] private Slider slider_projSize;
    [SerializeField] private Slider slider_projLife;
    [SerializeField] private Slider slider_windForce;
    [SerializeField] private Slider slider_iceFreeze;
    [SerializeField] private Slider slider_electricLiveTime;
    [SerializeField] private Slider slider_enemySpawn;
    [SerializeField] private Slider slider_enemySpeed;
    [SerializeField] private Slider slider_enemyHealth;
    [SerializeField] private Slider slider_necromancerHealth;
    [SerializeField] private Slider slider_enemyDamage;
    [SerializeField] private Slider slider_respawnTime;
    [SerializeField] private Slider slider_wispMoveSpeed;
    [SerializeField] private Slider slider_playerHealth;
    [SerializeField] private Slider slider_crystalHealth;
    [SerializeField] private Slider slider_crystalHealthRegen;
    [SerializeField] private Slider slider_crystalHealthRegenRate;
    [SerializeField] private Slider slider_crystalHealthRegenDelay;
    [SerializeField] private Slider slider_CTFScore;
    [SerializeField] private Slider slider_completionTimer;
    [SerializeField] private Slider slider_selfDestructTimer;
    [SerializeField] private Slider slider_enemySpawnCap;
    [SerializeField] private Slider slider_hockeyMaxScore;
    [SerializeField] private Slider slider_puckDamage;
    [SerializeField] private Slider slider_puckSpeedDecayRate;
    [SerializeField] private Slider slider_puckSpeedDecreaseRate;
    [SerializeField] private Slider slider_puckBaseSpeed;
    [SerializeField] private Slider slider_puckMaxSpeed;
    [SerializeField] private Slider slider_puckHitIncreaseSpeed;
    [SerializeField] private Slider slider_necroMaxScore;
    [SerializeField] private Slider slider_riftBossHealth;
    [SerializeField] private Slider slider_runeSpawnInterval;
    [SerializeField] private Slider slider_deathBoltCooldown;
    [SerializeField] private Slider slider_forceFieldCooldown;

    // UI text (set in editor)
    [SerializeField] private Text txt_playerMoveSpeed;
    [SerializeField] private Text txt_windSpeed;
    [SerializeField] private Text txt_iceSpeed;
    [SerializeField] private Text txt_electricSpeed;
    [SerializeField] private Text txt_windCooldown;
    [SerializeField] private Text txt_iceCooldown;
    [SerializeField] private Text txt_electricCooldown;
    [SerializeField] private Text txt_magicMissileSpeed;
    [SerializeField] private Text txt_magicMissileHeal;
    [SerializeField] private Text txt_magicMissileFireRate;
    [SerializeField] private Text txt_projSize;
    [SerializeField] private Text txt_projLife;
    [SerializeField] private Text txt_windForce;
    [SerializeField] private Text txt_iceFreeze;
    [SerializeField] private Text txt_electricLiveTime;
    [SerializeField] private Text txt_enemySpawn;
    [SerializeField] private Text txt_enemySpeed;
    [SerializeField] private Text txt_enemyHealth;
    [SerializeField] private Text txt_necromancerHealth;
    [SerializeField] private Text txt_enemyDamage;
    [SerializeField] private Text txt_respawnTime;
    [SerializeField] private Text txt_wispMoveSpeed;
    [SerializeField] private Text txt_playerHealth;
    [SerializeField] private Text txt_crystalHealth;
    [SerializeField] private Text txt_crystalHealthRegen;
    [SerializeField] private Text txt_crystalHealthRegenRate;
    [SerializeField] private Text txt_crystalHealthRegenDelay;
    [SerializeField] private Text txt_CTFScore;
    [SerializeField] private Text txt_completionTimer;
    [SerializeField] private Text txt_selfDestructTimer;
    [SerializeField] private Text txt_enemySpawnCap;
    [SerializeField] private Text txt_hockeyMaxScore;
    [SerializeField] private Text txt_puckDamage;
    [SerializeField] private Text txt_puckSpeedDecayRate;
    [SerializeField] private Text txt_puckSpeedDecreaseRate;
    [SerializeField] private Text txt_puckBaseSpeed;
    [SerializeField] private Text txt_puckMaxSpeed;
    [SerializeField] private Text txt_puckHitIncreaseSpeed;
    [SerializeField] private Text txt_necroMaxScore;
    [SerializeField] private Text txt_riftBossHealth;
    [SerializeField] private Text txt_runeSpawnInterval;
    [SerializeField] private Text txt_deathBoltCooldown;
    [SerializeField] private Text txt_forceFieldCooldown;

    [SerializeField] private Text txt_playerLabel;
    [SerializeField] private Text txt_objectiveLabel;
    

    /*/////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

    // Slider change callbacks
    public void ChangePlayerSpeed(float f_playerSpeedIn) {
        txt_playerMoveSpeed.text = string.Format("{0:0}%",((slider_playerMoveSpeed.value / 6f)*100));
        Constants.PlayerStats.C_MovementSpeed = f_playerSpeedIn;
        Constants.PlayerStats.C_WispMovementSpeed = f_playerSpeedIn / 4f;
    }

    public void ChangePlayerWispSpeed(float f_playerWispSpeedIn) {
        txt_wispMoveSpeed.text = string.Format("{0:0}%",((slider_wispMoveSpeed.value / slider_wispMoveSpeed.maxValue)*100));
        Constants.PlayerStats.C_WispMovementSpeed = f_playerWispSpeedIn;
    }

    public void ChangeMagicMissileSpeed(float f_magicMissileSpeedIn) {
        txt_magicMissileSpeed.text = slider_magicMissileSpeed.value.ToString();
        Constants.SpellStats.C_MagicMissileSpeed = f_magicMissileSpeedIn;
    }

    public void ChangeMagicMissileHeal(float f__magicMissileHealIn) {
        txt_magicMissileHeal.text = slider_magicMissileHeal.value.ToString();
        Constants.SpellStats.C_MagicMissileHeal = (int)f__magicMissileHealIn;
    }

    public void ChangeMagicMissileFireRate(float f__magicMissileRate) {
        float value = f__magicMissileRate / 20.0f;
        txt_magicMissileFireRate.text = string.Format("{0:0}%",((slider_magicMissileFireRate.value / 15f)*100));
        Constants.SpellStats.C_MagicMissileCooldown = value;
    }

    public void ChangeWindSpeed(float f_windSpeedIn) {
        txt_windSpeed.text = slider_windSpeed.value.ToString();
        Constants.SpellStats.C_WindSpeed = f_windSpeedIn;
    }

    public void ChangeIceSpeed(float f_iceSpeedIn) {
        txt_iceSpeed.text = slider_iceSpeed.value.ToString();
        Constants.SpellStats.C_IceSpeed = f_iceSpeedIn;
    }

    public void ChangeElectricSpeed(float f_electricSpeedIn) {
        txt_electricSpeed.text = slider_electricSpeed.value.ToString();
        Constants.SpellStats.C_ElectricSpeed = f_electricSpeedIn;
    }

    public void ChangeWindCooldown(float f_windCooldownIn) {
        txt_windCooldown.text = string.Format("{0:0}%",((slider_windCooldown.value / 5f)*100));
        Constants.SpellStats.C_WindCooldown = f_windCooldownIn / 5f;
    }

    public void ChangeIceCooldown(float f_iceCooldownIn) {
        txt_iceCooldown.text = string.Format("{0:0}%",((slider_iceCooldown.value / 5f)*100));
        Constants.SpellStats.C_IceCooldown = f_iceCooldownIn;
    }

    public void ChangeElectricCooldown(float f_electricCooldownIn) {
        txt_electricCooldown.text = string.Format("{0:0}%",((slider_electricCooldown.value / 8f)*100));
        Constants.SpellStats.C_ElectricCooldown = f_electricCooldownIn;
    }

    public void ChangeProjectileSize(float f_projSizeIn) {
        float roundedVal = Mathf.Round(slider_projSize.value * 100f) / 100f;
        txt_projSize.text = string.Format("{0:0}%",((roundedVal / 0.5f)*100));
        Constants.SpellStats.C_PlayerProjectileSize = roundedVal;
    }

    public void ChangeSpellLifetime(float f_projLifeIn) {
        txt_projLife.text = slider_projLife.value.ToString();
        Constants.SpellStats.C_SpellLiveTime = f_projLifeIn;
    }

    public void ChangeWindForce(float f_windForceIn) {
        float value = f_windForceIn * 250.0f;
        txt_windForce.text = string.Format("{0:0}%",((slider_windForce.value / 6f)*100));
        Constants.SpellStats.C_WindForce = value;
    }

    public void ChangeFreezeDuration(float f_iceFreezeIn) {
        txt_iceFreeze.text = slider_iceFreeze.value.ToString();
        Constants.SpellStats.C_IceFreezeTime = f_iceFreezeIn;
    }

    public void ChangeElectricLiveTime(float f_electricLiveTimeIn) {
        txt_electricLiveTime.text = string.Format("{0:0}%",((slider_electricLiveTime.value / 5f)*100));
        Constants.SpellStats.C_ElectricAOELiveTime = f_electricLiveTimeIn;
    }

    public void ChangeEnemySpawnRate(float f_enemySpawnIn) {
        txt_enemySpawn.text = slider_enemySpawn.value.ToString();
        Constants.RiftStats.C_VolatilityEnemySpawnTimer = f_enemySpawnIn;
    }

    public void ChangeEnemySpeed(float f_enemySpeedIn) {
        float value = f_enemySpeedIn / 10;
        txt_enemySpeed.text = string.Format("{0:0}%",((slider_enemySpeed.value / 12f)*100));
        Constants.EnemyStats.C_EnemyBaseSpeed = value;
    }

    public void ChangeEnemyHealth(float f_enemyHealthIn) {
        float value = f_enemyHealthIn * 25.0f;
        txt_enemyHealth.text = value.ToString();
        Constants.EnemyStats.C_EnemyHealth = (int)value;
    }

    public void ChangeNecromancerHealth(float f_neckHealthIn)
    {
        float value = f_neckHealthIn * 50.0f;
        txt_necromancerHealth.text = string.Format("{0:0}%",((slider_necromancerHealth.value / 8f)*100));
        Constants.EnemyStats.C_NecromancerHealth = (int)value;
    }

    public void ChangeEnemyDamage(float f_enemyDamageIn) {
        float value = f_enemyDamageIn * 5.0f;
        txt_enemyDamage.text = string.Format("{0:0}%",((slider_enemyDamage.value / 5f)*100));
        Constants.EnemyStats.C_EnemyDamage = (int)value;
    }

    public void ChangePlayerHealth(float f_playerHealthIn) {
        float value = f_playerHealthIn * 50.0f;
        txt_playerHealth.text = string.Format("{0:0}%",((slider_playerHealth.value / 6f)*100));
        Constants.PlayerStats.C_MaxHealth = (int)value;
    }

    public void ChangeRespawnTime(float f_respawnTimeIn) {
        txt_respawnTime.text = string.Format("{0:0}%",((slider_respawnTime.value / 5f)*100));
        Constants.PlayerStats.C_RespawnTimer = f_respawnTimeIn;
    }

    //public void ChangeCrystalHealth(float f_crystalHealthIn) {
    //    float value = f_crystalHealthIn * 50.0f;
    //    txt_crystalHealth.text = value.ToString();
    //    Constants.ObjectiveStats.C_CrystalMaxHealth = (int)value;
    //}

    //public void ChangeCrystalHealthRegen(float f_crystalHealthIn)
    //{
    //    float value = f_crystalHealthIn;
    //    txt_crystalHealthRegen.text = value.ToString();
    //    Constants.ObjectiveStats.C_CrystalRegenHeal = (int)value;
    //}

    //public void ChangeCrystalHealthRegenRate(float f_crystalHealthIn)
    //{
    //    float value = f_crystalHealthIn;
    //    txt_crystalHealthRegenRate.text = value.ToString();
    //    Constants.ObjectiveStats.C_CrystalHealRate = (int)value;
    //}

    //public void ChangeCrystalHealthRegenDelay(float f_crystalHealthIn)
    //{
    //    float value = f_crystalHealthIn;
    //    txt_crystalHealthRegenDelay.text = value.ToString();
    //    Constants.ObjectiveStats.C_CrystalHealDelay = (int)value;
    //}

    public void ChangeObjMaxScore(float f_CTFScoreIn) {
        txt_CTFScore.text = string.Format("{0:0}%",((slider_CTFScore.value / 3f)*100));
        Constants.ObjectiveStats.C_CTFMaxScore = (int)f_CTFScoreIn;
        Constants.ObjectiveStats.C_NecromancersMaxScore = (int)f_CTFScoreIn;
        Constants.ObjectiveStats.C_HockeyMaxScore = (int)f_CTFScoreIn;
    }

    public void ChangeCompletionTimer(float f_timerIn) {
        float value = f_timerIn * 5.0f;
        txt_completionTimer.text = value.ToString();
        Constants.ObjectiveStats.C_PotatoCompletionTimer = (int)value;
    }

    public void ChangeSelfDestructTimer(float f_timerIn) {
        float value = f_timerIn * 5.0f;
        txt_selfDestructTimer.text = value.ToString();
        Constants.ObjectiveStats.C_PotatoSelfDestructTimer = (int)value;
    }

    //public void ChangeEnemySpawnCap(float f_capIn) {
    //    txt_enemySpawnCap.text = slider_enemySpawnCap.value.ToString();
    //    Constants.EnemyStats.C_EnemySpawnCapPerSide = (int)(f_capIn - 1);
    //}

    //public void ChangeHockeyMaxScore(float f_score) {
    //    txt_hockeyMaxScore.text = slider_hockeyMaxScore.value.ToString();
    //    Constants.ObjectiveStats.C_HockeyMaxScore = (int)f_score;
    //}

    public void ChangePuckDamage(float f_damage) {
        float value = f_damage * 5.0f;
        txt_puckDamage.text = string.Format("{0:0}%",((slider_puckDamage.value / 2f)*100));
        Constants.ObjectiveStats.C_PuckDamage = (int)value;
    }

    public void ChangePuckSpeedDecayRate(float f_decay) {
        txt_puckSpeedDecayRate.text = slider_puckSpeedDecayRate.value.ToString();
        Constants.ObjectiveStats.C_PuckSpeedDecayRate = (int)f_decay;
    }

    public void ChangePuckSpeedDecreaseRate(float f_decrease) {
        txt_puckSpeedDecreaseRate.text = slider_puckSpeedDecreaseRate.value.ToString();
        Constants.ObjectiveStats.C_PuckSpeedDecreaseAmount = (int)f_decrease;
    }

    public void ChangePuckBaseSpeed(float f_speed) {
        txt_puckBaseSpeed.text = string.Format("{0:0}%",((slider_puckBaseSpeed.value / 10f)*100));
        Constants.ObjectiveStats.C_PuckBaseSpeed = (int)f_speed;
    }

    public void ChangePuckMaxSpeed(float f_speed) {
        txt_puckMaxSpeed.text = string.Format("{0:0}%",((slider_puckMaxSpeed.value / 15f)*100));
        Constants.ObjectiveStats.C_PuckMaxSpeed = (int)f_speed;
    }

    public void ChangePuckHitIncreaseSpeed(float f_hit) {
        txt_puckHitIncreaseSpeed.text = slider_puckHitIncreaseSpeed.value.ToString();
        Constants.ObjectiveStats.C_PuckSpeedHitIncrease = (int)f_hit;
    }

    //public void ChangeNecroMaxScore(float f_score) {
    //    txt_necroMaxScore.text = slider_necroMaxScore.value.ToString();
    //    Constants.ObjectiveStats.C_NecromancersMaxScore = (int)f_score;
    //}

    public void ChangeRiftBossHealth(float f_riftBossHealthIn) {
        float value = f_riftBossHealthIn * 250.0f;
        txt_riftBossHealth.text = string.Format("{0:0}%",((slider_riftBossHealth.value / 16f)*100));
        Constants.ObjectiveStats.C_RiftBossMaxHealth = (int)value;
    }

    public void ChangeRuneSpawnInterval(float f_interval)
    {
        txt_runeSpawnInterval.text = slider_runeSpawnInterval.value.ToString();
        Constants.ObjectiveStats.C_RuneSpawnInterval = (int)f_interval;
    }

    public void ChangeDeathBoltCooldown(float f_cooldown)
    {
        txt_deathBoltCooldown.text = slider_deathBoltCooldown.value.ToString();
        Constants.ObjectiveStats.C_DeathBoltCooldown = (int)f_cooldown;
    }

    public void ChangeForceFieldCooldown(float f_cooldown)
    {
        txt_forceFieldCooldown.text = slider_forceFieldCooldown.value.ToString();
        Constants.ObjectiveStats.C_ForceFieldCooldown = (int)f_cooldown;
    }

    // Light buttons up as they are selected
    public void LightUp() {
        if (currentMenu == 0) {
            txt_playerLabel.color = new Color(20f/255f,1f,252f/255f);
            txt_objectiveLabel.color = Color.white;
        } else {
            txt_objectiveLabel.color = new Color(20f/255f,1f,252f/255f);
            txt_playerLabel.color = Color.white;
        }
    }

    // Show the proper menu on click
    public void MenuSwitch(int which) {
        //loop
        if (which > 1) {
            currentMenu = 0;
        } else if (which < 0) { 
            currentMenu = 1;
        } else {
            currentMenu = which;
        }
         LightUp();
        if (currentMenu == 0) {
            go_playerMenu.SetActive(true);
            go_objectiveMenu.SetActive(false);
            slider_playerMoveSpeed.Select();
            slider_playerMoveSpeed.OnSelect(null);
        } else {
            go_objectiveMenu.SetActive(true);
            go_playerMenu.SetActive(false);
            slider_CTFScore.Select();
            slider_CTFScore.OnSelect(null);
        }
    }

    /*/////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

    // Get initial values from Constants.cs
    void Start() {

        //----------------------------
        // Player

        // Player Speed
        txt_playerMoveSpeed.text = string.Format("{0:0}%",((Constants.PlayerStats.C_MovementSpeed / 6f)*100));
        slider_playerMoveSpeed.value = Constants.PlayerStats.C_MovementSpeed;

        // Player Wisp Speed
        txt_wispMoveSpeed.text = Constants.PlayerStats.C_WispMovementSpeed.ToString();
        slider_wispMoveSpeed.value = Constants.PlayerStats.C_WispMovementSpeed;

        // Player Health
        txt_playerHealth.text = string.Format("{0:0}%",(((Constants.PlayerStats.C_MaxHealth / 50f) / 6f)*100));
        slider_playerHealth.value = Constants.PlayerStats.C_MaxHealth / 50;
        
        // Respawn Rate
        txt_respawnTime.text = string.Format("{0:0}%",(((Constants.PlayerStats.C_RespawnTimer) / 5f)*100));
        slider_respawnTime.value = Constants.PlayerStats.C_RespawnTimer;


        //----------------------------
        // Enemy

        // Enemy Spawn Rate
        txt_enemySpawn.text = Constants.RiftStats.C_VolatilityEnemySpawnTimer.ToString();
        slider_enemySpawn.value = Constants.RiftStats.C_VolatilityEnemySpawnTimer;

        // Enemy Speed
        txt_enemySpeed.text = string.Format("{0:0}%",(((Constants.EnemyStats.C_EnemyBaseSpeed * 10) / 12f)*100));
        slider_enemySpeed.value = Constants.EnemyStats.C_EnemyBaseSpeed * 10;

        // Enemy Health
        txt_enemyHealth.text = Constants.EnemyStats.C_EnemyHealth.ToString();
        slider_enemyHealth.value = Constants.EnemyStats.C_EnemyHealth / 25;

        // Necromancer Health
        txt_necromancerHealth.text = string.Format("{0:0}%",(((Constants.EnemyStats.C_NecromancerHealth / 50f) / 8f)*100));
        slider_necromancerHealth.value = Constants.EnemyStats.C_NecromancerHealth / 50;

        // Enemy Damage
        txt_enemyDamage.text = string.Format("{0:0}%",(((Constants.EnemyStats.C_EnemyDamage / 5f) / 5f)*100));
        slider_enemyDamage.value = Constants.EnemyStats.C_EnemyDamage / 5;

        // Enemy Spawn Cap
        txt_enemySpawnCap.text = Constants.EnemyStats.C_EnemySpawnCapPerSide.ToString();
        slider_enemySpawnCap.value = Constants.EnemyStats.C_EnemySpawnCapPerSide;

        //----------------------------
        // Spell

        // Wind Spell Speed
        txt_windSpeed.text = Constants.SpellStats.C_WindSpeed.ToString();
        slider_windSpeed.value = Constants.SpellStats.C_WindSpeed;

        // Ice Spell Speed
        txt_iceSpeed.text = Constants.SpellStats.C_IceSpeed.ToString();
        slider_iceSpeed.value = Constants.SpellStats.C_IceSpeed;

        // Electric Spell Speed
        txt_electricSpeed.text = Constants.SpellStats.C_ElectricSpeed.ToString();
        slider_electricSpeed.value = Constants.SpellStats.C_ElectricSpeed;

        // Wind Spell Cooldown
        txt_windCooldown.text =  string.Format("{0:0}%",((Constants.SpellStats.C_WindCooldown)*100));
        slider_windCooldown.value = Constants.SpellStats.C_WindCooldown * 5f;

        // Ice Spell Cooldown
        txt_iceCooldown.text = string.Format("{0:0}%",((Constants.SpellStats.C_IceCooldown / 5f)*100));
        slider_iceCooldown.value = Constants.SpellStats.C_IceCooldown;

        // Electric Spell Cooldown
        txt_electricCooldown.text = string.Format("{0:0}%",((Constants.SpellStats.C_ElectricCooldown / 8f)*100));
        slider_electricCooldown.value = Constants.SpellStats.C_ElectricCooldown;

        // Magic Missile Speed
        txt_magicMissileSpeed.text = Constants.SpellStats.C_MagicMissileSpeed.ToString();
        slider_magicMissileSpeed.value = Constants.SpellStats.C_MagicMissileSpeed;

        // Magic Missile Heal
        txt_magicMissileHeal.text = Constants.SpellStats.C_MagicMissileHeal.ToString();
        slider_magicMissileHeal.value = Constants.SpellStats.C_MagicMissileHeal;

        // Magic Missile Fire Rate
        txt_magicMissileFireRate.text = string.Format("{0:0}%",(((Constants.SpellStats.C_MagicMissileCooldown) / 0.75f)*100));
        slider_magicMissileFireRate.value = Constants.SpellStats.C_MagicMissileCooldown * 20;

        // Projectile Size 
        txt_projSize.text = string.Format("{0:0}%",((Constants.SpellStats.C_PlayerProjectileSize / 0.5f)*100));
        slider_projSize.value = Constants.SpellStats.C_PlayerProjectileSize;

        // Projectile Live Time
        txt_projLife.text = Constants.SpellStats.C_SpellLiveTime.ToString();
        slider_projLife.value = Constants.SpellStats.C_SpellLiveTime;

        // Wind Force
        txt_windForce.text = string.Format("{0:0}%",(((Constants.SpellStats.C_WindForce / 250f) / 6f)*100));
        slider_windForce.value = Constants.SpellStats.C_WindForce / 250;

        // Ice Freeze Duration
        txt_iceFreeze.text = Constants.SpellStats.C_IceFreezeTime.ToString();
        slider_iceFreeze.value = Constants.SpellStats.C_IceFreezeTime;

        // Electric AOE Live-Time
        txt_electricLiveTime.text = string.Format("{0:0}%",((Constants.SpellStats.C_ElectricAOELiveTime / 5f)*100));
        slider_electricLiveTime.value = Constants.SpellStats.C_ElectricAOELiveTime;


        //----------------------------
        // Objective

        // CTF Score
        txt_CTFScore.text = string.Format("{0:0}%",((Constants.ObjectiveStats.C_CTFMaxScore / 3f)*100));
        slider_CTFScore.value = Constants.ObjectiveStats.C_CTFMaxScore;
        
        //// Crystal Health
        //txt_crystalHealth.text = Constants.ObjectiveStats.C_CrystalMaxHealth.ToString();
        //slider_crystalHealth.value = Constants.ObjectiveStats.C_CrystalMaxHealth / 50;

        //// Crystal Health Regen
        //txt_crystalHealthRegen.text = Constants.ObjectiveStats.C_CrystalRegenHeal.ToString();
        //slider_crystalHealthRegen.value = Constants.ObjectiveStats.C_CrystalRegenHeal;

        //// Crystal Health Regen Rate
        //txt_crystalHealthRegenRate.text = Constants.ObjectiveStats.C_CrystalHealRate.ToString();
        //slider_crystalHealthRegenRate.value = Constants.ObjectiveStats.C_CrystalHealRate;

        //// Crystal Health Regen Delay
        //txt_crystalHealthRegenDelay.text = Constants.ObjectiveStats.C_CrystalHealDelay.ToString();
        //slider_crystalHealthRegenDelay.value = Constants.ObjectiveStats.C_CrystalHealDelay;

        //// Hot Potato Completion Timer 
        //txt_completionTimer.text = Constants.ObjectiveStats.C_PotatoCompletionTimer.ToString();
        //slider_completionTimer.value = Constants.ObjectiveStats.C_PotatoCompletionTimer / 5;

        //// Hot Potato Self Destruct Timer
        //txt_selfDestructTimer.text = Constants.ObjectiveStats.C_PotatoSelfDestructTimer.ToString();
        //slider_selfDestructTimer.value = Constants.ObjectiveStats.C_PotatoSelfDestructTimer / 5;

        //// Hockey Max Score
        //txt_hockeyMaxScore.text = Constants.ObjectiveStats.C_HockeyMaxScore.ToString();
        //slider_hockeyMaxScore.value = Constants.ObjectiveStats.C_HockeyMaxScore;

        // Hockey Puck Damage
        txt_puckDamage.text = string.Format("{0:0}%",(((Constants.ObjectiveStats.C_PuckDamage / 5f) / 2f)*100));
        slider_puckDamage.value = Constants.ObjectiveStats.C_PuckDamage / 5;

        // Hockey Puck Base Speed
        txt_puckBaseSpeed.text = string.Format("{0:0}%",((Constants.ObjectiveStats.C_PuckBaseSpeed / 10f)*100));
        slider_puckBaseSpeed.value = Constants.ObjectiveStats.C_PuckBaseSpeed;

        // Hockey Puck Max Speed
        txt_puckMaxSpeed.text = string.Format("{0:0}%",((Constants.ObjectiveStats.C_PuckMaxSpeed / 15f)*100));
        slider_puckMaxSpeed.value = Constants.ObjectiveStats.C_PuckMaxSpeed;

        // Hockey Puck Hit Increase Speed
        txt_puckHitIncreaseSpeed.text = Constants.ObjectiveStats.C_PuckSpeedHitIncrease.ToString();
        slider_puckHitIncreaseSpeed.value = Constants.ObjectiveStats.C_PuckSpeedHitIncrease;

        // Hockey Puck Speed Decay Rate
        txt_puckSpeedDecayRate.text = Constants.ObjectiveStats.C_PuckSpeedDecayRate.ToString();
        slider_puckSpeedDecayRate.value = Constants.ObjectiveStats.C_PuckSpeedDecayRate;

        // Hockey Puck Speed Decrease Amount
        txt_puckSpeedDecreaseRate.text = Constants.ObjectiveStats.C_PuckSpeedDecreaseAmount.ToString();
        slider_puckSpeedDecreaseRate.value = Constants.ObjectiveStats.C_PuckSpeedDecreaseAmount;

        //// Necromancers to Defeat
        //txt_necroMaxScore.text = Constants.ObjectiveStats.C_NecromancersMaxScore.ToString();
        //slider_necroMaxScore.value = Constants.ObjectiveStats.C_NecromancersMaxScore;

        // Rift Boss Max Health
        txt_riftBossHealth.text = string.Format("{0:0}%",(((Constants.ObjectiveStats.C_RiftBossMaxHealth / 250f) / 16f)*100));
        slider_riftBossHealth.value = Constants.ObjectiveStats.C_RiftBossMaxHealth / 250;

        // Rift Boss Rune Spawn Interval
        txt_runeSpawnInterval.text = Constants.ObjectiveStats.C_RuneSpawnInterval.ToString();
        slider_runeSpawnInterval.value = Constants.ObjectiveStats.C_RuneSpawnInterval;

        // Rift Boss Death Bolt Cooldown
        txt_deathBoltCooldown.text = Constants.ObjectiveStats.C_DeathBoltCooldown.ToString();
        slider_deathBoltCooldown.value = Constants.ObjectiveStats.C_DeathBoltCooldown;

        // Force Field Cooldown
        txt_forceFieldCooldown.text = Constants.ObjectiveStats.C_ForceFieldCooldown.ToString();
        slider_forceFieldCooldown.value = Constants.ObjectiveStats.C_ForceFieldCooldown;

        //---------------------------
        // Organize menu buttons
        butt_buttonArray[0] = butt_playerSelect;
        butt_buttonArray[1] = butt_spellSelect;
        butt_buttonArray[2] = butt_enemySelect;
        butt_buttonArray[3] = butt_objectiveSelect;

        // Organize menus
        go_menuArray[0] = go_playerMenu;
        go_menuArray[1] = go_objectiveMenu;
        currentMenu = 0;

        p_uiPlayer = ReInput.players.GetPlayer(0);
        MenuSwitch(currentMenu);
    }

    private void FixedUpdate() {
        if (go_topMenu.activeSelf) {
            if (p_uiPlayer.GetButtonDown("UIPageRight")) {
                MenuSwitch(++currentMenu);
            } else if (p_uiPlayer.GetButtonDown("UIPageLeft")) {
                MenuSwitch(--currentMenu);
            } else if (p_uiPlayer.GetButtonDown("UICancel")) {
                psc_master.CloseParams();
            }
        }
    }
}
