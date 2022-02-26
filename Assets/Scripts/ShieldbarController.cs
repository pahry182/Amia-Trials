using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldbarController : MonoBehaviour
{
    private UnitBase _ub;
    private Vector3 localScale;
    private float defaultX;

    private void Awake()
    {
        _ub = transform.parent.GetComponent<UnitBase>();
    }

    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
        defaultX = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (_ub.currentShd <= 0)
        {
            localScale.x = 0;
        }
        localScale.x = defaultX * (_ub.currentShd / _ub.maxShd);
        transform.localScale = localScale;
    }

    //private void UpdateBar(Vector3)
}
