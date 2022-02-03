using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBarController : MonoBehaviour
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
        localScale.x = defaultX * (_ub.currentMp / _ub.maxMp);
        transform.localScale = localScale;
    }

    //private void UpdateBar(Vector3)
}
