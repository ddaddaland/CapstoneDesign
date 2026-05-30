using UnityEngine;

public class m01EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;
    public int enemyCount = 0;
    private bool isCleared = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject spawnedEnemy = Instantiate(Enemy, child.transform.position, child.transform.rotation);
            m01EnemyHit enemyHit = spawnedEnemy.GetComponentInChildren<m01EnemyHit>();
            if(enemyHit != null)
            {
                enemyHit.enemySpawner = this;
            }
            enemyCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCount <= 0 && isCleared == false)
        {
            isCleared = true;
            GameManager.instance.GameClear();
        }
            
    }
}
