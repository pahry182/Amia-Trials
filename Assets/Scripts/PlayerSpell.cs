using System.Collections;
using System.Collections.Generic;
using UnityEngine;
`
public class PlayerSpell : MonoBehaviour
{
    private UnitBase _ub;

    private void Awake()
    {
        _ub.GetComponent<UnitBase>();
    }


}
