using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParryController : MonoBehaviour {
    #region Variables and Declarations
    [SerializeField]
    private Constants.Global.Color e_color;
    #endregion
    
    #region Unity Overrides
    // Use this for initialization
    void OnTriggerEnter(Collider other) {
        SpellTarget target;
        if (target = other.gameObject.GetComponent<SpellTarget>()) {
            target.ApplySpellEffect(Constants.SpellStats.SpellType.WIND, e_color, Constants.SpellStats.C_WindDamage, transform.forward.normalized);
        }
    }
    #endregion
}
