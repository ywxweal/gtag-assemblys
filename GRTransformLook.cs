using System;
using UnityEngine;

// Token: 0x020005CC RID: 1484
public class GRTransformLook : MonoBehaviour
{
	// Token: 0x06002433 RID: 9267 RVA: 0x000B63CD File Offset: 0x000B45CD
	private void Awake()
	{
		if (this.followPlayer)
		{
			this.lookTarget = Camera.main.transform;
		}
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x000B63E7 File Offset: 0x000B45E7
	private void LateUpdate()
	{
		base.transform.LookAt(this.lookTarget);
		base.transform.rotation *= Quaternion.Euler(this.offsetRotation);
	}

	// Token: 0x0400294A RID: 10570
	public bool followPlayer;

	// Token: 0x0400294B RID: 10571
	public Transform lookTarget;

	// Token: 0x0400294C RID: 10572
	public Vector3 offsetRotation;
}
