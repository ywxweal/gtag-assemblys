using System;
using System.Collections.Generic;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x020009A0 RID: 2464
[RequireComponent(typeof(MeshRenderer))]
public class MaterialUVOffsetListSetter : MonoBehaviour, IBuildValidation
{
	// Token: 0x06003B07 RID: 15111 RVA: 0x00119C3C File Offset: 0x00117E3C
	private void Awake()
	{
		this.matPropertyBlock = new MaterialPropertyBlock();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.GetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06003B08 RID: 15112 RVA: 0x00119C68 File Offset: 0x00117E68
	public void SetUVOffset(int listIndex)
	{
		if (listIndex >= this.uvOffsetList.Count || listIndex < 0)
		{
			Debug.LogError("Invalid uv offset list index provided.");
			return;
		}
		if (this.matPropertyBlock == null || this.meshRenderer == null)
		{
			Debug.LogError("MaterialUVOffsetListSetter settings are incorrect somehow, please fix", base.gameObject);
			this.Awake();
			return;
		}
		Vector2 vector = this.uvOffsetList[listIndex];
		this.matPropertyBlock.SetVector(this.shaderPropertyID, new Vector4(1f, 1f, vector.x, vector.y));
		this.meshRenderer.SetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06003B09 RID: 15113 RVA: 0x00119D0C File Offset: 0x00117F0C
	public bool BuildValidationCheck()
	{
		if (base.GetComponent<MeshRenderer>() == null)
		{
			Debug.LogError("missing a mesh renderer for the materialuvoffsetlistsetter", base.gameObject);
			return false;
		}
		if (base.GetComponentInParent<EdMeshCombinerMono>() != null && base.GetComponentInParent<EdDoNotMeshCombine>() == null)
		{
			Debug.LogError("the meshrenderer is going to getcombined, that will likely cause issues for the materialuvoffsetlistsetter", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x04003FC5 RID: 16325
	[SerializeField]
	private List<Vector2> uvOffsetList = new List<Vector2>();

	// Token: 0x04003FC6 RID: 16326
	private MeshRenderer meshRenderer;

	// Token: 0x04003FC7 RID: 16327
	private MaterialPropertyBlock matPropertyBlock;

	// Token: 0x04003FC8 RID: 16328
	private int shaderPropertyID = Shader.PropertyToID("_BaseMap_ST");
}
