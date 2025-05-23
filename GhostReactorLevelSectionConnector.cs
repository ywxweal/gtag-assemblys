using System;
using UnityEngine;

// Token: 0x02000584 RID: 1412
public class GhostReactorLevelSectionConnector : MonoBehaviour
{
	// Token: 0x0600227C RID: 8828 RVA: 0x000ACC6B File Offset: 0x000AAE6B
	public void Init()
	{
		GameEntityManager.instance.RequestCreateItem(this.gateEntity.name.GetStaticHash(), this.gateSpawnPoint.position, this.gateSpawnPoint.rotation, 0L);
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000023F4 File Offset: 0x000005F4
	public void DeInit()
	{
	}

	// Token: 0x040026A4 RID: 9892
	public Transform hubAnchor;

	// Token: 0x040026A5 RID: 9893
	public Transform sectionAnchor;

	// Token: 0x040026A6 RID: 9894
	public Transform gateSpawnPoint;

	// Token: 0x040026A7 RID: 9895
	public GameEntity gateEntity;

	// Token: 0x040026A8 RID: 9896
	public GhostReactorLevelSectionConnector.Direction direction;

	// Token: 0x02000585 RID: 1413
	public enum Direction
	{
		// Token: 0x040026AA RID: 9898
		Down = -1,
		// Token: 0x040026AB RID: 9899
		Forward,
		// Token: 0x040026AC RID: 9900
		Up
	}
}
