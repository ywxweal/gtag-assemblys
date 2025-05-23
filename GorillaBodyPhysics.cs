using System;
using UnityEngine;

// Token: 0x02000490 RID: 1168
public class GorillaBodyPhysics : MonoBehaviour
{
	// Token: 0x06001C8B RID: 7307 RVA: 0x0008B4DC File Offset: 0x000896DC
	private void FixedUpdate()
	{
		this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
	}

	// Token: 0x04001F97 RID: 8087
	public GameObject bodyCollider;

	// Token: 0x04001F98 RID: 8088
	public Vector3 bodyColliderOffset;

	// Token: 0x04001F99 RID: 8089
	public Transform headsetTransform;
}
