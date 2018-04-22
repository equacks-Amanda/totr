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
    public Animator riftAnim;

    private bool canShoot = false;
    private bool isRunning = false;
    private bool isChasing = false;

    private Rigidbody rb;
    private float _fireRate;
    private float _bulletLifetime;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(FinalTrial());
        originalPos = apprentice.transform.position;
        _fireRate = 0.5f;
        forceField.SetActive(false);
        skeleton.SetActive(false);
        _bulletLifetime = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            apprentice.transform.Translate(Vector3.left * Time.deltaTime * 3.0f);
        }
        if (isChasing)
        {
            skeleton.transform.Translate(Vector3.forward * Time.deltaTime * 1.0f);
        }
        if (canShoot)
        {
            if (_fireRate <= 0f)
            {
                GameObject go_spell1 = Instantiate(magicMissile, apprentice.transform.position, apprentice.transform.rotation);
                go_spell1.GetComponent<Rigidbody>().velocity = Vector3.forward * Constants.SpellStats.C_MagicMissileSpeed;
                Destroy(go_spell1, _bulletLifetime);
                _fireRate = 1.0f;
                if (!isRunning)
                {
                    apprenticeAnim.SetTrigger("shoot");
                }
            }
            _fireRate -= Time.deltaTime;
        }
        else if (!canShoot)
        {
            apprenticeAnim.SetTrigger("stop");
        }
    }

    IEnumerator FinalTrial()
    {
        //Phase 1: Attack Rift Head and dodge death bolt.
        Shoot();
        yield return new WaitForSeconds(1f);
        StrafeLeft();
        FireBolt();
        GameObject bolt = Instantiate(deathBolt, riftHead.transform.position, riftHead.transform.rotation);
        bolt.GetComponent<Rigidbody>().velocity = Vector3.back * Constants.SpellStats.C_MagicMissileSpeed;
        yield return new WaitForSeconds(1f);
        isRunning = false;
        apprenticeAnim.SetTrigger("stop");
        yield return new WaitForSeconds(1f);
        ResetPosition();

        //Phase 2: Destroy Skeleton to deactivate shields.
        skeleton.SetActive(true);
        forceField.SetActive(true);
        isChasing = true;
        canShoot = true;
        Destroy(skeleton, 2f);
        yield return new WaitForSeconds(2.0f);
        forceField.SetActive(false);
        canShoot = false;
        yield return new WaitForSeconds(1.5f);


        Debug.Log("End of Instruction.");
    }

    void Shoot()
    {
        apprenticeAnim.SetTrigger("shoot");
        GameObject go_spell1 = Instantiate(magicMissile, apprentice.transform.position, apprentice.transform.rotation);
        go_spell1.GetComponent<Rigidbody>().velocity = Vector3.forward * Constants.SpellStats.C_MagicMissileSpeed;
        Destroy(go_spell1, 0.8f);
    }

    void ResetPosition()
    {
        apprentice.transform.position = originalPos;
    }

    void StrafeLeft()
    {
        apprenticeAnim.SetTrigger("strafeLeft");
        isRunning = true;
    }
    void FireBolt()
    {
        GameObject bolt = Instantiate(deathBolt, riftHead.transform.position, riftHead.transform.rotation);
        bolt.GetComponent<Rigidbody>().velocity = Vector3.back * Constants.SpellStats.C_MagicMissileSpeed;
        Destroy(bolt, 2.0f);
    }
}
