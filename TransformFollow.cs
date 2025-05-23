using System;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
public class TransformFollow : MonoBehaviour
{
	// Token: 0x06002B39 RID: 11065 RVA: 0x000D4853 File Offset: 0x000D2A53
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x000D4868 File Offset: 0x000D2A68
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x04003045 RID: 12357
	public Transform transformToFollow;

	// Token: 0x04003046 RID: 12358
	public Vector3 offset;

	// Token: 0x04003047 RID: 12359
	public Vector3 prevPos;
}
