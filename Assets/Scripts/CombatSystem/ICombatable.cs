using UnityEngine;

public interface ICombatable
{
    Transform CombatTransform { get; }
    Collider CombatCollider { get; }
}

