using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRenderer _renderer = new CameraRenderer();

    public CustomRenderPipeline()
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
    }
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera cam in cameras)
        {
            _renderer.Render(context,cam);
        }
    }
}