using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x020005A6 RID: 1446
public class GREnemyRanged : MonoBehaviour
{
	// Token: 0x06002357 RID: 9047 RVA: 0x000B11D0 File Offset: 0x000AF3D0
	private void SoftResetThrowableHead()
	{
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
		this.headLightReset = true;
		this.spitterLightTurnOffTime = Time.timeAsDouble + this.spitterLightTurnOffDelay;
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000B123C File Offset: 0x000AF43C
	private void ForceResetThrowableHead()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x000B12A0 File Offset: 0x000AF4A0
	private void EnableVFXForShoulderHead()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(true);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000B12FC File Offset: 0x000AF4FC
	private void EnableVFXForHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(true);
		this.spitterHeadInHandLight.SetActive(true);
		this.spitterHeadInHandVFX.SetActive(true);
	}

	// Token: 0x0600235B RID: 9051 RVA: 0x000B1358 File Offset: 0x000AF558
	private void DisableHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadInHand.SetActive(false);
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x000B1370 File Offset: 0x000AF570
	private void DisableHeadOnShoulderAndHeadInHand()
	{
		this.headLightReset = false;
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x000B13D4 File Offset: 0x000AF5D4
	public static GREnemyRanged Get(GameEntityId id)
	{
		GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(id);
		if (!(gameEntity == null))
		{
			return gameEntity.GetComponent<GREnemyRanged>();
		}
		return null;
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000B1400 File Offset: 0x000AF600
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.visibilityLayerMask = LayerMask.GetMask(new string[] { "Default" });
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.InitializeRandoms();
		this.abilityStagger.Setup(Vector3.zero, this.agent, this.anim, base.transform, this.rigidBody);
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000B14D1 File Offset: 0x000AF6D1
	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Unity.Mathematics.Random((uint)(this.entity.id.GetNetId() + 1));
		this.patrolGroanSoundRandom = new Unity.Mathematics.Random((uint)(this.entity.id.GetNetId() + 10));
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000B150E File Offset: 0x000AF70E
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000B1540 File Offset: 0x000AF740
	public void Setup(int patrolPathId)
	{
		this.SetPatrolPath(patrolPathId);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, true);
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[this.nextPatrolNode];
		}
		else
		{
			this.SetBehavior(GREnemyRanged.Behavior.Idle, true);
		}
		if (this.hpShellMax > 0)
		{
			this.SetBodyState(GREnemyRanged.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyRanged.BodyState.Bones, true);
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000B15C4 File Offset: 0x000AF7C4
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 9)
		{
			return;
		}
		this.SetBehavior((GREnemyRanged.Behavior)newState, false);
	}

	// Token: 0x06002363 RID: 9059 RVA: 0x000B15D8 File Offset: 0x000AF7D8
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyRanged.BodyState)newState, false);
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x000B15EB File Offset: 0x000AF7EB
	public void SetPatrolPath(int patrolPathId)
	{
		this.patrolPath = GhostReactor.instance.GetPatrolPath(patrolPathId);
	}

	// Token: 0x06002365 RID: 9061 RVA: 0x000B15FE File Offset: 0x000AF7FE
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x000B1607 File Offset: 0x000AF807
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000B1610 File Offset: 0x000AF810
	public void SetBehavior(GREnemyRanged.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.behaviorEndTime = 1.0;
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttack)
			{
				this.SoftResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			this.ForceResetThrowableHead();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.agent.SetIsPathing(true, true);
			this.agent.SetDisableNetworkSync(false);
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedIdle", 0.1f, 1f);
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 0.5f);
			this.navAgent.speed = this.patrolSpeed;
			this.CalculateNextPatrolGroan();
			break;
		case GREnemyRanged.Behavior.Search:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedIdle", 0.1f, 1f);
			this.navAgent.speed = this.patrolSpeed;
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.PlayAnim("GREnemyRangedIdle", 0.1f, 1f);
			this.behaviorEndTime = 1.0;
			if (GameEntityManager.instance.IsAuthority())
			{
				GameEntityManager.instance.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 1f);
			this.navAgent.speed = this.chaseSpeed;
			this.EnableVFXForShoulderHead();
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			this.PlayAnim("GREnemyRangedAttack", 0.1f, 1f);
			this.navAgent.speed = 0f;
			this.navAgent.velocity = Vector3.zero;
			this.headRemovaltime = PhotonNetwork.Time + (double)this.headRemovalFrame;
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.PlayAnim("GREnemyRangedCrawl", 0.1f, 1f);
			this.navAgent.speed = this.chaseSpeed;
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.PlayAnim("GREnemyRangedFlashed", 0.1f, 1f);
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

	// Token: 0x06002368 RID: 9064 RVA: 0x000B1900 File Offset: 0x000AFB00
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x000B1930 File Offset: 0x000AFB30
	public void SetBodyState(GREnemyRanged.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
		{
			GameObject gameObject = this.fxDeath;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			this.agent.SetIsPathing(true, true);
			this.ForceResetThrowableHead();
			for (int i = 0; i < this.colliders.Count; i++)
			{
				this.colliders[i].enabled = true;
			}
			break;
		}
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.hpBonesMax;
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.hpShellMax;
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
		{
			this.agent.SetIsPathing(false, true);
			this.DisableHeadOnShoulderAndHeadInHand();
			for (int j = 0; j < this.colliders.Count; j++)
			{
				this.colliders[j].enabled = false;
			}
			GhostReactorManager.instance.ReportEnemyDeath();
			break;
		}
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.hpBonesMax;
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.hpShellMax;
			break;
		}
		this.RefreshBody();
		if (GameEntityManager.instance.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x000B1A74 File Offset: 0x000AFC74
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemyRanged.Hide(this.bones, true);
			GREnemyRanged.Hide(this.always, true);
			return;
		case GREnemyRanged.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemyRanged.Hide(this.bones, false);
			GREnemyRanged.Hide(this.always, false);
			return;
		case GREnemyRanged.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemyRanged.Hide(this.bones, true);
			GREnemyRanged.Hide(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600236B RID: 9067 RVA: 0x000B1B0E File Offset: 0x000AFD0E
	public void CalculateNextPatrolGroan()
	{
		if (this.lastPartrolAmbientSoundTime < this.lastStateChange)
		{
			this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
		}
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000B1B44 File Offset: 0x000AFD44
	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x0600236D RID: 9069 RVA: 0x000B1B9A File Offset: 0x000AFD9A
	private void Update()
	{
		if (GameEntityManager.instance.IsAuthority())
		{
			this.UpdateAwareness();
			this.OnUpdateAuthority(Time.deltaTime);
		}
		else
		{
			this.OnUpdateRemote(Time.deltaTime);
		}
		this.UpdateShared();
	}

	// Token: 0x0600236E RID: 9070 RVA: 0x000B1BD0 File Offset: 0x000AFDD0
	public void UpdateAwareness()
	{
		if (!GameEntityManager.instance.IsAuthority())
		{
			return;
		}
		float num = float.MaxValue;
		this.bestTargetPlayer = null;
		this.bestTargetNetPlayer = null;
		if (this.currBehavior == GREnemyRanged.Behavior.Patrol || this.currBehavior == GREnemyRanged.Behavior.Search)
		{
			GREnemyRanged.tempRigs.Clear();
			GREnemyRanged.tempRigs.Add(VRRig.LocalRig);
			VRRigCache.Instance.GetAllUsedRigs(GREnemyRanged.tempRigs);
			Vector3 position = base.transform.position;
			Vector3 vector = base.transform.rotation * Vector3.forward;
			float num2 = this.sightDist * this.sightDist;
			float num3 = Mathf.Cos(this.sightFOV);
			for (int i = 0; i < GREnemyRanged.tempRigs.Count; i++)
			{
				VRRig vrrig = GREnemyRanged.tempRigs[i];
				GRPlayer component = vrrig.GetComponent<GRPlayer>();
				if (component.State != GRPlayer.GRPlayerState.Ghost)
				{
					Vector3 vector2 = vrrig.transform.position - position;
					float sqrMagnitude = vector2.sqrMagnitude;
					if (sqrMagnitude <= num2)
					{
						float num4 = 0f;
						if (sqrMagnitude > 0f)
						{
							num4 = Mathf.Sqrt(sqrMagnitude);
							if (Vector3.Dot(vector2 / num4, vector) < num3)
							{
								goto IL_0168;
							}
						}
						if (num4 < num && Physics.RaycastNonAlloc(new Ray(this.headTransform.position, vector2), GREnemyChaser.visibilityHits, vector2.magnitude, this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1)
						{
							num = num4;
							this.bestTargetPlayer = component;
							this.bestTargetNetPlayer = vrrig.OwningNetPlayer;
						}
					}
				}
				IL_0168:;
			}
		}
	}

	// Token: 0x0600236F RID: 9071 RVA: 0x000B1D5C File Offset: 0x000AFF5C
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.UpdatePatrol();
			if (this.bestTargetPlayer != null)
			{
				this.targetPlayer = this.bestTargetNetPlayer;
				this.audioSource.clip = this.chaseSound;
				this.audioSource.volume = this.chaseSoundVolume;
				this.audioSource.Play();
				this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
			}
			break;
		case GREnemyRanged.Behavior.Search:
			this.UpdateSearch();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
		{
			bool flag = true;
			if (this.targetPlayer != null)
			{
				GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
				if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
				{
					Vector3 position = grplayer.transform.position;
					Vector3 position2 = base.transform.position;
					Vector3 vector = position - position2;
					float magnitude = vector.magnitude;
					if (magnitude < this.loseSightDist)
					{
						flag = false;
					}
					bool flag2 = Physics.RaycastNonAlloc(new Ray(this.headTransform.position, position - this.headTransform.position), GREnemyChaser.visibilityHits, Mathf.Min(Vector3.Distance(position, this.headTransform.position), this.sightDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore) < 1;
					if (flag2)
					{
						this.lastSeenTargetTime = Time.timeAsDouble;
					}
					if (flag2)
					{
						this.lastSeenTargetPosition = position;
						this.lastSeenTargetTime = Time.timeAsDouble;
					}
					if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
					{
						this.searchPosition = position;
					}
					else
					{
						flag = true;
					}
					this.agent.RequestDestination(this.lastSeenTargetPosition);
					Vector3 vector2 = vector / magnitude;
					float num = Mathf.Lerp(this.rangedAttackDistMin, this.rangedAttackDistMax, 0.5f);
					this.rangedFiringPosition = position - vector2 * num;
					this.rangedTargetPosition = position;
					Vector3 vector3 = Vector3.up * 0.4f;
					this.rangedTargetPosition += vector3;
					if (magnitude < this.rangedAttackDistMax)
					{
						this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackChargeTime;
						this.SetBehavior(GREnemyRanged.Behavior.RangedAttack, false);
						GhostReactorManager.instance.RequestFireProjectile(this.entity.id, this.rangedProjectileFirePoint.position, this.rangedTargetPosition, PhotonNetwork.Time + (double)this.rangedAttackChargeTime);
					}
				}
			}
			this.agent.RequestDestination(this.rangedFiringPosition);
			if (flag)
			{
				this.SetBehavior(GREnemyRanged.Behavior.Search, false);
			}
			break;
		}
		case GREnemyRanged.Behavior.RangedAttack:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				if (this.targetPlayer != null)
				{
					GRPlayer grplayer2 = GRPlayer.Get(this.targetPlayer.ActorNumber);
					if (grplayer2 != null && grplayer2.State == GRPlayer.GRPlayerState.Alive)
					{
						this.rangedTargetPosition = grplayer2.transform.position;
					}
				}
				this.agent.RequestDestination(this.rangedFiringPosition);
				this.SetBehavior(GREnemyRanged.Behavior.RangedAttackCooldown, false);
				this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackRecoverTime;
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				this.behaviorEndTime = Time.timeAsDouble;
			}
			this.agent.RequestDestination(this.rangedFiringPosition);
			break;
		case GREnemyRanged.Behavior.Flashed:
			if (Time.timeAsDouble >= this.behaviorEndTime)
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		}
		GREnemyChaser.UpdateFacing(base.transform, this.navAgent, this.targetPlayer);
	}

	// Token: 0x06002370 RID: 9072 RVA: 0x000B2134 File Offset: 0x000B0334
	public void OnUpdateRemote(float dt)
	{
		GREnemyRanged.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyRanged.Behavior.Patrol)
		{
			if (behavior == GREnemyRanged.Behavior.Stagger)
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

	// Token: 0x06002371 RID: 9073 RVA: 0x000B2164 File Offset: 0x000B0364
	public void UpdateShared()
	{
		if (this.rangedAttackQueued)
		{
			if (!this.headRemoved && PhotonNetwork.Time > this.headRemovaltime)
			{
				this.headRemoved = true;
				this.EnableVFXForHeadInHand();
			}
			if (PhotonNetwork.Time > this.queuedFiringTime)
			{
				this.rangedAttackQueued = false;
				this.FireRangedAttack(this.queuedFiringPosition, this.queuedTargetPosition);
			}
		}
		if (this.headLightReset && Time.timeAsDouble > this.spitterLightTurnOffTime)
		{
			this.spitterHeadOnShouldersLight.SetActive(false);
			this.headLightReset = false;
		}
		if (this.projectileHasImpacted && Time.timeAsDouble > this.projectileImpactTime + 2.0)
		{
			Object.Destroy(this.rangedProjectileInstance);
			this.rangedProjectileInstance = null;
		}
	}

	// Token: 0x06002372 RID: 9074 RVA: 0x000B221C File Offset: 0x000B041C
	public void UpdatePatrol()
	{
		if (this.patrolPath == null)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Idle, false);
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

	// Token: 0x06002373 RID: 9075 RVA: 0x000B22F5 File Offset: 0x000B04F5
	public void UpdatePatrolRemote()
	{
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x000B230C File Offset: 0x000B050C
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
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, false);
		}
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x000B23B4 File Offset: 0x000B05B4
	public void OnHitByClub(GRTool tool, Vector3 startPos, Vector3 impulse)
	{
		if (this.currBodyState != GREnemyRanged.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyRanged.BodyState.Shell && this.armor != null)
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
			this.SetBodyState(GREnemyRanged.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyRanged.Behavior.Dying, false);
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
		this.SetBehavior(GREnemyRanged.Behavior.Stagger, false);
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000B2530 File Offset: 0x000B0730
	public void OnHitByFlash(GRTool tool)
	{
		if (this.currBodyState == GREnemyRanged.BodyState.Shell)
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
				this.SetBodyState(GREnemyRanged.BodyState.Bones, false);
			}
			else if (tool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = tool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.SetBehavior(GREnemyRanged.Behavior.Search, false);
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
		this.SetBehavior(GREnemyRanged.Behavior.Flashed, false);
	}

	// Token: 0x06002377 RID: 9079 RVA: 0x000B2692 File Offset: 0x000B0892
	public void TryHitEnemy(GRTool tool, Vector3 startPos, Vector3 impulse)
	{
		this.agent.RequestImpact(tool, startPos, impulse, 0);
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x000B26A3 File Offset: 0x000B08A3
	public void TryFlashEnemy(GRTool tool)
	{
		this.agent.RequestImpact(tool, tool.transform.position, tool.transform.eulerAngles, 1);
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000B26C8 File Offset: 0x000B08C8
	public void OnImpact(GRTool tool, Vector3 startPos, Vector3 impulse, byte impulseData)
	{
		if (impulseData == 0)
		{
			this.OnHitByClub(tool, startPos, impulse);
			return;
		}
		this.OnHitByFlash(tool);
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000B0F2F File Offset: 0x000AF12F
	public void HitPlayer(GRPlayer player)
	{
		if (player.State == GRPlayer.GRPlayerState.Alive)
		{
			player.ChangePlayerState(GRPlayer.GRPlayerState.Ghost);
			player.PlayHitFx(base.transform.position);
		}
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000B26E0 File Offset: 0x000B08E0
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

	// Token: 0x0600237C RID: 9084 RVA: 0x000B2721 File Offset: 0x000B0921
	public void RequestRangedAttack(Vector3 firingPosition, Vector3 targetPosition, double fireTime)
	{
		this.rangedAttackQueued = true;
		this.queuedFiringTime = fireTime;
		this.queuedFiringPosition = firingPosition;
		this.queuedTargetPosition = targetPosition;
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000B2740 File Offset: 0x000B0940
	private void FireRangedAttack(Vector3 launchPosition, Vector3 targetPosition)
	{
		this.DisableHeadInHand();
		if (this.rangedProjectileInstance != null)
		{
			Object.Destroy(this.rangedProjectileInstance);
			this.rangedProjectileInstance = null;
		}
		this.rangedProjectileInstance = Object.Instantiate<GameObject>(this.rangedProjectilePrefab, launchPosition, Quaternion.identity);
		this.projectileHasImpacted = false;
		Collider componentInChildren = this.rangedProjectileInstance.GetComponentInChildren<Collider>();
		if (componentInChildren != null)
		{
			for (int i = 0; i < this.colliders.Count; i++)
			{
				Physics.IgnoreCollision(componentInChildren, this.colliders[i]);
			}
		}
		this.rangedProjectileInstance.GetComponent<CollisionEventNotifier>().CollisionEnterEvent += this.OnProjectileCollisionEnter;
		targetPosition.y = launchPosition.y;
		float num = Vector3.Distance(launchPosition, targetPosition);
		float num2 = 0.5f * Mathf.Asin(Mathf.Clamp01(9.8f * num / (this.projectileSpeed * this.projectileSpeed))) * 57.29578f;
		Vector3 vector = (targetPosition - launchPosition).normalized;
		vector = Quaternion.AngleAxis(num2, Vector3.Cross(vector, Vector3.up)) * vector;
		this.rangedProjectileInstance.GetComponent<Rigidbody>().velocity = vector * this.projectileSpeed;
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000B2870 File Offset: 0x000B0A70
	private void OnProjectileCollisionEnter(CollisionEventNotifier eventNotifier, Collision collision)
	{
		eventNotifier.CollisionEnterEvent -= this.OnProjectileCollisionEnter;
		ParticleSystem componentInChildren = eventNotifier.gameObject.GetComponentInChildren<ParticleSystem>();
		AudioSource componentInChildren2 = eventNotifier.gameObject.GetComponentInChildren<AudioSource>();
		if (componentInChildren != null)
		{
			componentInChildren.Play();
		}
		if (componentInChildren2 != null)
		{
			componentInChildren2.Play();
		}
		MeshRenderer componentInChildren3 = eventNotifier.gameObject.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren3 != null)
		{
			componentInChildren3.enabled = false;
		}
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		if ((VRRig.LocalRig.GetMouthPosition() - eventNotifier.gameObject.transform.position).sqrMagnitude < this.projectileHitRadius * this.projectileHitRadius)
		{
			GhostReactorManager.instance.RequestEnemyHitPlayer(GhostReactor.EnemyType.Ranged, this.entity.id, VRRig.LocalRig.GetComponent<GRPlayer>());
		}
	}

	// Token: 0x040027B3 RID: 10163
	public GameEntity entity;

	// Token: 0x040027B4 RID: 10164
	public GameAgent agent;

	// Token: 0x040027B5 RID: 10165
	public GRArmorEnemy armor;

	// Token: 0x040027B6 RID: 10166
	public Animation anim;

	// Token: 0x040027B7 RID: 10167
	public GRAbilityStagger abilityStagger;

	// Token: 0x040027B8 RID: 10168
	public List<Renderer> bones;

	// Token: 0x040027B9 RID: 10169
	public List<Renderer> always;

	// Token: 0x040027BA RID: 10170
	public Transform coreMarker;

	// Token: 0x040027BB RID: 10171
	public GRCollectible corePrefab;

	// Token: 0x040027BC RID: 10172
	public Transform headTransform;

	// Token: 0x040027BD RID: 10173
	public int hpShellMax = 3;

	// Token: 0x040027BE RID: 10174
	public int hpBonesMax = 3;

	// Token: 0x040027BF RID: 10175
	public float sightDist;

	// Token: 0x040027C0 RID: 10176
	public float loseSightDist;

	// Token: 0x040027C1 RID: 10177
	public float sightFOV;

	// Token: 0x040027C2 RID: 10178
	public float sightLostFollowStopTime = 0.5f;

	// Token: 0x040027C3 RID: 10179
	public float searchTime = 5f;

	// Token: 0x040027C4 RID: 10180
	public float chaseSpeed = 2f;

	// Token: 0x040027C5 RID: 10181
	public Color chaseColor = Color.red;

	// Token: 0x040027C6 RID: 10182
	public AudioClip chaseSound;

	// Token: 0x040027C7 RID: 10183
	public float chaseSoundVolume;

	// Token: 0x040027C8 RID: 10184
	public float rangedAttackDistMin = 6f;

	// Token: 0x040027C9 RID: 10185
	public float rangedAttackDistMax = 8f;

	// Token: 0x040027CA RID: 10186
	public float rangedAttackChargeTime = 0.5f;

	// Token: 0x040027CB RID: 10187
	public float rangedAttackRecoverTime = 2f;

	// Token: 0x040027CC RID: 10188
	public float projectileSpeed = 5f;

	// Token: 0x040027CD RID: 10189
	public float projectileHitRadius = 1f;

	// Token: 0x040027CE RID: 10190
	public GameObject rangedProjectilePrefab;

	// Token: 0x040027CF RID: 10191
	public Transform rangedProjectileFirePoint;

	// Token: 0x040027D0 RID: 10192
	public float patrolSpeed = 1f;

	// Token: 0x040027D1 RID: 10193
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x040027D2 RID: 10194
	public NavMeshAgent navAgent;

	// Token: 0x040027D3 RID: 10195
	public AudioSource audioSource;

	// Token: 0x040027D4 RID: 10196
	public AudioClip damagedSound;

	// Token: 0x040027D5 RID: 10197
	public float damagedSoundVolume;

	// Token: 0x040027D6 RID: 10198
	public GameObject fxDamaged;

	// Token: 0x040027D7 RID: 10199
	public AudioClip deathSound;

	// Token: 0x040027D8 RID: 10200
	public float deathSoundVolume;

	// Token: 0x040027D9 RID: 10201
	public GameObject fxDeath;

	// Token: 0x040027DA RID: 10202
	public double lastStateChange;

	// Token: 0x040027DB RID: 10203
	public float ambientSoundVolume = 0.5f;

	// Token: 0x040027DC RID: 10204
	public double ambientSoundDelayMin = 5.0;

	// Token: 0x040027DD RID: 10205
	public double ambientSoundDelayMax = 10.0;

	// Token: 0x040027DE RID: 10206
	public AudioClip[] ambientPatrolSounds;

	// Token: 0x040027DF RID: 10207
	private double lastPartrolAmbientSoundTime;

	// Token: 0x040027E0 RID: 10208
	private double nextPatrolGroanTime;

	// Token: 0x040027E1 RID: 10209
	private Unity.Mathematics.Random patrolGroanSoundDelayRandom;

	// Token: 0x040027E2 RID: 10210
	private Unity.Mathematics.Random patrolGroanSoundRandom;

	// Token: 0x040027E3 RID: 10211
	public bool debugLog;

	// Token: 0x040027E4 RID: 10212
	public GameObject spitterHeadOnShoulders;

	// Token: 0x040027E5 RID: 10213
	public GameObject spitterHeadOnShouldersLight;

	// Token: 0x040027E6 RID: 10214
	public GameObject spitterHeadOnShouldersVFX;

	// Token: 0x040027E7 RID: 10215
	public GameObject spitterHeadInHand;

	// Token: 0x040027E8 RID: 10216
	public GameObject spitterHeadInHandLight;

	// Token: 0x040027E9 RID: 10217
	public GameObject spitterHeadInHandVFX;

	// Token: 0x040027EA RID: 10218
	public double spitterLightTurnOffDelay = 0.75;

	// Token: 0x040027EB RID: 10219
	private bool headLightReset;

	// Token: 0x040027EC RID: 10220
	private double spitterLightTurnOffTime;

	// Token: 0x040027ED RID: 10221
	[FormerlySerializedAs("headRemovalInterval")]
	public float headRemovalFrame = 0.23333333f;

	// Token: 0x040027EE RID: 10222
	private double headRemovaltime;

	// Token: 0x040027EF RID: 10223
	private bool headRemoved;

	// Token: 0x040027F0 RID: 10224
	private Transform target;

	// Token: 0x040027F1 RID: 10225
	[ReadOnly]
	public int hp;

	// Token: 0x040027F2 RID: 10226
	[ReadOnly]
	public GREnemyRanged.Behavior currBehavior;

	// Token: 0x040027F3 RID: 10227
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x040027F4 RID: 10228
	[ReadOnly]
	public GREnemyRanged.BodyState currBodyState;

	// Token: 0x040027F5 RID: 10229
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x040027F6 RID: 10230
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x040027F7 RID: 10231
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x040027F8 RID: 10232
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x040027F9 RID: 10233
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x040027FA RID: 10234
	[ReadOnly]
	public Vector3 rangedFiringPosition;

	// Token: 0x040027FB RID: 10235
	[ReadOnly]
	public Vector3 rangedTargetPosition;

	// Token: 0x040027FC RID: 10236
	[ReadOnly]
	private GRPlayer bestTargetPlayer;

	// Token: 0x040027FD RID: 10237
	[ReadOnly]
	private NetPlayer bestTargetNetPlayer;

	// Token: 0x040027FE RID: 10238
	private bool rangedAttackQueued;

	// Token: 0x040027FF RID: 10239
	private double queuedFiringTime;

	// Token: 0x04002800 RID: 10240
	private Vector3 queuedFiringPosition;

	// Token: 0x04002801 RID: 10241
	private Vector3 queuedTargetPosition;

	// Token: 0x04002802 RID: 10242
	private GameObject rangedProjectileInstance;

	// Token: 0x04002803 RID: 10243
	private bool projectileHasImpacted;

	// Token: 0x04002804 RID: 10244
	private double projectileImpactTime;

	// Token: 0x04002805 RID: 10245
	private Rigidbody rigidBody;

	// Token: 0x04002806 RID: 10246
	private List<Collider> colliders;

	// Token: 0x04002807 RID: 10247
	private LayerMask visibilityLayerMask;

	// Token: 0x04002808 RID: 10248
	private Color defaultColor;

	// Token: 0x04002809 RID: 10249
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x0400280A RID: 10250
	private List<VRRig> vrRigs = new List<VRRig>();

	// Token: 0x020005A7 RID: 1447
	private enum HitType
	{
		// Token: 0x0400280C RID: 10252
		Club,
		// Token: 0x0400280D RID: 10253
		Flash
	}

	// Token: 0x020005A8 RID: 1448
	public enum Behavior
	{
		// Token: 0x0400280F RID: 10255
		Idle,
		// Token: 0x04002810 RID: 10256
		Patrol,
		// Token: 0x04002811 RID: 10257
		Search,
		// Token: 0x04002812 RID: 10258
		Stagger,
		// Token: 0x04002813 RID: 10259
		Dying,
		// Token: 0x04002814 RID: 10260
		SeekRangedAttackPosition,
		// Token: 0x04002815 RID: 10261
		RangedAttack,
		// Token: 0x04002816 RID: 10262
		RangedAttackCooldown,
		// Token: 0x04002817 RID: 10263
		Flashed,
		// Token: 0x04002818 RID: 10264
		Count
	}

	// Token: 0x020005A9 RID: 1449
	public enum BodyState
	{
		// Token: 0x0400281A RID: 10266
		Destroyed,
		// Token: 0x0400281B RID: 10267
		Bones,
		// Token: 0x0400281C RID: 10268
		Shell,
		// Token: 0x0400281D RID: 10269
		Count
	}
}
