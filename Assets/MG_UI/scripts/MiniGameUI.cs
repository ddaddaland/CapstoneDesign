using UnityEngine;
using UnityEngine.UI;

public class MiniGameUI : MonoBehaviour
{
    public static MiniGameUI uiInstance;
    public Image timerBar;
    public float maxTime = 5f;
    private float passTime;


    public void ResetTimer()
    {
        passTime = maxTime;
        timerBar.fillAmount = 1f;
    }
    void Start()
    {
        passTime = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (passTime>0)
        {
            passTime -= Time.deltaTime;
            timerBar.fillAmount = passTime / maxTime;
        }
    }
}
