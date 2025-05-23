using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class SkeletonPathingNode : MonoBehaviour
{
	// Token: 0x06000566 RID: 1382 RVA: 0x0001F6FF File Offset: 0x0001D8FF
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000649 RID: 1609
	public bool ejectionPoint;

	// Token: 0x0400064A RID: 1610
	public SkeletonPathingNode[] connectedNodes;

	// Token: 0x0400064B RID: 1611
	public float distanceToExitNode;
}
