using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusHUDController : MonoBehaviour
{
    private UnitBase _ub;
    private Transform _statusBar;
    private TextMeshPro _statusText;
    private Vector3 localScale;
    private float defaultX;
    private UnitAnimState state;
    private UnitAnimState lastState;
    private float initialFrozenDuration;
    private float initialStunDuration;

    private void Awake()
    {
        _ub = GetComponentInParent<UnitBase>();
        _statusBar = transform.GetChild(0).GetComponent<Transform>();
        _statusText = GetComponentInChildren<TextMeshPro>();
        EnableStatus(false);
    }

    private void EnableStatus(bool value)
    {
        _statusBar.gameObject.SetActive(value);
        _statusText.gameObject.SetActive(value);
    }

    private void Start()
    {
        localScale = _statusBar.localScale;
        defaultX = _statusBar.localScale.x;
    }

    private void Update()
    {
        state = _ub.unitState;
        if (state == UnitAnimState.stunned && lastState != UnitAnimState.stunned)
        {
            lastState = state;
            initialStunDuration = _ub.stunDuration;
            localScale.x = defaultX;
            _statusBar.localScale = localScale;
        }
        else if (state == UnitAnimState.frozen && lastState != UnitAnimState.frozen)
        {
            lastState = state;
            initialFrozenDuration = _ub.frozenDuration;
            localScale.x = defaultX;
            _statusBar.localScale = localScale;
        }
        else if (state != lastState)
        {
            lastState = state;
        }
        
        if (_ub.stunDuration > 0)
        {
            EnableStatus(true);
            _statusText.text = "Stunned";
            localScale.x = defaultX * (_ub.stunDuration / initialStunDuration);
            _statusBar.localScale = localScale;
        }
        else if (_ub.frozenDuration > 0)
        {
            EnableStatus(true);
            _statusText.text = "Frozen";
            localScale.x = defaultX * (_ub.frozenDuration / initialFrozenDuration);
            _statusBar.localScale = localScale;
        }
        else
        {
            EnableStatus(false);
        }
    }


}
