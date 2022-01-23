using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpell : MonoBehaviour
{
    [Serializable]
    public class Spell
    {
        public string spellName;
        public Image cdFillImage;
        public UnitElement spellElement;
        public float cooldown;
        public float manaCost;
        public float baseDamage;
        public float manaIncrement;
        public float damageIncrement;        

        public float currentCd;

        public void UpdateCooldown()
        {
            if (currentCd > 0)
            {
                currentCd -= Time.deltaTime;
                cdFillImage.fillAmount -= Time.deltaTime * 1 / cooldown;
            }
        }

        public void StartSpellCooldown()
        {
            currentCd = cooldown;
            cdFillImage.fillAmount = 1f;
        }

        public void UpdateIncrement(int _level)
        {
            manaCost += manaIncrement * _level;
            baseDamage += damageIncrement * _level;
        }
    }
    private UnitBase _ub;
    private int lastLevel;
    public Spell[] spellList;
    public GameObject specialEffect;

    [Header("Fire Burst")]
    public float fireBurst_fireResDown = 0.16f;

    [Header("Water Jet-Shot")]
    public float jet_critChance = 25f;
    public float jet_critDamage = 2.25f;

    [Header("Lightning Bolt")]
    public float bolt_stunChance = 10;
    public float bolt_stunDuration = 1.5f;

    [Header("Wind Slash")]
    public float windSlash_resDown = 0.08f;

    [Header("Frost Nova")]
    public float nova_freezeChance = 25;
    public float nova_freezeDuration = 5f;

    [Header("Iluminate")]
    public float illuminate_defDown = 7;
    public float illuminate_defDownIncrement = 0.8f;

    [Header("Unholy Judgement")]
    public float judge_hpPercentage = 0.3f;


    private void Awake()
    {
        _ub = GetComponent<UnitBase>();
    }

    private void Update()
    {
        UpdateCooldown();
        UpdateSpells();
    }

    private void UpdateSpells()
    {
        int level = _ub.unitLevel;
        if(lastLevel == level)
        {
            return;
        }
        else
        {
            lastLevel = _ub.unitLevel;
            for (int i = 0; i < spellList.Length; i++)
            {
                spellList[i].UpdateIncrement(_ub.unitLevel);
            }
        }
    }

    private void UpdateCooldown()
    {
        for (int i = 0; i < spellList.Length; i++)
        {
            spellList[i].UpdateCooldown();
        }
    }

    private IEnumerator FireBurst()
    {
        Spell fireBurst = spellList[0];
        if (_ub.currentMp >= fireBurst.manaCost)
        {
            print(fireBurst.spellName + " No Mana");
        }
        else if (fireBurst.currentCd > 0)
        {
            print(fireBurst.spellName + " Cooldown");
        }
        else
        {
            yield return new WaitForSeconds(0);
            GameObject _temp = Instantiate(specialEffect, _ub._UnitAI.targetPosition.position, Quaternion.identity);
            _temp.GetComponent<TimeLife>().life = 2f;
            _ub.Cast();
            _ub.currentMp -= fireBurst.manaCost;
            _ub.DealDamage(fireBurst.baseDamage, true, fireBurst.spellElement);
            fireBurst.StartSpellCooldown();
        }
        
    }
}
