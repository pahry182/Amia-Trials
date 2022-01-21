using UnityEngine;
using TMPro;
using System.Collections;

public enum UnitType
{
    Human,
    Beast,
    Fiend,
    COUNT
}

public enum UnitElement
{
    Neutral,
    Fire,
    Water,
    Lightning,
    Earth,
    Wind,
    Ice,
    Light,
    Dark,
    COUNT
}

public enum UnitAnimState
{
    idle,
    moving,
    attacking,
    special
}

public class UnitBase : MonoBehaviour
{
    private Rigidbody2D _rb;
    private UnitAI _UnitAI;
    [HideInInspector] public SpriteRenderer _sr;
    [HideInInspector] public TextMeshPro _levelText;

    [Header("Basic Stat")]
    public string unitName = "Amia";
    public int unitLevel = 1;
    public float currentHp = 100f;
    public float currentMp = 100f;
    public float currentXp;
    public float maxHp = 100f;
    public float maxMp = 100f;
    public float maxXp = 100f;
    public float att = 10f;
    public float critChance = 10;
    public float critMultiplier = 0.5f;
    public float def = 2f;
    public float spellRes = 0.15f;
    public float movSpeed = 1f;
    public float attSpeed = 1f;

    [Header("Growth Stat")]
    public float growthHp;
    public float growthMp;
    public float growthAtt;
    public float growthDef;

    [Header("Advanced Stat")]
    public UnitType unitType = UnitType.Human;
    public UnitElement unitElement = UnitElement.Neutral;
    public bool isBoss;
    public float humanKiller = 0f;
    public float beastKiller = 0f;
    public float fiendKiller = 0f;

    [Header("Elemental Affinity")]
    public float fireAtt;
    public float waterAtt;
    public float lightningAtt;
    public float earthAtt;
    public float windAtt;
    public float iceAtt;
    public float lightAtt;
    public float darkAtt;
    [Header("Elemental Resistances")]
    public float fireRes;
    public float waterRes;
    public float lightningRes;
    public float earthRes;
    public float windRes;
    public float iceRes;
    public float lightRes;
    public float darkRes;

    [Header("Functionality Stat")]
    public UnitAnimState unitState = UnitAnimState.idle;
    public bool isUnitDead;
    public bool isManastriking;
    public bool isMysticFielding;
    public float attCooldown;
    public float stunDuration;
    public float frozenDuration;

    //def/att /2 *10 = total taken damage
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _UnitAI = GetComponent<UnitAI>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _levelText = GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        if (attCooldown > 0)
        {
            attCooldown -= Time.deltaTime;
        }
        if (attCooldown <= 0 && unitState != UnitAnimState.special)
        {
            unitState = UnitAnimState.idle;
        }
        
        _levelText.text = "Lv. " + unitLevel;
    }

    public void Attack()
    {
        if (attCooldown <= 0 && !isUnitDead && unitState == UnitAnimState.attacking)
        {
            attCooldown = attSpeed;
            Jump();
            DealDamage(att);
        }
    }

    public void Cast()
    {
        StartCoroutine(Casting());
    }

    private IEnumerator Casting()
    {
        unitState = UnitAnimState.special;
        GetComponentInChildren<Animator>().SetBool("isUnitSpecial", true);
        yield return new WaitForSeconds(0.4f);
        GetComponentInChildren<Animator>().SetBool("isUnitSpecial", false);
        unitState = UnitAnimState.idle;
    }

    public void Jump()
    {
        _rb.AddForce(transform.up * 4, ForceMode2D.Impulse);
    }

    public void DealDamage(float amount)
    {
        UnitBase target = _UnitAI.target;
        float targetDamageReduction = (0.06f * target.def) / (1 + 0.06f * target.def);
        amount = CalculateKillers(amount, target);
        amount -= Mathf.Round(amount * targetDamageReduction);
        if (target.currentHp - amount < 1)
        {
            DeclareDeath(target);
        }
        else
        {
            target.currentHp -= amount;
        }
        if (isManastriking) isManastriking = false;
        print(gameObject.tag + " " + amount);
        GameManager.Instance.StatisticTrackDamageDealt(amount, gameObject);
    }

    private float CalculateKillers(float _amount, UnitBase _target)
    {
        switch (_target.unitType)
        {
            case UnitType.Human:
                break;
            case UnitType.Beast:
                _amount += _amount * beastKiller;
                break;
            case UnitType.Fiend:
                _amount += _amount * fiendKiller;
                break;
            case UnitType.COUNT:
                break;
            default:
                break;
        }
        return _amount;
    }

    public void DeclareDeath(UnitBase target)
    {
        target.currentHp = 0;
        target.isUnitDead = true;
        target.GetComponentInChildren<Animator>().SetBool("isUnitDead", true);
        if (target.isBoss)
        {
            GainExp((int)((Random.Range(45, 75) * (0.65 * target.unitLevel)) + target.unitLevel) * 2);
        } 
        else
        {
            GainExp((int)((Random.Range(25, 55) * (0.6 * target.unitLevel)) + target.unitLevel) * 2);
        }
        GameManager.Instance.StatisticTrackKill(target, gameObject);
        GameManager.Instance.CheckDeath(target.gameObject);

    }

    public void GainExp(int amount)
    {
        currentXp += amount;
        while (currentXp >= maxXp)
        {
            currentXp -= maxXp;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        maxXp = (int)((30 + maxXp) * 1.02);
        maxHp += growthHp;
        maxMp += growthMp;
        att += growthAtt;
        if (unitLevel % 4 == 0)
        {
            def += growthDef;
            att += growthAtt - 1;
        }
        unitLevel += 1;
    }

    public void ReviveUnit()
    {
        currentHp = maxHp;
        currentMp = maxMp;
        isUnitDead = false;
        GetComponentInChildren<Animator>().SetBool("isUnitDead", false);
    }
}
