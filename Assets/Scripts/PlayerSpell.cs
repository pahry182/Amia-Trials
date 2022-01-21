using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpell : MonoBehaviour
{
    private UnitBase _ub;
    private int lastLevel;

    [Header("Fire Brust")]
    public GameObject specialEffect;
    public Image fireBrust_image;
    public float fireResistanceDown = 0.16f;
    public float fireBrust_manaCost;
    public float fireBrust_cd = 3;
    public float fireBrust_currentCd;


    private void Awake()
    {
        _ub = GetComponent<UnitBase>();
    }

    private void Update()
    {
        UpdateManaCost();
    }

    private void UpdateManaCost()
    {
        if(lastLevel == _ub.unitLevel)
        {
            return;
        }
        else
        {
            lastLevel = _ub.unitLevel;
            fireBrust_manaCost = 14 + 1.5f * _ub.unitLevel;
            print(fireBrust_manaCost +"EWEKKKKKKKKKK");
        }
    }
}
