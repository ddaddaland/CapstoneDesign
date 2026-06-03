using UnityEngine;
using UnityEngine.UI;

public class TapTargetGame : MonoBehaviour
{
    public RectTransform gameArea;
    public GameObject targetPrefab;

    private GameObject currentTarget;
    private int hitCount = 0;
    private const int RequiredHits = 3;
    private bool running = false;

    void Start()
    {
        hitCount = 0;
        running = true;
        SpawnTarget();
    }

    void SpawnTarget()
    {
        if (currentTarget != null) Destroy(currentTarget);
        if (targetPrefab == null || gameArea == null) return;

        float halfW = gameArea.rect.width * 0.5f;
        float halfH = gameArea.rect.height * 0.5f;

        currentTarget = Instantiate(targetPrefab, gameArea);
        currentTarget.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            Random.Range(-(halfW - 80f), halfW - 80f),
            Random.Range(-(halfH - 80f), halfH - 80f));

        var button = currentTarget.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnTapTarget);
    }

    void OnTapTarget()
    {
        if (!running) return;

        hitCount++;
        if (hitCount >= RequiredHits)
        {
            running = false;
            Debug.Log("게임 성공! (클리어)");
            if (currentTarget != null) Destroy(currentTarget);
            return;
        }
        SpawnTarget();
    }
}