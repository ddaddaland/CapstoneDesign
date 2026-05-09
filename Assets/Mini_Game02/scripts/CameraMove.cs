using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform car;
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 cameraPos = new Vector3(0, car.position.y -3f, -10);
        transform.position = cameraPos;
    }
}
