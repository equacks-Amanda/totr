using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTrailInstructions : MonoBehaviour
{
    public GameObject apprentice;
    public GameObject magicMissile;
    public GameObject riftHead;
    public GameObject deathBolt;
    public GameObject forceField;
    public GameObject skeleton;


    Vector3 originalPos;
    Vector3 necromancerPos;

    public Animator apprenticeAnim;

    private bool canShoot = false;
    private bool isRunning = false;
    private bool isChasing = false;

    private Rigidbody rb;
    private float _fireRate;
    private float _bulletLifetime;
	private bool isMissile = false;
	private Vector3 start;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(FinalTrial());
        originalPos = apprentice.transform.position;
        _fireRate = 0.5f;
        forceField.SetActive(false);
        skeleton.SetActive(false);
        _bulletLifetime = 0.4f;
		start = magicMissile.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            apprentice.transform.Translate(Vector3.left * Time.unscaledDeltaTime * 3.0f);
        }
        if (isChasing)
        {
            skeleton.transform.Translate(Vector3.forward * Time.unscaledDeltaTime * 1.0f);
        }
        if (canShoot)
        {
            if (_fireRate <= 0f)
            {
                isMissile = true;
				magicMissile.SetActive(true);
                StartCoroutine("ResetBullet");
                _fireRate = 1.0f;
            }
            _fireRate -= Time.unscaledDeltaTime;
        }
		if(isMissile)
			magicMissile.transform.Translate(Vector3.down * Time.unscaledDeltaTime * 10.0f);
    }
	
	IEnumerator ResetBullet(){
		yield return new WaitForSecondsRealtime(_bulletLifetime);
		magicMissile.transform.position = start;
		magicMissile.SetActive(false);
		isMissile = false;
	}

    IEnumerator FinalTrial()
    {
        //Phase 1: Attack Rift Head and dodge death bolt.
        Shoot();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1f));
        StrafeLeft();
        FireBolt();
        GameObject bolt = Instantiate(deathBolt, riftHead.transform.position, riftHead.transform.rotation);
        bolt.GetComponent<Rigidbody>().velocity = Vector3.back * Constants.SpellStats.C_MagicMissileSpeed;
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1f));
        isRunning = false;
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1f));
        ResetPosition();

        //Phase 2: Destroy Skeleton to deactivate shields.
        apprenticeAnim.Play("2Fire", -1, 0f);
        skeleton.SetActive(true);
        forceField.SetActive(true);
        isChasing = true;
        canShoot = true;
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(2.0f));
        skeleton.SetActive(false);
        forceField.SetActive(false);
        canShoot = false;
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1.5f));


        Debug.Log("End of Instruction.");
    }

    void Shoot()
    {
        apprenticeAnim.Play("FinalTrial", -1, 0f);
    }

    void ResetPosition()
    {
        apprentice.transform.position = originalPos;
    }

    void StrafeLeft()
    {
        isRunning = true;
    }
    void FireBolt()
    {
        GameObject bolt = Instantiate(deathBolt, riftHead.transform.position, riftHead.transform.rotation);
        bolt.GetComponent<Rigidbody>().velocity = Vector3.back * Constants.SpellStats.C_MagicMissileSpeed;
        Destroy(bolt, 2.0f);
    }
}
