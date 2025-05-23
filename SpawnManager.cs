using System;
using UnityEngine;

// Token: 0x020009E4 RID: 2532
public class SpawnManager : MonoBehaviour
{
	// Token: 0x06003C91 RID: 15505 RVA: 0x0012116E File Offset: 0x0011F36E
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
