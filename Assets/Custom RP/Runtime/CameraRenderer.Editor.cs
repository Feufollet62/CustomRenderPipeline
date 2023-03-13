using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    partial void DrawGizmos();
    partial void DrawUnsupportedShaders();
    partial void PrepareForSceneWindow ();
    partial void PrepareBuffer ();
    
    #if UNITY_EDITOR
    
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
    
    string SampleName {get; set;}
    
    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }
    
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
    
    partial void PrepareForSceneWindow ()
    {
        if (_camera.cameraType == CameraType.SceneView) ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
    }
    
    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        _buffer.name = SampleName = _camera.name;
        Profiler.EndSample();
    }
    
    #else

	const string SampleName = bufferName;
    
    #endif
}