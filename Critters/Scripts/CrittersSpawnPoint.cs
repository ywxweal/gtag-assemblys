using System;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000E1F RID: 3615
	public class CrittersSpawnPoint : MonoBehaviour
	{
		// Token: 0x06005A7B RID: 23163 RVA: 0x001B90C0 File Offset: 0x001B72C0
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
