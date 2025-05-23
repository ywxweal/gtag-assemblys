using System;
using UnityEngine;

// Token: 0x020005B7 RID: 1463
public class GRPatrolPathNode : MonoBehaviour
{
	// Token: 0x060023A7 RID: 9127 RVA: 0x000B36F4 File Offset: 0x000B18F4
	public void OnDrawGizmosSelected()
	{
		if (base.transform.parent == null)
		{
			return;
		}
		GRPatrolPath component = base.transform.parent.GetComponent<GRPatrolPath>();
		if (component == null)
		{
			return;
		}
		component.OnDrawGizmosSelected();
	}
}
