using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action onSpellUse;

    public void MysticFieldTrigger()
    {
        if (onSpellUse == null) return;
        onSpellUse();
    }
}
