using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D8B RID: 3467
	public class PlanarSound : MonoBehaviour
	{
		// Token: 0x06005619 RID: 22041 RVA: 0x001A31E6 File Offset: 0x001A13E6
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		// Token: 0x0600561A RID: 22042 RVA: 0x001A320C File Offset: 0x001A140C
		protected void LateUpdate()
		{
			if (!this.hasCamera)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 vector = transform.parent.InverseTransformPoint(this.cameraXform.position);
			vector.y = 0f;
			if (this.limitDistance && vector.sqrMagnitude > this.maxDistance * this.maxDistance)
			{
				vector = vector.normalized * this.maxDistance;
			}
			transform.localPosition = vector;
		}

		// Token: 0x040059E9 RID: 23017
		private Transform cameraXform;

		// Token: 0x040059EA RID: 23018
		private bool hasCamera;

		// Token: 0x040059EB RID: 23019
		[SerializeField]
		private bool limitDistance;

		// Token: 0x040059EC RID: 23020
		[SerializeField]
		private float maxDistance = 1f;
	}
}
