using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VATAnimator : MonoBehaviour
{
    [ReadOnly] MeshRenderer meshRenderer;
    [Required, SerializeField, AssetsOnly] private VATAnimationSO animation; 
    [ShowInInspector] private float currentTime = 0f;
    [SerializeField] private float duration = 1f;

    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        mpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= duration)
        {
            currentTime -= duration;
        }
        
        var normalizedTime = currentTime / duration;
        
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_NormalizeTime",normalizedTime);
        meshRenderer.SetPropertyBlock(mpb);
        
    }
}
