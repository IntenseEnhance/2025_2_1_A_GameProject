using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("상호 작용 설정")]
    public float interactionRange = 2.0f;           // 상호작용 범위
    public LayerMask interactionLayerMask = 1;     // 상호작용할 레이어
    public KeyCode interactionKey = KeyCode.E;      // 상호 작용 키 (E 키)

    [Header("UI 설정")]
    public Text interactionText;                   // 상호작용 UI 텍스트
    public GameObject interactionUI;                // 상호작용 UI 패널

    private Transform playerTransform;
    private InteractableObject currentInteractable; // 감지된 오브젝트 담는 클래스

    void Start()
    {
        playerTransform = transform;
        HideInteractionUI();
    }

    void Update()
    {
        CheckForInteractables();
        HandleInteractionInput();
    }

    void HandleInteractionInput() // 인터랙션 입력 함수
    {
        if (currentInteractable != null && Input.GetKeyDown(interactionKey)) // 인터랙션 키(E)를 눌렀을 때
        {
            currentInteractable.Interact(); // 행동을 진행 한다.
        }
    }

    void CheckForInteractables()
    {
        // 플레이어 앞쪽 방향으로 체크
        Vector3 checkPosition = playerTransform.position + playerTransform.forward * (interactionRange * 0.5f);

        Collider[] hitColliders = Physics.OverlapSphere(checkPosition, interactionRange, interactionLayerMask);

        InteractableObject closestInteractable = null;
        float closestDistance = float.MaxValue;

        // 가장 가까운 상호 작용 오브젝트 찾기
        foreach (Collider collider in hitColliders)
        {
            InteractableObject interactable = collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                float distance = Vector3.Distance(playerTransform.position, collider.transform.position);

                // 플레이어가 바라보는 방향에 있는지 확인 (각도 체크)
                Vector3 directionToObject = (collider.transform.position - playerTransform.position).normalized;
                float angle = Vector3.Angle(playerTransform.forward, directionToObject);

                // 앞쪽 90도 범위 내 && 가장 가까운 오브젝트
                if (angle < 90f && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        // 상호 작용 오브젝트 변경 체크
        if (closestInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnPlayerExit(); // 이전 오브젝트에서 나감
            }

            currentInteractable = closestInteractable;

            if (currentInteractable != null)
            {
                currentInteractable.OnPlayerEnter(); // 새 오브젝트 선택
                ShowInteractionUI(currentInteractable.GetInteractionText());
            }
            else
            {
                HideInteractionUI();
            }
        }
    }

    void ShowInteractionUI(string text) // 인터랙션 창을 연다.
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.text = text;
        }
    }

    void HideInteractionUI() // 인터랙션 창을 닫는다.
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}