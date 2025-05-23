using System;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class GorillaTriggerColliderHandIndicator : MonoBehaviour
{
	// Token: 0x06002C1E RID: 11294 RVA: 0x000D9834 File Offset: 0x000D7A34
	private void Update()
	{
		this.currentVelocity = (base.transform.position - this.lastPosition) / Time.deltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x000D986D File Offset: 0x000D7A6D
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x04003250 RID: 12880
	public Vector3 currentVelocity;

	// Token: 0x04003251 RID: 12881
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04003252 RID: 12882
	public bool isLeftHand;

	// Token: 0x04003253 RID: 12883
	public GorillaThrowableController throwableController;
}
