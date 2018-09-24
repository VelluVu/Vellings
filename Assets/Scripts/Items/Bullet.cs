using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    float bulletSpeed;
    float bulletTime;
    int bulletdmg;
    public Rigidbody2D rb;

    private void Start()
    {
        bulletSpeed = 0.6f;
        bulletTime = 5f;
        bulletdmg = 10;
        rb.velocity = transform.right * bulletSpeed;
        StartCoroutine(BulletTime());
    }
    IEnumerator BulletTime()
    {
        yield return new WaitForSecondsRealtime(bulletTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if ( enemy != null)
        {
            
            enemy.EnemyTakeDamage(bulletdmg);
        }
        Destroy(gameObject);
    }
}
