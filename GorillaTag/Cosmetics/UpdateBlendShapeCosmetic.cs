using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DFC RID: 3580
	public class UpdateBlendShapeCosmetic : MonoBehaviour
	{
		// Token: 0x0600589A RID: 22682 RVA: 0x001B3FEB File Offset: 0x001B21EB
		private void Awake()
		{
			this.targetWeight = this.blendStartWeight;
			this.currentWeight = 0f;
		}

		// Token: 0x0600589B RID: 22683 RVA: 0x001B4004 File Offset: 0x001B2204
		private void Update()
		{
			this.currentWeight = Mathf.Lerp(this.currentWeight, this.targetWeight, Time.deltaTime * this.blendSpeed);
			this.skinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeIndex, this.currentWeight);
		}

		// Token: 0x0600589C RID: 22684 RVA: 0x001B4040 File Offset: 0x001B2240
		public void SetBlendValue(bool leftHand, float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x0600589D RID: 22685 RVA: 0x001B4066 File Offset: 0x001B2266
		public void SetBlendValue(float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x001B408C File Offset: 0x001B228C
		public void FullyBlend()
		{
			this.targetWeight = this.maxBlendShapeWeight;
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x001B409A File Offset: 0x001B229A
		public void ResetBlend()
		{
			this.targetWeight = 0f;
		}

		// Token: 0x060058A0 RID: 22688 RVA: 0x001B40A7 File Offset: 0x001B22A7
		public float GetBlendValue()
		{
			return this.skinnedMeshRenderer.GetBlendShapeWeight(this.blendShapeIndex);
		}

		// Token: 0x04005DF6 RID: 24054
		[SerializeField]
		private SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04005DF7 RID: 24055
		public float maxBlendShapeWeight = 100f;

		// Token: 0x04005DF8 RID: 24056
		[SerializeField]
		private int blendShapeIndex;

		// Token: 0x04005DF9 RID: 24057
		[SerializeField]
		private float blendSpeed = 10f;

		// Token: 0x04005DFA RID: 24058
		[SerializeField]
		private float blendStartWeight;

		// Token: 0x04005DFB RID: 24059
		[SerializeField]
		private bool invertPassedBlend;

		// Token: 0x04005DFC RID: 24060
		[Tooltip("If enabled, inverts the passed blend value (0 becomes 1, .2 becomes .8, .65 becomes .45, etc)")]
		private float targetWeight;

		// Token: 0x04005DFD RID: 24061
		private float currentWeight;
	}
}
