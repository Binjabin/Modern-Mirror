using UnityEngine;

public class Paintable : MonoBehaviour {
    const int TEXTURE_SIZE = 1024;
    [SerializeField] int textureSizeMultiplier = 4;
    public float extendsIslandOffset = 1;

    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture maskRenderTexture;
    RenderTexture supportTexture;
    RenderTexture blendTexture;

    public RenderTexture currentMask;
    
    Renderer rend;

    int maskTextureID = Shader.PropertyToID("_MaskTexture");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;

    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;
    public RenderTexture getBlend() => blendTexture; 

    public RenderTexture getCurrent() => currentMask;

    public void RequestInit() {
        int thisTextureSize = TEXTURE_SIZE * textureSizeMultiplier;
        maskRenderTexture = new RenderTexture(thisTextureSize, thisTextureSize, 0);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        extendIslandsRenderTexture = new RenderTexture(thisTextureSize, thisTextureSize, 0);
        extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        blendTexture = new RenderTexture(thisTextureSize, thisTextureSize, 0);
        blendTexture.filterMode = FilterMode.Bilinear;

        uvIslandsRenderTexture = new RenderTexture(thisTextureSize, thisTextureSize, 0);
        uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(thisTextureSize, thisTextureSize, 0);
        supportTexture.filterMode =  FilterMode.Bilinear;
        
        currentMask = new RenderTexture(thisTextureSize, thisTextureSize, 0);
        currentMask.filterMode =  FilterMode.Bilinear;

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

        PaintManager.instance.initTextures(this);
    }

    void OnDisable(){
        maskRenderTexture.Release();
        uvIslandsRenderTexture.Release();
        extendIslandsRenderTexture.Release();
        supportTexture.Release();
        blendTexture.Release();
    }
}