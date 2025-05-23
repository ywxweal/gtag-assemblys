using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200070F RID: 1807
public class CustomMapTelemetryTrigger : MonoBehaviour
{
	// Token: 0x06002D17 RID: 11543 RVA: 0x000DEE13 File Offset: 0x000DD013
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x000DEE33 File Offset: 0x000DD033
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapTelemetry.IsActive)
		{
			CustomMapTelemetry.StartMapTracking();
		}
	}
}
