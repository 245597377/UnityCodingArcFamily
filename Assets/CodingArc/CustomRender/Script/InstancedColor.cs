using UnityEngine;

public class InstancedColor : MonoBehaviour {

	static MaterialPropertyBlock propertyBlock;

	static int colorID = Shader.PropertyToID("_Color");

	[SerializeField]
	public Color color = Color.white;

	void Awake () {
		OnValidate();
	}

	void OnValidate () {
        Refresh();
    }

    public void Refresh()
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
        propertyBlock.SetColor(colorID, color);
        GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
}