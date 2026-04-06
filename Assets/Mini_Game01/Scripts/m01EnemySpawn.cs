using UnityEngine;

public class m01EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
            Instantiate(Enemy, child.transform.position, child.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
