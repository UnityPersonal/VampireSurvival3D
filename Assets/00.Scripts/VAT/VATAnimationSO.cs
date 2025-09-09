using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class VATAnimationSO : ScriptableObject
{
    [ReadOnly] public Texture2D clipTexture;
    [ReadOnly] public Vector2 bound = Vector2.zero;
    [SerializeField] public VATClip[] clips;
    
    public const int IDLE_CLIP_INDEX = 0;
    public const int MOVE_CLIP_INDEX = 1;
    public const int ATTACK_CLIP_INDEX = 2;
    public const int DEATH_CLIP_INDEX = 3;
    
    public const int CLIP_COUNT = 4;
    
    
    [Serializable]
    public struct VATClip
    {
        [SerializeField] bool isLooping;
        public bool IsLooping => isLooping;
        [ReadOnly,SerializeField] int clipBeginIndexInTexture;// texture에서의 index
        public int ClipBeginIndexInTexture => clipBeginIndexInTexture;
        [ReadOnly,SerializeField] int clipLength; // texture에서의 길이
        public int ClipLength => clipLength;
        [ReadOnly,SerializeField] float duration;
        public float Duration => duration;
        
        [ReadOnly,SerializeField] AnimationClip originalClip;

        public VATClip(
            int clipBeginIndexInTexture,
            int clipLength,
            float duration,
            AnimationClip originalClip)
        {
            isLooping = false;
            this.clipBeginIndexInTexture = clipBeginIndexInTexture;
            this.clipLength = clipLength;
            this.duration = duration;
            this.originalClip = originalClip;
        }
    }
}
