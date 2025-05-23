using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000E1C RID: 3612
	public class CrittersKillVolume : MonoBehaviour
	{
		// Token: 0x06005A75 RID: 23157 RVA: 0x001B8FF8 File Offset: 0x001B71F8
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody)
			{
				CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
				if (component.IsNotNull())
				{
					component.gameObject.SetActive(false);
				}
			}
		}
	}
}
