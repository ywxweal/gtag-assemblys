using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DAF RID: 3503
	[ExecuteAlways]
	public class ScubaWatchWearable : MonoBehaviour
	{
		// Token: 0x060056CA RID: 22218 RVA: 0x001A725C File Offset: 0x001A545C
		protected void Update()
		{
			GTPlayer instance = GTPlayer.Instance;
			if (this.onLeftHand)
			{
				if (instance.LeftHandWaterVolume != null)
				{
					this.currentDepth = Mathf.Max(-instance.LeftHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastLeftHandPosition), 0f);
				}
				else
				{
					this.currentDepth = 0f;
				}
			}
			else if (instance.RightHandWaterVolume != null)
			{
				this.currentDepth = Mathf.Max(-instance.RightHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastRightHandPosition), 0f);
			}
			else
			{
				this.currentDepth = 0f;
			}
			float num = (this.currentDepth - this.depthRange.x) / (this.depthRange.y - this.depthRange.x);
			float num2 = Mathf.Lerp(this.dialRotationRange.x, this.dialRotationRange.y, num);
			this.dialNeedle.localRotation = this.initialDialRotation * Quaternion.AngleAxis(num2, this.dialRotationAxis);
		}

		// Token: 0x04005AC1 RID: 23233
		public bool onLeftHand;

		// Token: 0x04005AC2 RID: 23234
		[Tooltip("The transform that will be rotated to indicate the current depth.")]
		public Transform dialNeedle;

		// Token: 0x04005AC3 RID: 23235
		[Tooltip("If your rotation is not zeroed out then click the Auto button to use the current rotation as 0.")]
		public Quaternion initialDialRotation;

		// Token: 0x04005AC4 RID: 23236
		[Tooltip("The range of depth values that the dial will rotate between.")]
		public Vector2 depthRange = new Vector2(0f, 20f);

		// Token: 0x04005AC5 RID: 23237
		[Tooltip("The range of rotation values that the dial will rotate between.")]
		public Vector2 dialRotationRange = new Vector2(0f, 360f);

		// Token: 0x04005AC6 RID: 23238
		[Tooltip("The axis that the dial will rotate around.")]
		public Vector3 dialRotationAxis = Vector3.right;

		// Token: 0x04005AC7 RID: 23239
		[Tooltip("The current depth of the player.")]
		[DebugOption]
		private float currentDepth;
	}
}
