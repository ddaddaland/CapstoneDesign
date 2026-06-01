using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PinGameManager : MonoBehaviour
{
    public static PinGameManager instance = null;

    public bool isGameOver = false;

    [SerializeField]
    private TextMeshProUGUI textGoal;

    [SerializeField]
    public int goal;

    [SerializeField]
    private Color green;
    [SerializeField]
    private Color red;

    [SerializeField]
    private SpriteRenderer targetFaceRenderer;

    [SerializeField]
    private Sprite normalFace;

    [SerializeField]
    private Sprite winFace;

    [SerializeField]
    private Sprite loseFace;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        textGoal.SetText(goal.ToString());

        if (targetFaceRenderer != null && normalFace != null)
        {
            targetFaceRenderer.sprite = normalFace;
        }
    }

    void Update()
    {
        // Escape, R키 제거
    }

    public void DecreaseGoal()
    {
        goal -= 1;
        textGoal.SetText(goal.ToString());

        if (goal <= 0)
        {
            SetGameOver(true);
        }
    }

    public void SetGameOver(bool success)
    {
        if(isGameOver == false)
        {
            isGameOver = true;

            Camera.main.backgroundColor = success ? green : red;

            if (targetFaceRenderer != null)
            {
                targetFaceRenderer.sprite = success ? winFace : loseFace;
            }

            StartCoroutine(EndRoutine(success));
        }
    }

    private IEnumerator EndRoutine(bool success)
    {
        // 결과 잠깐 보여주고 팀장 GameManager 호출
        yield return new WaitForSeconds(0.5f);

        if (success)
            GameManager.instance.GameClear();
        else
            GameManager.instance.GameOver();
    }
}
