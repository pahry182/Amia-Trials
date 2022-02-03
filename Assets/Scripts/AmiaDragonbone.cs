using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using AnimationState = DragonBones.AnimationState;
using System.Linq;

public class AmiaDragonbone : MonoBehaviour
{
    private UnityArmatureComponent _player;
    private UnitAI _thisUnitAI;
    private UnitBase _thisUnit;
    public string[] attackAnims, idleAnims, moveAnims;
    private bool isAllowChange;
    private UnitAnimState lastState;

    private void Awake()
    {
        _player = GetComponentInChildren<UnityArmatureComponent>();
        _thisUnitAI = GetComponentInChildren<UnitAI>();
        _thisUnit = GetComponentInChildren<UnitBase>();
    }

    void Start()
    {
        List<AnimationState> states = _player.animation.GetStates();
    }

    void Update()
    {
        if(_thisUnitAI.unitDir < 0)
        {
            _player._armature.flipX = true;
        }
        if (_thisUnitAI.unitDir > 0)
        {
            _player._armature.flipX = false;
        }

        if(_thisUnit.unitState == UnitAnimState.attacking)
        {
            ForceChangeAnim(attackAnims);
        }

        else if (_thisUnit.unitState == UnitAnimState.idle)
        {
            ForceChangeAnim(idleAnims);
        }

        else if (_thisUnit.unitState == UnitAnimState.moving)
        {
            ForceChangeAnim(moveAnims);
        }
    }

    private void ForceChangeAnim(string[] anims)
    {
        UnitAnimState state = _thisUnit.unitState;
        if (lastState != state)
        {
            lastState = state;
            if (state != UnitAnimState.idle && !_player.animation.isCompleted)
            {
                _player.animation.Stop();
                return;
            }
        }

        if (!_player.animation.isPlaying)
        {
            int select = Random.Range(0, anims.Length);
            _player.animation.Play(anims[select], 1);
        }
    }
}
