using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10;

    [Header("점프 설정")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;      // 중력 속도 추가
    public float landingDuration = 0.3f; // 착지 후 착지 지속 시간

    [Header("공격 설정")]
    public float attackDuration = 0.8f; // 공격 지속 시간
    public bool canMoveWhileAttacking = false; // 공격 중 이동 가능 여부

    [Header("컴포넌트")]
    public Animator animator;

    private CharacterController controller;
    private Camera playerCamera;

    // 현재 상태
    private float currentSpeed;
    private bool isAttacking = false;   // 공격중인지 체크
    private bool isLanding = false;     // 착지중인지 확인
    private float landingTimer;          // 착지 타이머

    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded;            // 이전 프레임에 땅에 있었는지
    private float attackTimer;

    // Start이 지나면 첫 번째 프레임 업데이트 전에 호출됨
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        // 초기화
        isGrounded = controller.isGrounded;
        wasGrounded = isGrounded;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        HandleLanding();
        HandleMovement();
        HandleJump();
        HandleAttack();
        UpdateAnimator();
    }

    void CheckGrounded()
    {
        // 이전 상태 저장
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded; // 캐릭터 컨트롤러에서 받아온다.

        // 땅에서 떨어졌을 때 (지금 프레임은 땅이 아니고, 이전 프레임은 땅)
        if (!isGrounded && wasGrounded)
        {
            Debug.Log("떨어지기 시작");
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

            // 착지 모션 트리거 및 착지 상태 시작
            if (!wasGrounded && animator != null)
            {
                animator.SetTrigger("landTrigger");
                isLanding = true;
                landingTimer = landingDuration;
                Debug.Log("착지");
            }
        }
    }

    void HandleLanding()
    {
        if (isLanding)
        {
            landingTimer -= Time.deltaTime; // 랜딩 타이머 시간 만큼 못 움직임

            if (landingTimer <= 0)
            {
                isLanding = false; // 착지 완료
            }
        }
    }

    void HandleMovement() // 이동 함수 제작
    {
        // 공격 중이거나 착지 중일 때 움직임 제한
        if ((isAttacking && !canMoveWhileAttacking) || isLanding)
        {
            currentSpeed = 0;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 둘 중 하나라도 입력이 있을 때
        if (horizontal != 0 || vertical != 0)
        {
            // 카메라가 보는 방향이 앞으로 되게 설정
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;

            // 이미지에 잘린 부분에 대한 일반적인 로직 추정 (베껴쓰기 요청에 따라 생략)
            // ...
        }
    }

    void HandleJump()
    {
        // 땅 위에 있을 때만 점프를 할 수 있다.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("jumpTrigger");
            }
        }

        // 땅에 있지 않을 경우 중력 적용
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // 최종 이동 적용
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAttack()
    {
        if (isAttacking) // 공격 중일 때
        {
            attackTimer -= Time.deltaTime; // 타이머를 감소시킨다.

            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }

        // 공격 중이 아닐 때 키(Alpha1)를 누르면 공격
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)
        {
            isAttacking = true; // 공격중 표시
            attackTimer = attackDuration; // 타이머 리필

            if (animator != null)
            {
                animator.SetTrigger("attackTrigger");
            }
        }
    }

    void UpdateAnimator()
    {
        // 전체 최대 속도 (runSpeed) 기준으로 0 ~ 1 계산
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);

        animator.SetFloat("speed", animatorSpeed);
        animator.SetBool("isGrounded", isGrounded);

        // 캐릭터의 Y 축 속도가 음수로 내려가면 떨어지고 있다고 판단
        bool isFalling = !isGrounded && velocity.y < -0.1f;
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isLanding", isLanding);
    }
}