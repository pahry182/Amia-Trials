using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    private Vector3 localScale;
    private UnitBase _ub;

    private void Awake()
    {
        _ub = transform.parent.GetComponent<UnitBase>();
    }

    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        localScale.x = (_ub.currentHp / _ub.maxHp);
        transform.localScale = localScale;
    }
}
