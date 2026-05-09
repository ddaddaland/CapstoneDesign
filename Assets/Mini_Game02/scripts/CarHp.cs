using UnityEngine;

public class CarHp : MonoBehaviour
{
    public int HP = 3;

    public float invincibleTime = 2f;
    private float currentTime = 0f;
    private bool isHit = false;

    void Start()
    {
    }

    void Update()
    {
        if (isHit)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= invincibleTime)
            {
                isHit = false;
                currentTime = 0f;
            }
        }
    }

    // 2. 충돌 감지 (Trigger 방식)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OBSTACLE") && !isHit)
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    void TakeDamage(int damage)
    {
        HP -= damage;
        isHit = true;
        currentTime = 0f; 

        if (HP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("게임 오버!");
    }
}
