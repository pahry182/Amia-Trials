using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class TestDragonbones : MonoBehaviour
{
    private UnityArmatureComponent _player;
    private UnitAI _thisUnitAI;
    private UnitBase _thisUnit;
    public string[] animStrings;

    private void Awake()
    {
        _player = GetComponentInChildren<UnityArmatureComponent>();
        _thisUnitAI = GetComponentInChildren<UnitAI>();
        _thisUnit = GetComponentInChildren<UnitBase>();
    }

    void Start()
    {
        
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
            ChangeAnim(animStrings[0]);
        }
    }

    private void ChangeAnim(string animName)
    {
        if (_player.animation.lastAnimationName != animName)
        {
            _player.animation.Reset();
        }
        if (!_player.animation.isPlaying)
        {
            _player.animation.Play(animName, 1);
        }
    }
}
