using UnityEngine;

public class CarMove : MonoBehaviour
{
    public float moveSpeed = 20.0f;
    public float changeSpeed = 20.0f;
    public float[] carPos = { -3.75f, -1.25f, 1.25f, 3.75f};
    private int currentRoad = 1;
    public float tiltAngle = 15f;
    public float rotationSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentRoad--;
            if (currentRoad< 0) currentRoad = 0;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentRoad++;
            if (currentRoad > 3) currentRoad = 3;
        }

        float targetX = carPos[currentRoad];

        float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * changeSpeed);
        transform.position = new Vector3(newX, transform.position.y + (moveSpeed * Time.deltaTime), transform.position.z);

        float distanceX = targetX - transform.position.x;

        float targetZ = 0f; 

        if (distanceX > 0.1f)
        {
            targetZ = -tiltAngle;
        }
        else if (distanceX < -0.1f)
        {
            targetZ = tiltAngle;
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
