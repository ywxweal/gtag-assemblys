using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020005A2 RID: 1442
public class GREnemyChaser : MonoBehaviour
{
	// Token: 0x06002332 RID: 9010 RVA: 0x000AFD28 File Offset: 0x000ADF28
	public static GREnemyChaser Get(GameEntityId id)
	{
		GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(id);
		if (!(gameEntity == null))
		{
			return gameEntity.GetComponent<GREnemyChaser>();
		}
		return null;
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000AFD54 File Offset: 0x000ADF54
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.visibilityLayerMask = LayerMask.GetMask(new string[] { "Default" });
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.InitializeRandoms();
		this.abilityStagger.Setup(Vector3.zero, this.agent, this.anim, base.transform, this.rigidBody);
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x000AFE34 File Offset: 0x000AE034
	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Unity.Mathematics.Random((uint)(this.entity.id.GetNetId() + 1));
		this.patrolGroanSoundRandom = new Unity.Mathematics.Random((uint)(this.entity.id.GetNetId() + 10));
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x000AFE71 File Offset: 0x000AE071
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x000AFEA4 File Offset: 0x000AE0A4
	public void Setup(int patrolPathId)
	{
		this.SetPatrolPath(patrolPathId);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, true);
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[this.nextPatrolNode];
		}
		else
		{
			this.SetBehavior(GREnemyChaser.Behavior.Idle, true);
		}
		if (this.hpShellMax > 0)
		{
			this.SetBodyState(GREnemyChaser.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyChaser.BodyState.Bones, true);
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x000AFF28 File Offset: 0x000AE128
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 8)
		{
			return;
		}
		this.SetBehavior((GREnemyChaser.Behavior)newState, false);
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x000AFF3B File Offset: 0x000AE13B
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyChaser.BodyState)newState, false);
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x000AFF50 File Offset: 0x000AE150
	public void SetPatrolPath(int patrolPathId)
	{
		GRPatrolPath grpatrolPath = GhostReactor.instance.GetPatrolPath(patrolPathId);
		this.patrolPath = grpatrolPath;
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x000AFF70 File Offset: 0x000AE170
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x000AFF79 File Offset: 0x000AE179
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x000AFF84 File Offset: 0x000AE184
	public void SetBehavior(GREnemyChaser.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.behaviorEndTime = 1.0;
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.agent.SetIsPathing(true, true);
			this.agent.SetDisableNetworkSync(false);
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyChaserIdle", 0.3f, 1f);
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyChaserCrawl", 0.3f, 0.5f);
			this.navAgent.speed = this.patrolSpeed;
			this.CalculateNextPatrolGroan();
			break;
		case GREnemyChaser.Behavior.Search:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyChaserIdle", 0.3f, 1f);
			this.navAgent.speed = this.patrolSpeed;
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.PlayAnim("GREnemyChaserIdle", 0.1f, 1f);
			this.behaviorEndTime = 1.0;
			if (GameEntityManager.instance.IsAuthority())
			{
				GameEntityManager.instance.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			break;
		case GREnemyChaser.Behavior.Chase:
			this.PlayAnim("GREnemyChaserCrawl", 0.1f, 1.5f);
			this.navAgent.speed = this.chaseSpeed;
			break;
		case GREnemyChaser.Behavior.Attack:
			this.PlayAnim("GREnemyChaserAttack", 0.1f, 1f);
			this.navAgent.speed = 0.1f;
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.PlayAnim("GREnemyChaserFlashed", 0.1f, 1f);
			this.behaviorEndTime = Time.timeAsDouble + 0.25;
			this.agent.SetIsPathing(false, true);
			this.agent.SetDisableNetworkSync(true);
			break;
		}
		this.RefreshBody();
		if (GameEntityManager.instance.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000B020C File Offset: 0x000AE40C
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000B023C File Offset: 0x000AE43C
	public void SetBodyState(GREnemyChaser.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
		{
			GameObject gameObject = this.fxDeath;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			this.agent.SetIsPathing(true, true);
			for (int i = 0; i < this.colliders.Count; i++)
			{
				this.colliders[i].enabled = true;
			}
			break;
		}
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.hpBonesMax;
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.hpShellMax;
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
		{
			this.agent.SetIsPathing(false, true);
			for (int j = 0; j < this.colliders.Count; j++)
			{
				this.colliders[j].enabled = false;
			}
			GhostReactorManager.instance.ReportEnemyDeath();
			break;
		}
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.hpBonesMax;
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.hpShellMax;
			break;
		}
		this.RefreshBody();
		if (GameEntityManager.instance.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000B0374 File Offset: 0x000AE574
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemyChaser.Hide(this.bones, true);
			GREnemyChaser.Hide(this.always, true);
			return;
		case GREnemyChaser.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemyChaser.Hide(this.bones, false);
			GREnemyChaser.Hide(this.always, false);
			return;
		case GREnemyChaser.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemyChaser.Hide(this.bones, true);
			GREnemyChaser.Hide(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000B040E File Offset: 0x000AE60E
	public void CalculateNextPatrolGroan()
	{
		if (this.lastPartrolAmbientSoundTime < this.lastStateChange)
		{
			this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
		}
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000B0444 File Offset: 0x000AE644
	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x000B049A File Offset: 0x000AE69A
	private void Update()
	{
		this.OnThink();
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000B04B0 File Offset: 0x000AE6B0
	public void OnThink()
	{
		if (!GameEntityManager.instance.IsAuthority())
		{
			return;
		}
		float num = float.MaxValue;
		GRPlayer grplayer = null;
		NetPlayer netPlayer = null;
		if (this.currBehavior == GREnemyChaser.Behavior.Patrol || this.currBehavior == GREnemyChaser.Behavior.Search)
		{
			GREnemyChaser.tempRigs.Clear();
			GREnemyChaser.tempRigs.Add(VRRig.LocalRig);
			VRRigCache.Instance.GetAllUsedRigs(GREnemyChaser.tempRigs);
			Vector3 position = this.headTransform.position;
			Vector3 vector = this.headTransform.rotation * Vector3.forward;
			float num2 = this.sightDist * this.sightDist;
			float num3 = Mathf.Cos(this.sightFOV);
			for (int i = 0; i < GREnemyChaser.tempRigs.Count; i++)
			{
				VRRig vrrig = GREnemyChaser.tempRigs[i];
				GRPlayer component = vrrig.GetComponent<GRPlayer>();
				if (component.State != GRPlayer.GRPlayerState.Ghost)
				{
					Vector3 mouthPosition = vrrig.GetMouthPosition();
					Vector3 vector2 = mouthPosition - position;
					float sqrMagnitude = vector2.sqrMagnitude;
					if (sqrMagnitude <= num2)
					{
						float num4 = 0f;
						if (sqrMagnitude > 0f)
						{
							num4 = Mathf.Sqrt(sqrMagnitude);
							if (Vector3.Dot(vector2 / num4, vector) < num3)
							{
								goto IL_0180;
							}
						}
						if (num4 < num && Physics.RaycastNonAlloc(new Ray(this.headTransform.position, mouthPosition - this.headTransform.position), GREnemyChaser.visibilityHits, Mathf.Min(Vector3.Distance(mouthPosition, this.headTransform.position), this.sightDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1)
						{
							num = num4;
							grplayer = component;
							netPlayer = vrrig.OwningNetPlayer;
						}
					}
				}
				IL_0180:;
			}
			if (grplayer != null)
			{
				this.targetPlayer = netPlayer;
				this.chaseSoundBank.Play();
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
			}
		}
		if (this.currBehavior == GREnemyChaser.Behavior.Chase)
		{
			bool flag = true;
			if (this.targetPlayer != null)
			{
				GRPlayer grplayer2 = GRPlayer.Get(this.targetPlayer.ActorNumber);
				if (grplayer2 != null && grplayer2.State == GRPlayer.GRPlayerState.Alive)
				{
					float num5 = this.loseSightDist * this.loseSightDist;
					Vector3 position2 = grplayer2.transform.position;
					Vector3 position3 = base.transform.position;
					float sqrMagnitude2 = (position2 - position3).sqrMagnitude;
					if (sqrMagnitude2 < num5)
					{
						flag = false;
					}
					if (Physics.RaycastNonAlloc(new Ray(this.headTransform.position, position2 - this.headTransform.position), GREnemyChaser.visibilityHits, Mathf.Min(Vector3.Distance(position2, this.headTransform.position), this.sightDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1)
					{
						this.lastSeenTargetPosition = position2;
						this.lastSeenTargetTime = Time.timeAsDouble;
					}
					if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
					{
						this.searchPosition = position2;
					}
					else
					{
						flag = true;
					}
					this.agent.RequestDestination(this.lastSeenTargetPosition);
					if (sqrMagnitude2 < 4f)
					{
						this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
					}
				}
			}
			if (flag)
			{
				this.SetBehavior(GREnemyChaser.Behavior.Search, false);
			}
		}
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000B07BB File Offset: 0x000AE9BB
	public void OnUpdate(float dt)
	{
		if (GameEntityManager.instance.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000B07DC File Offset: 0x000AE9DC
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Patrol:
			this.UpdatePatrol();
			break;
		case GREnemyChaser.Behavior.Search:
			this.UpdateSearch();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				}
			}
			break;
		case GREnemyChaser.Behavior.Attack:
			this.UpdateAttack();
			break;
		case GREnemyChaser.Behavior.Flashed:
			if (Time.timeAsDouble >= this.behaviorEndTime)
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				}
			}
			break;
		}
		GREnemyChaser.UpdateFacing(base.transform, this.navAgent, this.targetPlayer);
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000B08A4 File Offset: 0x000AEAA4
	public static void UpdateFacing(Transform transform, NavMeshAgent navAgent, NetPlayer targetPlayer)
	{
		Vector3 vector = transform.forward;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				Vector3 position = grplayer.transform.position;
				Vector3 position2 = transform.position;
				Vector3 vector2 = position - position2;
				vector2.y = 0f;
				float magnitude = vector2.magnitude;
				if (magnitude > 0f)
				{
					vector = vector2 / magnitude;
				}
			}
		}
		else
		{
			Vector3 desiredVelocity = navAgent.desiredVelocity;
			desiredVelocity.y = 0f;
			float magnitude2 = desiredVelocity.magnitude;
			if (magnitude2 > 0f)
			{
				vector = desiredVelocity / magnitude2;
			}
		}
		transform.rotation = Quaternion.LookRotation(vector);
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000B0958 File Offset: 0x000AEB58
	public void OnUpdateRemote(float dt)
	{
		GREnemyChaser.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyChaser.Behavior.Patrol)
		{
			if (behavior == GREnemyChaser.Behavior.Stagger)
			{
				this.abilityStagger.Update(dt);
				return;
			}
		}
		else
		{
			this.UpdatePatrolRemote();
		}
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000B0988 File Offset: 0x000AEB88
	public void UpdatePatrol()
	{
		if (this.patrolPath == null)
		{
			this.SetBehavior(GREnemyChaser.Behavior.Idle, false);
			return;
		}
		if (this.target == null)
		{
			return;
		}
		if ((this.target.transform.position - base.transform.position).sqrMagnitude < 0.25f)
		{
			this.nextPatrolNode = (this.nextPatrolNode + 1) % this.patrolPath.patrolNodes.Count;
			this.target = this.patrolPath.patrolNodes[this.nextPatrolNode];
		}
		if (this.target != null)
		{
			this.agent.RequestDestination(this.target.transform.position);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x000B0A61 File Offset: 0x000AEC61
	public void UpdatePatrolRemote()
	{
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000B0A78 File Offset: 0x000AEC78
	public void UpdateSearch()
	{
		Vector3 vector = this.searchPosition - base.transform.position;
		Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude < 0.15f)
		{
			Vector3 vector3 = this.lastSeenTargetPosition - this.searchPosition;
			vector3.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector3;
		}
		this.agent.RequestDestination(this.searchPosition);
		if (Time.timeAsDouble - this.lastSeenTargetTime > (double)this.searchTime)
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, false);
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000B0B20 File Offset: 0x000AED20
	private void UpdateAttack()
	{
		if (Time.timeAsDouble - this.behaviorStartTime > 0.20000000298023224)
		{
			this.navAgent.velocity = this.navAgent.velocity.normalized * this.chaseSpeed * 2f;
			this.navAgent.speed = this.chaseSpeed * 2f;
		}
		if (Time.timeAsDouble - this.behaviorStartTime > 0.75)
		{
			this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
		}
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000B0BB0 File Offset: 0x000AEDB0
	public void OnHitByClub(GRTool tool, Vector3 startPos, Vector3 impulse)
	{
		if (this.currBodyState != GREnemyChaser.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyChaser.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(startPos);
			}
			return;
		}
		this.hp--;
		this.audioSource.PlayOneShot(this.damagedSound, this.damagedSoundVolume);
		if (this.fxDamaged != null)
		{
			this.fxDamaged.SetActive(false);
			this.fxDamaged.SetActive(true);
		}
		if (this.hp <= 0)
		{
			this.audioSource.PlayOneShot(this.deathSound, this.deathSoundVolume);
			if (this.fxDeath != null)
			{
				this.fxDeath.SetActive(false);
				this.fxDeath.SetActive(true);
			}
			this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		Vector3 vector2 = impulse;
		vector2.y = 0f;
		Vector3 vector3 = vector2;
		this.abilityStagger.Setup(vector3, this.agent, this.anim, base.transform, this.rigidBody);
		this.SetBehavior(GREnemyChaser.Behavior.Stagger, false);
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x000B0D2C File Offset: 0x000AEF2C
	public void OnHitByFlash(GRTool grTool)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Shell)
		{
			this.hp--;
			if (this.armor != null)
			{
				this.armor.SetHp(this.hp);
			}
			if (this.hp <= 0)
			{
				if (this.armor != null)
				{
					this.armor.PlayDestroyFx(this.armor.transform.position);
				}
				this.SetBodyState(GREnemyChaser.BodyState.Bones, false);
			}
			else if (grTool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = grTool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.RefreshBody();
			}
			else
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.RefreshBody();
			}
		}
		this.SetBehavior(GREnemyChaser.Behavior.Flashed, false);
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x000B0E86 File Offset: 0x000AF086
	public void TryHitEnemy(GRTool tool, Vector3 startPos, Vector3 impulse)
	{
		this.agent.RequestImpact(tool, startPos, impulse, 0);
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x000B0E97 File Offset: 0x000AF097
	public void TryFlashEnemy(GRTool tool)
	{
		this.agent.RequestImpact(tool, tool.transform.position, tool.transform.eulerAngles, 1);
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x000B0EBC File Offset: 0x000AF0BC
	public void OnImpact(GRTool tool, Vector3 startPos, Vector3 impulse, byte impulseData)
	{
		if (impulseData == 0)
		{
			this.OnHitByClub(tool, startPos, impulse);
			return;
		}
		this.OnHitByFlash(tool);
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x000B0ED4 File Offset: 0x000AF0D4
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyChaser.Behavior.Attack)
		{
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
			if (component != null && component.gamePlayer.IsLocal())
			{
				GhostReactorManager.instance.RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, component);
			}
			GRBreakable component2 = attachedRigidbody.GetComponent<GRBreakable>();
			if (component2 != null)
			{
				component2.TryHit(null);
			}
		}
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x000B0F4F File Offset: 0x000AF14F
	public void HitPlayer(GRPlayer player)
	{
		if (player.State == GRPlayer.GRPlayerState.Alive)
		{
			player.ChangePlayerState(GRPlayer.GRPlayerState.Ghost);
			player.PlayHitFx(base.transform.position);
		}
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x000B0F74 File Offset: 0x000AF174
	private bool FindNearestVisiblePlayer(out GRPlayer grPlayer, out NetPlayer netPlayer)
	{
		float num = float.MaxValue;
		GRPlayer grplayer = null;
		NetPlayer netPlayer2 = null;
		GREnemyChaser.tempRigs.Clear();
		GREnemyChaser.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyChaser.tempRigs);
		Vector3 position = this.headTransform.position;
		Vector3 vector = this.headTransform.rotation * Vector3.forward;
		float num2 = this.sightDist * this.sightDist;
		float num3 = Mathf.Cos(this.sightFOV);
		for (int i = 0; i < GREnemyChaser.tempRigs.Count; i++)
		{
			VRRig vrrig = GREnemyChaser.tempRigs[i];
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if (component.State != GRPlayer.GRPlayerState.Ghost)
			{
				Vector3 mouthPosition = vrrig.GetMouthPosition();
				Vector3 vector2 = mouthPosition - position;
				float sqrMagnitude = vector2.sqrMagnitude;
				if (sqrMagnitude <= num2)
				{
					float num4 = 0f;
					if (sqrMagnitude > 0f)
					{
						num4 = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(vector2 / num4, vector) < num3)
						{
							goto IL_015C;
						}
					}
					if (num4 < num && Physics.RaycastNonAlloc(new Ray(this.headTransform.position, mouthPosition - this.headTransform.position), GREnemyChaser.visibilityHits, Mathf.Min(Vector3.Distance(mouthPosition, this.headTransform.position), this.sightDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1)
					{
						num = num4;
						grplayer = component;
						netPlayer2 = vrrig.OwningNetPlayer;
					}
				}
			}
			IL_015C:;
		}
		if (grplayer != null && netPlayer2 != null)
		{
			grPlayer = grplayer;
			netPlayer = netPlayer2;
			return true;
		}
		grPlayer = null;
		netPlayer = null;
		return false;
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x000B1110 File Offset: 0x000AF310
	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x0400276A RID: 10090
	public GameEntity entity;

	// Token: 0x0400276B RID: 10091
	public GameAgent agent;

	// Token: 0x0400276C RID: 10092
	public GRArmorEnemy armor;

	// Token: 0x0400276D RID: 10093
	public Animation anim;

	// Token: 0x0400276E RID: 10094
	public GRAbilityStagger abilityStagger;

	// Token: 0x0400276F RID: 10095
	public List<Renderer> bones;

	// Token: 0x04002770 RID: 10096
	public List<Renderer> always;

	// Token: 0x04002771 RID: 10097
	public Transform coreMarker;

	// Token: 0x04002772 RID: 10098
	public GRCollectible corePrefab;

	// Token: 0x04002773 RID: 10099
	public Transform headTransform;

	// Token: 0x04002774 RID: 10100
	public int hpShellMax = 3;

	// Token: 0x04002775 RID: 10101
	public int hpBonesMax = 3;

	// Token: 0x04002776 RID: 10102
	public float sightDist;

	// Token: 0x04002777 RID: 10103
	public float loseSightDist;

	// Token: 0x04002778 RID: 10104
	public float sightFOV;

	// Token: 0x04002779 RID: 10105
	public float sightLostFollowStopTime = 0.5f;

	// Token: 0x0400277A RID: 10106
	public float searchTime = 5f;

	// Token: 0x0400277B RID: 10107
	public float chaseSpeed = 2f;

	// Token: 0x0400277C RID: 10108
	public Color chaseColor = Color.red;

	// Token: 0x0400277D RID: 10109
	public SoundBankPlayer chaseSoundBank;

	// Token: 0x0400277E RID: 10110
	public float patrolSpeed = 1f;

	// Token: 0x0400277F RID: 10111
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04002780 RID: 10112
	public NavMeshAgent navAgent;

	// Token: 0x04002781 RID: 10113
	public AudioSource audioSource;

	// Token: 0x04002782 RID: 10114
	public AudioClip damagedSound;

	// Token: 0x04002783 RID: 10115
	public float damagedSoundVolume;

	// Token: 0x04002784 RID: 10116
	public GameObject fxDamaged;

	// Token: 0x04002785 RID: 10117
	public AudioClip deathSound;

	// Token: 0x04002786 RID: 10118
	public float deathSoundVolume;

	// Token: 0x04002787 RID: 10119
	public GameObject fxDeath;

	// Token: 0x04002788 RID: 10120
	public double lastStateChange;

	// Token: 0x04002789 RID: 10121
	public float ambientSoundVolume = 0.5f;

	// Token: 0x0400278A RID: 10122
	public double ambientSoundDelayMin = 5.0;

	// Token: 0x0400278B RID: 10123
	public double ambientSoundDelayMax = 10.0;

	// Token: 0x0400278C RID: 10124
	public AudioClip[] ambientPatrolSounds;

	// Token: 0x0400278D RID: 10125
	private double lastPartrolAmbientSoundTime;

	// Token: 0x0400278E RID: 10126
	private double nextPatrolGroanTime;

	// Token: 0x0400278F RID: 10127
	private Unity.Mathematics.Random patrolGroanSoundDelayRandom;

	// Token: 0x04002790 RID: 10128
	private Unity.Mathematics.Random patrolGroanSoundRandom;

	// Token: 0x04002791 RID: 10129
	private Transform target;

	// Token: 0x04002792 RID: 10130
	[ReadOnly]
	public int hp;

	// Token: 0x04002793 RID: 10131
	[ReadOnly]
	public GREnemyChaser.Behavior currBehavior;

	// Token: 0x04002794 RID: 10132
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04002795 RID: 10133
	[ReadOnly]
	public GREnemyChaser.BodyState currBodyState;

	// Token: 0x04002796 RID: 10134
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04002797 RID: 10135
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04002798 RID: 10136
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04002799 RID: 10137
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x0400279A RID: 10138
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x0400279B RID: 10139
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x0400279C RID: 10140
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x0400279D RID: 10141
	private LayerMask visibilityLayerMask;

	// Token: 0x0400279E RID: 10142
	private Rigidbody rigidBody;

	// Token: 0x0400279F RID: 10143
	private List<Collider> colliders;

	// Token: 0x040027A0 RID: 10144
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x020005A3 RID: 1443
	public enum HitType
	{
		// Token: 0x040027A2 RID: 10146
		Club,
		// Token: 0x040027A3 RID: 10147
		Flash
	}

	// Token: 0x020005A4 RID: 1444
	public enum Behavior
	{
		// Token: 0x040027A5 RID: 10149
		Idle,
		// Token: 0x040027A6 RID: 10150
		Patrol,
		// Token: 0x040027A7 RID: 10151
		Search,
		// Token: 0x040027A8 RID: 10152
		Stagger,
		// Token: 0x040027A9 RID: 10153
		Dying,
		// Token: 0x040027AA RID: 10154
		Chase,
		// Token: 0x040027AB RID: 10155
		Attack,
		// Token: 0x040027AC RID: 10156
		Flashed,
		// Token: 0x040027AD RID: 10157
		Count
	}

	// Token: 0x020005A5 RID: 1445
	public enum BodyState
	{
		// Token: 0x040027AF RID: 10159
		Destroyed,
		// Token: 0x040027B0 RID: 10160
		Bones,
		// Token: 0x040027B1 RID: 10161
		Shell,
		// Token: 0x040027B2 RID: 10162
		Count
	}
}
