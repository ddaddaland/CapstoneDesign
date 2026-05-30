using UnityEngine;
using UnityEngine.UI;

public class m01Timer : MonoBehaviour
{
    public Slider timeSlider;
    public float maxTime = 5.0f;

    private float currentTime;
    private bool isTimeOver = false;

    void Start()
    {
        currentTime = maxTime;
        timeSlider.maxValue = maxTime;
        timeSlider.value = currentTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeOver) return;
        currentTime -= Time.deltaTime;
        timeSlider.value = currentTime;
        if(currentTime <= 0 && isTimeOver == false)
        {
            isTimeOver = true;
            GameOver();
        }
    }
    void GameOver()
    {
        GameManager.instance.GameOver();
    }
}
