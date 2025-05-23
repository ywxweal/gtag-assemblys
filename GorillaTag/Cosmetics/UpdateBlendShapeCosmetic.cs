using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DFC RID: 3580
	public class UpdateBlendShapeCosmetic : MonoBehaviour
	{
		// Token: 0x06005899 RID: 22681 RVA: 0x001B3F13 File Offset: 0x001B2113
		private void Awake()
		{
			this.targetWeight = this.blendStartWeight;
			this.currentWeight = 0f;
		}

		// Token: 0x0600589A RID: 22682 RVA: 0x001B3F2C File Offset: 0x001B212C
		private void Update()
		{
			this.currentWeight = Mathf.Lerp(this.currentWeight, this.targetWeight, Time.deltaTime * this.blendSpeed);
			this.skinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeIndex, this.currentWeight);
		}

		// Token: 0x0600589B RID: 22683 RVA: 0x001B3F68 File Offset: 0x001B2168
		public void SetBlendValue(bool leftHand, float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x0600589C RID: 22684 RVA: 0x001B3F8E File Offset: 0x001B218E
		public void SetBlendValue(float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x0600589D RID: 22685 RVA: 0x001B3FB4 File Offset: 0x001B21B4
		public void FullyBlend()
		{
			this.targetWeight = this.maxBlendShapeWeight;
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x001B3FC2 File Offset: 0x001B21C2
		public void ResetBlend()
		{
			this.targetWeight = 0f;
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x001B3FCF File Offset: 0x001B21CF
		public float GetBlendValue()
		{
			return this.skinnedMeshRenderer.GetBlendShapeWeight(this.blendShapeIndex);
		}

		// Token: 0x04005DF5 RID: 24053
		[SerializeField]
		private SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04005DF6 RID: 24054
		public float maxBlendShapeWeight = 100f;

		// Token: 0x04005DF7 RID: 24055
		[SerializeField]
		private int blendShapeIndex;

		// Token: 0x04005DF8 RID: 24056
		[SerializeField]
		private float blendSpeed = 10f;

		// Token: 0x04005DF9 RID: 24057
		[SerializeField]
		private float blendStartWeight;

		// Token: 0x04005DFA RID: 24058
		[SerializeField]
		private bool invertPassedBlend;

		// Token: 0x04005DFB RID: 24059
		[Tooltip("If enabled, inverts the passed blend value (0 becomes 1, .2 becomes .8, .65 becomes .45, etc)")]
		private float targetWeight;

		// Token: 0x04005DFC RID: 24060
		private float currentWeight;
	}
}
