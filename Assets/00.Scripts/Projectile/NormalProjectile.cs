using Sirenix.OdinInspector;
using UnityEngine;

public class NormalProjectile : Projectile
{
    [Title("Special Settings")]
    [SerializeField] private int penetration = 2;
    
    [SerializeField] private GameObject HitEffectPrefab;
    
    public override void OnHit()
    {
        penetration--;
        Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
        if (penetration == 0)
        {
            Dispose();
        }
    }
}