using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class CTF_UnitTests
{
    #region Object References
    GameObject CTFCamera;
    GameObject CTFMaestro;
    GameObject CTFCanvas;
    GameObject CTFRift;

    GameObject CTFRedObjective;
    GameObject CTFRedFlag;
    GameObject CTFRedGoal;
    GameObject CTFRedPlayer;
    GameObject CTFRedPlayer2;

    GameObject CTFBlueObjective;
    GameObject CTFBlueFlag;
    GameObject CTFBlueGoal;
    //GameObject CTFBluePlayer;
    #endregion

    #region Before and After Methods
    [OneTimeSetUp]
    public void StartUp()
    {  // StartUp runs once before running any test cases.
        Debug.Log("In StartUp");
        Constants.UnitTests.C_RunningCTFTests = true;
        SpawnCamera();
        SpawnMaestro();
        SpawnCanvas();
        SpawnRift();
    }

    [OneTimeTearDown]
    public void CleanUp()
    {   // CleanUp runs once after all test cases are finished.
        Debug.Log("In CleanUp");
        GameObject.Destroy(CTFRift);
        GameObject.Destroy(CTFCanvas);
        GameObject.Destroy(CTFMaestro);
        GameObject.Destroy(CTFCamera);
    }

    [SetUp]
    public void SetUp()
    { // SetUp runs before every test case
        Debug.Log("In SetUp");
    }

    [TearDown]
    public void TearDown()
    {  // TearDown runs after every test case
        Debug.Log("In TearDown");
        if (CTFRedPlayer) GameObject.Destroy(CTFRedPlayer);
        if (CTFRedObjective) GameObject.Destroy(CTFRedObjective);
        //if (CTFBluePlayer) GameObject.Destroy(CTFBluePlayer);
        if (CTFBlueObjective) GameObject.Destroy(CTFBlueObjective);
    }
    #endregion

    #region Tests
    [UnityTest]
    public IEnumerator CTFPickup()
    {
        Debug.Log("In CTFPickup");

        SpawnRedObjective();
        SpawnRedPlayer();

        PickupFlag(CTFRedPlayer, CTFRedFlag);

        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.True(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag is not picked up.");
        Assert.True(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is not held by player.");
    }

    [UnityTest]
    public IEnumerator CTFDrop()
    {
        Debug.Log("In CTFDrop");

        SpawnRedObjective();
        SpawnRedPlayer();

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().Interact();

        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag was not dropped.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is still held by player.");
    }

    [UnityTest]
    public IEnumerator CTFPickupOppositeFlag()
    {
        Debug.Log("In CTFPickupOppositeFlag");

        SpawnBlueObjective();
        SpawnRedPlayer();

        PickupFlag(CTFRedPlayer, CTFBlueFlag);

        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFBlueFlag.GetComponent<FlagController>().IsPickedUp(), "Flag was picked up.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player.");
    }

    [UnityTest]
    public IEnumerator CTFScore()
    {
        Debug.Log("In CTFScore");

        SpawnRedObjective();
        Assert.True(CTFRedObjective.GetComponent<CaptureTheFlagObjective>().Score == 0, "Score is not zero.");
        SpawnRedPlayer();

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.transform.position = CTFRedGoal.transform.position;

        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag is picked up after score.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player after score.");
        Assert.True(CTFRedFlag.transform.localPosition == Constants.ObjectiveStats.C_BlueFlagSpawn, "Flag is not returned to initial spawn position.");
        Assert.True(CTFRedObjective.GetComponent<CaptureTheFlagObjective>().Score == 1, "Score was not incremented correctly.");
    }

    [UnityTest]
    public IEnumerator CTFStealFlag()
    {
        Debug.Log("In CTFStealFlag");

        SpawnRedObjective();
        Assert.True(CTFRedObjective.GetComponent<CaptureTheFlagObjective>().Score == 0, "Score is not zero.");
        SpawnRedPlayer();
        SpawnRedPlayer();

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        PickupFlag(CTFRedPlayer2, CTFRedFlag);

        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.True(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag is not picked up.");
        Assert.True(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is not held by player.");
        Assert.False(CTFRedPlayer2.GetComponent<PlayerController>().HasFlag, "Flag was stolen.");
    }

    [UnityTest]
    public IEnumerator CTFDropFlagOnHit()
    {
        Debug.Log("In CTFDropFlagOnHit");

        SpawnRedObjective();
        SpawnRedPlayer();

        // wind spell
        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.WIND, Constants.Global.Color.RED, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag picked up after red wind spell.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player after red wind spell.");

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.WIND, Constants.Global.Color.BLUE, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag picked up after blue wind spell.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player after blue wind spell.");

        // magic missile
        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.MAGICMISSILE, Constants.Global.Color.RED, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.True(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag is dropped after red magic missile.");
        Assert.True(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is not held by player after red magic missile.");
        CTFRedPlayer.GetComponent<PlayerController>().Interact();
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.MAGICMISSILE, Constants.Global.Color.BLUE, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag picked up after blue magic missile.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player after blue magic missile.");

        // electricity
        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.ELECTRICITYAOE, Constants.Global.Color.RED, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.True(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag is dropped after red electricity spell.");
        Assert.True(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is not held by player after red electricity spell.");
        CTFRedPlayer.GetComponent<PlayerController>().Interact();
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.ELECTRICITYAOE, Constants.Global.Color.BLUE, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag picked up after blue electricity spell.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player after blue electricity spell.");

        // ice
        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.ICE, Constants.Global.Color.RED, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag picked up after red ice spell.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player after red ice spell.");

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().ApplySpellEffect(Constants.SpellStats.SpellType.ICE, Constants.Global.Color.BLUE, 0, Vector3.zero);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag picked up after blue ice spell.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is held by player after blue ice spell.");
    }

    [UnityTest]
    public IEnumerator CTFDropFlagOnDeath()
    {
        Debug.Log("In CTFDropFlagOnDeath");

        SpawnRedObjective();
        SpawnRedPlayer();

        PickupFlag(CTFRedPlayer, CTFRedFlag);
        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        CTFRedPlayer.GetComponent<PlayerController>().TakeDamage(Constants.PlayerStats.C_MaxHealth, Constants.Global.DamageType.WIND);

        yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        Assert.True(CTFRedPlayer.GetComponent<PlayerController>().Wisp, "Player is not wisped.");
        Assert.False(CTFRedFlag.GetComponent<FlagController>().IsPickedUp(), "Flag was not dropped.");
        Assert.False(CTFRedPlayer.GetComponent<PlayerController>().HasFlag, "Flag is still held by player.");
    }

    //flag crosses portal
    //flag dropped on rift?
    //indicators
    //spells don't affect flag
    //spells don't affect goal
    //keep other team's score correct


    [UnityTest]
    public IEnumerator CTFCompleteObjective()
    {
        Debug.Log("In CTFCompleteObjective");

        SpawnRedObjective();
        SpawnRedPlayer();

        for (int i = 0; i < Constants.ObjectiveStats.C_CTFMaxScore; i++)
        {
            PickupFlag(CTFRedPlayer, CTFRedFlag);
            yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
            CTFRedPlayer.transform.position = CTFRedGoal.transform.position;
            yield return new WaitForSeconds(Constants.UnitTests.C_WaitTime);
        }

        Assert.True(CTFRedObjective.GetComponent<CaptureTheFlagObjective>().Score == Constants.ObjectiveStats.C_CTFMaxScore, "Score was not incremented correctly.");
        Assert.True(CTFRedObjective.GetComponent<CaptureTheFlagObjective>().IsComplete, "Objective was not registered as complete.");
    }
    #endregion


    #region Spawn Helper Methods
    void SpawnCamera()
    {
        GameObject CTFCameraPrefab = Resources.Load("Unit Tests/PerspectiveCam") as GameObject;
        CTFCamera = GameObject.Instantiate(CTFCameraPrefab);
    }

    void SpawnMaestro()
    {
        GameObject CTFMaestroPrefab = Resources.Load("Unit Tests/Maestro") as GameObject;
        CTFMaestro = GameObject.Instantiate(CTFMaestroPrefab);
    }

    void SpawnCanvas()
    {
        GameObject CTFCanvasPrefab = Resources.Load("Unit Tests/Canvas") as GameObject;
        CTFCanvas = GameObject.Instantiate(CTFCanvasPrefab);
    }

    void SpawnRift()
    {
        GameObject CTFRiftPrefab = Resources.Load("Unit Tests/CTF_Rift") as GameObject;
        CTFRift = GameObject.Instantiate(CTFRiftPrefab);
    }

    void SpawnRedObjective()
    {
        GameObject CTFRedObjectivePrefab = Resources.Load("Unit Tests/CTF_RedObjective") as GameObject;
        CTFRedObjective = GameObject.Instantiate(CTFRedObjectivePrefab);
        CTFRedObjective.GetComponent<CaptureTheFlagObjective>().Activate(1);

        // get reference to child objects
        CTFRedFlag = CTFRedObjective.transform.GetChild(0).gameObject;
        CTFRedGoal = CTFRedObjective.transform.GetChild(1).gameObject;
    }

    void SpawnBlueObjective()
    {
        GameObject CTFBlueObjectivePrefab = Resources.Load("Unit Tests/CTF_BlueObjective") as GameObject;
        CTFBlueObjective = GameObject.Instantiate(CTFBlueObjectivePrefab);
        CTFBlueObjective.GetComponent<CaptureTheFlagObjective>().Activate(1);

        // get reference to child objects
        CTFBlueFlag = CTFBlueObjective.transform.GetChild(0).gameObject;
        CTFBlueGoal = CTFBlueObjective.transform.GetChild(1).gameObject;
    }

    void SpawnRedPlayer()
    {
        GameObject CTFRedPlayerPrefab = Resources.Load("Unit Tests/CTF_RedPlayer") as GameObject;
        if (!CTFRedPlayer) CTFRedPlayer = GameObject.Instantiate(CTFRedPlayerPrefab);
        else CTFRedPlayer2 = GameObject.Instantiate(CTFRedPlayerPrefab);
    }
    #endregion

    #region Other Helper Methods
    void PickupFlag(GameObject player, GameObject flag)
    {
        player.transform.position = flag.transform.position;
        player.GetComponent<PlayerController>().Interact();
    }
    #endregion

}
