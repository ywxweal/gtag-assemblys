using System;
using UnityEngine;

// Token: 0x020009E4 RID: 2532
public class SpawnManager : MonoBehaviour
{
	// Token: 0x06003C90 RID: 15504 RVA: 0x00121096 File Offset: 0x0011F296
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
