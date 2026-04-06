using UnityEngine;

public class m01CrossMove : MonoBehaviour
{
    private Vector3 mousePos;
    private Vector3 recoilOffset;
    private float recoverySpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoverySpeed);
        transform.position = mousePos + recoilOffset;
        
    }

    public void AddRecoil(float recoil)
    {
        recoilOffset += Vector3.up * recoil;
    }
}
