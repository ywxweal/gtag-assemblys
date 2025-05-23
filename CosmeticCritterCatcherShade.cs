using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class CosmeticCritterCatcherShade : CosmeticCritterCatcher
{
	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000420 RID: 1056 RVA: 0x00018169 File Offset: 0x00016369
	// (set) Token: 0x06000421 RID: 1057 RVA: 0x00018171 File Offset: 0x00016371
	public Vector3 LastTargetPosition { get; private set; }

	// Token: 0x06000422 RID: 1058 RVA: 0x0001817A File Offset: 0x0001637A
	public float GetActionTimeFrac()
	{
		return this.targetHoldTime / this.maxHoldTime;
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00018189 File Offset: 0x00016389
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 0.25f, 0.5f);
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0001819C File Offset: 0x0001639C
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (this.heartbeatCooldown > 0.5f || (this.currentTarget != null && this.currentTarget != critter))
		{
			return CosmeticCritterAction.None;
		}
		if (critter is CosmeticCritterShadeFleeing && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 0f))
		{
			if (this.targetHoldTime >= this.minSecondsLockedToCatch && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
		else
		{
			if (!(critter is CosmeticCritterShadeHidden) || !this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 0f))
			{
				return CosmeticCritterAction.None;
			}
			if (this.targetHoldTime >= this.secondsToReveal)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00018268 File Offset: 0x00016468
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		if (!base.ValidateRemoteCatchAction(critter, catchAction, serverTime))
		{
			return false;
		}
		if (critter is CosmeticCritterShadeFleeing)
		{
			if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius + 1f && this.targetHoldTime >= this.minSecondsLockedToCatch * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 2f))
			{
				return true;
			}
		}
		else if (critter is CosmeticCritterShadeHidden)
		{
			if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None && this.targetHoldTime >= this.secondsToReveal * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 2f))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0001833C File Offset: 0x0001653C
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.currentTarget = critter;
		float num = (PhotonNetwork.InRoom ? ((float)(PhotonNetwork.Time - serverTime)) : 0f);
		this.heartbeatCooldown = 1f + num;
		this.targetHoldTime += num;
		if (!(critter is CosmeticCritterShadeFleeing))
		{
			if (critter is CosmeticCritterShadeHidden)
			{
				this.maxHoldTime = this.secondsToReveal;
				if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None)
				{
					(this.optionalLinkedSpawner as CosmeticCritterSpawnerShadeFleeing).SetSpawnPosition(critter.transform.position);
					this.currentTarget = null;
					this.targetHoldTime = 0f;
				}
			}
			return;
		}
		this.maxHoldTime = this.minSecondsLockedToCatch;
		if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
		{
			this.shadeRevealer.ShadeCaught();
			this.currentTarget = null;
			this.targetHoldTime = 0f;
			return;
		}
		CosmeticCritterAction cosmeticCritterAction = catchAction & CosmeticCritterAction.ShadeHeartbeat;
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00018406 File Offset: 0x00016606
	protected override void Awake()
	{
		base.Awake();
		this.shadeRevealer = this.transferrableObject as ShadeRevealer;
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00018420 File Offset: 0x00016620
	protected void LateUpdate()
	{
		if (this.heartbeatCooldown > 0f)
		{
			this.heartbeatCooldown -= Time.deltaTime;
			if (this.heartbeatCooldown < 0f)
			{
				this.heartbeatCooldown = 0f;
				this.currentTarget = null;
				return;
			}
			this.targetHoldTime = Mathf.Min(this.targetHoldTime + Time.deltaTime, this.maxHoldTime);
			if (this.currentTarget is CosmeticCritterShadeFleeing)
			{
				if (!base.IsLocal || this.heartbeatCooldown > 0.4f)
				{
					this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.LOCKED);
				}
				Vector3 normalized = (this.catchOrigin.position - this.currentTarget.transform.position).normalized;
				(this.currentTarget as CosmeticCritterShadeFleeing).pullVector += this.vacuumSpeed * Time.deltaTime * normalized;
				return;
			}
			if (this.currentTarget is CosmeticCritterShadeHidden && (!base.IsLocal || this.heartbeatCooldown > 0.4f))
			{
				this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.TRACKING);
				return;
			}
		}
		else if (this.targetHoldTime > 0f)
		{
			this.targetHoldTime = Mathf.Max(this.targetHoldTime - Time.deltaTime, 0f);
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00018569 File Offset: 0x00016769
	protected override void OnEnable()
	{
		base.OnEnable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x0001858E File Offset: 0x0001678E
	protected override void OnDisable()
	{
		base.OnDisable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x04000496 RID: 1174
	[SerializeField]
	private float secondsToReveal = 1f;

	// Token: 0x04000497 RID: 1175
	[SerializeField]
	private float minSecondsLockedToCatch = 1f;

	// Token: 0x04000498 RID: 1176
	[SerializeField]
	private Transform catchOrigin;

	// Token: 0x04000499 RID: 1177
	[SerializeField]
	private float catchRadius = 1f;

	// Token: 0x0400049A RID: 1178
	[SerializeField]
	private float vacuumSpeed = 3f;

	// Token: 0x0400049B RID: 1179
	private ShadeRevealer shadeRevealer;

	// Token: 0x0400049C RID: 1180
	private CosmeticCritter currentTarget;

	// Token: 0x0400049D RID: 1181
	private float targetHoldTime;

	// Token: 0x0400049E RID: 1182
	private float maxHoldTime;

	// Token: 0x040004A0 RID: 1184
	private const float HEARTBEAT_DELAY = 1f;

	// Token: 0x040004A1 RID: 1185
	private float heartbeatCooldown;
}
