using UnityEngine;

public abstract class Augment : ScriptableObject
{
    public int AugmentID;
    public string AugmentName;
    public Sprite AugmentIcon;
    [TextArea]
    public string AugmentDescription;

    public abstract void Upgrade(WeaponController controller);

}