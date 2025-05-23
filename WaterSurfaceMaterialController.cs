using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	// Token: 0x06000479 RID: 1145 RVA: 0x00019D21 File Offset: 0x00017F21
	protected void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.ApplyProperties();
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00019D40 File Offset: 0x00017F40
	private void ApplyProperties()
	{
		this.matPropBlock.SetVector(WaterSurfaceMaterialController.shaderProp_ScrollSpeedAndScale, new Vector4(this.ScrollX, this.ScrollY, this.Scale, 0f));
		if (this.renderer)
		{
			this.renderer.SetPropertyBlock(this.matPropBlock);
		}
	}

	// Token: 0x04000528 RID: 1320
	public float ScrollX = 0.6f;

	// Token: 0x04000529 RID: 1321
	public float ScrollY = 0.6f;

	// Token: 0x0400052A RID: 1322
	public float Scale = 1f;

	// Token: 0x0400052B RID: 1323
	private Renderer renderer;

	// Token: 0x0400052C RID: 1324
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x0400052D RID: 1325
	private static readonly int shaderProp_ScrollSpeedAndScale = Shader.PropertyToID("_ScrollSpeedAndScale");
}
