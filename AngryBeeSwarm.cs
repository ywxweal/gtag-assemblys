using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag.Rendering;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

// Token: 0x020000FD RID: 253
[NetworkBehaviourWeaved(3)]
public class AngryBeeSwarm : NetworkComponent
{
	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000650 RID: 1616 RVA: 0x00024258 File Offset: 0x00022458
	public bool isDormant
	{
		get
		{
			return this.currentState == AngryBeeSwarm.ChaseState.Dormant;
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00024264 File Offset: 0x00022464
	protected override void Awake()
	{
		base.Awake();
		AngryBeeSwarm.instance = this;
		this.targetPlayer = null;
		this.currentState = AngryBeeSwarm.ChaseState.Dormant;
		this.grabTimestamp = -this.minGrabCooldown;
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x000242B8 File Offset: 0x000224B8
	private void InitializeSwarm()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.beeAnimator.transform.localPosition = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00024304 File Offset: 0x00022504
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = AngryBeeSwarm.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			AngryBeeSwarm.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case AngryBeeSwarm.ChaseState.Dormant:
				if (Application.isEditor && Keyboard.current[Key.Space].wasPressedThisFrame)
				{
					this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
				}
				break;
			case AngryBeeSwarm.ChaseState.InitialEmerge:
				if (Time.time > this.emergeStartedTimestamp + this.totalTimeToEmerge)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Chasing;
				}
				break;
			case (AngryBeeSwarm.ChaseState)3:
				break;
			case AngryBeeSwarm.ChaseState.Chasing:
				if (this.followTarget == null || this.targetPlayer == null || Time.time > this.NextRefreshClosestPlayerTimestamp)
				{
					this.ChooseClosestTarget();
					if (this.followTarget != null)
					{
						this.BoredToDeathAtTimestamp = -1f;
					}
					else if (this.BoredToDeathAtTimestamp < 0f)
					{
						this.BoredToDeathAtTimestamp = Time.time + this.boredAfterDuration;
					}
				}
				if (this.BoredToDeathAtTimestamp >= 0f && Time.time > this.BoredToDeathAtTimestamp)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Dormant;
				}
				else if (!(this.followTarget == null) && (this.followTarget.position - this.beeAnimator.transform.position).magnitude < this.catchDistance)
				{
					float num = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
					if (this.followTarget.position.y > num)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Grabbing;
					}
				}
				break;
			default:
				if (chaseState == AngryBeeSwarm.ChaseState.Grabbing)
				{
					if (Time.time > this.grabTimestamp + this.grabDuration)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Dormant;
					}
				}
				break;
			}
		}
		if (this.lastState != this.currentState)
		{
			this.OnChangeState(this.currentState);
			this.lastState = this.currentState;
		}
		this.UpdateState();
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x000244E8 File Offset: 0x000226E8
	public void UpdateState()
	{
		AngryBeeSwarm.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			if (NetworkSystem.Instance.InRoom)
			{
				this.SwarmEmergeUpdateShared();
				return;
			}
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				return;
			}
			break;
		default:
			if (chaseState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.RiseGrabbedLocalPlayer();
				}
				this.GrabBodyShared();
			}
			break;
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00024577 File Offset: 0x00022777
	public void Emerge(Vector3 fromPosition, Vector3 toPosition)
	{
		base.transform.position = fromPosition;
		this.emergeFromPosition = fromPosition;
		this.emergeToPosition = toPosition;
		this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
		this.emergeStartedTimestamp = Time.time;
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x000245A8 File Offset: 0x000227A8
	private void OnChangeState(AngryBeeSwarm.ChaseState newState)
	{
		switch (newState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
			if (this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				base.transform.position = new Vector3(0f, -9999f, 0f);
				this.InitializeSwarm();
			}
			this.SetInitialRotations();
			return;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			this.emergeStartedTimestamp = Time.time;
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(0f);
			if (base.IsMine)
			{
				this.currentSpeed = 0f;
				this.ChooseClosestTarget();
			}
			this.SetInitialRotations();
			return;
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(1f);
			this.ResetPath();
			this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
			this.BoredToDeathAtTimestamp = -1f;
			return;
		default:
		{
			if (newState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.grabTimestamp = Time.time;
			this.beeAnimator.transform.localPosition = this.ghostOffsetGrabbingLocal;
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
			if (vrrig != null)
			{
				this.followTarget = vrrig.transform;
			}
			break;
		}
		}
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00024750 File Offset: 0x00022950
	private void ChooseClosestTarget()
	{
		float num = Mathf.Lerp(this.initialRangeLimit, this.finalRangeLimit, (Time.time + this.totalTimeToEmerge - this.emergeStartedTimestamp) / this.rangeLimitBlendDuration);
		float num2 = num * num;
		VRRig vrrig = null;
		float num3 = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
		foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
		{
			if (vrrig2.head != null && !(vrrig2.head.rigTarget == null) && vrrig2.head.rigTarget.position.y > num3)
			{
				float sqrMagnitude = (base.transform.position - vrrig2.head.rigTarget.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					vrrig = vrrig2;
				}
			}
		}
		if (vrrig != null)
		{
			this.targetPlayer = vrrig.creator;
			this.followTarget = vrrig.head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
		}
		else
		{
			this.targetPlayer = null;
			this.followTarget = null;
		}
		this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x000248BC File Offset: 0x00022ABC
	private void SetInitialRotations()
	{
		this.beeAnimator.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x000248D4 File Offset: 0x00022AD4
	private void SwarmEmergeUpdateShared()
	{
		if (Time.time < this.emergeStartedTimestamp + this.totalTimeToEmerge)
		{
			float num = (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge;
			if (base.IsMine)
			{
				base.transform.position = Vector3.Lerp(this.emergeFromPosition, this.emergeToPosition, (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge);
			}
			this.beeAnimator.SetEmergeFraction(num);
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0002494C File Offset: 0x00022B4C
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTimestamp + this.minGrabCooldown)
		{
			this.grabTimestamp = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTimestamp + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.velocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x000249FC File Offset: 0x00022BFC
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.pathPoints[this.pathPoints.Count - 1] = destination;
		Vector3 vector = this.pathPoints[this.currentPathPointIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentPathPointIdx + 1 < this.pathPoints.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentPathPointIdx++;
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00024B0C File Offset: 0x00022D0C
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.pathPoints = new List<Vector3>();
		foreach (Vector3 vector in this.path.corners)
		{
			this.pathPoints.Add(vector + Vector3.up * this.heightAboveNavmesh);
		}
		this.pathPoints.Add(destination);
		this.currentPathPointIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00024BE0 File Offset: 0x00022DE0
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00024BEC File Offset: 0x00022DEC
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseInterval)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			float num = ZoneShaderSettings.GetWaterY() + this.MinHeightAboveWater;
			Vector3 position = this.followTarget.position;
			if (position.y < num)
			{
				position.y = num;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, position, this.currentSpeed * Time.deltaTime);
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00024CA4 File Offset: 0x00022EA4
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.beeAnimator.transform.localPosition = this.noisyOffset;
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00024D11 File Offset: 0x00022F11
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000661 RID: 1633 RVA: 0x00024D4D File Offset: 0x00022F4D
	// (set) Token: 0x06000662 RID: 1634 RVA: 0x00024D77 File Offset: 0x00022F77
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe BeeSwarmData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BeeSwarmData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BeeSwarmData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00024DA2 File Offset: 0x00022FA2
	public override void WriteDataFusion()
	{
		this.Data = new BeeSwarmData(this.targetPlayer.ActorNumber, (int)this.currentState, this.currentSpeed);
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00024DC8 File Offset: 0x00022FC8
	public override void ReadDataFusion()
	{
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(this.Data.TargetActorNumber);
		this.currentState = (AngryBeeSwarm.ChaseState)this.Data.CurrentState;
		if (float.IsFinite(this.Data.CurrentSpeed))
		{
			this.currentSpeed = this.Data.CurrentSpeed;
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00024E30 File Offset: 0x00023030
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender == null || !info.Sender.Equals(PhotonNetwork.MasterClient))
		{
			return;
		}
		NetPlayer netPlayer = this.targetPlayer;
		stream.SendNext((netPlayer != null) ? netPlayer.ActorNumber : (-1));
		stream.SendNext(this.currentState);
		stream.SendNext(this.currentSpeed);
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00024E98 File Offset: 0x00023098
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num);
		this.currentState = (AngryBeeSwarm.ChaseState)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		if (float.IsFinite(num2))
		{
			this.currentSpeed = num2;
		}
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00024EFC File Offset: 0x000230FC
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00024F1A File Offset: 0x0002311A
	public void OnJoinedRoom()
	{
		Debug.Log("Here");
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeSwarm();
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00024F38 File Offset: 0x00023138
	private void TestEmerge()
	{
		this.Emerge(this.testEmergeFrom.transform.position, this.testEmergeTo.transform.position);
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00024FEC File Offset: 0x000231EC
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00025004 File Offset: 0x00023204
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400078B RID: 1931
	public static AngryBeeSwarm instance;

	// Token: 0x0400078C RID: 1932
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x0400078D RID: 1933
	public Transform followTarget;

	// Token: 0x0400078E RID: 1934
	[SerializeField]
	private float velocityStep = 1f;

	// Token: 0x0400078F RID: 1935
	private float currentSpeed;

	// Token: 0x04000790 RID: 1936
	[SerializeField]
	private float velocityIncreaseInterval = 20f;

	// Token: 0x04000791 RID: 1937
	public Vector3 noisyOffset;

	// Token: 0x04000792 RID: 1938
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04000793 RID: 1939
	private float emergeStartedTimestamp;

	// Token: 0x04000794 RID: 1940
	private float grabTimestamp;

	// Token: 0x04000795 RID: 1941
	private float lastSpeedIncreased;

	// Token: 0x04000796 RID: 1942
	[SerializeField]
	private float totalTimeToEmerge;

	// Token: 0x04000797 RID: 1943
	[SerializeField]
	private float catchDistance;

	// Token: 0x04000798 RID: 1944
	[SerializeField]
	private float grabDuration;

	// Token: 0x04000799 RID: 1945
	[SerializeField]
	private float grabSpeed = 1f;

	// Token: 0x0400079A RID: 1946
	[SerializeField]
	private float minGrabCooldown;

	// Token: 0x0400079B RID: 1947
	[SerializeField]
	private float initialRangeLimit;

	// Token: 0x0400079C RID: 1948
	[SerializeField]
	private float finalRangeLimit;

	// Token: 0x0400079D RID: 1949
	[SerializeField]
	private float rangeLimitBlendDuration;

	// Token: 0x0400079E RID: 1950
	[SerializeField]
	private float boredAfterDuration;

	// Token: 0x0400079F RID: 1951
	public NetPlayer targetPlayer;

	// Token: 0x040007A0 RID: 1952
	public AngryBeeAnimator beeAnimator;

	// Token: 0x040007A1 RID: 1953
	public AngryBeeSwarm.ChaseState currentState;

	// Token: 0x040007A2 RID: 1954
	public AngryBeeSwarm.ChaseState lastState;

	// Token: 0x040007A3 RID: 1955
	public NetPlayer grabbedPlayer;

	// Token: 0x040007A4 RID: 1956
	private bool targetIsOnNavMesh;

	// Token: 0x040007A5 RID: 1957
	private const float navMeshSampleRange = 5f;

	// Token: 0x040007A6 RID: 1958
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x040007A7 RID: 1959
	public float hapticDuration = 1.5f;

	// Token: 0x040007A8 RID: 1960
	public float MinHeightAboveWater = 0.5f;

	// Token: 0x040007A9 RID: 1961
	public float PlayerMinHeightAboveWater = 0.5f;

	// Token: 0x040007AA RID: 1962
	public float RefreshClosestPlayerInterval = 1f;

	// Token: 0x040007AB RID: 1963
	private float NextRefreshClosestPlayerTimestamp = 1f;

	// Token: 0x040007AC RID: 1964
	private float BoredToDeathAtTimestamp = -1f;

	// Token: 0x040007AD RID: 1965
	[SerializeField]
	private Transform testEmergeFrom;

	// Token: 0x040007AE RID: 1966
	[SerializeField]
	private Transform testEmergeTo;

	// Token: 0x040007AF RID: 1967
	private Vector3 emergeFromPosition;

	// Token: 0x040007B0 RID: 1968
	private Vector3 emergeToPosition;

	// Token: 0x040007B1 RID: 1969
	private NavMeshPath path;

	// Token: 0x040007B2 RID: 1970
	public List<Vector3> pathPoints;

	// Token: 0x040007B3 RID: 1971
	public int currentPathPointIdx;

	// Token: 0x040007B4 RID: 1972
	private float nextPathTimestamp;

	// Token: 0x040007B5 RID: 1973
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BeeSwarmData _Data;

	// Token: 0x020000FE RID: 254
	public enum ChaseState
	{
		// Token: 0x040007B7 RID: 1975
		Dormant = 1,
		// Token: 0x040007B8 RID: 1976
		InitialEmerge,
		// Token: 0x040007B9 RID: 1977
		Chasing = 4,
		// Token: 0x040007BA RID: 1978
		Grabbing = 8
	}
}
