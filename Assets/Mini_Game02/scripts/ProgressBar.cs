using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider progressSlider;

    public Transform car;
    public float start = 0f;
    public float end = 1000f;
    private bool isCleared = false;

    void Start()
    {
        start = car.position.y;
    }

    void Update()
    {
        float current = Mathf.Clamp(car.position.y, start, end);

        float totalDistance = end - start;
        float movedDistance = current - start;

        float progressValue = movedDistance / totalDistance;

        progressSlider.value = progressValue;
        if(progressSlider.value >= 1f && isCleared == false)
        {
            isCleared = true;
            GameClear();
        }
    }
    void GameClear()
    {
        GameManager.instance.GameClear();
    }
}
