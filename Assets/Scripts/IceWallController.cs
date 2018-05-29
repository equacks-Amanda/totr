/*  Spell Controller - Sam Caulker
 * 
 *  Desc:   Destroys ice wall a set time after creation
 * 
 */

using UnityEngine;
using System.Collections;

public class IceWallController : MonoBehaviour {

    [SerializeField]
    private GameObject OnDestroyParticle;

    [SerializeField]
    private Collider boxCollider;

    [SerializeField]
    ShaderEffect dissolveWall;

#region Unity Overrides
    void Start () {
        //Destroy(gameObject, Constants.SpellStats.C_IceWallLiveTime);
        StartCoroutine(WaitToDestroy());
	}

    IEnumerator WaitToDestroy()
    {
         yield return new WaitForSeconds(Constants.SpellStats.C_IceWallLiveTime);
        boxCollider.enabled = false;
        dissolveWall.ParamIncrease(3f, true, "_DisintegrateAmount");
    }
#endregion
}
