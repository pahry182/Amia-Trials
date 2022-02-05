using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Transform = UnityEngine.Transform;

public class UnitAI : MonoBehaviour
{
    private UnitBase _thisUnit;
    [HideInInspector] public Transform targetPosition;
    private bool isTargetInAttackRange;

    public UnitBase target;
    public LayerMask targetLayer;
    public UnityEvent<GameObject> onOpponentDetected;

    [Range(.1f, 10)] public float attRange = 1f;
    public string targetTag;

    [Header("Gizmo Parameter")]
    public Color gizmoColor = Color.green;
    public bool gizmoShow = true;

    [Header("Direction")]
    [HideInInspector] public float unitDir;
    private bool isThreshold = false, seventyPercent = false, fivetyPercent = false, thirtyPercent = false;

    public bool opponentDetected { get; internal set; }

    private void Awake()
    {
        _thisUnit = GetComponent<UnitBase>();
    }

    private void Start()
    {
        StartCoroutine(GameStart());
    }

    private void Update()
    {
        if (GameManager.Instance.isBattleStarted)
        {
            AttackRangeDetection();
        }

        if(targetPosition != null)
        {
            unitDir = transform.position.x - targetPosition.position.x;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isBattleStarted)
        {
            MoveToTarget();
        }
    }

    IEnumerator GameStart()
    {
        yield return new WaitUntil(() => GameManager.Instance.isBattleStarted == true);
        yield return new WaitUntil(() => GameManager.Instance.isEnemyPresent == true);
        DetectTarget();
    }

    private void OnDrawGizmos()
    {
        if (gizmoShow)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, attRange);
        }
    }

    public void DetectTarget()
    {
        try
        {
            targetPosition = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
            target = targetPosition.GetComponent<UnitBase>();
        }
        catch (System.Exception)
        {
            print("Pong");
        }
    }

    void MoveToTarget()
    {
        if (targetPosition && 
            !isTargetInAttackRange && 
            !target.isUnitDead && 
            !_thisUnit.isUnitDead &&
            (_thisUnit.unitState == UnitAnimState.idle || _thisUnit.unitState == UnitAnimState.moving)
            )
        {
            _thisUnit.unitState = UnitAnimState.moving;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition.position, _thisUnit.movSpeed * Time.deltaTime);
        }        
    }

    void AttackRangeDetection()
    {
        var collider = Physics2D.OverlapCircle(transform.position, attRange, targetLayer);
        opponentDetected = collider != null;
        if (target == null) return;
        if (opponentDetected && !target.isUnitDead && !_thisUnit.isUnitDead &&
            (_thisUnit.unitState == UnitAnimState.idle || _thisUnit.unitState == UnitAnimState.moving))
        {
            onOpponentDetected?.Invoke(collider.gameObject);
            _thisUnit.unitState = UnitAnimState.attacking;
            isTargetInAttackRange = true;
            if (gameObject.tag == "Enemy")
            {
                EnemyAttackPattern();
            }
            else
            {
                _thisUnit.Attack();
            }
        }
        else
        {
            isTargetInAttackRange = false;
        }
    }


    private void EnemyAttackPattern()
    {
        int select = Random.Range(0, 100);
        if (_thisUnit.isBoss)
        {
            BossAttackPattern();
        }
        else if (select < _thisUnit.specialAttackChance)
        {
            _thisUnit.EnemySpecialAttack();
        }
        else
        {
            _thisUnit.Attack();
        }
    }

    private void BossAttackPattern()
    {
        if (_thisUnit.currentHp <= _thisUnit.maxHp * 0.7f && !seventyPercent)
        {
            seventyPercent = true;
            _thisUnit.EnemyThreshold();
        }
        else if (_thisUnit.currentHp <= _thisUnit.maxHp * 0.5f && !fivetyPercent)
        {
            fivetyPercent = true;
            _thisUnit.EnemyThreshold();
        }
        else if (_thisUnit.currentHp <= _thisUnit.maxHp * 0.3f && !thirtyPercent)
        {
            thirtyPercent = true;
            _thisUnit.EnemyThreshold();
        }
        else
        {
            _thisUnit.Attack();
        }
    }
}
