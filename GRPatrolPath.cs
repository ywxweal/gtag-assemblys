using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B6 RID: 1462
public class GRPatrolPath : MonoBehaviour
{
	// Token: 0x060023A4 RID: 9124 RVA: 0x000B3598 File Offset: 0x000B1798
	private void Awake()
	{
		this.patrolNodes = new List<Transform>(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.patrolNodes.Add(base.transform.GetChild(i));
		}
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000B35E8 File Offset: 0x000B17E8
	public void OnDrawGizmosSelected()
	{
		if (this.patrolNodes == null || base.transform.childCount != this.patrolNodes.Count)
		{
			this.patrolNodes = new List<Transform>(base.transform.childCount);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				this.patrolNodes.Add(base.transform.GetChild(i));
			}
		}
		if (this.patrolNodes != null)
		{
			for (int j = 0; j < this.patrolNodes.Count; j++)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(this.patrolNodes[j].transform.position, Vector3.one * 0.5f);
				if (j < this.patrolNodes.Count - 1)
				{
					Gizmos.DrawLine(this.patrolNodes[j].transform.position, this.patrolNodes[j + 1].transform.position);
				}
			}
		}
	}

	// Token: 0x04002863 RID: 10339
	[NonSerialized]
	public List<Transform> patrolNodes;

	// Token: 0x04002864 RID: 10340
	public int index;
}
