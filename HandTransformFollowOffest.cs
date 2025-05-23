using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000528 RID: 1320
[Serializable]
internal class HandTransformFollowOffest
{
	// Token: 0x06001FF3 RID: 8179 RVA: 0x000A114C File Offset: 0x0009F34C
	internal void UpdatePositionRotation()
	{
		if (this.followTransform == null || this.targetTransforms == null)
		{
			return;
		}
		this.position = this.followTransform.position + this.followTransform.rotation * this.positionOffset * GTPlayer.Instance.scale;
		this.rotation = this.followTransform.rotation * this.rotationOffset;
		foreach (Transform transform in this.targetTransforms)
		{
			transform.position = this.position;
			transform.rotation = this.rotation;
		}
	}

	// Token: 0x040023E8 RID: 9192
	internal Transform followTransform;

	// Token: 0x040023E9 RID: 9193
	[SerializeField]
	private Transform[] targetTransforms;

	// Token: 0x040023EA RID: 9194
	[SerializeField]
	internal Vector3 positionOffset;

	// Token: 0x040023EB RID: 9195
	[SerializeField]
	internal Quaternion rotationOffset;

	// Token: 0x040023EC RID: 9196
	private Vector3 position;

	// Token: 0x040023ED RID: 9197
	private Quaternion rotation;
}
