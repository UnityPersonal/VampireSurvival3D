using UnityEngine;

[CreateAssetMenu(fileName = "Augment", menuName = "Augment")]
public class Augment : ScriptableObject
{
    public string AugmentName;
    public Sprite AugmentIcon;
    [TextArea]
    public string AugmentDescription;

}