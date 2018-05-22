 /*  Player Controller - Sam Caulker
 * 
 *  Desc:   Facilitates player interactions
 * 
 */

using UnityEngine;
using Rewired;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class PlayerController : SpellTarget {
#region Variables and Declarations
    [SerializeField] private GameObject go_playerCapsule;   // player main body
    [SerializeField] private GameObject go_playerWisp;      // player wisp body
    [SerializeField] private GameObject go_magicMissileShot;    // magic missile object
    [SerializeField] private GameObject go_windShot;            // wind spell object
    [SerializeField] private GameObject go_iceShot;             // ice spell object
    [SerializeField] private GameObject go_electricShot;        // electric spell object
    [SerializeField] private GameObject[] go_hats;              // The potential hats on this player.
    [SerializeField] private Transform t_spellSpawn;    // location spells are instantiated
    [SerializeField] private Transform t_flagPos;       // location on character model of flag
    [SerializeField] private GameObject go_interactCollider;  // activated with button-press to interact with objectives
    [SerializeField] private GameObject go_parryShield;       // activated with right stick click
    [SerializeField] private GameObject go_healingVFX;			// healing particle effect 
    [SerializeField] private GameObject go_windHitParticles;
    [SerializeField] private PauseController pauc_pause;        // for pausing

    [SerializeField] private SkinnedMeshRenderer smr_playerBody;            //These are for visual cue on the player model
    [SerializeField] private SkinnedMeshRenderer smr_playerOutfit;

    [SerializeField] private Texture txtr_bodyNormal;                       //These are for invulnerability.
    [SerializeField] private Texture txtr_bodyFlash;
    [SerializeField] private Texture txtr_bodyGooed;
    [SerializeField] private Texture txtr_outfitNormal;
    [SerializeField] private Material mat_hatFlash;
    [SerializeField] private Color col_outfitNormal;
    [SerializeField] private Color col_outfitFlash;

    [SerializeField] private Material mat_bodyNormal;                       //These are for freezing.
    [SerializeField] private Material mat_outfitNormal;
    [SerializeField] private Material mat_freeze;
    [SerializeField] private Material mat_freezeExtend;

    private int i_playerNumber;             // designates player's number for controller mappings
    private Player p_player;                // rewired player for input control
    private Constants.Global.Side e_side;   // identifies which side of the rift player is on
    private float f_canMove = 1;            // identifies if the player is frozen
    private bool isWisp = false;            // player has "died"
    private bool isInvuln = false;          // player is invincible (after respawn)
    private bool b_iceboltMode = false;     // player is controlling an icebolt
    private GameObject go_icebolt;          // the icebolt the player is controlling
    private GameObject go_flagObj;          // flag game object; if not null, player is carrying flag
    private bool b_stepOk = true;           // time to play next footstep noise has elapsed
    private bool b_deathAnimOK = false;     // used to determine if death animation should play or not.
    private GameObject go_activeHat;        // the hat that's visible on the player.
    private Material mat_hatNormal;         // This is for freezing/invuln visuals.
    private MeshRenderer mr_hat;    
    

    // Spells
    private float f_nextWind = 0;				    // time next wind spell can be cast
	private float f_nextIce = 0;                    // time next ice spell can be cast
	private float f_nextElectric = 0;               // time next electric spell can be cast
	private float f_nextMagicMissile = 0;           // time next magic missile can be cast
    private float f_nextCast = 0;                   // time next spell in general can be cast (does not include magic missile)
    private float f_iceCharge;              // current charge of ice spell
    private float f_windCharge;             // current charge of wind spell
    private float f_electricCharge;         // current charge of electric charge
    private float f_projectileSize;         // size of spell objects

    #region Getters and Setters
    public Constants.Global.Side Side {
        get { return e_side; }
    }

    public bool Wisp {
        get { return isWisp; }
    }

    public bool Invulnerable {
        get { return isInvuln; }
    }

    public bool HasFlag {
        get { return go_flagObj != null; }
    }

    public int Num {
        get { return i_playerNumber; }
        set { i_playerNumber = value; }
    }

    public bool IceBoltMode {
        get { return b_iceboltMode; }
        set { b_iceboltMode = value; }
    }

    public float NextWind {
        get { return f_nextWind; }
    }

    public float NextIce {
        get { return f_nextIce; }
    }

    public float NextElectric {
        get { return f_nextElectric; }
    }

    public GameObject Capsule {
        get { return go_playerCapsule; }
    }
    #endregion
#endregion

#region Player Controller Methods
    override public void ApplySpellEffect(Constants.SpellStats.SpellType spell, Constants.Global.Color color, float damage, Vector3 direction) {
        if (isWisp) {
            return;
        }

        switch (spell) {
            case Constants.SpellStats.SpellType.WIND:
				maestro.PlayPlayerHit();
				if(color == e_color) maestro.PlayAnnouncementFriendlyFire();
                DropFlag();

                if (Constants.UnitTests.C_RunningCTFTests)
                    return;

                StartCoroutine(WindPush(Constants.PlayerStats.C_PlayerWindPushMultiplier,direction,false));
                TakeDamage(damage, Constants.Global.DamageType.WIND);
                go_windHitParticles.SetActive(true);
                Invoke("TurnOffWindHitParticles", 0.5f);
                anim.SetTrigger("windTrigger");
                break;
            case Constants.SpellStats.SpellType.ICE:
				maestro.PlayPlayerHit();
				if(color == e_color) maestro.PlayAnnouncementFriendlyFire();
                DropFlag();

                if (Constants.UnitTests.C_RunningCTFTests)
                    return;

                f_canMove = 0;
                TakeDamage(damage, Constants.Global.DamageType.ICE);
                anim.SetTrigger("freezeTrigger");
                anim.SetBool("freezeBool", true);
                smr_playerBody.materials =  new Material[] { mat_freeze, mat_freezeExtend };
                smr_playerOutfit.materials = new Material[] { mat_freeze, mat_freezeExtend };
                mr_hat.materials = new Material[] { mat_freeze, mat_freezeExtend };
                Invoke("Unfreeze", Constants.SpellStats.C_IceFreezeTime);
                break;
            case Constants.SpellStats.SpellType.ELECTRICITYAOE:
                if(e_color != color) {
					if(b_electricDamageSoundOk){
						maestro.PlayEnemyHit();
						b_electricDamageSoundOk = false;
						StartCoroutine("AdmitElectricDamageSound");
					}
                    DropFlag();

                    if (Constants.UnitTests.C_RunningCTFTests)
                        return;

                    if (f_canMove != 0) {
                        f_canMove = Constants.SpellStats.C_ElectricAOESlowDownMultiplier;
                        smr_playerBody.material.mainTexture = txtr_bodyGooed;
                        smr_playerOutfit.material.mainTexture = txtr_bodyGooed;
                        anim.SetTrigger("gooTrigger");
                    }
                    TakeDamage(damage, Constants.Global.DamageType.ELECTRICITY);
                }
                break;
            case Constants.SpellStats.SpellType.MAGICMISSILE:
                if (e_color != color) {
					maestro.PlayPlayerHit();
                    DropFlag();

                    if (Constants.UnitTests.C_RunningCTFTests)
                        return;

                    TakeDamage(damage, Constants.Global.DamageType.MAGICMISSILE);
                    anim.SetTrigger("hitTrigger");
                }
                else {
                    Heal(damage / 2.5f);
                }
                break;
        }
    }

    override public void NegateSpellEffect(Constants.SpellStats.SpellType spell) {
        if (spell == Constants.SpellStats.SpellType.ELECTRICITYAOE) {
            f_canMove = 1;
        }
    }

    private void Move() {
        float f_inputX = p_player.GetAxis("MoveHorizontal");
        float f_inputZ = p_player.GetAxis("MoveVertical");
        float f_aimInputX = p_player.GetAxis("AimHorizontal");
        float f_aimInputZ = p_player.GetAxis("AimVertical");
		//float f_lookDirection;

        Vector3 v3_moveDir = new Vector3(f_inputX, 0, f_inputZ).normalized;
		Vector3 v3_aimDir = new Vector3(f_aimInputX, 0, f_aimInputZ).normalized;


        if (go_playerCapsule.activeSelf)    // only set anim triggers if player model is on
        {
            //f_lookDirection = f_inputX + f_aimInputX;

            //anim.SetFloat ("runSpeed", v3_moveDir.magnitude);
            //anim.SetFloat ("lookDirection", v3_aimDir.magnitude);
            anim.SetFloat("runSpeedX", f_inputX);
            anim.SetFloat("runSpeedZ", f_inputZ);
            anim.SetFloat("aimDirectionX", f_aimInputX);
            anim.SetFloat("aimDirectionZ", f_aimInputZ);
        }

		if (v3_aimDir.magnitude > 0) {
			transform.rotation = Quaternion.LookRotation(v3_aimDir);
		}

        if (isWisp) {
			rb.velocity = (v3_moveDir * Constants.PlayerStats.C_WispMovementSpeed) * f_canMove;
		} else if (b_iceboltMode) {
            rb.velocity = Vector3.zero;
        }
		else {
			rb.velocity = (v3_moveDir * Constants.PlayerStats.C_MovementSpeed) * f_canMove;
			if(v3_moveDir.magnitude > 0 && b_stepOk){
				b_stepOk = false;
				maestro.PlayPlayerFootstep();
				maestro.PlayPlayerClothing();
			}	
		}
	}

    #region Reset Helper Functions
    private void TurnOffParryShield() {
        go_parryShield.SetActive(false);
    }

    private void TurnOffWindHitParticles() {
        go_windHitParticles.SetActive(false);
    }

    public void CleanOffGoo() {
        smr_playerBody.material.mainTexture = txtr_bodyNormal;
        smr_playerOutfit.material.mainTexture = txtr_outfitNormal;
    }

    private void TurnOffInteractCollider() {
        go_interactCollider.transform.localPosition = new Vector3(go_interactCollider.transform.localPosition.x, -1000.0f, go_interactCollider.transform.localPosition.z);
        Invoke("ResetInteractCollider", 0.05f);
    }

    private void ResetInteractCollider() {
        go_interactCollider.SetActive(false);
        go_interactCollider.transform.localPosition = new Vector3(go_interactCollider.transform.localPosition.x, transform.position.y, go_interactCollider.transform.localPosition.z);
    }

    private void Unfreeze() {
		anim.SetBool ("freezeBool", false);
        Invoke("UnfreezeSync", 1.85f);
    }

    private void UnfreezeSync() {
        f_canMove = 1;
        smr_playerBody.materials = new Material[] { mat_bodyNormal };
        smr_playerOutfit.materials = new Material[] { mat_outfitNormal };
        mr_hat.materials = new Material[] { mat_hatNormal };
    }
    #endregion

    #region Health and Damage
    private void PlayerDeath(bool riftDeath) {
		maestro.PlayAnnouncementWispGeneric();
        DropFlag();
        TurnOffInteractCollider();
        isWisp = true;
        if (Constants.UnitTests.C_RunningCTFTests)
            return;
        if (SceneManager.GetActiveScene().name != "WarmUp") {
            riftController.IncreaseVolatility(Constants.RiftStats.C_VolatilityIncrease_PlayerDeath);
        } 
		maestro.PlayPlayerDie();
        if (!riftDeath) {
            DissolvePlayer();
        } else {
            go_playerWisp.SetActive(true);
            go_playerCapsule.SetActive(false);
        }

        InvokeRepeating("RespawnTimer", 0.0f, Constants.PlayerStats.C_RespawnTimer / Constants.PlayerStats.C_RespawnHealthSubDivision);

        f_nextMagicMissile = 0;
        f_nextWind = 0;
        f_nextIce = 0;
        f_nextElectric = 0;
        f_nextCast = 0;
        Invoke("PlayerRespawn", Constants.PlayerStats.C_RespawnTimer);
    }

    private void RespawnTimer() {
        f_health += Constants.PlayerStats.C_MaxHealth / Constants.PlayerStats.C_RespawnHealthSubDivision;
    }
	
	private void DissolvePlayer(){
        b_deathAnimOK = true;
		StartCoroutine(DoWispFadeIn());
		StartCoroutine(DoDissolvePlayer());
	}

	private IEnumerator DoWispFadeIn(){
		go_playerWisp.SetActive(true);
		yield return new WaitUntil(() => se_dissolve.currentParamValue >= .6f);
		se_fader.ParamIncrease(3f, false, "_Brightness");
	}

	private IEnumerator DoDissolvePlayer(){
		se_dissolve.ParamIncrease(2f, false, "_DisintegrateAmount");
		yield return new WaitUntil(() => se_dissolve.isFinished);
		se_dissolve.isFinished = false;
		go_playerCapsule.SetActive(false);
	}
 

    private void PlayerRespawn() {
        CancelInvoke("RespawnTimer");
        isWisp = false;
        isInvuln = true;
        Invoke("EndInvuln", Constants.PlayerStats.C_InvulnTime);
        InvokeRepeating("InvulnFlicker", 0f, 0.25f);
		maestro.PlayPlayerSpawn();
		maestro.PlayAnnouncementPlayerSpawn();
        if (b_deathAnimOK) {
            ReconstructPlayer();
            b_deathAnimOK = false;
        } else {
            go_playerWisp.SetActive(false);
            go_playerCapsule.SetActive(true);
            se_dissolve.ResetMaterials();
        } 
        f_health = Constants.PlayerStats.C_MaxHealth;
    }
	
	private void ReconstructPlayer(){
		StartCoroutine(DoConstructPlayer());
		StartCoroutine(DoWispFadeOut());
	}
	
    private IEnumerator DoWispFadeOut(){
        se_fader.ParamDecrease(5f, false, "_Brightness");
        yield return new WaitUntil(() => se_fader.isFinished);
        se_fader.isFinished = false;
        go_playerWisp.SetActive(false);
    }
	
	private IEnumerator DoConstructPlayer(){
		yield return new WaitUntil(() => se_fader.currentParamValue <= .7f);
		go_playerCapsule.SetActive(true);
		se_dissolve.ParamDecrease(3f, false, "_DisintegrateAmount");
	}

	public void TakeDamage(float damage, Constants.Global.DamageType d) {
		if (!isWisp && !isInvuln) {
			maestro.PlayAnnouncmentPlayerHit(i_playerNumber,d);
			f_health -= damage;
            //DamageVisualOn();
			if (f_health <= 0.0f) {
                PlayerDeath(d == Constants.Global.DamageType.RIFT);
			}
		}
	}
    
    public void Heal(float heal) {
        if (!isWisp) {
            float tempHp = f_health + heal;
            if (tempHp >= Constants.PlayerStats.C_MaxHealth) {
                f_health = Constants.PlayerStats.C_MaxHealth;
            } else {
                f_health = tempHp;
            }
            HealVisualOn();
        }
    }

    private void EndInvuln() {
        isInvuln = false;
        smr_playerBody.material.mainTexture = txtr_bodyNormal;
        smr_playerOutfit.material.color = col_outfitNormal;
        mr_hat.material = mat_hatNormal;
        se_dissolve.ResetMaterials();
        se_fader.ResetMaterials();
        CancelInvoke("InvulnFlicker");
    }

    private void InvulnFlicker() {
        if (smr_playerBody.material.mainTexture == txtr_bodyNormal) {
            smr_playerBody.material.mainTexture = txtr_bodyFlash;
        } else {
            smr_playerBody.material.mainTexture = txtr_bodyNormal;
        }

        if (smr_playerOutfit.material.color == col_outfitNormal) {
            smr_playerOutfit.material.color = col_outfitFlash;
        } else {
            smr_playerOutfit.material.color = col_outfitNormal;
        }

        if (mr_hat.material.name.Contains("Hat")) {
            mr_hat.material = mat_hatFlash;
        } else {
            mr_hat.material = mat_hatNormal;
        }
    }
    #endregion

    #region Flag Stuff
    // there's no good way to do any of this
    public void Pickup(GameObject flag) {
        if (!isWisp && f_canMove != 0) {
            flag.transform.SetParent(t_flagPos);
            flag.transform.localPosition = Vector3.zero;
            go_flagObj = flag;
        }
	}

	public void DropFlag() {
		if(go_flagObj) {
            go_flagObj.GetComponent<FlagController>().DropFlag();
			go_flagObj = null;
		}
	}

    public void Interact() {
        if (go_flagObj) {
            DropFlag();
        }
        else {
            go_interactCollider.SetActive(true);
        }
        Invoke("TurnOffInteractCollider", 0.05f);
    }
    #endregion

	private void StepDelay(){
		b_stepOk = true;
	}

    //public void DamageVisualOn() {
    //    go_playerCapsule.GetComponent<MeshRenderer>().material.color = Color.yellow;
    //    //Call screenshake here.
    //    Invoke("DamageVisualOff", 0.1666f * 2);
    //}

    //public void DamageVisualOff() {
    //    go_playerCapsule.GetComponent<MeshRenderer>().material.color = col_originalColor;
    //}

    public void HealVisualOn() {
        Instantiate(go_healingVFX,transform.position, Quaternion.identity);
        //go_playerCapsule.GetComponent<MeshRenderer>().material.color = Color.green;
        //Call screenshake here.
        //Invoke("HealVisualOff", 1.0f);
    }

    //public void HealVisualOff() {
    //    go_healingVFX.SetActive(false);
    //    //go_playerCapsule.GetComponent<MeshRenderer>().material.color = col_originalColor;
    //}
    #endregion

    #region Unity Overrides
    protected override void Start() {
        maestro = Maestro.Instance;
        riftController = RiftController.Instance;

        if (Constants.UnitTests.C_RunningCTFTests)
            return;


        p_player = ReInput.players.GetPlayer(i_playerNumber);
        f_health = Constants.PlayerStats.C_MaxHealth;
        f_projectileSize = Constants.SpellStats.C_PlayerProjectileSize;

        //Get the active hat from the array and set the material and mesh renderer accordingly.
        foreach(GameObject go in go_hats) {
            if (go.activeSelf && go_activeHat == null) {
                go_activeHat = go;
            }
        }
        mr_hat = go_activeHat.GetComponent<MeshRenderer>();
        mat_hatNormal = mr_hat.materials[0];
		
		if (transform.position.x > 0)
			e_side = Constants.Global.Side.RIGHT;
		else
			e_side = Constants.Global.Side.LEFT;

        InvokeRepeating("StepDelay", Constants.PlayerStats.C_StepSoundDelay, Constants.PlayerStats.C_StepSoundDelay);

    }

    protected virtual void Update()
    {
        // pause
        if (p_player.GetButtonDown("Menu") && Time.timeScale == 1) {
            Debug.Log("Check");
            pauc_pause.Pause(this);
        }
    }


	protected virtual void FixedUpdate() {
        if (Constants.UnitTests.C_RunningCTFTests)
            return;

        if (go_playerCapsule.activeSelf)    // only set anim triggers if player model is on
            anim.SetBool("iceSpellBool", b_iceboltMode);

        // position
        if (transform.position.x > 0)
            e_side = Constants.Global.Side.RIGHT;
        else
            e_side = Constants.Global.Side.LEFT;

        Move();

        if (isWisp)
            return;

        // update spell timers
		f_nextMagicMissile += Time.deltaTime;
		f_nextWind += Time.deltaTime;
        f_nextIce += Time.deltaTime;
        f_nextElectric += Time.deltaTime;
		f_nextCast += Time.deltaTime;

        // interact
        if (p_player.GetButtonDown("Interact")) {
            Interact();
        }
        //button up no longer needed because hot potato is out
        //if (p_player.GetButtonUp("Interact")) {
        //    TurnOffInteractCollider();
        //}

        // spells
        if (p_player.GetButtonUp("IceSpell")) {
            b_iceboltMode = false;
            Destroy(go_icebolt);
        }

        if (go_flagObj)
            return;

        // Magic Missile (Auto-fire)
        if (f_nextMagicMissile > Constants.SpellStats.C_MagicMissileCooldown) {
            if (p_player.GetButton("MagicMissile")) {
                maestro.PlayMagicMissileShoot();
				anim.SetTrigger ("attackTrigger");
                f_nextMagicMissile = 0;
				GameObject go_spell = Instantiate(go_magicMissileShot, t_spellSpawn.position, t_spellSpawn.rotation);

                Physics.IgnoreCollision(GetComponent<Collider>(), go_spell.GetComponent<Collider>());
                go_spell.transform.localScale = new Vector3(f_projectileSize, f_projectileSize, f_projectileSize);
                go_spell.GetComponent<Rigidbody>().velocity = transform.forward * Constants.SpellStats.C_MagicMissileSpeed;
                SpellController sc_firing = go_spell.GetComponent<SpellController>();
                sc_firing.Init(this, e_color, 0);
                if (e_color == Constants.Global.Color.BLUE) {
                    go_spell.layer = LayerMask.NameToLayer("BlueMM");
                }
                else {
                    go_spell.layer = LayerMask.NameToLayer("RedMM");
                }
            }                
	    }
        // Wind Parry Spell
		if(f_nextWind > Constants.SpellStats.C_WindCooldown){
			if (p_player.GetButtonDown("WindSpell")) {
				f_nextWind = 0;
				maestro.PlayWindShoot();
                anim.SetTrigger("windspellTrigger");
                go_parryShield.SetActive(true);
				Invoke("TurnOffParryShield", 0.75f);
			}
		}
        // Ice Spell
        if (f_nextIce > Constants.SpellStats.C_IceCooldown && f_nextCast > Constants.SpellStats.C_NextSpellDelay) {
            if (p_player.GetButtonDown("IceSpell")) {
				maestro.PlaySpellCharge();
				maestro.PlayIceShoot();
                b_iceboltMode = true;
                f_nextIce = 0;
                f_nextCast = 0;
                GameObject go_spell = Instantiate(go_iceShot, t_spellSpawn.position, t_spellSpawn.rotation);

                Physics.IgnoreCollision(GetComponent<Collider>(), go_spell.GetComponent<Collider>());
                go_spell.transform.localScale = new Vector3(f_projectileSize, f_projectileSize, f_projectileSize);
                go_spell.GetComponent<Rigidbody>().velocity = transform.forward * Constants.SpellStats.C_IceSpeed;
                go_icebolt = go_spell;
                IceSpellController sc_firing = go_spell.GetComponent<IceSpellController>();
                sc_firing.Init(this, e_color, 0);
            }
		}
        // Electric Spell
        if (f_nextElectric > Constants.SpellStats.C_ElectricCooldown && f_nextCast > Constants.SpellStats.C_NextSpellDelay) {
            if (p_player.GetButtonDown("ElectricitySpell"))
				maestro.PlaySpellCharge();
            if (p_player.GetButtonTimePressed("ElectricitySpell") != 0) {
                f_electricCharge += p_player.GetButtonTimePressed("ElectricitySpell");
		    	anim.SetFloat ("gooCharge", f_electricCharge);
            }
            if (p_player.GetButtonUp("ElectricitySpell")) {
				maestro.PlayElectricShoot();
				anim.SetTrigger ("goospellTrigger");
                f_nextElectric = 0;
			    f_nextCast = 0;
			    GameObject go_spell = Instantiate(go_electricShot, t_spellSpawn.position, t_spellSpawn.rotation);

                Physics.IgnoreCollision(GetComponent<Collider>(), go_spell.GetComponent<Collider>());
                go_spell.transform.localScale = new Vector3(f_projectileSize, f_projectileSize, f_projectileSize);
                go_spell.GetComponent<Rigidbody>().velocity = transform.forward * Constants.SpellStats.C_ElectricSpeed;
                SpellController sc_firing = go_spell.GetComponent<SpellController>();
                sc_firing.Init(this, e_color, f_electricCharge);
                f_electricCharge = 0;
                anim.SetFloat("gooCharge", f_electricCharge);
            }        
		}
       
    }
#endregion
}
