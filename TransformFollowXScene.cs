using System;
using UnityEngine;

// Token: 0x020006C7 RID: 1735
public class TransformFollowXScene : MonoBehaviour
{
	// Token: 0x06002B3D RID: 11069 RVA: 0x000D4971 File Offset: 0x000D2B71
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000D4984 File Offset: 0x000D2B84
	private void Start()
	{
		this.refToFollow.TryResolve<Transform>(out this.transformToFollow);
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x000D4998 File Offset: 0x000D2B98
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x0400304A RID: 12362
	public XSceneRef refToFollow;

	// Token: 0x0400304B RID: 12363
	private Transform transformToFollow;

	// Token: 0x0400304C RID: 12364
	public Vector3 offset;

	// Token: 0x0400304D RID: 12365
	public Vector3 prevPos;
}
