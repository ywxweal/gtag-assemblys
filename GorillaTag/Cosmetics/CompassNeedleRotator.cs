using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DB0 RID: 3504
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x060056CC RID: 22220 RVA: 0x001A73B2 File Offset: 0x001A55B2
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x060056CD RID: 22221 RVA: 0x001A73D0 File Offset: 0x001A55D0
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float num = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, num, Space.World);
		}

		// Token: 0x04005AC8 RID: 23240
		private const float smoothTime = 0.005f;

		// Token: 0x04005AC9 RID: 23241
		private float currentVelocity;
	}
}
