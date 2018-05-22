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
    private bool magicMissileRun = false;

    private Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(CaptureTheGem());
        originalPos = apprentice1.transform.position;
        gemPosition = gem.transform.position;
        apprentice2.SetActive(false);
        magicMissile.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (apprentince1Run)
        {
            apprentice1.transform.Translate(Vector3.forward * Time.unscaledDeltaTime * 5.0f);
        }
        if (apprentince2Run)
        {
            apprentice2.transform.Translate(Vector3.forward * Time.unscaledDeltaTime * 3.0f);
        }
		if (magicMissile != null && magicMissileRun)
			magicMissile.transform.Translate(Vector3.down * Time.unscaledDeltaTime * 10.0f);
    }

    IEnumerator CaptureTheGem()
    {
        //Phase 1: Grab the Gem and take it to the goal.
        //yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.5f));
        MoveToGem();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.8f));
        GrabGem();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1));
        GoalReached();
        DropGem();
        gem.SetActive(false);
        //apprentice1.GetComponent<PlayerController>().DropFlag();
        gem.transform.position = gemPosition;
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1));


        //Phase 2: Opponenet Intercepts
        //anim1.Play("Run");
        //animation["Run"].time = 0.0;
        anim1.Play("Run", -1, 0f);
        apprentice1.transform.position = originalPos;
        gem.SetActive(true);
        apprentice2.SetActive(true);
		magicMissile.SetActive(true);
        StartCoroutine(EnemyInterception());
        //yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.5f));
        MoveToGem();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.8f));
        GrabGem();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.5f));
        DropGem();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.5f));
        GoalReached();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1));



        Debug.Log("End of Instruction.");
    }

    IEnumerator EnemyInterception()
    {
        //yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.5f));
        Chase();
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(0.6f));
		magicMissileRun = true;
		Destroy(magicMissile, 0.8f);
        yield return StartCoroutine(CoroutineUnscaledWait.WaitForSecondsUnscaled(1.2f));
        apprentince2Run = false;
        //anim2.SetTrigger("stop");

    }

    void MoveToGem()
    {
        apprentince1Run = true;
    }

    void GrabGem()
    {
        gem.transform.parent = apprentice1.transform;
    }

    void GoalReached()
    {
        apprentince1Run = false;
    }

    void Chase()
    {
        apprentince2Run = true;
    }

    void DropGem()
    {
        gem.transform.parent = gameObject.transform;
    }
}
