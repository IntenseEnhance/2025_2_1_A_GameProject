using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //

public class GameManager : MonoBehaviour
{
    [Header("���� ����")]
    public int playerScore = 0;
    public int itemsCollected = 0;

    [Header("UI ����")]
    public Text scoreText;
    public Text itemCountText;
    public Text gameStatusText;

    public static GameManager Instance; // �̱��� ����

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ������ ���� �Լ�
    public void CollectItem()
    {
        itemsCollected++;
        Debug.Log($"������ ����! (�� : {itemsCollected}��)");
    }

    // UI ������Ʈ �Լ�
    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "���� : " + playerScore;
        }

        if (itemCountText != null)
        {
            itemCountText.text = "������ : " + itemsCollected + "��";
        }
    }
}