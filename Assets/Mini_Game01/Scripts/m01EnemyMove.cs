
using UnityEngine;

public class m01EnemyMove : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float moveTime = 1f;
    private float movePassTime = 0f;
    private int ufoXDirection = 0;
    private int ufoYDirection = 0;
    void ClampToScreen()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        float clampedX = Mathf.Clamp(viewPos.x, 0.01f, 0.99f);
        float clampedY = Mathf.Clamp(viewPos.y, 0.01f, 0.99f);
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(clampedX, clampedY, viewPos.z));
    }
    bool IsXOutside(Vector3 pos)
    {
        pos = Camera.main.WorldToViewportPoint(pos);
        if (pos.x <= 0f || pos.x >= 1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool IsYOutside(Vector3 pos)
    {
        pos = Camera.main.WorldToViewportPoint(pos);
        if (pos.y <= 0f || pos.y >= 1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movePassTime = moveTime;
    }

    void Update()
    {
        if (movePassTime >= moveTime)
        {
            movePassTime = 0;
            ufoXDirection = Random.Range(-1, 2);
            ufoYDirection = Random.Range(-1, 2);
        }
        else
        {
            movePassTime += Time.deltaTime;
        }
        if (IsXOutside(transform.position))
        {
            ufoXDirection *= -1;
            ClampToScreen();
            movePassTime = 0;
        }
        if (IsYOutside(transform.position))
        {
            ufoYDirection *= -1;
            ClampToScreen();
            movePassTime = 0;
        }
        transform.Translate(ufoXDirection * transform.right * moveSpeed * Time.deltaTime);
        transform.Translate(ufoYDirection * transform.up * moveSpeed * Time.deltaTime);
    }
}
