using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBounce : MonoBehaviour {

    [SerializeField] float f_ampX;
    [SerializeField] float f_ampY;
    [SerializeField] float f_omegaX;
    [SerializeField] float f_omegaY;

    public bool verticalBounce;
    public bool horizBounce;

    float index;
    float x;
    float y;

    [SerializeField] float start_z;
    [SerializeField] float start_y;
    [SerializeField] float start_x;

    private void Update()
    {
        index += Time.unscaledDeltaTime;
        x = f_ampX * Mathf.Sin(f_omegaX * index) + start_x;

        y = (f_ampY * Mathf.Cos(f_omegaY * index)) + start_y;

        if (verticalBounce && horizBounce)
            transform.localPosition = new Vector3(x, y, start_z);
        else if (verticalBounce)
            transform.localPosition = new Vector3(transform.localPosition.x, y, start_z);
        else
            transform.localPosition = new Vector3(x, transform.localPosition.y, start_z);
    }

}