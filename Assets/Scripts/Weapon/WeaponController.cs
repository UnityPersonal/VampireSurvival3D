using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    private int projectileID = 0;
    private ObjectPool<Projectile> projectilePool;

    private static void DestroyProjectile(Projectile obj)
    {
    }

    private static void ReleaseProjectile(Projectile obj)
    {
        obj.isDisposed = true;
        obj.gameObject.SetActive(false);
    }

    private static void GetProjectile(Projectile obj)
    {
        obj.gameObject.SetActive(true);
        obj.Init();
    }

    private Projectile CreateProjectile()
    {
        var instance = Instantiate(projectile);
        instance.gameObject.name = $"Projectile_{projectileID++}";
        instance.Create(this);
        return instance;
    }

    [SerializeField] LayerMask aimLayer;
    [SerializeField, AssetsOnly] Projectile projectile;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float aimRadius = 5f;
    private float fireTime = 0f;

    struct AimTarget
    {
        public Vector3 aimVector;
        public float sqrMagnitude;
    }

    private void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(CreateProjectile, GetProjectile, ReleaseProjectile, DestroyProjectile );
    }

    public void Dispose(Projectile obj)
    {
        projectilePool.Release(obj);
    }

    void Update()
    {
        if (fireTime + fireRate < Time.time)
        {
            Transform playerTransform  = transform;
            Vector3 aimDirection = playerTransform.forward;
            
            // aim search
            var collisons= Physics.OverlapSphere( playerTransform.position, aimRadius, aimLayer.value);
            if (collisons.Length > 0)
            {
                var target = new AimTarget[collisons.Length];
                for (int i = 0; i < collisons.Length; i++)
                {
                    var t = target[i];
                    t.aimVector = collisons[i].transform.position - playerTransform.position;
                    t.sqrMagnitude = t.aimVector.sqrMagnitude;
                    target[i] = t;
                }
            
                target.Sort((l,r) => l.sqrMagnitude.CompareTo(r.sqrMagnitude));
                //Debug.Log($"Final Target of {target.Length}: {target[0].aimVector} {target[0].sqrMagnitude}");
                aimDirection = target[0].aimVector.normalized;
            }
            
            // spawn and fire projectile
            var instance = projectilePool.Get();
            instance.transform.position = playerTransform.position;
            instance.Setup(aimDirection);
            
            fireTime = Time.time;
        }
        
        
    }
}
