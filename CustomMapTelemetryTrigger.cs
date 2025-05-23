using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200070F RID: 1807
public class CustomMapTelemetryTrigger : MonoBehaviour
{
	// Token: 0x06002D16 RID: 11542 RVA: 0x000DED6F File Offset: 0x000DCF6F
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x000DED8F File Offset: 0x000DCF8F
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.StartMapTracking();
		}
	}
}
