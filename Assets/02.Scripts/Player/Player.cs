using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("MoveSpeed")]
    [SerializeField] private float walkSpeed;

    [Header("ClickPosition")]
    [SerializeField] private Transform clickTransform;

    #region PlayerComponent
    private NavMeshAgent _agent;
    private Animator _animator;
    private PlayerStateMachine _state;
    private Rigidbody _rigidBody;
    #endregion

    #region PlayerValue
    private Ray _mouseRay;
    private Camera _mainCamera;
    private bool isNext;
    #endregion

    #region Property
    public NavMeshAgent Agent { get { return _agent; } }    
    public Animator Anim { get { return _animator; } }
    public PlayerStateMachine State { get { return _state; } }
    public Transform ClickObject { get { return clickTransform; } }
    public Camera MainCamera { get {  return _mainCamera; } }
    public Rigidbody RigidBody { get { return _rigidBody; } }   
    public Ray MouseRay { get { return _mouseRay; } set { _mouseRay = value; } }
    public bool IsNext { get { return isNext; } set { isNext = value; } }
    #endregion

    #region Animation 
    public readonly int IsComboAttack1 = Animator.StringToHash("IsComboAttack1");
    public readonly int IsComboAttack2 = Animator.StringToHash("IsComboAttack2");
    public readonly int IsComboAttack3 = Animator.StringToHash("IsComboAttack3");
    #endregion

    private void Awake()
    {
        InitializePlayer();
        InitializeState();
    }

    private void InitializePlayer()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _agent.speed = walkSpeed;
        clickTransform.gameObject.SetActive(false);
    }

    private void InitializeState()
    {
        _state = gameObject.AddComponent<PlayerStateMachine>();
        _state.AddState(_State.Idle, new IdleState(this));
        _state.AddState(_State.Move , new MoveState(this));  
        _state.AddState(_State.ComboAttack1, new FirstAttackState(this));
        _state.AddState(_State.ComboAttack2, new SecondAttackState(this));
        _state.AddState(_State.ComboAttack3, new ThirdAttackState(this));  
    }

    public void ChangeNextAttack()
    {
        isNext = true;
    }
}

public abstract class PlayerState : BaseState
{
    protected Player _player;
    public PlayerState(Player player)
    {
        _player = player;
    }

    protected void AttackRotation()
    {
        _player.MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_player.MouseRay, out RaycastHit hit, 100))
        {
            Vector3 lookPosition = new Vector3(hit.point.x, _player.transform.position.y, hit.point.z);

            float distance = Vector3.Distance(_player.transform.position, hit.point);

            if (distance > 0.1f)
            {
                _player.transform.LookAt(lookPosition);
            }
        }
    }

    //���ݽ� ������ �����̴� �޼ҵ�
    protected IEnumerator AttackMove()
    {
        float startTime = Time.time;

        float moveSpeed = 2.0f;

        Vector3 direction = _player.transform.forward;

        while(Time.time < startTime + 0.2f)
        {
            _player.RigidBody.AddForce(direction * moveSpeed,ForceMode.Impulse);
            yield return null;
        }
    }
}

public class IdleState : PlayerState
{
    public IdleState(Player player) : base(player) { }

    public override void StateEnter()
    {
        _player.Agent.enabled = false;
        _player.Anim.applyRootMotion = true;
    }

    public override void StateUpdate()
    {
        ChangeMove();
        ChangeAttack();
    }

    public override void StateExit()
    {
        _player.Agent.enabled = true;
        _player.Anim.applyRootMotion = false;
    }

    private void ChangeMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.MouseRay = _player.MainCamera.ScreenPointToRay(Input.mousePosition);

            _player.State.ChangeState(_State.Move);
        }
    }

    private void ChangeAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttackRotation();
            _player.State.ChangeState(_State.ComboAttack1);
        }
    }
}

public class MoveState : PlayerState
{
    public MoveState(Player player) : base(player) { }

    //���� ����.
    public override void StateEnter()
    {
        _player.Agent.isStopped = false;

        RayCast();
    }

    //���� ���� ������Ʈ.
    public override void StateUpdate()
    {
        ClickMove();
    }

    //���� ����.
    public override void StateExit()
    {
        _player.Agent.SetDestination(_player.transform.position);
    }

    //�̵�ó�� �޼ҵ�
    private void ClickMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _player.MouseRay = _player.MainCamera.ScreenPointToRay(Input.mousePosition);

            RayCast();
        }
        else if (_player.Agent.remainingDistance < 0.1f)
        {
            ActiveTargetObject(false);

            _player.State.ChangeState(_State.Idle);
        }

        AnimationMoveMent();
        ChangeAttack();
    }

    //����ĳ��Ʈ�� �̵� ������ �����ϴ� �޼ҵ�
    private void RayCast()
    {
        if (Physics.Raycast(_player.MouseRay, out RaycastHit hit, Mathf.Infinity))
        {
            _player.Agent.SetDestination(hit.point);

            _player.ClickObject.position = new Vector3(hit.point.x, 0.01f, hit.point.z);

            ActiveTargetObject(true);
        }
    }

    //Ÿ�� ������Ʈ Ȱ��ȭ, ��Ȱ��ȭ �޼ҵ�
    private void ActiveTargetObject(bool isActive)
    {
        _player.ClickObject.gameObject.SetActive(isActive);
    }

    //Agent �ӵ��� ���� �̵� �ִϸ��̼� �޼ҵ�
    private void AnimationMoveMent()
    {
        Vector3 currentVelocity = _player.Agent.velocity;

        float speed = currentVelocity.magnitude;

        _player.Anim.SetFloat("Move", speed);
    }

    private void ChangeAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttackRotation();
            ActiveTargetObject(false);

            _player.Agent.isStopped = true;

            _player.Anim.SetFloat("Move", 0);

            _player.State.ChangeState(_State.ComboAttack1);
        }
    }
}

public class FirstAttackState : PlayerState
{
    public FirstAttackState(Player player) : base(player) { }

    public override void StateEnter()
    {
        InitializeFirstAttack();
    }

    public override void StateUpdate()
    {
        OnFirstAttackUpdate();
    }

    public override void StateExit()
    {
        _player.Anim.SetBool(_player.IsComboAttack1, false);
    }

    //�׺�޽� ��Ȱ��ȭ, ��Ʈ��� Ȱ��ȭ
    private void InitializeFirstAttack()
    {
        _player.Agent.enabled = false;
        _player.Anim.applyRootMotion = true;
        _player.Anim.SetBool(_player.IsComboAttack1, true);
        _player.IsNext = false;
        _player.StartCoroutine(AttackMove());
    }

    //ù��° ���� ������Ʈ
    private void OnFirstAttackUpdate()
    {
        var animatorStateInfo = _player.Anim.GetCurrentAnimatorStateInfo(0);

        if (animatorStateInfo.IsName("attack01") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            _player.Anim.SetTrigger("ComboFail");
            _player.State.ChangeState(_State.Idle);
        }

        if (Input.GetMouseButtonDown(0) && _player.IsNext)
        {
            AttackRotation();
            _player.State.ChangeState(_State.ComboAttack2);
        }
    } 
}

public class SecondAttackState : PlayerState
{
    public SecondAttackState(Player player) : base(player) { }

    public override void StateEnter()
    {
        InitializeSecondAttack();
    }

    public override void StateUpdate()
    {
        OnSecondAttackUpdate();
    }

    public override void StateExit()
    {
        _player.Anim.SetBool(_player.IsComboAttack2, false);
    }

    private void InitializeSecondAttack()
    {
        _player.Anim.SetBool(_player.IsComboAttack2, true);
        _player.IsNext = false;
        _player.StartCoroutine(AttackMove());
    }

    private void OnSecondAttackUpdate()
    {
        var animatorStateInfo = _player.Anim.GetCurrentAnimatorStateInfo(0);

        if (animatorStateInfo.IsName("attack02") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            _player.Anim.SetTrigger("ComboFail");
            _player.State.ChangeState(_State.Idle);
        }

        if (Input.GetMouseButtonDown(0) && _player.IsNext)
        {
            AttackRotation();
            _player.State.ChangeState(_State.ComboAttack3);
        }
    }
}

public class ThirdAttackState : PlayerState
{
    public ThirdAttackState(Player player) : base(player) { }

    public override void StateEnter()
    {
        InitializeThirdAttack();
    }

    public override void StateUpdate()
    {
        OnThirdAttackUpDate();
    }

    public override void StateExit()
    {
        _player.Anim.SetBool(_player.IsComboAttack3, false);
    }

    private void InitializeThirdAttack()
    {
        _player.Anim.SetBool(_player.IsComboAttack3, true);
        _player.IsNext = false;
        _player.StartCoroutine(AttackMove());
    }

    private void OnThirdAttackUpDate()
    {
        var animatorStateInfo = _player.Anim.GetCurrentAnimatorStateInfo(0);

        if (animatorStateInfo.IsName("attack03") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            _player.State.ChangeState(_State.Idle);
        }
    }
    
    /*
    // 스킬 사용
    private void UseSkill(int slotIndex)
    {
        SkillData skillData = GameManager.Instance.GetSkillData(slotIndex);
        if (skillData != null)
        {
            Debug.Log($"{skillData.name} 스킬 사용!");
            inGameUI.UseSkill(slotIndex);
            // 스킬의 쿨타임, 데미지, 범위 등의 데이터를 이용해 스킬 실행
        }
    }
     */
}