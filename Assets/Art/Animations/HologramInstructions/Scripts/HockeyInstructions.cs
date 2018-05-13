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

    private bool puckMove = false;
    private bool push1 = false;
    private bool push2 = false;

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
            apprentice1.transform.Translate(Vector3.forward * Time.deltaTime * 3.5f);
        }
        if (apprentince2Run)
        {
            apprentice2.transform.Translate(Vector3.forward * Time.deltaTime * 5f);
        }
        if (puckMove)
        {
            if(push1)
            {
                puck.transform.Translate(Vector3.up * Time.deltaTime * 10f);
            }
            if (push2)
            {
                puck.transform.Translate(Vector3.left * Time.deltaTime * 10f);
            }
        }
    }

    IEnumerator CrystalHockey()
    {
        //Phase 1: Wind parry the puck into the goal.
        MoveToPuck();
        anim1.Play("Wind Parry", -1, 0f);
        yield return new WaitForSeconds(1.1f);
        pushGem();
        push1 = true;
        yield return new WaitForSeconds(0.5f);
        GoalReached();
        puck.transform.position = puckPosition;
        puck.transform.rotation = puckRotation;
        yield return new WaitForSeconds(1);

        //Phase 2: Opponent Intercepts
        anim1.Play("Wind Parry", -1, 0f);
        apprentice1.transform.position = originalPos;
        puck.SetActive(true);
        windParry1.SetActive(false);
        apprentice2.SetActive(true);
        MoveToPuck();
        StartCoroutine(EnemyInterception());
        MoveToPuck();
        yield return new WaitForSeconds(1.1f);
        pushGem();
    }

    IEnumerator EnemyInterception()
    {
        Chase();
        anim2.Play("Wind Parry", -1, 0f);
        yield return new WaitForSeconds(1.5f);
        windParry2.SetActive(true);
        apprentince2Run = false;
        push1 = false;
        push2 = true;
        yield return new WaitForSeconds(2);
        puck.SetActive(false);



        Debug.Log("End of Instruction.");
    }

    void MoveToPuck()
    {
        apprentince1Run = true;
    }

    void pushGem()
    {
        apprentince1Run = false;
        windParry1.SetActive(true);
        puckMove = true;

    }

    void GoalReached()
    {
        apprentince1Run = false;
        puck.SetActive(false);
        puckMove = false;
        puck.GetComponent<Rigidbody>().velocity = Vector3.zero;
        puck.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    void Chase()
    {
        apprentince2Run = true;
    }
}
