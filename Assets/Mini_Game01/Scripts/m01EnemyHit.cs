using System.Collections;
using UnityEngine;

public class m01EnemyHit : MonoBehaviour
{
    public GameObject explosion;
    public int hp = 5;
    private MeshRenderer meshRenderer;
    private Color originalColor;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }
    public void TakeDamage()
    {
        hp--;
        if (hp <= 0)
        {
            GameObject tmp = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(tmp, 3.0f);
            Destroy(gameObject);
        }
        StartCoroutine(HitFlahEffect());
    }

    IEnumerator HitFlahEffect()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = originalColor;

    }
}
