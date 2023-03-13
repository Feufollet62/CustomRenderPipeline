using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
	static int _baseColorId = Shader.PropertyToID("_BaseColor");
	[SerializeField] Color baseColor = Color.white;
	static MaterialPropertyBlock block;
	
	private void Awake()
	{
		OnValidate();
	}
	
	private void OnValidate()
	{
		if (block == null) block = new MaterialPropertyBlock();
		block.SetColor(_baseColorId, baseColor);
		
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}