using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = -50f;

    [SerializeField]
    private float minSpeed = 50f;

    [SerializeField]
    private float maxSpeed = 100f;

    void Start()
    {

    }

    void Update()
    {
        if(PinGameManager.instance.isGameOver == false)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
    }

    public void SetRandomSpeed()
    {
        float speed = Random.Range(minSpeed, maxSpeed);

        if (Random.value < 0.5f)
        {
            speed *= -1f;
        }

        rotateSpeed = speed;
    }
}