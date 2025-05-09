using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ITriggerCheckable
{
    [field: SerializeField] public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    [SerializeField] public int DamageAmount { get; set; } = 1;
    public Rigidbody2D RigidBody { get; set; }
    public bool IsFacingRight { get; set; } = true;

    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get; set; }


    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyFollowState FollowState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public EnemyStunState StunState { get; set; }

    #endregion

    #region Scriptable Object Variables
    [SerializeField] private EnemyIdleSOBase EnemyIdleBase;

    [SerializeField] private EnemyFollowSOBase EnemyFollowBase;

    [SerializeField] private EnemyAttackSOBase EnemyAttackBase;
    [SerializeField] private EnemyStunSOBase EnemyStunBase;

    public EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
    public EnemyFollowSOBase EnemyFollowBaseInstance { get; set; }
    public EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }
    public EnemyStunSOBase EnemyStunBaseInstance { get; set; }
    #endregion

    #region Attack Cooldown
    [SerializeField] private float attackCooldown = 2f;
    public bool isAttackInCooldown = false;
    public bool isAttacking = false;
    #endregion

    public AudioManager audioManager;
    [SerializeField] private LootDrop lootDrop;
    [SerializeField] private Transform lootOrigin;


    private void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
        if (EnemyFollowBase != null) EnemyFollowBaseInstance = Instantiate(EnemyFollowBase);
        EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);
        if (EnemyStunBase != null) EnemyStunBaseInstance = Instantiate(EnemyStunBase);

        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        if (EnemyFollowBase != null) FollowState = new EnemyFollowState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        if (EnemyStunBase != null) StunState = new EnemyStunState(this, StateMachine);
    }

    public void Start()
    {
        CurrentHealth = MaxHealth;

        RigidBody = GetComponent<Rigidbody2D>();

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        if (EnemyFollowBase != null) EnemyFollowBaseInstance.Initialize(gameObject, this);
        EnemyAttackBaseInstance.Initialize(gameObject, this);
        if (EnemyStunBase != null) EnemyStunBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region Health / Die Functions

    public bool Damage(int damageAmount)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    public void Die()
    {
        lootDrop?.DropLoot(lootOrigin);
        RigidBody.bodyType = RigidbodyType2D.Kinematic;
        gameObject.SetActive(false);
    }
    #endregion

    #region Movement Functions
    public void MoveEnemy(Vector2 velocity)
    {
        RigidBody.linearVelocity = velocity;
        CheckForLeftOrRightFacing(velocity);
    }

    public void CheckForLeftOrRightFacing(Vector2 velocity)
    {
        if (IsFacingRight && velocity.x < 0f)
        {
            Vector3 rotator = new(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
        else if (!IsFacingRight && velocity.x > 0f)
        {
            Vector3 rotator = new(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
    }
    #endregion

    #region Distance Checks
    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }
    #endregion

    #region Animation Triggers
    private void AnimationTriggerEvent(AnimationTriggerType animationTriggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(animationTriggerType);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
    }
    #endregion

    #region Attack Cooldown Methods 
    public IEnumerator StartAttackCooldown()
    {
        isAttackInCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttackInCooldown = false;
    }
    #endregion
}