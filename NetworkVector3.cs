using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009A4 RID: 2468
internal class NetworkVector3
{
	// Token: 0x170005D8 RID: 1496
	// (get) Token: 0x06003B2F RID: 15151 RVA: 0x0011AFAE File Offset: 0x001191AE
	public Vector3 CurrentSyncTarget
	{
		get
		{
			return this._currentSyncTarget;
		}
	}

	// Token: 0x06003B30 RID: 15152 RVA: 0x0011AFB8 File Offset: 0x001191B8
	public void SetNewSyncTarget(Vector3 newTarget)
	{
		Vector3 currentSyncTarget = this.CurrentSyncTarget;
		(ref currentSyncTarget).SetValueSafe(in newTarget);
		this.distanceTraveled = currentSyncTarget - this._currentSyncTarget;
		this._currentSyncTarget = currentSyncTarget;
		this.lastSetNetTime = PhotonNetwork.Time;
	}

	// Token: 0x06003B31 RID: 15153 RVA: 0x0011AFFC File Offset: 0x001191FC
	public Vector3 GetPredictedFuture()
	{
		float num = (float)(PhotonNetwork.Time - this.lastSetNetTime) * (float)PhotonNetwork.SerializationRate;
		Vector3 vector = this.distanceTraveled * num;
		return this._currentSyncTarget + vector;
	}

	// Token: 0x06003B32 RID: 15154 RVA: 0x0011B037 File Offset: 0x00119237
	public void Reset()
	{
		this._currentSyncTarget = Vector3.zero;
		this.distanceTraveled = Vector3.zero;
		this.lastSetNetTime = 0.0;
	}

	// Token: 0x04003FCC RID: 16332
	private double lastSetNetTime;

	// Token: 0x04003FCD RID: 16333
	private Vector3 _currentSyncTarget = Vector3.zero;

	// Token: 0x04003FCE RID: 16334
	private Vector3 distanceTraveled = Vector3.zero;
}
