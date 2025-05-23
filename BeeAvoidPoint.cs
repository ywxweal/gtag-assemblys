using System;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class BeeAvoidPoint : MonoBehaviour
{
	// Token: 0x06000682 RID: 1666 RVA: 0x00026003 File Offset: 0x00024203
	private void Start()
	{
		BeeSwarmManager.RegisterAvoidPoint(base.gameObject);
		FlockingManager.RegisterAvoidPoint(base.gameObject);
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0002601B File Offset: 0x0002421B
	private void OnDestroy()
	{
		BeeSwarmManager.UnregisterAvoidPoint(base.gameObject);
		FlockingManager.UnregisterAvoidPoint(base.gameObject);
	}
}
