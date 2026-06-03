using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombFuseGame : MonoBehaviour
{
    [Header("심지 설정")]
    public int fusePointCount = 50;
    public float fuseLength = 4f;
    public float fuseWaveHeight = 0.4f;
    public float fuseWaveFrequency = 3f;
    public float fuseBurnSpeed = 4f;
    public float clickRadius = 0.4f;

    [Header("클릭 설정")]
    public int requiredClicks = 10;         // 몇 번 클릭해야 꺼지는지

    [Header("심지 시작 오프셋 (폭탄 기준)")]
    public Vector3 fuseOffset = new Vector3(0f, 0.5f, 0f);

    [Header("Line Renderer")]
    public LineRenderer fuseLine;
    public LineRenderer burnLine;

    [Header("불꽃 오브젝트")]
    public Transform spark;

    [Header("폭탄 스프라이트")]
    public SpriteRenderer bombRenderer;
    public Sprite bombNormal;
    public Sprite bombExplode;
    public Sprite bombSafe;

    private List<Vector3> fusePoints = new List<Vector3>();
    private float burnProgress = 0f;
    private bool gameRunning = false;
    private bool isEnded = false;

    private int currentClicks = 0;
    private Vector3 sparkOriginalScale;     // 불씨 원래 크기

    void Start()
    {
        if (bombRenderer != null) bombRenderer.sprite = bombNormal;
        if (spark != null) sparkOriginalScale = spark.localScale;
        GenerateFuse();
        StartCoroutine(StartCountdown());
    }

    private void GenerateFuse()
    {
        fusePoints.Clear();

        Vector3 startPos = transform.position + fuseOffset;
        float randomOffset = Random.Range(0f, 100f);

        for (int i = 0; i < fusePointCount; i++)
        {
            float t = (float)i / (fusePointCount - 1);
            float x = startPos.x + t * fuseLength;
            float sinWave = Mathf.Sin(t * fuseWaveFrequency * Mathf.PI * 2f) * fuseWaveHeight;
            float noise = (Mathf.PerlinNoise(t * 3f + randomOffset, 0f) - 0.5f) * fuseWaveHeight;
            float y = startPos.y + sinWave * 0.5f + noise * 0.5f;
            fusePoints.Add(new Vector3(x, y, 0f));
        }

        // 심지 라인 (검은색)
        if (fuseLine != null)
        {
            fuseLine.positionCount = fusePointCount;
            fuseLine.SetPositions(fusePoints.ToArray());
            fuseLine.startWidth = 0.05f;
            fuseLine.endWidth = 0.05f;
            fuseLine.startColor = Color.black;
            fuseLine.endColor = Color.black;
        }

        if (burnLine != null)
            burnLine.positionCount = 0;

        burnProgress = fusePointCount - 1;
        if (spark != null)
            spark.position = fusePoints[fusePointCount - 1];
    }

    private IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(0.8f);
        gameRunning = true;
    }

    void Update()
    {
        if (!gameRunning || isEnded) return;

        // 심지 타들어가기
        burnProgress -= fuseBurnSpeed * Time.deltaTime;

        if (burnProgress <= 0f)
        {
            burnProgress = 0f;
            StartCoroutine(ExplodeRoutine());
            return;
        }

        int currentIndex = Mathf.Clamp(Mathf.FloorToInt(burnProgress), 0, fusePointCount - 1);

        // 불꽃 위치 보간
        float frac = burnProgress - Mathf.Floor(burnProgress);
        Vector3 sparkPos;
        if (currentIndex < fusePointCount - 1)
            sparkPos = Vector3.Lerp(fusePoints[currentIndex], fusePoints[currentIndex + 1], frac);
        else
            sparkPos = fusePoints[currentIndex];

        if (spark != null)
            spark.position = sparkPos;

        // 안 탄 부분만 표시
        if (fuseLine != null)
        {
            fuseLine.positionCount = currentIndex + 1;
            for (int i = 0; i <= currentIndex; i++)
                fuseLine.SetPosition(i, fusePoints[i]);
        }

        if (burnLine != null)
            burnLine.positionCount = 0;

        // 클릭 판정
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            float dist = Vector3.Distance(mouseWorld, sparkPos);

            if (dist <= clickRadius)
            {
                currentClicks++;

                // 클릭할수록 불씨 작아지기
                float scaleRatio = 1f - ((float)currentClicks / requiredClicks);
                scaleRatio = Mathf.Max(scaleRatio, 0.1f);  // 최소 크기 유지
                if (spark != null)
                    spark.localScale = sparkOriginalScale * scaleRatio;

                // n번 클릭하면 성공
                if (currentClicks >= requiredClicks)
                    StartCoroutine(SuccessRoutine());
            }
        }
    }

    private IEnumerator SuccessRoutine()
    {
        isEnded = true;
        gameRunning = false;

        if (bombRenderer != null) bombRenderer.sprite = bombSafe;
        if (spark != null) spark.gameObject.SetActive(false);
        if (fuseLine != null) fuseLine.positionCount = 0;
        if (burnLine != null) burnLine.positionCount = 0;

        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance != null)
            GameManager.instance.GameClear();
        else
            Debug.Log("GameClear!");
    }

    private IEnumerator ExplodeRoutine()
    {
        isEnded = true;
        gameRunning = false;

        if (bombRenderer != null) bombRenderer.sprite = bombExplode;
        if (spark != null) spark.gameObject.SetActive(false);
        if (fuseLine != null) fuseLine.positionCount = 0;
        if (burnLine != null) burnLine.positionCount = 0;

        StartCoroutine(ShakeRoutine());

        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance != null)
            GameManager.instance.GameOver();
        else
            Debug.Log("GameOver!");
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < 0.4f)
        {
            float x = originalPos.x + Random.Range(-0.2f, 0.2f);
            float y = originalPos.y + Random.Range(-0.2f, 0.2f);
            transform.position = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}