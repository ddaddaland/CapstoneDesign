using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField]
    private GameObject pinObject;

    [SerializeField]
    private TextMeshProUGUI guideText;

    private Pin currPin;

    void Start()
    {
        // 시작할 때 guideText 숨기기
        if (guideText != null)
            guideText.gameObject.SetActive(false);

        Reload();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currPin != null && PinGameManager.instance.isGameOver == false)
        {
            currPin.Launch();
            currPin = null;
            Invoke("Reload", 0.2f);
        }
    }

    void Reload()
    {
        if (PinGameManager.instance.isGameOver == false)
        {
            GameObject pin = Instantiate(pinObject, transform.position, Quaternion.identity);
            currPin = pin.GetComponent<Pin>();
        }
    }
}