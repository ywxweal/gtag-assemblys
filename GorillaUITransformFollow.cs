using System;
using UnityEngine;

// Token: 0x0200064E RID: 1614
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x06002851 RID: 10321 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000C90EC File Offset: 0x000C72EC
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04002D35 RID: 11573
	public Transform transformToFollow;

	// Token: 0x04002D36 RID: 11574
	public Vector3 offset;

	// Token: 0x04002D37 RID: 11575
	public bool doesMove;
}
