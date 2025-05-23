using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000D9E RID: 3486
	public sealed class ZoneLiquidEffectable : MonoBehaviour
	{
		// Token: 0x06005675 RID: 22133 RVA: 0x001A503F File Offset: 0x001A323F
		private void Awake()
		{
			this.childRenderers = base.GetComponentsInChildren<Renderer>(false);
		}

		// Token: 0x06005676 RID: 22134 RVA: 0x000023F4 File Offset: 0x000005F4
		private void OnEnable()
		{
		}

		// Token: 0x06005677 RID: 22135 RVA: 0x000023F4 File Offset: 0x000005F4
		private void OnDisable()
		{
		}

		// Token: 0x04005A3B RID: 23099
		public float radius = 1f;

		// Token: 0x04005A3C RID: 23100
		[NonSerialized]
		public bool inLiquidVolume;

		// Token: 0x04005A3D RID: 23101
		[NonSerialized]
		public bool wasInLiquidVolume;

		// Token: 0x04005A3E RID: 23102
		[NonSerialized]
		public Renderer[] childRenderers;
	}
}
