using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    
    
    #if UNITY_EDITOR
    
    partial void DrawUnsupportedShaders ();
    
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
    
    partial void DrawUnsupportedShaders()
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
    
    #endif
}