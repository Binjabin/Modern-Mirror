using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class PaintManager : Singleton<PaintManager>{

    public Shader texturePaint;
    public Shader extendIslands;
    public Shader alphaBlend;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int blendOpID = Shader.PropertyToID("_BlendOp");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");
    int alphaBlendID = Shader.PropertyToID("_AlphaBlend");

    Material paintMaterial;
    Material extendMaterial;
    Material blendMaterial;

    CommandBuffer command;

    public override void Awake()
    {
        base.Awake();

        //Paintable[] paintables = FindObjectsOfType<Paintable>();


        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);
        blendMaterial = new Material(alphaBlend);
        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;

        //foreach (Paintable paintable in paintables)
        //{
            //paintable.RequestInit();
        //}
    }

    public void initTextures(Paintable paintable){
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        RenderTexture blend = paintable.getBlend();
        Renderer rend = paintable.getRenderer();

        command.SetRenderTarget(mask);
        command.SetRenderTarget(extend);
        command.SetRenderTarget(support);

        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }


    public void paint(Paintable paintable, Vector3 pos, float radius = 1f, float hardness = .5f, float strength = .5f, Color? color = null){
        RenderTexture mask = paintable.getMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        RenderTexture blend = paintable.getBlend();
        RenderTexture current = paintable.getCurrent();
        Renderer rend = paintable.getRenderer();

        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, support);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);


        command.SetRenderTarget(mask);
        command.DrawRenderer(rend, paintMaterial, 0);
        command.CopyTexture(mask, current);
        command.SetRenderTarget(support);
        command.Blit(mask, support);
        command.SetRenderTarget(extend);
        command.Blit(mask, extend, extendMaterial);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }

}
