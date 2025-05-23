using System;
using UnityEngine;

// Token: 0x020006C7 RID: 1735
public class TransformFollowXScene : MonoBehaviour
{
	// Token: 0x06002B3C RID: 11068 RVA: 0x000D48CD File Offset: 0x000D2ACD
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x000D48E0 File Offset: 0x000D2AE0
	private void Start()
	{
		this.refToFollow.TryResolve<Transform>(out this.transformToFollow);
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000D48F4 File Offset: 0x000D2AF4
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x04003048 RID: 12360
	public XSceneRef refToFollow;

	// Token: 0x04003049 RID: 12361
	private Transform transformToFollow;

	// Token: 0x0400304A RID: 12362
	public Vector3 offset;

	// Token: 0x0400304B RID: 12363
	public Vector3 prevPos;
}
