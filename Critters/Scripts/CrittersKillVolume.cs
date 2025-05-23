using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02000E1C RID: 3612
	public class CrittersKillVolume : MonoBehaviour
	{
		// Token: 0x06005A76 RID: 23158 RVA: 0x001B90D0 File Offset: 0x001B72D0
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
