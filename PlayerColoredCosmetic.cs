using System;
using UnityEngine;

// Token: 0x02000466 RID: 1126
public class PlayerColoredCosmetic : MonoBehaviour
{
	// Token: 0x06001BB2 RID: 7090 RVA: 0x00088098 File Offset: 0x00086298
	public void Awake()
	{
		for (int i = 0; i < this.coloringRules.Length; i++)
		{
			this.coloringRules[i].Init();
		}
	}

	// Token: 0x06001BB3 RID: 7091 RVA: 0x000880CC File Offset: 0x000862CC
	private void OnEnable()
	{
		if (!this.didInit)
		{
			this.didInit = true;
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && GorillaTagger.Instance != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
		}
		if (this.rig != null)
		{
			this.rig.OnColorChanged += this.UpdateColor;
			this.UpdateColor(this.rig.playerColor);
		}
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x00088155 File Offset: 0x00086355
	private void OnDisable()
	{
		if (this.rig != null)
		{
			this.rig.OnColorChanged -= this.UpdateColor;
		}
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x0008817C File Offset: 0x0008637C
	private void UpdateColor(Color color)
	{
		foreach (PlayerColoredCosmetic.ColoringRule coloringRule in this.coloringRules)
		{
			coloringRule.Apply(color);
		}
	}

	// Token: 0x04001EBE RID: 7870
	private bool didInit;

	// Token: 0x04001EBF RID: 7871
	private VRRig rig;

	// Token: 0x04001EC0 RID: 7872
	[SerializeField]
	private PlayerColoredCosmetic.ColoringRule[] coloringRules;

	// Token: 0x02000467 RID: 1127
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x06001BB7 RID: 7095 RVA: 0x000881B0 File Offset: 0x000863B0
		public void Init()
		{
			this.hashId = new ShaderHashId(this.shaderColorProperty);
			Material[] sharedMaterials = this.meshRenderer.sharedMaterials;
			this.instancedMaterial = new Material(sharedMaterials[this.materialIndex]);
			sharedMaterials[this.materialIndex] = this.instancedMaterial;
			this.meshRenderer.sharedMaterials = sharedMaterials;
		}

		// Token: 0x06001BB8 RID: 7096 RVA: 0x00088207 File Offset: 0x00086407
		public void Apply(Color color)
		{
			this.instancedMaterial.SetColor(this.hashId, color);
		}

		// Token: 0x04001EC1 RID: 7873
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04001EC2 RID: 7874
		private ShaderHashId hashId;

		// Token: 0x04001EC3 RID: 7875
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04001EC4 RID: 7876
		[SerializeField]
		private int materialIndex;

		// Token: 0x04001EC5 RID: 7877
		private Material instancedMaterial;
	}
}
