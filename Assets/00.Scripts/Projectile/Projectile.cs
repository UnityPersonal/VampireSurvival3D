using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour , IDisposable
{
    public bool isDisposed = false;
    private WeaponController owner;
    [SerializeField] private float lifetime = 3;
    private float currentLife = 0;
    [SerializeField] private float moveSpeed;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;
    [SerializeField] private float attackPower = 1f;
    
    public void Dispose()
    {
        Debug.Log("Projectile Disposed");
        if (isDisposed == false)
        {
            isDisposed  = true;
            owner.Dispose(this);
        }
    }
    
    public void Create(WeaponController owner)
    {
        this.owner = owner;
    }

    public void Init()
    {
        Debug.Log($"Projectile Init");
        isDisposed  = false;
        currentLife  = lifetime;
    }

    public void Setup(Vector3 position, Vector3 direction)
    {
        rb.position = position;
        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    private void Update()
    {
        currentLife -= Time.deltaTime;
        if (currentLife <= 0)
        {
            Debug.Log($"Projectile Life End");
            Dispose();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isDisposed)
            return;
        
        if( CombatManager.TryLookup(other , out ICombatable combatable))
        {
            combatable.TakeDamage(new DealEventArgs(attackPower));
        }
        Debug.Log($"Projectile Trigger Enter");
        Dispose();
    }


}
