using System;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class PlayerGameEventLocationTrigger : MonoBehaviour
{
	// Token: 0x060007FE RID: 2046 RVA: 0x0002C724 File Offset: 0x0002A924
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			PlayerGameEvents.TriggerEnterLocation(this.locationName);
		}
	}

	// Token: 0x0400098B RID: 2443
	[SerializeField]
	private string locationName;
}
