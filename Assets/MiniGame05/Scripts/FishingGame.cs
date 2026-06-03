using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;

public class FishingGame : MonoBehaviour
{
    [Header("게이지 설정")]
    public float gaugeMin = 0f;
    public float gaugeMax = 100f;
    public float gaugeSpeed = 40f;
    
    [Header("UI")]
    public Slider gaugeSlider;
    public Image fillImage;
    public Image greenZoneImage;
    public TMP_Text instructionText;

    [Header("게임 이미지")]
    public Image gameImage;             // 이미지 표시할 UI Image
    public Sprite fishingSprite;        // 낚시 중 사진
    public Sprite successSprite;        // 성공 사진
    public Sprite failSprite;           // 실패 사진

    [Header("색상")]
    public Color normalColor = Color.red;
    public Color greenColor = Color.green;

    private float greenMin;
    private float greenMax;
    private float currentValue;
    private float direction = 1f;
    private bool gameRunning = false;
    private bool stopped = false;

    void Start()
    {
        gaugeSlider.minValue = gaugeMin;
        gaugeSlider.maxValue = gaugeMax;
        currentValue = gaugeMin;
        direction = 1f;

        // 시작할 때 낚시 이미지로
        SetGameImage(fishingSprite);

        RandomizeGreenZone();
        StartCoroutine(StartCountdown());
    }

    private void RandomizeGreenZone()
    {
        float greenSize = 20f;
        greenMin = Random.Range(gaugeMin, gaugeMax - greenSize);
        greenMax = greenMin + greenSize;

        SetupGreenZone();
    }

    private void SetupGreenZone()
    {
        if (greenZoneImage == null) return;

        float totalRange = gaugeMax - gaugeMin;
        float greenRange = greenMax - greenMin;

        RectTransform sliderRect = gaugeSlider.GetComponent<RectTransform>();
        RectTransform greenRect = greenZoneImage.GetComponent<RectTransform>();

        float sliderWidth = sliderRect.rect.width;
        float greenWidth = (greenRange / totalRange) * sliderWidth;
        float greenPos = ((greenMin - gaugeMin) / totalRange) * sliderWidth - sliderWidth / 2f;

        greenRect.sizeDelta = new Vector2(greenWidth, greenRect.sizeDelta.y);
        greenRect.anchoredPosition = new Vector2(greenPos + greenWidth / 2f, 0f);
    }

    private IEnumerator StartCountdown()
    {
        instructionText.text = "준비!";
        yield return new WaitForSeconds(0.8f);
        instructionText.text = "SPACE로 멈춰!";
        gameRunning = true;
    }

    void Update()
    {
        if (!gameRunning || stopped) return;

        currentValue += direction * gaugeSpeed * Time.deltaTime;

        if (currentValue >= gaugeMax)
        {
            currentValue = gaugeMax;
            direction = -1f;
        }
        else if (currentValue <= gaugeMin)
        {
            currentValue = gaugeMin;
            direction = 1f;
        }

        gaugeSlider.value = currentValue;

        bool inGreen = currentValue >= greenMin && currentValue <= greenMax;
        fillImage.color = inGreen ? greenColor : normalColor;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stopped = true;
            StartCoroutine(JudgeResult());
        }
    }

    private IEnumerator JudgeResult()
    {
        gameRunning = false;

        bool inGreen = currentValue >= greenMin && currentValue <= greenMax;

        if (inGreen)
        {
            instructionText.text = "낚시 성공! 🎣";
            fillImage.color = greenColor;
            SetGameImage(successSprite);    // 성공 이미지로 교체
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.GameClear();
        }
        else
        {
            instructionText.text = "실패...";
            fillImage.color = Color.red;
            SetGameImage(failSprite);       // 실패 이미지로 교체
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.GameOver();
        }
    }

    // 이미지 교체 함수
    private void SetGameImage(Sprite sprite)
    {
        if (gameImage == null || sprite == null) return;
        gameImage.sprite = sprite;
    }
}