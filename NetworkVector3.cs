using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009A4 RID: 2468
internal class NetworkVector3
{
	// Token: 0x170005D8 RID: 1496
	// (get) Token: 0x06003B2E RID: 15150 RVA: 0x0011AED6 File Offset: 0x001190D6
	public Vector3 CurrentSyncTarget
	{
		get
		{
			return this._currentSyncTarget;
		}
	}

	// Token: 0x06003B2F RID: 15151 RVA: 0x0011AEE0 File Offset: 0x001190E0
	public void SetNewSyncTarget(Vector3 newTarget)
	{
		Vector3 currentSyncTarget = this.CurrentSyncTarget;
		(ref currentSyncTarget).SetValueSafe(in newTarget);
		this.distanceTraveled = currentSyncTarget - this._currentSyncTarget;
		this._currentSyncTarget = currentSyncTarget;
		this.lastSetNetTime = PhotonNetwork.Time;
	}

	// Token: 0x06003B30 RID: 15152 RVA: 0x0011AF24 File Offset: 0x00119124
	public Vector3 GetPredictedFuture()
	{
		float num = (float)(PhotonNetwork.Time - this.lastSetNetTime) * (float)PhotonNetwork.SerializationRate;
		Vector3 vector = this.distanceTraveled * num;
		return this._currentSyncTarget + vector;
	}

	// Token: 0x06003B31 RID: 15153 RVA: 0x0011AF5F File Offset: 0x0011915F
	public void Reset()
	{
		this._currentSyncTarget = Vector3.zero;
		this.distanceTraveled = Vector3.zero;
		this.lastSetNetTime = 0.0;
	}

	// Token: 0x04003FCB RID: 16331
	private double lastSetNetTime;

	// Token: 0x04003FCC RID: 16332
	private Vector3 _currentSyncTarget = Vector3.zero;

	// Token: 0x04003FCD RID: 16333
	private Vector3 distanceTraveled = Vector3.zero;
}
