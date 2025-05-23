using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200055C RID: 1372
public class GameAgent : MonoBehaviour
{
	// Token: 0x1400004C RID: 76
	// (add) Token: 0x0600212D RID: 8493 RVA: 0x000A6668 File Offset: 0x000A4868
	// (remove) Token: 0x0600212E RID: 8494 RVA: 0x000A66A0 File Offset: 0x000A48A0
	public event GameAgent.StateChangedEvent onBodyStateChanged;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x0600212F RID: 8495 RVA: 0x000A66D8 File Offset: 0x000A48D8
	// (remove) Token: 0x06002130 RID: 8496 RVA: 0x000A6710 File Offset: 0x000A4910
	public event GameAgent.StateChangedEvent onBehaviorStateChanged;

	// Token: 0x06002131 RID: 8497 RVA: 0x000A6748 File Offset: 0x000A4948
	public static GameAgent Get(GameEntityId id)
	{
		GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(id);
		if (!(gameEntity == null))
		{
			return gameEntity.GetComponent<GameAgent>();
		}
		return null;
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Awake()
	{
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000A6774 File Offset: 0x000A4974
	private void OnEnable()
	{
		GameAgentManager.instance.AddGameAgent(this);
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000A6783 File Offset: 0x000A4983
	private void OnDisable()
	{
		GameAgentManager.instance.RemoveGameAgent(this);
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x000A6792 File Offset: 0x000A4992
	public void OnBehaviorStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBehaviorStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x000A67A5 File Offset: 0x000A49A5
	public void OnBodyStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBodyStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x000A67B8 File Offset: 0x000A49B8
	public void OnThink()
	{
		if (this.navAgent.isOnNavMesh)
		{
			this.lastPosOnNavMesh = this.navAgent.transform.position;
		}
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x000A67DD File Offset: 0x000A49DD
	public bool IsOnNavMesh()
	{
		return this.navAgent != null && this.navAgent.isOnNavMesh;
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000A67FA File Offset: 0x000A49FA
	public Vector3 GetLastPosOnNavMesh()
	{
		return this.lastPosOnNavMesh;
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000A6804 File Offset: 0x000A4A04
	public void RequestDestination(Vector3 dest)
	{
		if (!GameEntityManager.instance.IsAuthority())
		{
			return;
		}
		if (!this.IsOnNavMesh())
		{
			dest = this.lastPosOnNavMesh;
		}
		if (Vector3.Distance(this.lastRequestedDest, dest) < 0.5f)
		{
			return;
		}
		this.lastRequestedDest = dest;
		if (GameEntityManager.instance.IsAuthority())
		{
			GameAgentManager.instance.RequestDestination(this, dest);
		}
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000A6867 File Offset: 0x000A4A67
	public void RequestImpact(GRTool tool, Vector3 startPos, Vector3 impulse, byte impulseData)
	{
		GameAgentManager.instance.RequestImpact(this.entity, tool, startPos, impulse, impulseData);
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000A6880 File Offset: 0x000A4A80
	public void RequestBehaviorChange(byte behavior)
	{
		GameAgentManager.instance.RequestBehavior(this, behavior);
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000A6890 File Offset: 0x000A4A90
	public void RequestStateChange(byte state)
	{
		GameAgentManager.instance.RequestState(this, state);
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000A68A0 File Offset: 0x000A4AA0
	public void ApplyDestination(Vector3 dest)
	{
		NavMeshHit navMeshHit;
		if (!NavMesh.SamplePosition(dest, out navMeshHit, 1.5f, -1))
		{
			return;
		}
		dest = navMeshHit.position;
		if (this.navAgent.isOnNavMesh)
		{
			this.navAgent.destination = dest;
		}
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000A68E0 File Offset: 0x000A4AE0
	public void SetDisableNetworkSync(bool disable)
	{
		this.disableNetworkSync = disable;
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000A68E9 File Offset: 0x000A4AE9
	public void SetIsPathing(bool isPathing, bool ignoreRigiBody = false)
	{
		this.navAgent.enabled = isPathing;
		if (!ignoreRigiBody && this.rigidBody != null)
		{
			this.rigidBody.isKinematic = isPathing;
		}
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x000A6914 File Offset: 0x000A4B14
	public void SetSpeed(float speed)
	{
		this.navAgent.speed = speed;
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x000A6922 File Offset: 0x000A4B22
	public void ApplyNetworkUpdate(Vector3 position, Quaternion rotation)
	{
		if (this.disableNetworkSync)
		{
			return;
		}
		base.transform.SetPositionAndRotation(position, rotation);
		if (this.rigidBody != null)
		{
			this.rigidBody.position = position;
			this.rigidBody.rotation = rotation;
		}
	}

	// Token: 0x0400257B RID: 9595
	public GameEntity entity;

	// Token: 0x0400257C RID: 9596
	public NavMeshAgent navAgent;

	// Token: 0x0400257D RID: 9597
	public Rigidbody rigidBody;

	// Token: 0x0400257E RID: 9598
	private bool disableNetworkSync;

	// Token: 0x0400257F RID: 9599
	private Vector3 lastPosOnNavMesh;

	// Token: 0x04002580 RID: 9600
	private Vector3 lastRequestedDest;

	// Token: 0x0200055D RID: 1373
	// (Invoke) Token: 0x06002145 RID: 8517
	public delegate void StateChangedEvent(byte newState);
}
