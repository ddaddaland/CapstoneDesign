using UnityEngine;

public class m01Shoot : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shootSound;
    public m01CrossMove crossMove;
    public float fireTime = 0.5f;
    public float firePassTime = 0.0f;
    public float recoilStrength = 1f;

    void Shoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, 15f);
        if (hit.collider != null)
            if(hit.collider.tag == "ENEMY")
            {
                m01EnemyHit enemy = hit.collider.GetComponent<m01EnemyHit>();
                if (enemy != null)
                    enemy.TakeDamage();
            }
    }
    private void Start()
    {
        firePassTime = fireTime;
    }
    void Update()
    {
        if (Time.timeScale == 0f)
            return;
        if (firePassTime >= fireTime) {
            if (Input.GetMouseButtonDown(0)) { 
                firePassTime = 0;
                audioSource.PlayOneShot(shootSound);
                Shoot();
                crossMove.AddRecoil(recoilStrength);
            }
        }
        else
        {
            firePassTime += Time.deltaTime;
        }
    }
}
