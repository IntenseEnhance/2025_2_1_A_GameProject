using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("�̵� ����")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10;

    [Header("���� ����")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;      // �߷� �ӵ� �߰�
    public float landingDuration = 0.3f; // ���� �� ���� ���� �ð�

    [Header("���� ����")]
    public float attackDuration = 0.8f; // ���� ���� �ð�
    public bool canMoveWhileAttacking = false; // ���� �� �̵� ���� ����

    [Header("������Ʈ")]
    public Animator animator;

    private CharacterController controller;
    private Camera playerCamera;

    // ���� ����
    private float currentSpeed;
    private bool isAttacking = false;   // ���������� üũ
    private bool isLanding = false;     // ���������� Ȯ��
    private float landingTimer;          // ���� Ÿ�̸�

    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded;            // ���� �����ӿ� ���� �־�����
    private float attackTimer;

    // Start�� ������ ù ��° ������ ������Ʈ ���� ȣ���
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        // �ʱ�ȭ
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
        // ���� ���� ����
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded; // ĳ���� ��Ʈ�ѷ����� �޾ƿ´�.

        // ������ �������� �� (���� �������� ���� �ƴϰ�, ���� �������� ��)
        if (!isGrounded && wasGrounded)
        {
            Debug.Log("�������� ����");
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

            // ���� ��� Ʈ���� �� ���� ���� ����
            if (!wasGrounded && animator != null)
            {
                animator.SetTrigger("landTrigger");
                isLanding = true;
                landingTimer = landingDuration;
                Debug.Log("����");
            }
        }
    }

    void HandleLanding()
    {
        if (isLanding)
        {
            landingTimer -= Time.deltaTime; // ���� Ÿ�̸� �ð� ��ŭ �� ������

            if (landingTimer <= 0)
            {
                isLanding = false; // ���� �Ϸ�
            }
        }
    }

    void HandleMovement() // �̵� �Լ� ����
    {
        // ���� ���̰ų� ���� ���� �� ������ ����
        if ((isAttacking && !canMoveWhileAttacking) || isLanding)
        {
            currentSpeed = 0;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �� �� �ϳ��� �Է��� ���� ��
        if (horizontal != 0 || vertical != 0)
        {
            // ī�޶� ���� ������ ������ �ǰ� ����
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;

            // �̹����� �߸� �κп� ���� �Ϲ����� ���� ���� (�������� ��û�� ���� ����)
            // ...
        }
    }

    void HandleJump()
    {
        // �� ���� ���� ���� ������ �� �� �ִ�.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("jumpTrigger");
            }
        }

        // ���� ���� ���� ��� �߷� ����
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // ���� �̵� ����
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAttack()
    {
        if (isAttacking) // ���� ���� ��
        {
            attackTimer -= Time.deltaTime; // Ÿ�̸Ӹ� ���ҽ�Ų��.

            if (attackTimer <= 0)
            {
                isAttacking = false;
            }
        }

        // ���� ���� �ƴ� �� Ű(Alpha1)�� ������ ����
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)
        {
            isAttacking = true; // ������ ǥ��
            attackTimer = attackDuration; // Ÿ�̸� ����

            if (animator != null)
            {
                animator.SetTrigger("attackTrigger");
            }
        }
    }

    void UpdateAnimator()
    {
        // ��ü �ִ� �ӵ� (runSpeed) �������� 0 ~ 1 ���
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);

        animator.SetFloat("speed", animatorSpeed);
        animator.SetBool("isGrounded", isGrounded);

        // ĳ������ Y �� �ӵ��� ������ �������� �������� �ִٰ� �Ǵ�
        bool isFalling = !isGrounded && velocity.y < -0.1f;
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isLanding", isLanding);
    }
}