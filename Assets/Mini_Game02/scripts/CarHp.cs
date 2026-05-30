using TMPro;
using UnityEngine;

public class CarHp : MonoBehaviour
{
    public int Hp = 3;

    public float invincibleTime = 0.5f;
    public TextMeshProUGUI hpText;
    private float currentTime = 0f;
    private bool isHit = false;

    void Start()
    {
        hpText.text = "HP : " + Hp;
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
        Hp -= damage;
        isHit = true;
        currentTime = 0f;
        hpText.text = "HP : " + Hp;
        if (Hp <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        GameManager.instance.GameOver();
    }
}
