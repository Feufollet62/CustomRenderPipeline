using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private ScriptableRenderContext _context;
    private Camera _camera;

    private const string BufferName = "Render Camera";
    private readonly CommandBuffer _buffer = new CommandBuffer {name = BufferName};

    private CullingResults _cullResults;
    
    private static readonly ShaderTagId 
        UnlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"), 
        LitShaderTagId = new ShaderTagId("CustomLit");
    
    public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
    {
        _context = context;
        _camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();
        
        if (!Cull()) return;
        
        Setup();
        DrawUnsupportedShaders();
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        DrawGizmos();
        Submit();
    }

    private void Setup()
    {
        _context.SetupCameraProperties(_camera);
        
        CameraClearFlags flags = _camera.clearFlags;
        
        _buffer.ClearRenderTarget
        (
            flags <= CameraClearFlags.Depth, 
            flags == CameraClearFlags.Color, 
            flags == CameraClearFlags.Color ? _camera.backgroundColor.linear : Color.clear
        );
        
        _buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }

    private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        SortingSettings sortingSettings = new SortingSettings(_camera) {criteria = SortingCriteria.CommonOpaque};

        DrawingSettings drawingSettings = new DrawingSettings(UnlitShaderTagId, sortingSettings)
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        };
        drawingSettings.SetShaderPassName(1, LitShaderTagId);
        
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque); // Draw opaques first
        
        _context.DrawRenderers(_cullResults, ref drawingSettings, ref filteringSettings);
        
        _context.DrawSkybox(_camera); // Then skybox
        
        sortingSettings.criteria = SortingCriteria.CommonTransparent; // then transparents
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        _context.DrawRenderers(_cullResults, ref drawingSettings, ref filteringSettings);
    }
    
    private void Submit()
    {
        _buffer.EndSample(SampleName);
        ExecuteBuffer();
        _context.Submit();
    }

    private void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
    
    bool Cull()
    {
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters parameters))
        {
            _cullResults = _context.Cull(ref parameters);
            return true;
        }
        return false;
    }
}