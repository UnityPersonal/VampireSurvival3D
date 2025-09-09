using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "VATBakeCommandSO", menuName = "VAT/VATBakeCommandSO")]
public class VATBakeCommandSO : ScriptableObject
{
    [Required,AssetsOnly] public AnimationClip idle;
    [Required,AssetsOnly] public AnimationClip move;
    [Required,AssetsOnly] public AnimationClip attack;
    [Required,AssetsOnly] public AnimationClip death;

    [Required,AssetsOnly] public GameObject animatedGameObject;

    public SkinnedMeshRenderer skinnedMeshRenderer
    {
        get
        {
            var smr = animatedGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            return smr;
        }
        
    }
    
    public bool isValid
    {
        get
        {
            bool clipEnable = true;
            clipEnable &= idle;
            clipEnable &= move;
            clipEnable &= attack;
            clipEnable &= death;
            
            clipEnable &= animatedGameObject;
            clipEnable &= skinnedMeshRenderer && skinnedMeshRenderer.sharedMesh;
            
            return clipEnable;
        }
    }

    public AnimationClip[] clips
    {
        get
        {
            var temps = new AnimationClip[VATAnimationSO.CLIP_COUNT];
            temps[0] = idle;
            temps[1] = move;
            temps[2] = attack;
            temps[3] = death;

            return temps;
        }
    }

}
