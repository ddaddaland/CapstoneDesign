using UnityEngine;

public class m01EnemyFloat : MonoBehaviour
{
    public float upDownSpeed = 2.0f;
    public float floatHeight = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        UfoFloat();
    }
    void UfoFloat()
    {
        float sinValue = Mathf.Sin(Time.time * upDownSpeed) * floatHeight;
        Vector3 tmp = transform.localPosition;
        tmp.y = sinValue;
        transform.localPosition = tmp;
    }
}
