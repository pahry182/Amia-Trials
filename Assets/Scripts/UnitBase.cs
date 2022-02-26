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

public enum Element
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
    special,
    stunned,
    frozen,
    die
}

public class UnitBase : MonoBehaviour
{
    private Rigidbody2D _rb;
    [HideInInspector] public UnitAI _UnitAI;
    [HideInInspector] public SpriteRenderer _sr;
    [HideInInspector] public TextMeshPro _levelText;

    [Header("Basic Stat")]
    public string unitName = "Amia";
    public int unitLevel = 1;
    public float currentHp = 100f;
    public float currentShd = 0f;
    public float currentMp = 100f;
    public float currentXp;
    public float maxHp = 100f;
    public float maxShd = 100f;
    public float maxMp = 100f;
    public float maxXp = 100f;
    public float manaRegen = 0;
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
    public float growthManaReg = 0;
    public float growthAtt;
    public float growthDef;

    [Header("Advanced Stat")]
    public UnitType unitType = UnitType.Human;
    public Element unitElement = Element.Neutral;
    public Element shdElement = Element.Neutral;
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
    public float currentChanneling;
    [HideInInspector] public bool isAttack = false;

    [Header("Animation")]
    public float channelTime = 0.8f;

    [Header("Sounds")]
    public string[] attSfx;
    public string[] castingSfx;
    public string[] dieSfx;
    public string[] manastrikeSfx;

    [Header("Enemy AI")]
    public int specialAttackChance = 20;

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
        if (isUnitDead)
        {
            unitState = UnitAnimState.die;
        }
        else if (stunDuration > 0)
        {
            stunDuration -= Time.deltaTime;
            unitState = UnitAnimState.stunned;
        }
        else if (frozenDuration > 0)
        {
            frozenDuration -= Time.deltaTime;
            unitState = UnitAnimState.frozen;
        }
        else if (attCooldown <= 0 && unitState != UnitAnimState.special && unitState != UnitAnimState.moving)
        {
            unitState = UnitAnimState.idle;
        }

        _levelText.text = "Lv. " + unitLevel;

        if (currentMp < maxMp && manaRegen != 0 && !isUnitDead)
        {
            currentMp += Time.deltaTime * manaRegen;
        }

        if (currentChanneling > 0)
        {
            currentChanneling -= Time.deltaTime;
        }

        if (currentShd <= 0)
        {
            shdElement = Element.Neutral;
        }
    }

    public void PlaySfxUnit(string[] sfx)
    {
        GameManager.Instance.PlaySfx(sfx[Random.Range(0, sfx.Length)]);
    }

    public void Attack()
    {
        if (attCooldown <= 0 && !isUnitDead && unitState == UnitAnimState.attacking)
        {
            PlaySfxUnit(attSfx);
            attCooldown = attSpeed;
            Jump();
            DealDamage(att);
        }
    }

    public void Cast(float additionalChannelTime)
    {
        StartCoroutine(Casting(channelTime+additionalChannelTime));
    }

    private IEnumerator Casting(float channelTime)
    {
        unitState = UnitAnimState.special;
        
        yield return new WaitForSeconds(channelTime);
        
        unitState = UnitAnimState.idle;
    }

    public void Jump()
    {
        if(gameObject.tag == "Enemy")
        {
            _rb.AddForce(transform.up * 4, ForceMode2D.Impulse);
        }
    }

    public void DealDamage(float amount, bool isSpellDamage = false, Element _spellElementType = Element.Neutral, bool isCrit = false)
    {
        UnitBase target = _UnitAI.target;
        amount = CalculateKillers(amount, target);
        if (isSpellDamage)
        {
            amount = CalculateElementalRelation(amount, _spellElementType, target);
            amount -= Mathf.Round(amount * target.spellRes);
        }
        else
        {
            if (target.def < 0)
            {
                float targetDamageIncrease = 2 - Mathf.Pow(0.94f, -target.def);
                amount += Mathf.Round(amount * (targetDamageIncrease - 1));
            }
            else
            {
                const float CReduction = 0.06f;
                float targetDamageReduction = (CReduction * target.def) / (1 + (CReduction * target.def));
                amount -= Mathf.Round(amount * targetDamageReduction);
            }
        }

        int select = Random.Range(0, 100);
        if (select < critChance)
        {
            amount += amount * critMultiplier;
            isCrit = true;
        }

        if (target.currentShd >= amount)
        {
            target.currentShd -= amount;
        }
        else if (target.currentShd < amount && target.currentShd > 0)
        {
            target.currentHp -= (amount - target.currentShd);
            target.currentShd = 0;
        }
        else
        {
            target.currentHp -= amount;
        }

        if (target.currentHp < 1)
        {
            DeclareDeath(target);
        }

        if (isManastriking) 
        {
            isManastriking = false;
            PlaySfxUnit(manastrikeSfx);
        }
        

        Vector3 pos = target.transform.position;
        pos.y += Random.Range(-0.2f, 0.2f);
        GameObject temp = Instantiate(GameManager.Instance.textDamage, pos, Quaternion.identity);
        temp.GetComponentInChildren<TextMeshPro>().color = ElementalDamageColor(_spellElementType);
        if (isCrit)
        {
            temp.GetComponentInChildren<TextMeshPro>().text = "Crit " + Mathf.Round(amount).ToString();
        }
        else
        {
            temp.GetComponentInChildren<TextMeshPro>().text = Mathf.Round(amount).ToString();
        }
        Destroy(temp, 3f);
        GameManager.Instance.StatisticTrackDamageDealt(amount, gameObject);
    }

    private Color32 ElementalDamageColor(Element _element)
    {
        switch (_element)
        {
            case Element.Fire:
                return new Color32(255, 48, 4, 255);
            case Element.Water:
                return new Color32(50, 110, 255, 255);
            case Element.Lightning:
                return new Color32(237, 20, 241, 255);
            case Element.Earth:
                return new Color32(203, 177, 41, 255);
            case Element.Wind:
                return new Color32(32, 254, 2, 255);
            case Element.Ice:
                return new Color32(0, 255, 232, 255);
            case Element.Light:
                return new Color32(255, 253, 139, 255);
            case Element.Dark:
                return new Color32(69, 69, 69, 255);
            default:
                return new Color32(255, 255, 255, 255);
        }
    }

    private float CalculateKillers(float _amount, UnitBase _target)
    {
        switch (_target.unitType)
        {
            case UnitType.Human:
                _amount += _amount * humanKiller;
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

    private float CalculateElementalRelation(float _amount, Element _spellElementType, UnitBase _target)
    {
        switch (_spellElementType)
        {
            case Element.Fire:
                _amount += _amount * (fireAtt - _target.fireRes);
                break;
            case Element.Water:
                _amount += _amount * (waterAtt - _target.waterRes);
                break;
            case Element.Lightning:
                _amount += _amount * (lightningAtt - _target.lightningRes);
                break;
            case Element.Earth:
                _amount += _amount * (earthAtt - _target.earthRes);
                break;
            case Element.Wind:
                _amount += _amount * (windAtt - _target.windRes);
                break;
            case Element.Ice:
                _amount += _amount * (iceAtt - _target.iceRes);
                break;
            case Element.Light:
                _amount += _amount * (lightAtt - _target.lightRes);
                break;
            case Element.Dark:
                _amount += _amount * (darkAtt - _target.darkRes);
                break;
        }
        return _amount;
    }

    public void DeclareDeath(UnitBase target)
    {
        target.PlaySfxUnit(target.dieSfx);
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

    public void GainShield(int amount)
    {
        if (currentShd + amount >= maxShd)
        {
            currentShd = maxShd;
        }
        else
        {
            currentShd += amount;
        }
    }

    public void LevelUp()
    {
        maxXp = (int)((30 + maxXp) * 1.02);
        maxHp += growthHp;
        maxShd = maxHp;
        maxMp += growthMp;
        manaRegen += growthManaReg;
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

        foreach (var item in GetComponent<PlayerSkill>().playerSkills)
        {
            item.currentCooldown = 0f;
        }

        GetComponent<PlayerSpell>().sharedCurrentCd = 0f;
        GetComponent<PlayerSpell>().sharedCd = 0f;
        GetComponentInChildren<Animator>().SetBool("isUnitDead", false);
    }

    public void EnemySpecialAttack()
    {
        if (attCooldown <= 0 && !isUnitDead && unitState == UnitAnimState.attacking)
        {
            PlaySfxUnit(attSfx);
            attCooldown = attSpeed;
            _rb.AddForce(transform.up * 6, ForceMode2D.Impulse);
            _UnitAI.target.stunDuration += 1;
            DealDamage(att*1.2f);
        }
    }

    public void EnemyThreshold()
    {
        int select = Random.Range(0, 2);
        switch (select)
        {
            case 0:
                EnemyThresholdAttack();
                return;
            case 1:
                EnemyThresholdHeal();
                return;
        }
    }
    private void EnemyThresholdAttack()
    {
        if (attCooldown <= 0 && !isUnitDead && unitState == UnitAnimState.attacking)
        {
            PlaySfxUnit(attSfx);
            attCooldown = attSpeed * 2;
            _rb.AddForce(transform.forward * 3, ForceMode2D.Impulse);
            _UnitAI.target.stunDuration += 2;
            DealDamage(att * 1.8f);
            var temp = Instantiate(GameManager.Instance.textDamage, transform.position, Quaternion.identity);
            temp.GetComponentInChildren<TextMeshPro>().text = "Threshold Attack!";
            Destroy(temp, 3f);
        }
    }

    private void EnemyThresholdHeal()
    {
        if (attCooldown <= 0 && !isUnitDead && unitState == UnitAnimState.attacking)
        {
            PlaySfxUnit(attSfx);
            attCooldown = attSpeed * 2;
            _rb.AddForce(transform.forward * 3, ForceMode2D.Impulse);
            _UnitAI.target.stunDuration += 4;
            currentHp += maxHp * 0.3f;
            var temp = Instantiate(GameManager.Instance.textDamage, transform.position, Quaternion.identity);
            temp.GetComponentInChildren<TextMeshPro>().text = "Threshold Heal!";
            Destroy(temp, 3f);
        }
    }
}
