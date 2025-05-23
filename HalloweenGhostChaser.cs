using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000657 RID: 1623
[NetworkBehaviourWeaved(5)]
public class HalloweenGhostChaser : NetworkComponent
{
	// Token: 0x0600288E RID: 10382 RVA: 0x000C9AA3 File Offset: 0x000C7CA3
	protected override void Awake()
	{
		base.Awake();
		this.spawnIndex = 0;
		this.targetPlayer = null;
		this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
		this.grabTime = -this.minGrabCooldown;
		this.possibleTarget = new List<NetPlayer>();
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x000C9AD8 File Offset: 0x000C7CD8
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000C9B0C File Offset: 0x000C7D0C
	private void InitializeGhost()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.lastHeadAngleTime = 0f;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Random.value * this.maxTimeToNextHeadAngle;
			this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			this.ghostBody.transform.localPosition = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000C9BAC File Offset: 0x000C7DAC
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			HalloweenGhostChaser.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case HalloweenGhostChaser.ChaseState.Dormant:
				if (Time.time >= this.nextTimeToChasePlayer)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				if (Time.time >= this.lastSummonCheck + this.summoningDuration)
				{
					this.lastSummonCheck = Time.time;
					this.possibleTarget.Clear();
					int num = 0;
					int i = 0;
					while (i < this.spawnTransforms.Length)
					{
						int num2 = 0;
						for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
						{
							if ((GorillaParent.instance.vrrigs[j].transform.position - this.spawnTransforms[i].position).magnitude < this.summonDistance)
							{
								this.possibleTarget.Add(GorillaParent.instance.vrrigs[j].creator);
								num2++;
								if (num2 >= this.summonCount)
								{
									break;
								}
							}
						}
						if (num2 >= this.summonCount)
						{
							if (!this.wasSurroundedLastCheck)
							{
								this.wasSurroundedLastCheck = true;
								break;
							}
							this.wasSurroundedLastCheck = false;
							this.isSummoned = true;
							this.currentState = HalloweenGhostChaser.ChaseState.Gong;
							break;
						}
						else
						{
							num++;
							i++;
						}
					}
					if (num == this.spawnTransforms.Length)
					{
						this.wasSurroundedLastCheck = false;
					}
				}
				break;
			case HalloweenGhostChaser.ChaseState.InitialRise:
				if (Time.time > this.timeRiseStarted + this.totalTimeToRise)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.Chasing;
				}
				break;
			case (HalloweenGhostChaser.ChaseState)3:
				break;
			case HalloweenGhostChaser.ChaseState.Gong:
				if (Time.time > this.timeGongStarted + this.gongDuration)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				break;
			default:
				if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
				{
					if (chaseState == HalloweenGhostChaser.ChaseState.Grabbing)
					{
						if (Time.time > this.grabTime + this.grabDuration)
						{
							this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
						}
					}
				}
				else
				{
					if (this.followTarget == null || this.targetPlayer == null)
					{
						this.ChooseRandomTarget();
					}
					if (!(this.followTarget == null) && (this.followTarget.position - this.ghostBody.transform.position).magnitude < this.catchDistance)
					{
						this.currentState = HalloweenGhostChaser.ChaseState.Grabbing;
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

	// Token: 0x06002892 RID: 10386 RVA: 0x000C9E44 File Offset: 0x000C8044
	public void UpdateState()
	{
		HalloweenGhostChaser.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			this.isSummoned = false;
			if (this.ghostMaterial.color == this.summonedColor)
			{
				this.ghostMaterial.color = this.defaultColor;
				return;
			}
			break;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.RiseHost();
				}
				this.MoveHead();
				return;
			}
			break;
		case (HalloweenGhostChaser.ChaseState)3:
		case HalloweenGhostChaser.ChaseState.Gong:
			break;
		default:
			if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (chaseState != HalloweenGhostChaser.ChaseState.Grabbing)
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
					this.MoveHead();
				}
			}
			else if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				this.MoveHead();
				return;
			}
			break;
		}
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x000C9F28 File Offset: 0x000C8128
	private void OnChangeState(HalloweenGhostChaser.ChaseState newState)
	{
		switch (newState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			if (this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				this.InitializeGhost();
			}
			else
			{
				this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			}
			this.SetInitialRotations();
			return;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			this.timeRiseStarted = Time.time;
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				if (!this.isSummoned)
				{
					this.currentSpeed = 0f;
					this.ChooseRandomTarget();
					this.SetInitialSpawnPoint();
				}
				else
				{
					this.currentSpeed = 3f;
				}
			}
			if (this.isSummoned)
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				this.ghostMaterial.color = this.summonedColor;
			}
			else
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlay();
				this.ghostMaterial.color = this.defaultColor;
			}
			this.SetInitialRotations();
			return;
		case (HalloweenGhostChaser.ChaseState)3:
			break;
		case HalloweenGhostChaser.ChaseState.Gong:
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				this.ChooseRandomTarget();
				this.SetInitialSpawnPoint();
				base.transform.position = this.spawnTransforms[this.spawnIndex].position;
			}
			this.timeGongStarted = Time.time;
			this.laugh.volume = 1f;
			this.laugh.GTPlayOneShot(this.gong, 1f);
			this.isSummoned = true;
			return;
		default:
			if (newState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (newState != HalloweenGhostChaser.ChaseState.Grabbing)
				{
					return;
				}
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.grabTime = Time.time;
				if (this.isSummoned)
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				}
				else
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlay();
				}
				this.leftArm.localEulerAngles = this.leftArmGrabbingLocal;
				this.rightArm.localEulerAngles = this.rightArmGrabbingLocal;
				this.leftHand.localEulerAngles = this.leftHandGrabbingLocal;
				this.rightHand.localEulerAngles = this.rightHandGrabbingLocal;
				this.ghostBody.transform.localPosition = this.ghostOffsetGrabbingLocal;
				this.ghostBody.transform.localEulerAngles = this.ghostGrabbingEulerRotation;
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				if (vrrig != null)
				{
					this.followTarget = vrrig.transform;
					return;
				}
			}
			else
			{
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.ResetPath();
			}
			break;
		}
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x000CA220 File Offset: 0x000C8420
	private void SetInitialSpawnPoint()
	{
		float num = 1000f;
		this.spawnIndex = 0;
		if (this.followTarget == null)
		{
			return;
		}
		for (int i = 0; i < this.spawnTransforms.Length; i++)
		{
			float magnitude = (this.followTarget.position - this.spawnTransformOffsets[i].position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				this.spawnIndex = i;
			}
		}
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x000CA290 File Offset: 0x000C8490
	private void ChooseRandomTarget()
	{
		int num = -1;
		if (this.possibleTarget.Count >= this.summonCount)
		{
			int randomTarget = Random.Range(0, this.possibleTarget.Count);
			num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == this.possibleTarget[randomTarget]);
			this.currentSpeed = 3f;
		}
		if (num == -1)
		{
			num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
		}
		this.possibleTarget.Clear();
		if (num < GorillaParent.instance.vrrigs.Count)
		{
			this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
			this.followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
			return;
		}
		this.targetPlayer = null;
		this.followTarget = null;
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000CA3A8 File Offset: 0x000C85A8
	private void SetInitialRotations()
	{
		this.leftArm.localEulerAngles = Vector3.zero;
		this.rightArm.localEulerAngles = Vector3.zero;
		this.leftHand.localEulerAngles = this.leftHandStartingLocal;
		this.rightHand.localEulerAngles = this.rightHandStartingLocal;
		this.ghostBody.transform.localPosition = Vector3.zero;
		this.ghostBody.transform.localEulerAngles = this.ghostStartingEulerRotation;
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x000CA424 File Offset: 0x000C8624
	private void MoveHead()
	{
		if (Time.time > this.nextHeadAngleTime)
		{
			this.skullTransform.localEulerAngles = this.headEulerAngles[Random.Range(0, this.headEulerAngles.Length)];
			this.lastHeadAngleTime = Time.time;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Mathf.Max(Random.value * this.maxTimeToNextHeadAngle, 0.05f);
		}
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x000CA490 File Offset: 0x000C8690
	private void RiseHost()
	{
		if (Time.time < this.timeRiseStarted + this.totalTimeToRise)
		{
			if (this.spawnIndex == -1)
			{
				this.spawnIndex = 0;
			}
			base.transform.position = this.spawnTransforms[this.spawnIndex].position + Vector3.up * (Time.time - this.timeRiseStarted) / this.totalTimeToRise * this.riseDistance;
			base.transform.rotation = this.spawnTransforms[this.spawnIndex].rotation;
		}
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x000CA52C File Offset: 0x000C872C
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTime + this.minGrabCooldown)
		{
			this.grabTime = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTime + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.velocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000CA5DC File Offset: 0x000C87DC
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.points[this.points.Count - 1] = destination;
		Vector3 vector = this.points[this.currentTargetIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentTargetIdx + 1 < this.points.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentTargetIdx++;
		}
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000CA6EC File Offset: 0x000C88EC
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.points = new List<Vector3>();
		foreach (Vector3 vector in this.path.corners)
		{
			this.points.Add(vector + Vector3.up * this.heightAboveNavmesh);
		}
		this.points.Add(destination);
		this.currentTargetIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x000CA7C0 File Offset: 0x000C89C0
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x000CA7CC File Offset: 0x000C89CC
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseTime)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(this.followTarget.position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.followTarget.position, this.currentSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.LookRotation(this.followTarget.position - base.transform.position, Vector3.up);
		}
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x000CA8A0 File Offset: 0x000C8AA0
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.childGhost.localPosition = this.noisyOffset;
		this.leftArm.localEulerAngles = this.noisyOffset * 20f;
		this.rightArm.localEulerAngles = this.noisyOffset * -20f;
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000CA93E File Offset: 0x000C8B3E
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x060028A0 RID: 10400 RVA: 0x000CA97A File Offset: 0x000C8B7A
	// (set) Token: 0x060028A1 RID: 10401 RVA: 0x000CA9A4 File Offset: 0x000C8BA4
	[Networked]
	[NetworkedWeaved(0, 5)]
	public unsafe HalloweenGhostChaser.GhostData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HalloweenGhostChaser.GhostData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HalloweenGhostChaser.GhostData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000CA9D0 File Offset: 0x000C8BD0
	public override void WriteDataFusion()
	{
		HalloweenGhostChaser.GhostData ghostData = default(HalloweenGhostChaser.GhostData);
		NetPlayer netPlayer = this.targetPlayer;
		ghostData.TargetActorNumber = ((netPlayer != null) ? netPlayer.ActorNumber : (-1));
		ghostData.CurrentState = (int)this.currentState;
		ghostData.SpawnIndex = this.spawnIndex;
		ghostData.CurrentSpeed = this.currentSpeed;
		ghostData.IsSummoned = this.isSummoned;
		this.Data = ghostData;
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000CAA40 File Offset: 0x000C8C40
	public override void ReadDataFusion()
	{
		int targetActorNumber = this.Data.TargetActorNumber;
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		this.currentState = (HalloweenGhostChaser.ChaseState)this.Data.CurrentState;
		this.spawnIndex = this.Data.SpawnIndex;
		float num = this.Data.CurrentSpeed;
		this.isSummoned = this.Data.IsSummoned;
		if (float.IsFinite(num))
		{
			this.currentSpeed = num;
		}
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000CAAC0 File Offset: 0x000C8CC0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		if (this.targetPlayer == null)
		{
			stream.SendNext(-1);
		}
		else
		{
			stream.SendNext(this.targetPlayer.ActorNumber);
		}
		stream.SendNext(this.currentState);
		stream.SendNext(this.spawnIndex);
		stream.SendNext(this.currentSpeed);
		stream.SendNext(this.isSummoned);
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000CAB5C File Offset: 0x000C8D5C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num);
		this.currentState = (HalloweenGhostChaser.ChaseState)stream.ReceiveNext();
		this.spawnIndex = (int)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		this.isSummoned = (bool)stream.ReceiveNext();
		if (float.IsFinite(num2))
		{
			this.currentSpeed = num2;
		}
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000CABF1 File Offset: 0x000C8DF1
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000CAC0F File Offset: 0x000C8E0F
	public void OnJoinedRoom()
	{
		Debug.Log("Here");
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeGhost();
			return;
		}
		this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000CACDF File Offset: 0x000C8EDF
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000CACF7 File Offset: 0x000C8EF7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04002D58 RID: 11608
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x04002D59 RID: 11609
	public Transform followTarget;

	// Token: 0x04002D5A RID: 11610
	public Transform childGhost;

	// Token: 0x04002D5B RID: 11611
	public float velocityStep = 1f;

	// Token: 0x04002D5C RID: 11612
	public float currentSpeed;

	// Token: 0x04002D5D RID: 11613
	public float velocityIncreaseTime = 20f;

	// Token: 0x04002D5E RID: 11614
	public float riseDistance = 2f;

	// Token: 0x04002D5F RID: 11615
	public float summonDistance = 5f;

	// Token: 0x04002D60 RID: 11616
	public float timeEncircled;

	// Token: 0x04002D61 RID: 11617
	public float lastSummonCheck;

	// Token: 0x04002D62 RID: 11618
	public float timeGongStarted;

	// Token: 0x04002D63 RID: 11619
	public float summoningDuration = 30f;

	// Token: 0x04002D64 RID: 11620
	public float summoningCheckCountdown = 5f;

	// Token: 0x04002D65 RID: 11621
	public float gongDuration = 5f;

	// Token: 0x04002D66 RID: 11622
	public int summonCount = 5;

	// Token: 0x04002D67 RID: 11623
	public bool wasSurroundedLastCheck;

	// Token: 0x04002D68 RID: 11624
	public AudioSource laugh;

	// Token: 0x04002D69 RID: 11625
	public List<NetPlayer> possibleTarget;

	// Token: 0x04002D6A RID: 11626
	public AudioClip defaultLaugh;

	// Token: 0x04002D6B RID: 11627
	public AudioClip deepLaugh;

	// Token: 0x04002D6C RID: 11628
	public AudioClip gong;

	// Token: 0x04002D6D RID: 11629
	public Vector3 noisyOffset;

	// Token: 0x04002D6E RID: 11630
	public Vector3 leftArmGrabbingLocal;

	// Token: 0x04002D6F RID: 11631
	public Vector3 rightArmGrabbingLocal;

	// Token: 0x04002D70 RID: 11632
	public Vector3 leftHandGrabbingLocal;

	// Token: 0x04002D71 RID: 11633
	public Vector3 rightHandGrabbingLocal;

	// Token: 0x04002D72 RID: 11634
	public Vector3 leftHandStartingLocal;

	// Token: 0x04002D73 RID: 11635
	public Vector3 rightHandStartingLocal;

	// Token: 0x04002D74 RID: 11636
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04002D75 RID: 11637
	public Vector3 ghostStartingEulerRotation;

	// Token: 0x04002D76 RID: 11638
	public Vector3 ghostGrabbingEulerRotation;

	// Token: 0x04002D77 RID: 11639
	public float maxTimeToNextHeadAngle;

	// Token: 0x04002D78 RID: 11640
	public float lastHeadAngleTime;

	// Token: 0x04002D79 RID: 11641
	public float nextHeadAngleTime;

	// Token: 0x04002D7A RID: 11642
	public float nextTimeToChasePlayer;

	// Token: 0x04002D7B RID: 11643
	public float maxNextTimeToChasePlayer;

	// Token: 0x04002D7C RID: 11644
	public float timeRiseStarted;

	// Token: 0x04002D7D RID: 11645
	public float totalTimeToRise;

	// Token: 0x04002D7E RID: 11646
	public float catchDistance;

	// Token: 0x04002D7F RID: 11647
	public float grabTime;

	// Token: 0x04002D80 RID: 11648
	public float grabDuration;

	// Token: 0x04002D81 RID: 11649
	public float grabSpeed = 1f;

	// Token: 0x04002D82 RID: 11650
	public float minGrabCooldown;

	// Token: 0x04002D83 RID: 11651
	public float lastSpeedIncreased;

	// Token: 0x04002D84 RID: 11652
	public Vector3[] headEulerAngles;

	// Token: 0x04002D85 RID: 11653
	public Transform skullTransform;

	// Token: 0x04002D86 RID: 11654
	public Transform leftArm;

	// Token: 0x04002D87 RID: 11655
	public Transform rightArm;

	// Token: 0x04002D88 RID: 11656
	public Transform leftHand;

	// Token: 0x04002D89 RID: 11657
	public Transform rightHand;

	// Token: 0x04002D8A RID: 11658
	public Transform[] spawnTransforms;

	// Token: 0x04002D8B RID: 11659
	public Transform[] spawnTransformOffsets;

	// Token: 0x04002D8C RID: 11660
	public NetPlayer targetPlayer;

	// Token: 0x04002D8D RID: 11661
	public GameObject ghostBody;

	// Token: 0x04002D8E RID: 11662
	public HalloweenGhostChaser.ChaseState currentState;

	// Token: 0x04002D8F RID: 11663
	public HalloweenGhostChaser.ChaseState lastState;

	// Token: 0x04002D90 RID: 11664
	public int spawnIndex;

	// Token: 0x04002D91 RID: 11665
	public NetPlayer grabbedPlayer;

	// Token: 0x04002D92 RID: 11666
	public Material ghostMaterial;

	// Token: 0x04002D93 RID: 11667
	public Color defaultColor;

	// Token: 0x04002D94 RID: 11668
	public Color summonedColor;

	// Token: 0x04002D95 RID: 11669
	public bool isSummoned;

	// Token: 0x04002D96 RID: 11670
	private bool targetIsOnNavMesh;

	// Token: 0x04002D97 RID: 11671
	private const float navMeshSampleRange = 5f;

	// Token: 0x04002D98 RID: 11672
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x04002D99 RID: 11673
	public float hapticDuration = 1.5f;

	// Token: 0x04002D9A RID: 11674
	private NavMeshPath path;

	// Token: 0x04002D9B RID: 11675
	public List<Vector3> points;

	// Token: 0x04002D9C RID: 11676
	public int currentTargetIdx;

	// Token: 0x04002D9D RID: 11677
	private float nextPathTimestamp;

	// Token: 0x04002D9E RID: 11678
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 5)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HalloweenGhostChaser.GhostData _Data;

	// Token: 0x02000658 RID: 1624
	public enum ChaseState
	{
		// Token: 0x04002DA0 RID: 11680
		Dormant = 1,
		// Token: 0x04002DA1 RID: 11681
		InitialRise,
		// Token: 0x04002DA2 RID: 11682
		Gong = 4,
		// Token: 0x04002DA3 RID: 11683
		Chasing = 8,
		// Token: 0x04002DA4 RID: 11684
		Grabbing = 16
	}

	// Token: 0x02000659 RID: 1625
	[NetworkStructWeaved(5)]
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct GhostData : INetworkStruct
	{
		// Token: 0x170003DE RID: 990
		// (get) Token: 0x060028AB RID: 10411 RVA: 0x000CAD0B File Offset: 0x000C8F0B
		// (set) Token: 0x060028AC RID: 10412 RVA: 0x000CAD19 File Offset: 0x000C8F19
		[Networked]
		public unsafe float CurrentSpeed
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed) = value;
			}
		}

		// Token: 0x04002DA5 RID: 11685
		[FieldOffset(0)]
		public int TargetActorNumber;

		// Token: 0x04002DA6 RID: 11686
		[FieldOffset(4)]
		public int CurrentState;

		// Token: 0x04002DA7 RID: 11687
		[FieldOffset(8)]
		public int SpawnIndex;

		// Token: 0x04002DA8 RID: 11688
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ReaderWriter@System_Single), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(12)]
		private FixedStorage@1 _CurrentSpeed;

		// Token: 0x04002DA9 RID: 11689
		[FieldOffset(16)]
		public NetworkBool IsSummoned;
	}
}
