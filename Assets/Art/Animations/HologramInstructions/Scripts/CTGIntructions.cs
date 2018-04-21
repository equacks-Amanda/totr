using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTGIntructions : MonoBehaviour
{
    public GameObject apprentice1;
    public GameObject apprentice2;
    public GameObject magicMissile;

    Vector3 originalPos;
    Vector3 gemPosition;

    public Animator anim1;
    public Animator anim2;
    public GameObject gem;

    private bool apprentince1Run = false;
    private bool apprentince2Run = false;

    private Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(CaptureTheGem());
        originalPos = apprentice1.transform.position;
        gemPosition = gem.transform.position;
        apprentice2.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (apprentince1Run)
        {
            apprentice1.transform.Translate(Vector3.forward * Time.deltaTime * 5.0f);
        }
        if (apprentince2Run)
        {
            apprentice2.transform.Translate(Vector3.forward * Time.deltaTime * 3.0f);
        }
    }

    IEnumerator CaptureTheGem()
    {
        //Phase 1: Grab the Gem and take it to the goal.
        //yield return new WaitForSeconds(0.5f);
        MoveToGem();
        yield return new WaitForSeconds(0.8f);
        GrabGem();
        yield return new WaitForSeconds(1);
        GoalReached();
        DropGem();
        gem.SetActive(false);
        //apprentice1.GetComponent<PlayerController>().DropFlag();
        gem.transform.position = gemPosition;
        yield return new WaitForSeconds(1);


        //Phase 2: Opponenet Intercepts
        apprentice1.transform.position = originalPos;
        gem.SetActive(true);
        apprentice2.SetActive(true);
        StartCoroutine(EnemyInterception());
        //yield return new WaitForSeconds(0.5f);
        MoveToGem();
        yield return new WaitForSeconds(0.8f);
        GrabGem();
        yield return new WaitForSeconds(0.5f);
        DropGem();
        yield return new WaitForSeconds(1);
        GoalReached();
        yield return new WaitForSeconds(1);
        Debug.Log("End of Instruction.");

        //apprentice1.GetComponent<PlayerController>().DropFlag();
    }

    IEnumerator EnemyInterception()
    {
        //yield return new WaitForSeconds(0.5f);
        Chase();
        yield return new WaitForSeconds(0.6f);
        Debug.Log("Fire.");
        GameObject go_spell = Instantiate(magicMissile, apprentice2.transform.position, apprentice2.transform.rotation);
        go_spell.GetComponent<Rigidbody>().velocity = Vector3.left * Constants.SpellStats.C_MagicMissileSpeed;
        yield return new WaitForSeconds(1.2f);
        apprentince2Run = false;
        anim2.SetTrigger("stop");

    }

    void MoveToGem()
    {
        apprentince1Run = true;
        anim1.SetTrigger("runToGem");
    }

    void GrabGem()
    {
        //apprentice1.GetComponent<PlayerController>().Pickup(gem);
        gem.transform.parent = apprentice1.transform;
    }

    void GoalReached()
    {
        apprentince1Run = false;
        anim1.SetTrigger("stop");
    }

    void Chase()
    {
        apprentince2Run = true;
        anim2.SetTrigger("runToGem");
    }

    void DropGem()
    {
        gem.transform.parent = null;
    }
}
