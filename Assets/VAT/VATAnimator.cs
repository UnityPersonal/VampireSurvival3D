using Sirenix.OdinInspector;
using UnityEngine;

public class VATAnimator : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [ShowInInspector] private float currentTime = 0f;
    [SerializeField] private float duration = 1f;

    private MaterialPropertyBlock mpb;
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
