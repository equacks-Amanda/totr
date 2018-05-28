/*  Rift Controller - Joe Chew
 * 
 *  Desc:   Facilitates Rift volatility and enemy spawn
 * 
 */

using System;
using System.Collections;
using UnityEngine;

public sealed class RiftController : MonoBehaviour {

#region ART ADDITIONS
    [SerializeField] private Material[] mat_floorEmissions;
    [SerializeField] private ParticleSystem ps_arenaFog;
#endregion

#region Variables and Declarations
    [SerializeField] private GameObject go_riftDeathBolt;
    [SerializeField] private GameObject go_boardClear;
    [SerializeField] private GameObject go_screenshake;
    public GameObject[] go_playerReferences;    // TODO: write a getter for this
    [SerializeField] private GameObject[] go_deathOrbs;
    [SerializeField]
    private RiftBossController rbc_redRiftBossController;
    [SerializeField]
    private RiftBossController rbc_blueRiftBossController;
    [SerializeField] private GameObject[] go_lightsRight;
    [SerializeField] private GameObject[] go_lightsLeft;

    // enemies
	[SerializeField] private GameObject[] go_skeletons;
	[SerializeField] private GameObject[] go_runes;

	[SerializeField] private GameObject[] go_necromancers;

    [SerializeField] private Animator anim;


    private int i_leftEnemies = 0;
    private int i_rightEnemies = 0;
    private int i_nextEnemySpawnIndex = 0;
    private GameObject[] go_rightEnemySpawners;
    private GameObject[] go_leftEnemySpawners;


    private int i_redObjectivesComplete = 0;
    private int i_blueObjectivesComplete = 0;

    private float f_enemySpeed;

	private int i_volatilityLevel;
    private float f_volatility;
    private float f_volatilityMultiplier;
    private Constants.RiftStats.Volatility e_currentVolatilityLevel;
    private Maestro maestro;     // reference to audio controller singleton

    private System.Random r_random = new System.Random();

    // Singleton
    private static RiftController instance;
    public static RiftController Instance {
        get { return instance; }
    }

    #region Getters and Setters
    public GameObject[] PlayerReferences {
        get { return go_playerReferences; }
    }

	public float EnemySpeed {
		get { return f_enemySpeed; }
	}

    public GameObject[] RightEnemySpawners {
        set { go_rightEnemySpawners = value; }
    }

    public GameObject[] LeftEnemySpawners {
        set { go_leftEnemySpawners = value; }
    }

    public Constants.Global.Color GetRiftBossWinningTeamColor() {
        if (rbc_redRiftBossController.Health < rbc_blueRiftBossController.Health) {
            return Constants.Global.Color.RED;
        }
        else {
            return Constants.Global.Color.BLUE;
        }
    }
    #endregion
#endregion

#region RiftController Methods
    #region Volatility
    public void IncreaseVolatility(float volatilityUp) {
		maestro.PlayAnnouncementVolatilityUp();
        volatilityUp += (volatilityUp * f_volatilityMultiplier);
        f_volatility += volatilityUp;
        if (f_volatility >= 100.0f && e_currentVolatilityLevel != Constants.RiftStats.Volatility.ONEHUNDRED) {
            e_currentVolatilityLevel = Constants.RiftStats.Volatility.ONEHUNDRED;
            Invoke("ResetVolatility", Constants.RiftStats.C_VolatilityResetTime);
            anim.SetTrigger("rawrTrigger");
			maestro.PlayRiftRoar();
            BoardClear();
        }
        else if (f_volatility >= 75.0f && e_currentVolatilityLevel != Constants.RiftStats.Volatility.SEVENTYFIVE) {
            e_currentVolatilityLevel = Constants.RiftStats.Volatility.SEVENTYFIVE;
			i_volatilityLevel = 4;
            EnterNewVolatilityLevel();
            anim.SetTrigger("rawrTrigger");
			maestro.PlayRiftRoar();
            anim.SetInteger("volatility", 4);
        }
        else if (f_volatility >= 65.0f && e_currentVolatilityLevel != Constants.RiftStats.Volatility.SIXTYFIVE) {
            e_currentVolatilityLevel = Constants.RiftStats.Volatility.SIXTYFIVE;
			i_volatilityLevel = 3;
            anim.SetTrigger("rawrTrigger");
			maestro.PlayRiftRoar();
            EnterNewVolatilityLevel();
        }
        else if (f_volatility >= 50.0f && e_currentVolatilityLevel != Constants.RiftStats.Volatility.FIFTY) {
            e_currentVolatilityLevel = Constants.RiftStats.Volatility.FIFTY;
			i_volatilityLevel = 3;
            anim.SetTrigger("rawrTrigger");
			maestro.PlayRiftRoar();
            anim.SetInteger("volatility", 3);
            EnterNewVolatilityLevel();
        }
        else if (f_volatility >= 35.0f && e_currentVolatilityLevel != Constants.RiftStats.Volatility.THIRTYFIVE) {
            e_currentVolatilityLevel = Constants.RiftStats.Volatility.THIRTYFIVE;
			i_volatilityLevel = 2;
            anim.SetTrigger("rawrTrigger");
			maestro.PlayRiftRoar();
            EnterNewVolatilityLevel();
        }
        else if (f_volatility >= 25.0f && e_currentVolatilityLevel != Constants.RiftStats.Volatility.TWENTYFIVE) {
            e_currentVolatilityLevel = Constants.RiftStats.Volatility.TWENTYFIVE;
			i_volatilityLevel = 2;
            anim.SetTrigger("rawrTrigger");
			maestro.PlayRiftRoar();
            anim.SetInteger("volatility", 2);
            EnterNewVolatilityLevel();
            Constants.Global.Color colorToAttack = DetermineWinningTeam();
            FireDeathBolts(gameObject.transform, colorToAttack);
        }
        else if (f_volatility >= 5.0f && e_currentVolatilityLevel != Constants.RiftStats.Volatility.FIVE) {
            e_currentVolatilityLevel = Constants.RiftStats.Volatility.FIVE;
			i_volatilityLevel = 1;
            anim.SetTrigger("rawrTrigger");
			maestro.PlayRiftRoar();
            anim.SetInteger("volatility", 1);
            EnterNewVolatilityLevel();
        }
        else if (f_volatility < 5.0f) {
			i_volatilityLevel = 0;
            EnterNewVolatilityLevel();
        }
    }

    private void EnterNewVolatilityLevel() {
        maestro.PlayVolatilityAmbience(i_volatilityLevel);
        switch (i_volatilityLevel) {
            case 0:
                // Change rift visual to L0
                e_currentVolatilityLevel = Constants.RiftStats.Volatility.ZERO;
                f_volatilityMultiplier = Constants.RiftStats.C_VolatilityMultiplier_L1;     // there is no L0, L1 is already 0
                f_enemySpeed = Constants.EnemyStats.C_EnemyBaseSpeed;
                break;
            case 1:
                // Change rift visual to L1
                f_volatilityMultiplier = Constants.RiftStats.C_VolatilityMultiplier_L1;
                break;
            case 2:
                // Change rift visual to L2
                f_volatilityMultiplier = Constants.RiftStats.C_VolatilityMultiplier_L2;
                break;
            case 3:
                // Change rift visual to L3
                f_volatilityMultiplier = Constants.RiftStats.C_VolatilityMultiplier_L3;
                break;
            case 4:
                // Change rift visual to L4
                f_volatilityMultiplier = Constants.RiftStats.C_VolatilityMultiplier_L4;
                break;
        }
    }

    public void ResetVolatility() {
        f_volatility = 0.0f;
		i_volatilityLevel = 0;
        EnterNewVolatilityLevel();
    }
    #endregion
    
    #region Rift Volatility Attacks and Effects
    private void BoardClear() {
		maestro.PlayAnnouncementBoardClear();
        Invoke("TurnOffBoardClear", 10f);
        go_boardClear.SetActive(true);
        go_screenshake.SetActive(true);
        foreach (GameObject player in go_playerReferences) {
            player.GetComponent<PlayerController>().TakeDamage(Constants.PlayerStats.C_MaxHealth,Constants.Global.DamageType.RIFT);
        }

		if (go_necromancers[0]) {
			for (int i = 0; i < go_skeletons.Length; i++) {
				if (go_skeletons[i].activeInHierarchy) {
                    if( go_skeletons[i].GetComponent<SkeletonController>() != null )
					    go_skeletons[i].GetComponent<SkeletonController>().TakeDamage(Constants.EnemyStats.C_EnemyHealth, Constants.Global.Color.NULL);
				}
			}

			for (int i = 0; i < go_necromancers.Length; i++) {
				if (go_necromancers[i].activeInHierarchy)
					go_necromancers[i].GetComponent<NecromancerController>().TakeDamage(Constants.EnemyStats.C_NecromancerHealth, Constants.Global.Color.NULL);
			}

			for (int i = 0; i < go_runes.Length; i++) {
				if (go_runes[i].activeInHierarchy)
					go_runes[i].SetActive(false);
			}
		}
    }

    private void TurnOffBoardClear()
    {
        go_boardClear.SetActive(false);
        go_screenshake.SetActive(false);
    }

	public void ActivateEnemy(Vector3 position) {
        // move skeleton into position - must happen before Init() is called
        go_skeletons[i_nextEnemySpawnIndex].transform.position = position;

        if (!(go_skeletons[i_nextEnemySpawnIndex].activeSelf)) {

			if (position.x < 0f) {
				go_skeletons[i_nextEnemySpawnIndex].GetComponentInChildren<SkeletonController>().Init(Constants.Global.Side.LEFT);
				i_leftEnemies++;
			}
			else {
				go_skeletons[i_nextEnemySpawnIndex].GetComponentInChildren<SkeletonController>().Init(Constants.Global.Side.RIGHT);
				i_rightEnemies++;
			}

			go_skeletons[i_nextEnemySpawnIndex].SetActive(true);
		}
		i_nextEnemySpawnIndex = (i_nextEnemySpawnIndex+1)%go_skeletons.Length;
	}

    public void ActivateRune(Vector3 position) {
	    for (int i = 0; i < go_runes.Length; i++) {
			if (!(go_runes[i].activeSelf)) {

				if (position.x < 0f) {
					go_runes[i].transform.position = position;
					go_runes[i].SetActive(true);
				}
				else {
					go_runes[i].transform.position = position;
					go_runes[i].SetActive(true);
				}
				break;
			}
        }
	}

    // Spawns one enemy on either side of the Rift, randomly chosen position
    public void SpawnEnemies() {
        int randLeft = UnityEngine.Random.Range(0, go_leftEnemySpawners.Length);
        int randRight = UnityEngine.Random.Range(0, go_rightEnemySpawners.Length);

        if (i_leftEnemies < Constants.EnemyStats.C_EnemySpawnCapPerSide) {
            Vector3 pos = go_leftEnemySpawners[randLeft].transform.position;
            CircularEnemySpawn(pos, Constants.Global.Side.LEFT);
        }
        if (i_rightEnemies < Constants.EnemyStats.C_EnemySpawnCapPerSide) {
            Vector3 pos = go_rightEnemySpawners[randRight].transform.position;
            CircularEnemySpawn(pos, Constants.Global.Side.RIGHT);
        }
    }

    // Spawns an enemy at a specified position
    public void SpawnEnemy(Vector3 position, Constants.Global.Side side) {
        // only spawn if below enemy side cap TODO: is this expected behavior?
        if ((side == Constants.Global.Side.LEFT && i_leftEnemies < Constants.EnemyStats.C_EnemySpawnCapPerSide) || (side == Constants.Global.Side.RIGHT && i_rightEnemies < Constants.EnemyStats.C_EnemySpawnCapPerSide)) {
			ActivateEnemy(position);
        }
    }

    // Spawns an enemy within a radius when a valid position is selected
    public void CircularEnemySpawn(Vector3 center, Constants.Global.Side side) {
        Vector3 spawnPos = RandomCircle(center, side, Constants.EnemyStats.C_SpawnRadius);

        // Checks to see if the spawn position is already occupied by anything with a collider
        // If it is, find a new spawn position for the enemy
        var hitColliders = Physics.OverlapSphere(spawnPos, 0.0005f);
        if (hitColliders.Length > 0) {
            CircularEnemySpawn(center, side);
        }
        else {
            SpawnEnemy(spawnPos, side);
        }
    }

    // Decrease enemy count per side on enemy death
    public void DecreaseEnemies(Constants.Global.Side side) {
        if (side == Constants.Global.Side.LEFT) {
            i_leftEnemies--;
        }
        else {
            i_rightEnemies--;
        }
    }

    // Gets a random Vector3 position within a given radius
    private Vector3 RandomCircle(Vector3 center, Constants.Global.Side side, float radius) {
        float ang = UnityEngine.Random.value * 360;
        Vector3 pos;

        // by absolute valueing the x position, we can tell the enemy which side it should of the map it should be on
        int s = (int)side;
        pos.x = s * Mathf.Abs(center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad));
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);

        // Reposition the enemies if they spawn outside of the map TODO: revisit once map is scaled, and these should be Constants anyway
        if (pos.z >= Constants.EnemyStats.C_MapBoundryZAxis) {
            float diff = pos.z - Constants.EnemyStats.C_MapBoundryZAxis;
            pos.z = pos.z - diff - 1;
        }
        else if (pos.z <= (-1.0f * Constants.EnemyStats.C_MapBoundryZAxis)) {
            float diff = pos.z + Constants.EnemyStats.C_MapBoundryZAxis;
            pos.z = pos.z - diff + 1;
        }

        if (pos.x >= Constants.EnemyStats.C_MapBoundryXAxis) {
            float diff = pos.x - Constants.EnemyStats.C_MapBoundryXAxis;
            pos.x = pos.x - diff - 1;
        }
        else if (pos.x <= (-1.0f * Constants.EnemyStats.C_MapBoundryXAxis)) {
            float diff = pos.x + Constants.EnemyStats.C_MapBoundryXAxis;
            pos.x = pos.x - diff + 1;
        }
        //Checks to see if the pos.x is the same side thats been passed in.  if not, then swap the side value
        if(!(pos.x >= 0 && s >= 0) && !(pos.x < 0 && s < 0)) {
            pos.x *= -1;
        }

        return pos;
    }

    // @Joe get this working
    // Joe: NO BITCH, I DO WHAT I WANT! >:(
    public void FireDeathBolts(Transform firePosition, Constants.Global.Color c) {
     // Only shoot at players of Color c, and don't collide with anything EXCEPT players (layer matrix, probably)

        float f_projectileSize = Constants.SpellStats.C_PlayerProjectileSize;
        firePosition.position = new Vector3(firePosition.position.x, 1.0f, firePosition.position.z);
        
        var array = new int[] { 0, 1, 2, 3 };
        new System.Random().Shuffle(array);

        for (int i = 0; i < 4; i++) {
            if (go_playerReferences[array[i]].GetComponent<PlayerController>().Color == c) {
                GameObject go_spell = Instantiate(go_riftDeathBolt, firePosition.position, firePosition.rotation);
                go_spell.transform.localScale = new Vector3(f_projectileSize, f_projectileSize, f_projectileSize);
                go_spell.GetComponent<Rigidbody>().velocity = go_playerReferences[array[i]].transform.position.normalized * Constants.RiftStats.C_VolatilityDeathboltSpeed;
                go_spell.transform.forward = -1 * (go_spell.GetComponent<Rigidbody>().velocity.normalized);
                break;
            }
        }
    }
    #endregion

    #region FireDeathbolt() Helper Functions
    public void IncrementObjectiveCount(Constants.Global.Color e_colorIn) {
        if (e_colorIn == Constants.Global.Color.BLUE) {
            i_blueObjectivesComplete++;
        }
        else if (e_colorIn == Constants.Global.Color.RED) {
            i_redObjectivesComplete++;
        }
    }

    private Constants.Global.Color DetermineWinningTeam() {
        if (i_blueObjectivesComplete > i_redObjectivesComplete) {
            return Constants.Global.Color.BLUE;
        }
        else if (i_redObjectivesComplete > i_blueObjectivesComplete) {
            return Constants.Global.Color.RED;
        }
        else {
            System.Random rand = new System.Random();
            int team = Convert.ToInt32(rand.Next(1, 2));

            if (team == 1) {
                return Constants.Global.Color.BLUE;
            }
            else {
                return Constants.Global.Color.RED;
            }
        }
    }
    #endregion

    // ART ADDITION: Organaize functions that occur during objective swap into one function call for Dark Magician.
    public void ObjectiveSwap( int level) {
        ActivateLights(level);
        SwapFloorEmissions(level);
        IncreaseFogDensity(level);
    }

    void ActivateLights(int level) {
        for (int i = 0; i < go_lightsLeft.Length; i++) { 
            go_lightsLeft[i].SetActive(false);
            go_lightsRight[i].SetActive(false);
        }

        go_lightsLeft[level].SetActive(true);
        go_lightsRight[level].SetActive(true);
    }

    // ART ADDITION : Adds small cracks of color to the floor determined by objective number.
    void SwapFloorEmissions( int level) {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");

       if( mat_floorEmissions != null )
        {
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i].GetComponent<Renderer>().material = mat_floorEmissions[level];
            }
        }
    }

    // ART ADDITION : Increases fog density determined by objective number.
    void IncreaseFogDensity( int level) {
        if(ps_arenaFog != null )
        {
            ps_arenaFog.emissionRate = level * 75;
        }
    }

    void PlayNoise() {
		maestro.PlayVolatilityNoise(i_volatilityLevel);
		Invoke("PlayNoise", r_random.Next(5,10));
	}

    public void ResetPlayers() {
        if (Constants.UnitTests.C_RunningCTFTests)
            return;

        go_playerReferences[0].transform.localPosition = Constants.PlayerStats.C_r1Start;
        go_playerReferences[1].transform.localPosition = Constants.PlayerStats.C_r2Start;
        go_playerReferences[2].transform.localPosition = Constants.PlayerStats.C_b1Start;
        go_playerReferences[3].transform.localPosition = Constants.PlayerStats.C_b2Start;
    }

    IEnumerator TurnOffDeathOrb(GameObject go_deathOrb, GameObject go_player) {
        yield return new WaitForSeconds(Constants.RiftStats.C_RiftTeleportDelay);
        go_player.transform.position = go_player.transform.position + (int)go_player.GetComponent<PlayerController>().Side * Constants.RiftStats.C_RiftTeleportOffset;
        go_player.SetActive(true);
        go_deathOrb.SetActive(false);
    }
#endregion

#region Unity Overrides
    void Awake() {
        instance = this;
    }

    void Start() {
		maestro = Maestro.Instance;
        ResetVolatility();
		Invoke("PlayNoise", r_random.Next(0,10));
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && !other.GetComponent<PlayerController>().Wisp && !other.GetComponent<PlayerController>().Invulnerable) {
            other.GetComponent<PlayerController>().TakeDamage(Constants.PlayerStats.C_MaxHealth, Constants.Global.DamageType.RIFT);
            //while (!isWisp)
            //{
            //    Debug.Log("I'm waiting for the player to be a wisp, because then they will have dropped the flag and I can move them across the rift.");
            //}

            for(int i =0; i<go_deathOrbs.Length; i++) {
                if (!go_deathOrbs[i].activeSelf) {
                    go_deathOrbs[i].transform.position = new Vector3(0.0f, other.gameObject.transform.position.y, other.gameObject.transform.position.z);
                    go_deathOrbs[i].SetActive(true);
                    other.gameObject.SetActive(false);
                    StartCoroutine(TurnOffDeathOrb(go_deathOrbs[i], other.gameObject));
                    break;
                }
            }
        }
    }
    #endregion
}
