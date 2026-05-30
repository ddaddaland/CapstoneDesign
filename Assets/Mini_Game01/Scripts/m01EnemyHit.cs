using System.Collections;
using UnityEngine;

public class m01EnemyHit : MonoBehaviour
{
    public GameObject explosion;
    public int hp = 5;
    public m01EnemySpawn enemySpawner;
    private MeshRenderer meshRenderer;
    private Color originalColor;
    private void Start()
    {

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }
    public void TakeDamage()
    {
        hp--;
        if (hp <= 0)
        {
            GameObject explosionEffect = Instantiate(explosion, transform.position, transform.rotation);
            enemySpawner.enemyCount--;
            Destroy(explosionEffect, 3.0f);
            Destroy(transform.parent.gameObject);
            return;
        }
        meshRenderer.material.color = Color.red;
        Invoke("ResetColor", 0.1f);
    }

    public void ResetColor()
    {
        meshRenderer.material.color = originalColor;

    }
}
