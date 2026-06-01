using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private float moveSpeed = 15f;
    private bool isPinned = false;
    private bool isLaunched = false;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if(isPinned == false && isLaunched == true)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            isPinned = true;
            transform.SetParent(collision.transform);
            PinGameManager.instance.DecreaseGoal();

            Rotate targetRotate = collision.GetComponent<Rotate>();
            if (targetRotate != null)
            {
                targetRotate.SetRandomSpeed();
            }
        }
        else if(collision.gameObject.tag == "Pin")
        {
            PinGameManager.instance.SetGameOver(false);
        }
    }

    public void Launch()
    {
        isLaunched = true;
    }
}