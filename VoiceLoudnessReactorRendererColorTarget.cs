using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A2C RID: 2604
[Serializable]
public class VoiceLoudnessReactorRendererColorTarget
{
	// Token: 0x06003DF4 RID: 15860 RVA: 0x00126A2C File Offset: 0x00124C2C
	public void Inititialize()
	{
		if (this._materials == null)
		{
			this._materials = new List<Material>(this.renderer.materials);
			this._materials[this.materialIndex].EnableKeyword(this.colorProperty);
			this.renderer.SetMaterials(this._materials);
			this.UpdateMaterialColor(0f);
		}
	}

	// Token: 0x06003DF5 RID: 15861 RVA: 0x00126A90 File Offset: 0x00124C90
	public void UpdateMaterialColor(float level)
	{
		Color color = this.gradient.Evaluate(level);
		if (this._lastColor == color)
		{
			return;
		}
		this._materials[this.materialIndex].SetColor(this.colorProperty, color);
		this._lastColor = color;
	}

	// Token: 0x0400426D RID: 17005
	[SerializeField]
	private string colorProperty = "_BaseColor";

	// Token: 0x0400426E RID: 17006
	public Renderer renderer;

	// Token: 0x0400426F RID: 17007
	public int materialIndex;

	// Token: 0x04004270 RID: 17008
	public Gradient gradient;

	// Token: 0x04004271 RID: 17009
	public bool useSmoothedLoudness;

	// Token: 0x04004272 RID: 17010
	public float scale = 1f;

	// Token: 0x04004273 RID: 17011
	private List<Material> _materials;

	// Token: 0x04004274 RID: 17012
	private Color _lastColor = Color.white;
}
