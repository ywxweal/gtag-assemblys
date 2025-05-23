using System;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000E1F RID: 3615
	public class CrittersSpawnPoint : MonoBehaviour
	{
		// Token: 0x06005A7C RID: 23164 RVA: 0x001B9198 File Offset: 0x001B7398
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
