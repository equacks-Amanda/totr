using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HockeyInstructions : MonoBehaviour
{
    public GameObject apprentice1;
    public GameObject apprentice2;
    public GameObject windParry1;
    public GameObject windParry2;


    Vector3 originalPos;
    Vector3 puckPosition;
    Quaternion puckRotation;

    public Animator anim1;
    public Animator anim2;
    public GameObject puck;

    private bool apprentince1Run = false;
    private bool apprentince2Run = false;

    private Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        originalPos = apprentice1.transform.position;
        puckPosition = puck.transform.position;
        puckRotation = puck.transform.rotation;
        apprentice2.SetActive(false);
        StartCoroutine(CrystalHockey());
    }

    // Update is called once per frame
    void Update ()
    {
	    if (apprentince1Run)
        {
            apprentice1.transform.Translate(Vector3.forward * Time.deltaTime * 3.0f);
        }
        if (apprentince2Run)
        {
            apprentice2.transform.Translate(Vector3.forward * Time.deltaTime * 4.8f);
        }
    }

    IEnumerator CrystalHockey()
    {
        //Phase 1: Wind parry the puck into the goal.
        yield return new WaitForSeconds(0.25f);
        MoveToPuck();
        yield return new WaitForSeconds(1.5f);
        pushGem();
        yield return new WaitForSeconds(0.5f);
        GoalReached();
        puck.transform.position = puckPosition;
        puck.transform.rotation = puckRotation;
        yield return new WaitForSeconds(1);
        
        //Phase 2: Opponent Intercepts
        apprentice1.transform.position = originalPos;
        puck.SetActive(true);
        windParry1.SetActive(false);
        apprentice2.SetActive(true);
        StartCoroutine(EnemyInterception());
        yield return new WaitForSeconds(0.25f);
        MoveToPuck();
        yield return new WaitForSeconds(1.5f);
        pushGem();
    }

    IEnumerator EnemyInterception()
    {
        Chase();
        yield return new WaitForSeconds(1.85f);
        windParry2.SetActive(true);
        apprentince2Run = false;
        anim2.SetTrigger("stop");
        yield return new WaitForSeconds(2);
        Debug.Log("End of Instruction.");

    }

    void MoveToPuck()
    {
        apprentince1Run = true;
        anim1.SetTrigger("run");
    }

    void pushGem()
    {
        anim1.SetTrigger("stop");
        apprentince1Run = false;
        windParry1.SetActive(true);
    }

    void GoalReached()
    {
        apprentince1Run = false;
        anim1.SetTrigger("stop");
        puck.SetActive(false);
        puck.GetComponent<Rigidbody>().velocity = Vector3.zero;
        puck.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    void Chase()
    {
        apprentince2Run = true;
        anim2.SetTrigger("run");
    }

    void DropGem()
    {
        puck.transform.parent = null;
    }
}
