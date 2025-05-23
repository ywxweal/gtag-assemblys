using System;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
public class TransformFollow : MonoBehaviour
{
	// Token: 0x06002B3A RID: 11066 RVA: 0x000D48F7 File Offset: 0x000D2AF7
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x000D490C File Offset: 0x000D2B0C
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x04003047 RID: 12359
	public Transform transformToFollow;

	// Token: 0x04003048 RID: 12360
	public Vector3 offset;

	// Token: 0x04003049 RID: 12361
	public Vector3 prevPos;
}
