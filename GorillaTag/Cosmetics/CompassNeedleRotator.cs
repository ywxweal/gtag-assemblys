using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DB0 RID: 3504
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x060056CD RID: 22221 RVA: 0x001A748A File Offset: 0x001A568A
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x060056CE RID: 22222 RVA: 0x001A74A8 File Offset: 0x001A56A8
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float num = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, num, Space.World);
		}

		// Token: 0x04005AC9 RID: 23241
		private const float smoothTime = 0.005f;

		// Token: 0x04005ACA RID: 23242
		private float currentVelocity;
	}
}
