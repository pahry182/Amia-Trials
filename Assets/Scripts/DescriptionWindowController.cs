using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionWindowController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _mana;
    [SerializeField] private TextMeshProUGUI _cooldown;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _damageHeader;
    [SerializeField] private TextMeshProUGUI _damage;
    [SerializeField] private GameObject closePanel; 

    public void GetData(PlayerSpell.Spell spell)
    {
        closePanel.SetActive(true);
        _name.text = spell.spellName;
        _description.text = spell.description;
        _mana.text = spell.manaCost.ToString();
        _cooldown.text = spell.cooldown.ToString();
        _damageHeader.text = "Damage:";
        _damage.text = spell.damage.ToString();
    }
    public void GetData(PlayerSkill.Skill skill)
    {
        closePanel.SetActive(true);
        _name.text = skill.skillName;
        _description.text = skill.description;
        _mana.text = skill.manaCost.ToString();
        _cooldown.text = skill.cooldown.ToString();
        _damageHeader.text = "Duration:";
        _damage.text = skill.buffDuration.ToString();
    }
}
