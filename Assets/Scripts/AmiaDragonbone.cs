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
    public string[] skillAnims, attackAnims, idleAnims, moveAnims, knockback, die;
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

        if (_thisUnit.unitState == UnitAnimState.die)
        {
            ForceChangeAnim(die);
        }
        else if (_thisUnit.unitState == UnitAnimState.special)
        {
            ForceChangeAnim(skillAnims);
        }
        else if (_thisUnit.unitState == UnitAnimState.attacking)
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
        else if (_thisUnit.unitState == UnitAnimState.stunned)
        {
            ForceChangeAnim(knockback);
        }
    }

    private void ForceChangeAnim(string[] anims, bool isLoop = true)
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
            if (_player.animation.lastAnimationName == die[0] && state == UnitAnimState.die) return;

            int select = Random.Range(0, anims.Length);

            if (state == UnitAnimState.special)
            {
                _player.animation.GotoAndPlayByTime(anims[select], 0.4f);
                return;
            }
            
            _player.animation.Play(anims[select], 1);
        }
    }
}
