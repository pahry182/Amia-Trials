using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using AnimationState = DragonBones.AnimationState;
using System.Linq;

public class SlimeDragonbone : MonoBehaviour
{
    private UnityArmatureComponent _thisArmature;
    private UnitAI _thisUnitAI;
    private UnitBase _thisUnit;
    private string attackAnim = "_attack";
    private string moveAnim = "_walk";
    private string dieAnim = "_death";
    private bool isAllowChange;
    private UnitAnimState lastState;

    private void Awake()
    {
        _thisArmature = GetComponentInChildren<UnityArmatureComponent>();
        _thisUnitAI = GetComponentInChildren<UnitAI>();
        _thisUnit = GetComponentInChildren<UnitBase>();
    }

    void Start()
    {
        switch (_thisUnit.unitElement)
        {
            case Element.Fire:
                ChangeAnimParameter("slimeapi");
                break;
            case Element.Water:
                ChangeAnimParameter("slimeair");
                break;
            case Element.Lightning:
                ChangeAnimParameter("slimepetir");
                break;
            case Element.Earth:
                ChangeAnimParameter("slimetanah");
                break;
            case Element.Wind:
                ChangeAnimParameter("slimeangin");
                break;
            case Element.Ice:
                ChangeAnimParameter("slimees");
                break;
            case Element.Light:
                ChangeAnimParameter("slimecahaya");
                break;
            case Element.Dark:
                ChangeAnimParameter("slimekegelapan");
                break;
        }
    }

    private void ChangeAnimParameter(string nameElement)
    {
        attackAnim = nameElement + attackAnim;
        moveAnim = nameElement + moveAnim;
        dieAnim = nameElement + dieAnim;
        print(attackAnim + moveAnim + dieAnim);
    }

    void Update()
    {
        if (_thisUnitAI.unitDir < 0)
        {
            _thisArmature._armature.flipX = true;
        }
        if (_thisUnitAI.unitDir > 0)
        {
            _thisArmature._armature.flipX = false;
        }

        if (_thisUnit.unitState == UnitAnimState.die)
        {
            ForceChangeAnim(dieAnim);
        }
        else if (_thisUnit.unitState == UnitAnimState.attacking)
        {
            ForceChangeAnim(attackAnim);
        }
        else if (_thisUnit.unitState == UnitAnimState.moving || _thisUnit.unitState == UnitAnimState.idle)
        {
            ForceChangeAnim(moveAnim);
        }

        
    }

    private void ForceChangeAnim(string anims, bool isLoop = true)
    {
        UnitAnimState state = _thisUnit.unitState;
        if (lastState != state)
        {
            lastState = state;
            if (state != UnitAnimState.idle && !_thisArmature.animation.isCompleted)
            {
                _thisArmature.animation.Stop();
                return;
            }
        }

        if (!_thisArmature.animation.isPlaying)
        {
            if (_thisArmature.animation.lastAnimationName == dieAnim && state == UnitAnimState.die) return;
            
            _thisArmature.animation.Play(anims, 1);
        }
    }
}
