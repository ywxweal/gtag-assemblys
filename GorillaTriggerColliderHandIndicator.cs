using System;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class GorillaTriggerColliderHandIndicator : MonoBehaviour
{
	// Token: 0x06002C1F RID: 11295 RVA: 0x000D98D8 File Offset: 0x000D7AD8
	private void Update()
	{
		this.currentVelocity = (base.transform.position - this.lastPosition) / Time.deltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x000D9911 File Offset: 0x000D7B11
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x04003252 RID: 12882
	public Vector3 currentVelocity;

	// Token: 0x04003253 RID: 12883
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04003254 RID: 12884
	public bool isLeftHand;

	// Token: 0x04003255 RID: 12885
	public GorillaThrowableController throwableController;
}
