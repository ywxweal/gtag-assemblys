using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000525 RID: 1317
public class ConditionalTrigger : MonoBehaviour, IRigAware
{
	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001FDF RID: 8159 RVA: 0x000A0F5C File Offset: 0x0009F15C
	private int intValue
	{
		get
		{
			return (int)this._tracking;
		}
	}

	// Token: 0x06001FE0 RID: 8160 RVA: 0x000A0F64 File Offset: 0x0009F164
	public void SetProximityFromRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._from = this._rig.transform;
		}
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x000A0FA2 File Offset: 0x0009F1A2
	public void SetProximityToRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._to = this._rig.transform;
		}
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000A0FE0 File Offset: 0x0009F1E0
	public void SetProximityFrom(Transform from)
	{
		this._from = from;
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x000A0FE9 File Offset: 0x0009F1E9
	public void SetProxmityTo(Transform to)
	{
		this._to = to;
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x000A0FF2 File Offset: 0x0009F1F2
	public void TrackedSet(TriggerCondition conditions)
	{
		this._tracking = conditions;
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000A0FFB File Offset: 0x0009F1FB
	public void TrackedAdd(TriggerCondition conditions)
	{
		this._tracking |= conditions;
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x000A100B File Offset: 0x0009F20B
	public void TrackedRemove(TriggerCondition conditions)
	{
		this._tracking &= ~conditions;
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000A0FF2 File Offset: 0x0009F1F2
	public void TrackedSet(int conditions)
	{
		this._tracking = (TriggerCondition)conditions;
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x000A0FFB File Offset: 0x0009F1FB
	public void TrackedAdd(int conditions)
	{
		this._tracking |= (TriggerCondition)conditions;
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000A100B File Offset: 0x0009F20B
	public void TrackedRemove(int conditions)
	{
		this._tracking &= (TriggerCondition)(~(TriggerCondition)conditions);
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x000A101C File Offset: 0x0009F21C
	public void TrackedClear()
	{
		this._tracking = TriggerCondition.None;
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000A1025 File Offset: 0x0009F225
	private void OnEnable()
	{
		this._timeSince = 0f;
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x000A1037 File Offset: 0x0009F237
	private void Update()
	{
		if (this.IsTracking(TriggerCondition.TimeElapsed))
		{
			this.TrackTimeElapsed();
		}
		if (this.IsTracking(TriggerCondition.Proximity))
		{
			this.TrackProximity();
			return;
		}
		this._distance = 0f;
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x000A1063 File Offset: 0x0009F263
	private void TrackTimeElapsed()
	{
		if (this._timeSince.HasElapsed(this._interval, true))
		{
			UnityEvent unityEvent = this.onTimeElapsed;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000A108C File Offset: 0x0009F28C
	private void TrackProximity()
	{
		if (!this._from || !this._to)
		{
			this._distance = 0f;
			return;
		}
		this._distance = Vector3.Distance(this._to.position, this._from.position);
		if (this._distance >= this._maxDistance)
		{
			UnityEvent unityEvent = this.onMaxDistance;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000A10FE File Offset: 0x0009F2FE
	private bool IsTracking(TriggerCondition condition)
	{
		return (this._tracking & condition) == condition;
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000A110B File Offset: 0x0009F30B
	private static void FindRig(out VRRig rig)
	{
		if (PhotonNetwork.InRoom)
		{
			rig = GorillaGameManager.StaticFindRigForPlayer(NetPlayer.Get(PhotonNetwork.LocalPlayer));
			return;
		}
		rig = VRRig.LocalRig;
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x000A112D File Offset: 0x0009F32D
	public void SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x040023D6 RID: 9174
	[Space]
	[SerializeField]
	private TriggerCondition _tracking;

	// Token: 0x040023D7 RID: 9175
	[Space]
	[SerializeField]
	private Transform _from;

	// Token: 0x040023D8 RID: 9176
	[SerializeField]
	private Transform _to;

	// Token: 0x040023D9 RID: 9177
	[SerializeField]
	private float _maxDistance;

	// Token: 0x040023DA RID: 9178
	[NonSerialized]
	private float _distance;

	// Token: 0x040023DB RID: 9179
	[Space]
	public UnityEvent onMaxDistance;

	// Token: 0x040023DC RID: 9180
	[SerializeField]
	private float _interval = 1f;

	// Token: 0x040023DD RID: 9181
	[NonSerialized]
	private TimeSince _timeSince;

	// Token: 0x040023DE RID: 9182
	[Space]
	public UnityEvent onTimeElapsed;

	// Token: 0x040023DF RID: 9183
	[Space]
	private VRRig _rig;
}
