using System;
using UnityEngine;

// Token: 0x02000494 RID: 1172
public class GorillaHandHistory : MonoBehaviour
{
	// Token: 0x06001C96 RID: 7318 RVA: 0x0008B6F4 File Offset: 0x000898F4
	private void Start()
	{
		this.direction = default(Vector3);
		this.lastPosition = default(Vector3);
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x0008B70E File Offset: 0x0008990E
	private void FixedUpdate()
	{
		this.direction = this.lastPosition - base.transform.position;
		this.lastLastPosition = this.lastPosition;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x04001FA9 RID: 8105
	public Vector3 direction;

	// Token: 0x04001FAA RID: 8106
	private Vector3 lastPosition;

	// Token: 0x04001FAB RID: 8107
	private Vector3 lastLastPosition;
}
