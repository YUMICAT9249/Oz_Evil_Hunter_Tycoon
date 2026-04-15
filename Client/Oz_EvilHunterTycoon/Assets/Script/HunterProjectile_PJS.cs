using UnityEngine;

// 헌터 기본 발사체 스크립트
public class HunterProjectile_PJS : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        { 
            Destroy(gameObject);
        }
    }
}
