using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "VATAnimationSO", menuName = "Scriptable Objects/VATAnimationSO")]
public class VATAnimationSO : ScriptableObject
{
    public Texture2D clipTexture;
    [SerializeField] public VATClip[] clips;
    
    public const int IDLE_CLIP_INDEX = 0;
    public const int MOVE_CLIP_INDEX = 1;
    public const int ATTACK_CLIP_INDEX = 2;
    public const int DEATH_CLIP_INDEX = 3;
    
    
    [Serializable]
    public struct VATClip
    {
        [SerializeField] bool isLooping;
        public bool IsLooping => isLooping;
        [ReadOnly,SerializeField] int clipBeginIndexInTexture;// texture에서의 index
        public int ClipBeginIndexInTexture => clipBeginIndexInTexture;
        [ReadOnly,SerializeField] int clipEndIndexInTexture; // texture에서의 index
        public int ClipEndIndexInTexture => clipEndIndexInTexture;
        [ReadOnly,SerializeField] float duration;
        public float Duration => duration;
        [ReadOnly,SerializeField] Vector2 bound;
        public Vector2 Bound => bound;
        
        [ReadOnly] AnimationClip originalClip;

        public VATClip(
            int clipBeginIndexInTexture,
            int clipEndIndexInTexture,
            float duration,
            Vector2 bound,
            AnimationClip originalClip)
        {
            isLooping = false;
            this.clipBeginIndexInTexture = clipBeginIndexInTexture;
            this.clipEndIndexInTexture = clipEndIndexInTexture;
            this.duration = duration;
            this.bound = bound;
            this.originalClip = originalClip;
        }
    }
}
