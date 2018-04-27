/*  Camera Facing Billboard - Sam Caulker
 * 
 *  Desc:   Makes player and enemy indicators follow their targets and face the requested camera
 *  
 */

using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour {
#region Variables and Declarations
    [SerializeField] private Camera cam_Camera;
    [SerializeField] private GameObject go_trackedObject;
    [SerializeField] private bool b_persistent;
    [SerializeField] private Vector3 v3_offset;
#endregion

#region Camera Facing Billboard Methods
    //public void Init(Camera cam, GameObject target) {
    //    cam_Camera = cam;
    //    go_trackedObject = target;
    //}

    private void SetInactive() {
        gameObject.SetActive(false);
    }
#endregion

#region Unity Overrides
    void OnEnable() {
        if (!b_persistent) {
            Invoke("SetInactive", 2.0f);  // non-player indicators do not persist after spawn
        }
    }

    void Update() {
        transform.LookAt(transform.position + cam_Camera.transform.rotation * Vector3.forward,
            cam_Camera.transform.rotation * Vector3.up);
        transform.position = go_trackedObject.transform.position + v3_offset;
    }
#endregion
}