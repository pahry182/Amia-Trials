using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public string skillName;
        [TextArea] public string description;
        public Image cdFillImage;
        public Image manaFillImage;
        public GameObject vfx;
        public float additionalChannelTime;
        public float cooldown;
        public float currentCooldown;
        public float manaCost;
        public float baseManaCost;
        public float manaIncrement;
        public float buffDuration;

        public void UpdateCdFill()
        {
            if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
            if (cdFillImage == null) return;
            cdFillImage.fillAmount -= Time.deltaTime * 1 / cooldown;
        }

        public void StartCdFill()
        {
            currentCooldown = cooldown;
            if (cdFillImage == null) return;
            cdFillImage.fillAmount = 1f;
        }

        public void UpdateIncrement(int _level)
        {
            manaCost = baseManaCost + manaIncrement * _level;
        }

        public void UpdateManaCost(float _currentMp)
        {
            if (manaFillImage == null) return;
            if (_currentMp < manaCost)
            {
                manaFillImage.color = new Color32(115, 128, 212, 255);
            }
            else
            {
                manaFillImage.color = new Color32(255, 255, 255, 255);
            }
        }
    }
    private UnitBase _ub;
    public Skill[] playerSkills;

    [Header("Hunter's Instinct")]
    public float additionalBeastKiller = 0.4f;

    [Header("Manastrike")]
    public float additionalFiendKiller = 0.75f;

    private void Awake()
    {
        _ub = GetComponent<UnitBase>();
    }

    private void Update()
    {
        foreach (var item in playerSkills)
        {
            item.UpdateCdFill();
            item.UpdateManaCost(_ub.currentMp);
        }
    }

    public Skill CheckSkillCondition(string name)
    {
        Skill currentSkill = null;
        for (int i = 0; i < playerSkills.Length; i++)
        {
            if (name == playerSkills[i].skillName)
            {
                currentSkill = playerSkills[i];
                break;
            }
        }

        if (_ub.currentMp <= currentSkill.manaCost)
        {
            print(currentSkill.skillName + " No Mana");
        }
        else if (currentSkill.currentCooldown > 0)
        {
            print(currentSkill.skillName + " Under Cooldown");
        }
        else if (_ub.unitState == UnitAnimState.stunned || _ub.unitState == UnitAnimState.frozen)
        {
            print(currentSkill.skillName + " Under Cooldown");
        }
        else
        {
            Destroy(Instantiate(currentSkill.vfx, transform.position, Quaternion.identity), 2f);
            _ub.currentMp -= currentSkill.manaCost;
            _ub.Cast(currentSkill.additionalChannelTime);
            _ub.PlaySfxUnit(_ub.castingSfx);
            currentSkill.StartCdFill();

            return currentSkill;
        }

        return null;
    }

    public void HuntersInstinctButton()
    {
        Skill currentSkill = CheckSkillCondition("Hunter's Instinct");
        if (currentSkill != null)
        {
            StartCoroutine(HuntersInstinct(currentSkill));
        }
    }

    public void ManastrikeButton()
    {
        Skill currentSkill = CheckSkillCondition("Manastrike");
        if (currentSkill != null)
        {
            print("Ponged");
            StartCoroutine(Manastrike());
        }
    }

    public void MysticFieldButton()
    {
        Skill currentSkill = CheckSkillCondition("Mystic Field");
        if (currentSkill != null)
        {
            StartCoroutine(MysticField(currentSkill));
        }
    }

    IEnumerator HuntersInstinct(Skill currentSkill)
    {
        //GameObject _temp = Instantiate(GameManager.Instance., transform.position, Quaternion.identity);
        
        _ub.beastKiller += additionalBeastKiller;
       
        yield return new WaitForSeconds(currentSkill.buffDuration);

        _ub.beastKiller -= additionalBeastKiller;
    }

    IEnumerator Manastrike()
    {
        //GameObject _temp = Instantiate(specialEffect, transform.position, Quaternion.identity);
        //_temp.GetComponent<TimeLife>().life = 1f;
        //_temp.GetComponent<SpriteRenderer>().material.color = Color.cyan;

        _ub.isManastriking = true;
        _ub.fiendKiller += additionalFiendKiller;

        yield return new WaitUntil(() => _ub.isManastriking == false);

        _ub.fiendKiller -= additionalFiendKiller;
    }

    IEnumerator MysticField(Skill currentSkill)
    {
        //GameObject _temp = Instantiate(specialEffect, transform.position, Quaternion.identity);
        //_temp.GetComponent<TimeLife>().life = 1f;
        //_temp.GetComponent<SpriteRenderer>().material.color = Color.white;

        _ub.isMysticFielding = true;

        yield return new WaitForSeconds(currentSkill.buffDuration);

        _ub.isMysticFielding = false;

    }
}
