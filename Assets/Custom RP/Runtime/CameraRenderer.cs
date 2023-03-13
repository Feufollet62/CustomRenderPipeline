using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera;

    private const string BufferName = "Render Camera";
    private readonly CommandBuffer _buffer = new CommandBuffer {name = BufferName};

    private CullingResults _cullResults;
    
    private static readonly ShaderTagId UnlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    
    // Bugged or unsupported shaders
    private static Material _errorMaterial;
    private static ShaderTagId[] _legacyShaderTagIds = 
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _context = context;
        _camera = camera;

        if (!Cull()) return;
        
        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        Submit();
    }

    private void Setup()
    {
        _context.SetupCameraProperties(_camera);
        _buffer.ClearRenderTarget(true, true, Color.clear);
        _buffer.BeginSample(BufferName);
        ExecuteBuffer();
    }

    private void DrawVisibleGeometry ()
    {
        SortingSettings sortingSettings = new SortingSettings(_camera) {criteria = SortingCriteria.CommonOpaque};
        DrawingSettings drawingSettings = new DrawingSettings(UnlitShaderTagId, sortingSettings);
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque); // Draw opaques first
        
        _context.DrawRenderers(_cullResults, ref drawingSettings, ref filteringSettings);
        
        _context.DrawSkybox(_camera); // Then skybox
        
        sortingSettings.criteria = SortingCriteria.CommonTransparent; // then transparents
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        _context.DrawRenderers(_cullResults, ref drawingSettings, ref filteringSettings);
    }
    
    private void DrawUnsupportedShaders()
    {
        if (_errorMaterial == null) _errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        
        DrawingSettings drawingSettings = new DrawingSettings(_legacyShaderTagIds[0], new SortingSettings(_camera))
        {
            overrideMaterial = _errorMaterial
        };
        
        for (int i = 1; i < _legacyShaderTagIds.Length; i++) 
        {
            drawingSettings.SetShaderPassName(i, _legacyShaderTagIds[i]);
        }
        
        FilteringSettings filteringSettings = FilteringSettings.defaultValue;
        
        _context.DrawRenderers(_cullResults, ref drawingSettings, ref filteringSettings);
    }
    
    private void Submit()
    {
        _buffer.EndSample(BufferName);
        ExecuteBuffer();
        _context.Submit();
    }

    private void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
    
    bool Cull ()
    {
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters parameters))
        {
            _cullResults = _context.Cull(ref parameters);
            return true;
        }
        return false;
    }
}