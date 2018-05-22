using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreOrbController : MonoBehaviour {

    [SerializeField] GameObject go_target;
    [SerializeField] float f_maxStepSize = 0.5f;

    private Objective objv_owned;
    private Vector3 v3_startPos;

    #region Getters and Setters
    public Objective Objective {
        get { return objv_owned; }
        set { objv_owned = value; }
    }

    public Vector3 StartPosition {
        get { return v3_startPos; }
        set { v3_startPos = value; }
    }
    #endregion
    // Use this for initialization
    #region Unity Overrides
    private void OnEnable() {
        //if (v3_startPos != null) {
            transform.position = v3_startPos;
        //} else {
        //    if (objv_owned.Color == Constants.Global.Color.BLUE) {
        //        transform.position = new Vector3(3.81f,0f,0f);
        //    } else {
        //        transform.position = new Vector3(-3.81f,0f,0f);
        //    }
            
        //}
        
        InvokeRepeating("MoveToTarget", 0.0f, 0.033f);
    }

    private void MoveToTarget() {
        transform.position = Vector3.MoveTowards(transform.position, go_target.transform.position, f_maxStepSize);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.tag);
        if (other.CompareTag("ScoreTarget")) {
            objv_owned.ScoreOrbHit(objv_owned.GetMax());
            CancelInvoke("MoveToTarget");
            transform.position = v3_startPos;
            gameObject.SetActive(false);
        }
    }
    #endregion
}
