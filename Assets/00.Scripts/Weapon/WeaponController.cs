using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    private int projectileID = 0;
    
    private ObjectPool<Projectile> projectilePool;

    [SerializeField, Required, ChildGameObjectsOnly] private Transform turret;
    [SerializeField, Required, ChildGameObjectsOnly] Transform firePoint;
    [SerializeField, Required, ChildGameObjectsOnly] Transform body;

    private static void DestroyProjectile(Projectile obj)
    {
        Destroy(obj.gameObject);
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
        projectilePool = new ObjectPool<Projectile>(CreateProjectile, GetProjectile, ReleaseProjectile, DestroyProjectile, true, 100,100);
    }

    public void Dispose(Projectile obj)
    {
        projectilePool.Release(obj);
    }

    private Collider[] currentFrameDetected;

    [SerializeField] float rotateSpeed = 50f;
    void RotateTurret(Vector3 direction)
    {
        Vector3 turretDirection = turret.forward;
        turret.forward = Vector3.Slerp(turretDirection, direction, Time.fixedDeltaTime * rotateSpeed);
    }

    void Update()
    {
        Transform weaponTransform  = transform;
        currentFrameDetected = Physics.OverlapSphere( weaponTransform.position, aimRadius, aimLayer.value);
        
        Vector3 aimDirection = turret.forward;

        if (currentFrameDetected.Length == 0)
        {
            aimDirection = body.forward; // 적이 없다면 현재 이동 방향을 향한다.
        }
        else
        {
            var target = new AimTarget[currentFrameDetected.Length];
            for (int i = 0; i < currentFrameDetected.Length; i++)
            {
                var t = target[i];
                t.aimVector = currentFrameDetected[i].transform.position - weaponTransform.position;
                t.sqrMagnitude = t.aimVector.sqrMagnitude;
                target[i] = t;
            }
            
            target.Sort((l,r) => l.sqrMagnitude.CompareTo(r.sqrMagnitude));
            //Debug.Log($"Final Target of {target.Length}: {target[0].aimVector} {target[0].sqrMagnitude}");
            aimDirection = target[0].aimVector.normalized;
        }
        
        RotateTurret(aimDirection);
        
        if (fireTime + fireRate < Time.time)
        {
            // spawn and fire projectile
            var instance = projectilePool.Get();
            instance.Setup(firePoint.position, turret.forward);

            fireTime = Time.time;
        }
        
        
    }
}
