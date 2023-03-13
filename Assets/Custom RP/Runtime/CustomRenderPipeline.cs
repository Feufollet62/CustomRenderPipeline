using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRenderer _renderer = new CameraRenderer();
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera cam in cameras)
        {
            _renderer.Render(context,cam);
        }
    }
}

public class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera;

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _context = context;
        _camera = camera;
        
        DrawVisibleGeometry();
        Submit();
    }

    private void DrawVisibleGeometry () {
        _context.DrawSkybox(_camera);
    }
    
    private void Submit()
    {
        _context.Submit();
    }
}