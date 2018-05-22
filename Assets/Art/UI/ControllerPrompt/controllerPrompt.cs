using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerPrompt : MonoBehaviour
{

    //Alpha Values
    private float minimum = 0.0f;
    private float maximum = 1f;

    public float fadeSpeed;

    public SpriteRenderer basicMissile;
    public SpriteRenderer windGust;
    public SpriteRenderer iceBolt;
    public SpriteRenderer stickyOoze;
	private Maestro maestro;

    private int instruction;

    void Awake()
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        basicMissile.color = new Color(1f, 1f, 1f, 0f);
        windGust.color = new Color(1f, 1f, 1f, 0f);
        iceBolt.color = new Color(1f, 1f, 1f, 0f);
        stickyOoze.color = new Color(1f, 1f, 1f, 0f);
		maestro = Maestro.Instance;
    }
    // Use this for initialization
    void Start()
    {
        instruction = 0;
        StartCoroutine(prompt());
    }

    // Update is called once per frame
    void Update()
    {
        float step = fadeSpeed * Time.deltaTime;

        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(GetComponent<SpriteRenderer>().color.a, maximum, step));
        if (instruction == 1)
        {
            basicMissile.color = new Color(1f, 1f, 1f, Mathf.Lerp(basicMissile.color.a, maximum, step));
        }
        if (instruction == 2)
        {
            basicMissile.color = new Color(1f, 1f, 1f, Mathf.Lerp(basicMissile.color.a, minimum, step));
            windGust.color = new Color(1f, 1f, 1f, Mathf.Lerp(windGust.color.a, maximum, step));
        }
        if (instruction == 3)
        {
            windGust.color = new Color(1f, 1f, 1f, Mathf.Lerp(windGust.color.a, minimum, step));
            iceBolt.color = new Color(1f, 1f, 1f, Mathf.Lerp(iceBolt.color.a, maximum, step));
        }
        if (instruction == 4)
        {
            iceBolt.color = new Color(1f, 1f, 1f, Mathf.Lerp(iceBolt.color.a, minimum, step));
            stickyOoze.color = new Color(1f, 1f, 1f, Mathf.Lerp(stickyOoze.color.a, maximum, step));
        }
        if (instruction == 5)
        {
            stickyOoze.color = new Color(1f, 1f, 1f, Mathf.Lerp(stickyOoze.color.a, minimum, step));
        }
    }


    IEnumerator prompt()
    {
        yield return new WaitForSecondsRealtime(maestro.As_voi.clip.length);
		yield return new WaitForSeconds(5.5f);
        instruction += 1;
        yield return new WaitForSeconds(8.5f);
        instruction += 1;
        yield return new WaitForSeconds(9f);
        instruction += 1;
        yield return new WaitForSeconds(8.5f);
        instruction += 1;
        yield return new WaitForSeconds(7f);
        instruction += 1;
    }
}
