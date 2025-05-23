using System;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class BeePerchPoint : MonoBehaviour
{
	// Token: 0x06000685 RID: 1669 RVA: 0x00026033 File Offset: 0x00024233
	public Vector3 GetPoint()
	{
		return base.transform.TransformPoint(this.localPosition);
	}

	// Token: 0x040007EB RID: 2027
	[SerializeField]
	private Vector3 localPosition;
}
