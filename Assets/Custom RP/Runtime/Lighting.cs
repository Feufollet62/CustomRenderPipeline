using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private const string bufferName = "Lighting";

    private CommandBuffer buffer = new CommandBuffer {name = bufferName};
    private static int
        _dirLightColorId = Shader.PropertyToID("_DirectionalLightColor"),
        _dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
    
    public void Setup(ScriptableRenderContext context)
    {
        buffer.BeginSample(bufferName);
        SetupDirectionalLight();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    private void SetupDirectionalLight()
    {
        Light light = RenderSettings.sun;
        buffer.SetGlobalVector(_dirLightColorId, light.color.linear * light.intensity);
        buffer.SetGlobalVector(_dirLightDirectionId, -light.transform.forward);
    }
}