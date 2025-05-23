using System;
using UnityEngine;

// Token: 0x0200064E RID: 1614
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x06002852 RID: 10322 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x06002853 RID: 10323 RVA: 0x000C9190 File Offset: 0x000C7390
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04002D37 RID: 11575
	public Transform transformToFollow;

	// Token: 0x04002D38 RID: 11576
	public Vector3 offset;

	// Token: 0x04002D39 RID: 11577
	public bool doesMove;
}
