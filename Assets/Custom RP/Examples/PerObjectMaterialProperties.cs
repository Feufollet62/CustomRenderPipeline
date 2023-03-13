using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
	static int _baseColorId = Shader.PropertyToID("_BaseColor");
	static int _cutoffId = Shader.PropertyToID("_Cutoff");
	static MaterialPropertyBlock block;
	
	[SerializeField] Color baseColor = Color.white;
	[SerializeField, Range(0f, 1f)] float cutoff = 0.5f;
	private void Awake()
	{
		OnValidate();
	}
	
	private void OnValidate()
	{
		if (block == null) block = new MaterialPropertyBlock();
		block.SetColor(_baseColorId, baseColor);
		block.SetFloat(_cutoffId, cutoff);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}