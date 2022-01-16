using UnityEngine;

public enum UnitType
{
    Human,
    Beast,
    Fiend
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
    Dark
}

public class UnitBase : MonoBehaviour
{
    private Rigidbody2D _rb;
    private UnitAI _UnitAI;

    [Header("Basic Stat")]
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

    [Header("Advanced Stat")]
    public UnitType unitType = UnitType.Human;
    public UnitElement unitElement = UnitElement.Neutral;
    public bool isBoss;
    public float humanKiller;
    public float beastKiller;
    public float fiendKiller;

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
    public bool isUnitDead;
    public float attCooldown;
    public float stunDuration;
    public float frozenDuration;

    //def/att /2 *10 = total taken damage
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _UnitAI = GetComponent<UnitAI>();
    }

    private void Update()
    {
        if (attCooldown > 0)
        {
            attCooldown -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (attCooldown <= 0 && !isUnitDead)
        {
            attCooldown = attSpeed;
            Jump();
            DealDamage(att);
        }
    }

    public void Jump()
    {
        _rb.AddForce(transform.up * 4, ForceMode2D.Impulse);
    }

    public void DealDamage(float amount)
    {
        UnitBase target = _UnitAI.target;
        float targetDamageReduction = (0.06f * target.def) / (1 + 0.06f * target.def);
        amount -= amount * targetDamageReduction;
        GameManager.Instance.StatisticTrackDamageDealt(amount, gameObject);
        if (target.currentHp - amount < 1)
        {
            DeclareDeath(target);
        }
        else
        {
            target.currentHp -= amount;
        }
    }

    public void DeclareDeath(UnitBase target)
    {
        target.currentHp = 0;
        target.isUnitDead = true;
        target.GetComponentInChildren<Animator>().SetBool("isUnitDead", true);
        GameManager.Instance.StatisticTrackKill(target, gameObject);
        GameManager.Instance.CheckDeath(target.gameObject);
    }

    public void ReviveUnit()
    {
        currentHp = maxHp;
        isUnitDead = false;
        GetComponentInChildren<Animator>().SetBool("isUnitDead", false);
    }
}
