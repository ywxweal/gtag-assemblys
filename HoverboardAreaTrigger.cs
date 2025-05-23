using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000669 RID: 1641
public class HoverboardAreaTrigger : MonoBehaviour
{
	// Token: 0x06002907 RID: 10503 RVA: 0x000CC707 File Offset: 0x000CA907
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x000CC727 File Offset: 0x000CA927
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(false, false);
		}
	}
}
