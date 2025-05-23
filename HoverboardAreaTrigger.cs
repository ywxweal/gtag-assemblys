using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000669 RID: 1641
public class HoverboardAreaTrigger : MonoBehaviour
{
	// Token: 0x06002908 RID: 10504 RVA: 0x000CC7AB File Offset: 0x000CA9AB
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x000CC7CB File Offset: 0x000CA9CB
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(false, false);
		}
	}
}
