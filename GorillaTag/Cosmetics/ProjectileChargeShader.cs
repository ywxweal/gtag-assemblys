using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DF1 RID: 3569
	public class ProjectileChargeShader : MonoBehaviour
	{
		// Token: 0x0600585C RID: 22620 RVA: 0x001B2CB8 File Offset: 0x001B0EB8
		private void Awake()
		{
			this.renderer = base.GetComponentInChildren<Renderer>();
			this.chargerMPB = new MaterialPropertyBlock();
		}

		// Token: 0x0600585D RID: 22621 RVA: 0x001B2CD4 File Offset: 0x001B0ED4
		public void UpdateChargeProgress(float value)
		{
			if (this.chargerMPB == null)
			{
				this.chargerMPB = new MaterialPropertyBlock();
			}
			if (this.renderer)
			{
				this.renderer.GetPropertyBlock(this.chargerMPB, 1);
				this.chargerMPB.SetVector(ProjectileChargeShader.UvShiftOffset, new Vector2(value, 0f));
				this.renderer.SetPropertyBlock(this.chargerMPB, 1);
			}
		}

		// Token: 0x04005D90 RID: 23952
		private Renderer renderer;

		// Token: 0x04005D91 RID: 23953
		private MaterialPropertyBlock chargerMPB;

		// Token: 0x04005D92 RID: 23954
		private static readonly int UvShiftOffset = Shader.PropertyToID("_UvShiftOffset");

		// Token: 0x04005D93 RID: 23955
		public int shaderAnimSteps = 4;
	}
}
