using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparent : MonoBehaviour {

	[SerializeField] private bool disable = false;
	[SerializeField] private Material transparentMaterial;
	private List<Material> originalMaterial = new List<Material>();
	private MeshRenderer[] meshRenderer;

	private void Start()
	{
		gameObject.layer = 13;
		meshRenderer = GetComponentsInChildren<MeshRenderer>();
	
		for(int i = 0; i < meshRenderer.Length; i++)
		{
			originalMaterial.Add(meshRenderer[i].material);
		}
	}

	public void MakeObjectTransparent()
	{
		if (disable)
		{
			for (int i = 0; i < meshRenderer.Length; i++)
			{
				meshRenderer[i].enabled = false;
			}
		}
		else
		{
			for (int i = 0; i < meshRenderer.Length; i++)
			{
				meshRenderer[i].material = transparentMaterial;
			}
		}
	}

	public void RevertObjectToOriginalMaterial()
	{
		if (disable)
		{
			for (int i = 0; i < meshRenderer.Length; i++)
			{
				meshRenderer[i].enabled = true;
			}
		}
		else
		{
			for (int i = 0; i < meshRenderer.Length; i++)
			{
				meshRenderer[i].material = originalMaterial[i];
			}
		}
	}
}
