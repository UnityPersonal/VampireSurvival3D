using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VATAnimator : MonoBehaviour
{
    [ReadOnly] MeshRenderer meshRenderer;
    [Required, SerializeField, AssetsOnly] private VATAnimationSO animation; 
    [ShowInInspector] private float currentTime = 0f;

    private MaterialPropertyBlock mpb;
    private float textureWidth;

    [ShowInInlineEditors] private int currentClipIndex = VATAnimationSO.MOVE_CLIP_INDEX;
    VATAnimationSO.VATClip currentClip;
    
    [ShowInInlineEditors] bool isPlaying = false;

    [ShowInInspector] private float beginNormalizedTime;
    [ShowInInspector] private float endNormalizedTime;
    
    
    
    public delegate void AnimationEndEvent(int clipIndex);
    public event AnimationEndEvent OnAnimationEnd;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        textureWidth = (float)animation.clipTexture.width;
    }

    void Start()
    {
        mpb = new MaterialPropertyBlock();
        
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetVector("_Bounds",animation.bound);
        meshRenderer.SetPropertyBlock(mpb);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying) return;
        
        currentTime += Time.deltaTime;
        if (currentTime >= currentClip.Duration)
        {
            if (currentClip.IsLooping)
            {
                currentTime -= currentClip.Duration;
            }
            else
            {
                isPlaying = false;
                OnAnimationEnd?.Invoke(currentClipIndex);
            }
        }
        
        var normalizedTime = currentTime / currentClip.Duration;
        
        beginNormalizedTime = currentClip.ClipBeginIndexInTexture / textureWidth;
        endNormalizedTime = (currentClip.ClipBeginIndexInTexture + currentClip.ClipLength) / textureWidth;
        
        var textureNormalizedTime = Mathf.Lerp(beginNormalizedTime, endNormalizedTime, normalizedTime);
        
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_NormalizeTime",textureNormalizedTime);
        meshRenderer.SetPropertyBlock(mpb);
    }

    public void Play(int clipIndex)
    {
        currentClipIndex = clipIndex;
        currentClip = animation.clips[currentClipIndex];
        currentTime = 0;
        isPlaying = true;
    }

}
