using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000779 RID: 1913
public class ProximityReactor : MonoBehaviour
{
	// Token: 0x170004BE RID: 1214
	// (get) Token: 0x06002FA0 RID: 12192 RVA: 0x000ED135 File Offset: 0x000EB335
	public float proximityRange
	{
		get
		{
			return this.proximityMax - this.proximityMin;
		}
	}

	// Token: 0x170004BF RID: 1215
	// (get) Token: 0x06002FA1 RID: 12193 RVA: 0x000ED144 File Offset: 0x000EB344
	public float distance
	{
		get
		{
			return this._distance;
		}
	}

	// Token: 0x170004C0 RID: 1216
	// (get) Token: 0x06002FA2 RID: 12194 RVA: 0x000ED14C File Offset: 0x000EB34C
	public float distanceLinear
	{
		get
		{
			return this._distanceLinear;
		}
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x000ED154 File Offset: 0x000EB354
	public void SetRigFrom()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.from = componentInParent.transform;
		}
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x000ED180 File Offset: 0x000EB380
	public void SetRigTo()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.to = componentInParent.transform;
		}
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x000ED1AA File Offset: 0x000EB3AA
	public void SetTransformFrom(Transform t)
	{
		this.from = t;
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x000ED1B3 File Offset: 0x000EB3B3
	public void SetTransformTo(Transform t)
	{
		this.to = t;
	}

	// Token: 0x06002FA7 RID: 12199 RVA: 0x000ED1BC File Offset: 0x000EB3BC
	private void Setup()
	{
		this._distance = 0f;
		this._distanceLinear = 0f;
	}

	// Token: 0x06002FA8 RID: 12200 RVA: 0x000ED1D4 File Offset: 0x000EB3D4
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06002FA9 RID: 12201 RVA: 0x000ED1DC File Offset: 0x000EB3DC
	private void Update()
	{
		if (!this.from || !this.to)
		{
			this._distance = 0f;
			this._distanceLinear = 0f;
			return;
		}
		Vector3 position = this.from.position;
		float magnitude = (this.to.position - position).magnitude;
		if (!this._distance.Approx(magnitude, 1E-06f))
		{
			UnityEvent<float> unityEvent = this.onProximityChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(magnitude);
			}
		}
		this._distance = magnitude;
		float num = (this.proximityRange.Approx0(1E-06f) ? 0f : MathUtils.LinearUnclamped(magnitude, this.proximityMin, this.proximityMax, 0f, 1f));
		if (!this._distanceLinear.Approx(num, 1E-06f))
		{
			UnityEvent<float> unityEvent2 = this.onProximityChangedLinear;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(num);
			}
		}
		this._distanceLinear = num;
		if (this._distanceLinear < 0f)
		{
			UnityEvent<float> unityEvent3 = this.onBelowMinProximity;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke(magnitude);
			}
		}
		if (this._distanceLinear > 1f)
		{
			UnityEvent<float> unityEvent4 = this.onAboveMaxProximity;
			if (unityEvent4 == null)
			{
				return;
			}
			unityEvent4.Invoke(magnitude);
		}
	}

	// Token: 0x04003623 RID: 13859
	public Transform from;

	// Token: 0x04003624 RID: 13860
	public Transform to;

	// Token: 0x04003625 RID: 13861
	[Space]
	public float proximityMin;

	// Token: 0x04003626 RID: 13862
	public float proximityMax = 1f;

	// Token: 0x04003627 RID: 13863
	[Space]
	[NonSerialized]
	private float _distance;

	// Token: 0x04003628 RID: 13864
	[NonSerialized]
	private float _distanceLinear;

	// Token: 0x04003629 RID: 13865
	[Space]
	public UnityEvent<float> onProximityChanged;

	// Token: 0x0400362A RID: 13866
	public UnityEvent<float> onProximityChangedLinear;

	// Token: 0x0400362B RID: 13867
	[Space]
	public UnityEvent<float> onBelowMinProximity;

	// Token: 0x0400362C RID: 13868
	public UnityEvent<float> onAboveMaxProximity;
}
