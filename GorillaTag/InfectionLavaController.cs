using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D27 RID: 3367
	public class InfectionLavaController : MonoBehaviour, IGorillaSerializeableScene, IGorillaSerializeable, ITickSystemPost, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x06005422 RID: 21538 RVA: 0x00197BEE File Offset: 0x00195DEE
		public static InfectionLavaController Instance
		{
			get
			{
				return InfectionLavaController.instance;
			}
		}

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x06005423 RID: 21539 RVA: 0x00197BF5 File Offset: 0x00195DF5
		public bool LavaCurrentlyActivated
		{
			get
			{
				return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
			}
		}

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x06005424 RID: 21540 RVA: 0x00197C05 File Offset: 0x00195E05
		public Plane LavaPlane
		{
			get
			{
				return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
			}
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x06005425 RID: 21541 RVA: 0x00197C22 File Offset: 0x00195E22
		public Vector3 SurfaceCenter
		{
			get
			{
				return this.lavaSurfacePlaneTransform.position;
			}
		}

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x06005426 RID: 21542 RVA: 0x00197C30 File Offset: 0x00195E30
		private int PlayerCount
		{
			get
			{
				int num = 1;
				GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
				if (gorillaGameManager != null && gorillaGameManager.currentNetPlayerArray != null)
				{
					num = gorillaGameManager.currentNetPlayerArray.Length;
				}
				return num;
			}
		}

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x06005427 RID: 21543 RVA: 0x00197C60 File Offset: 0x00195E60
		private bool InCompetitiveQueue
		{
			get
			{
				return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("COMPETITIVE");
			}
		}

		// Token: 0x06005428 RID: 21544 RVA: 0x00197C84 File Offset: 0x00195E84
		private void Awake()
		{
			if (InfectionLavaController.instance.IsNotNull())
			{
				Object.Destroy(base.gameObject);
				return;
			}
			InfectionLavaController.instance = this;
			RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
			RoomSystem.PlayerLeftEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerLeftEvent, new Action<NetPlayer>(this.OnPlayerLeftRoom));
			((IGuidedRefObject)this).GuidedRefInitialize();
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x06005429 RID: 21545 RVA: 0x00197D3F File Offset: 0x00195F3F
		protected void OnEnable()
		{
			if (!this.guidedRefsFullyResolved)
			{
				return;
			}
			this.VerifyReferences();
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x0600542A RID: 21546 RVA: 0x00197D56 File Offset: 0x00195F56
		void IGorillaSerializeableScene.OnSceneLinking(GorillaSerializerScene netObj)
		{
			this.networkObject = netObj;
		}

		// Token: 0x0600542B RID: 21547 RVA: 0x000D1CE3 File Offset: 0x000CFEE3
		protected void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x0600542C RID: 21548 RVA: 0x00197D60 File Offset: 0x00195F60
		private void VerifyReferences()
		{
			this.IfNullThenLogAndDisableSelf(this.lavaMeshTransform, "lavaMeshTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaVolume, "lavaVolume", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationRenderer, "lavaActivationRenderer", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationStartPos, "lavaActivationStartPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationEndPos, "lavaActivationEndPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier", -1);
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				this.IfNullThenLogAndDisableSelf(this.volcanoEffects[i], "volcanoEffects", i);
			}
		}

		// Token: 0x0600542D RID: 21549 RVA: 0x00197E14 File Offset: 0x00196014
		private void IfNullThenLogAndDisableSelf(Object obj, string fieldName, int index = -1)
		{
			if (obj != null)
			{
				return;
			}
			fieldName = ((index != -1) ? string.Format("{0}[{1}]", fieldName, index) : fieldName);
			Debug.LogError("InfectionLavaController: Disabling self because reference `" + fieldName + "` is null.", this);
			base.enabled = false;
		}

		// Token: 0x0600542E RID: 21550 RVA: 0x00197E64 File Offset: 0x00196064
		private void OnDestroy()
		{
			if (InfectionLavaController.instance == this)
			{
				InfectionLavaController.instance = null;
			}
			TickSystem<object>.RemovePostTickCallback(this);
			this.UpdateLava(0f);
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit -= this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x0600542F RID: 21551 RVA: 0x00197EDF File Offset: 0x001960DF
		// (set) Token: 0x06005430 RID: 21552 RVA: 0x00197EE7 File Offset: 0x001960E7
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005431 RID: 21553 RVA: 0x00197EF0 File Offset: 0x001960F0
		void ITickSystemPost.PostTick()
		{
			this.prevTime = this.currentTime;
			this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble);
			if (this.networkObject.HasAuthority)
			{
				this.UpdateReliableState(this.currentTime, ref this.reliableState);
			}
			this.UpdateLocalState(this.currentTime, this.reliableState);
			this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
			this.UpdateVolcanoActivationLava((float)this.reliableState.activationProgress);
			this.CheckLocalPlayerAgainstLava(this.currentTime);
		}

		// Token: 0x06005432 RID: 21554 RVA: 0x00197FB0 File Offset: 0x001961B0
		private void JumpToState(InfectionLavaController.RisingLavaState state)
		{
			this.reliableState.state = state;
			switch (state)
			{
			case InfectionLavaController.RisingLavaState.Drained:
			{
				for (int i = 0; i < this.volcanoEffects.Length; i++)
				{
					VolcanoEffects volcanoEffects = this.volcanoEffects[i];
					if (volcanoEffects != null)
					{
						volcanoEffects.SetDrainedState();
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Erupting:
			{
				for (int j = 0; j < this.volcanoEffects.Length; j++)
				{
					VolcanoEffects volcanoEffects2 = this.volcanoEffects[j];
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.SetEruptingState();
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Rising:
			{
				for (int k = 0; k < this.volcanoEffects.Length; k++)
				{
					VolcanoEffects volcanoEffects3 = this.volcanoEffects[k];
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.SetRisingState();
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Full:
			{
				for (int l = 0; l < this.volcanoEffects.Length; l++)
				{
					VolcanoEffects volcanoEffects4 = this.volcanoEffects[l];
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.SetFullState();
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Draining:
			{
				for (int m = 0; m < this.volcanoEffects.Length; m++)
				{
					VolcanoEffects volcanoEffects5 = this.volcanoEffects[m];
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.SetDrainingState();
					}
				}
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06005433 RID: 21555 RVA: 0x001980AC File Offset: 0x001962AC
		private void UpdateReliableState(double currentTime, ref InfectionLavaController.LavaSyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
				if (syncData.activationProgress > 1.0)
				{
					float playerCount = (float)this.PlayerCount;
					float num = (this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue);
					int num2 = Mathf.RoundToInt(playerCount * num);
					if (this.lavaActivationVoteCount >= num2)
					{
						for (int i = 0; i < this.lavaActivationVoteCount; i++)
						{
							this.lavaActivationVotePlayerIds[i] = 0;
						}
						this.lavaActivationVoteCount = 0;
						syncData.state = InfectionLavaController.RisingLavaState.Erupting;
						syncData.stateStartTime = currentTime;
						syncData.activationProgress = 1.0;
						for (int j = 0; j < this.volcanoEffects.Length; j++)
						{
							VolcanoEffects volcanoEffects = this.volcanoEffects[j];
							if (volcanoEffects != null)
							{
								volcanoEffects.SetEruptingState();
							}
						}
						return;
					}
				}
				else
				{
					float num3 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
					double activationProgress = syncData.activationProgress;
					syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num3);
					if (activationProgress > 0.0 && syncData.activationProgress <= 5E-324)
					{
						VolcanoEffects[] array = this.volcanoEffects;
						for (int k = 0; k < array.Length; k++)
						{
							array[k].OnVolcanoBellyEmpty();
						}
						return;
					}
				}
				break;
			case InfectionLavaController.RisingLavaState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.eruptTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Rising;
					syncData.stateStartTime = currentTime;
					for (int l = 0; l < this.volcanoEffects.Length; l++)
					{
						VolcanoEffects volcanoEffects2 = this.volcanoEffects[l];
						if (volcanoEffects2 != null)
						{
							volcanoEffects2.SetRisingState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Rising:
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Full;
					syncData.stateStartTime = currentTime;
					for (int m = 0; m < this.volcanoEffects.Length; m++)
					{
						VolcanoEffects volcanoEffects3 = this.volcanoEffects[m];
						if (volcanoEffects3 != null)
						{
							volcanoEffects3.SetFullState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Full:
				if (currentTime > syncData.stateStartTime + (double)this.fullTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Draining;
					syncData.stateStartTime = currentTime;
					for (int n = 0; n < this.volcanoEffects.Length; n++)
					{
						VolcanoEffects volcanoEffects4 = this.volcanoEffects[n];
						if (volcanoEffects4 != null)
						{
							volcanoEffects4.SetDrainingState();
						}
					}
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Draining:
				syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * Time.deltaTime);
				if (currentTime > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Drained;
					syncData.stateStartTime = currentTime;
					for (int num4 = 0; num4 < this.volcanoEffects.Length; num4++)
					{
						VolcanoEffects volcanoEffects5 = this.volcanoEffects[num4];
						if (volcanoEffects5 != null)
						{
							volcanoEffects5.SetDrainedState();
						}
					}
				}
				break;
			}
		}

		// Token: 0x06005434 RID: 21556 RVA: 0x00198398 File Offset: 0x00196598
		private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
		{
			switch (syncData.state)
			{
			default:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float num = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
				{
					if (volcanoEffects != null)
					{
						volcanoEffects.UpdateDrainedState(num);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Erupting:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float num2 = (float)(currentTime - syncData.stateStartTime);
				float num3 = Mathf.Clamp01(num2 / this.eruptTime);
				foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
				{
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.UpdateEruptingState(num2, this.eruptTime - num2, num3);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Rising:
			{
				float num4 = (float)(currentTime - syncData.stateStartTime) / this.riseTime;
				this.lavaProgressLinear = Mathf.Clamp01(num4);
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				float num5 = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
				{
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.UpdateRisingState(num5, this.riseTime - num5, this.lavaProgressLinear);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Full:
			{
				this.lavaProgressLinear = 1f;
				this.lavaProgressSmooth = 1f;
				float num6 = (float)(currentTime - syncData.stateStartTime);
				float num7 = Mathf.Clamp01(this.fullTime / num6);
				foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
				{
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.UpdateFullState(num6, this.fullTime - num6, num7);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Draining:
			{
				float num8 = (float)(currentTime - syncData.stateStartTime);
				float num9 = Mathf.Clamp01(num8 / this.drainTime);
				this.lavaProgressLinear = 1f - num9;
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
				{
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.UpdateDrainingState(num8, this.riseTime - num8, num9);
					}
				}
				return;
			}
			}
		}

		// Token: 0x06005435 RID: 21557 RVA: 0x001985E0 File Offset: 0x001967E0
		private void UpdateLava(float fillProgress)
		{
			this.lavaScale = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
			if (this.lavaMeshTransform != null)
			{
				this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, this.lavaScale);
			}
		}

		// Token: 0x06005436 RID: 21558 RVA: 0x0019864C File Offset: 0x0019684C
		private void UpdateVolcanoActivationLava(float activationProgress)
		{
			this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
			this.lavaActivationRenderer.material.SetColor(InfectionLavaController.shaderProp_BaseColor, this.lavaActivationGradient.Evaluate(activationProgress));
			this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
		}

		// Token: 0x06005437 RID: 21559 RVA: 0x001986C9 File Offset: 0x001968C9
		private void CheckLocalPlayerAgainstLava(double currentTime)
		{
			if (GTPlayer.Instance.InWater && GTPlayer.Instance.CurrentWaterVolume == this.lavaVolume)
			{
				this.LocalPlayerInLava(currentTime, false);
			}
		}

		// Token: 0x06005438 RID: 21560 RVA: 0x001986F6 File Offset: 0x001968F6
		private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				this.LocalPlayerInLava(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, true);
			}
		}

		// Token: 0x06005439 RID: 21561 RVA: 0x00198730 File Offset: 0x00196930
		private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
		{
			GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
			if (gorillaGameManager != null && gorillaGameManager.CanAffectPlayer(NetworkSystem.Instance.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
			{
				this.lastTagSelfRPCTime = currentTime;
				GameMode.ReportHit();
			}
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x00198782 File Offset: 0x00196982
		public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
		{
			if (projectile.gameObject.CompareTag("LavaRockProjectile"))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x0600543B RID: 21563 RVA: 0x001987A8 File Offset: 0x001969A8
		private void AddLavaRock(int playerId)
		{
			if (this.networkObject.HasAuthority && this.reliableState.state == InfectionLavaController.RisingLavaState.Drained)
			{
				float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
				this.reliableState.activationProgress = this.reliableState.activationProgress + (double)num;
				this.AddVoteForVolcanoActivation(playerId);
				VolcanoEffects[] array = this.volcanoEffects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnStoneAccepted(this.reliableState.activationProgress);
				}
			}
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x00198824 File Offset: 0x00196A24
		private void AddVoteForVolcanoActivation(int playerId)
		{
			if (this.networkObject.HasAuthority && this.lavaActivationVoteCount < 10)
			{
				bool flag = false;
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
					this.lavaActivationVoteCount++;
				}
			}
		}

		// Token: 0x0600543D RID: 21565 RVA: 0x00198888 File Offset: 0x00196A88
		private void RemoveVoteForVolcanoActivation(int playerId)
		{
			if (this.networkObject.HasAuthority)
			{
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						this.lavaActivationVotePlayerIds[i] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
						this.lavaActivationVoteCount--;
						return;
					}
				}
			}
		}

		// Token: 0x0600543E RID: 21566 RVA: 0x001988E4 File Offset: 0x00196AE4
		void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext((int)this.reliableState.state);
			stream.SendNext(this.reliableState.stateStartTime);
			stream.SendNext(this.reliableState.activationProgress);
			stream.SendNext(this.lavaActivationVoteCount);
			stream.SendNext(this.lavaActivationVotePlayerIds[0]);
			stream.SendNext(this.lavaActivationVotePlayerIds[1]);
			stream.SendNext(this.lavaActivationVotePlayerIds[2]);
			stream.SendNext(this.lavaActivationVotePlayerIds[3]);
			stream.SendNext(this.lavaActivationVotePlayerIds[4]);
			stream.SendNext(this.lavaActivationVotePlayerIds[5]);
			stream.SendNext(this.lavaActivationVotePlayerIds[6]);
			stream.SendNext(this.lavaActivationVotePlayerIds[7]);
			stream.SendNext(this.lavaActivationVotePlayerIds[8]);
			stream.SendNext(this.lavaActivationVotePlayerIds[9]);
		}

		// Token: 0x0600543F RID: 21567 RVA: 0x00198A04 File Offset: 0x00196C04
		void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
		{
			InfectionLavaController.RisingLavaState risingLavaState = (InfectionLavaController.RisingLavaState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = ((double)stream.ReceiveNext()).GetFinite();
			this.reliableState.activationProgress = ((double)stream.ReceiveNext()).ClampSafe(0.0, 2.0);
			this.lavaActivationVoteCount = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[0] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[1] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[2] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[3] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[4] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[5] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[6] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[7] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[8] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[9] = (int)stream.ReceiveNext();
			float num = this.lavaProgressSmooth;
			if (risingLavaState != this.reliableState.state)
			{
				this.JumpToState(risingLavaState);
			}
			this.UpdateLocalState((double)((float)NetworkSystem.Instance.SimTime), this.reliableState);
			this.localLagLavaProgressOffset = num - this.lavaProgressSmooth;
		}

		// Token: 0x06005440 RID: 21568 RVA: 0x00198B77 File Offset: 0x00196D77
		public void OnPlayerLeftRoom(NetPlayer otherNetPlayer)
		{
			this.RemoveVoteForVolcanoActivation(otherNetPlayer.ActorNumber);
		}

		// Token: 0x06005441 RID: 21569 RVA: 0x00198B88 File Offset: 0x00196D88
		private void OnLeftRoom()
		{
			for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] != NetworkSystem.Instance.LocalPlayerID)
				{
					this.RemoveVoteForVolcanoActivation(this.lavaActivationVotePlayerIds[i]);
				}
			}
		}

		// Token: 0x06005442 RID: 21570 RVA: 0x000023F4 File Offset: 0x000005F4
		void IGorillaSerializeableScene.OnNetworkObjectDisable()
		{
		}

		// Token: 0x06005443 RID: 21571 RVA: 0x000023F4 File Offset: 0x000005F4
		void IGorillaSerializeableScene.OnNetworkObjectEnable()
		{
		}

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x06005444 RID: 21572 RVA: 0x00198BCA File Offset: 0x00196DCA
		// (set) Token: 0x06005445 RID: 21573 RVA: 0x00198BD2 File Offset: 0x00196DD2
		int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06005446 RID: 21574 RVA: 0x00198BDB File Offset: 0x00196DDB
		void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
		{
			this.guidedRefsFullyResolved = true;
			this.VerifyReferences();
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06005447 RID: 21575 RVA: 0x00198BF0 File Offset: 0x00196DF0
		public void OnGuidedRefTargetDestroyed(int fieldId)
		{
			this.guidedRefsFullyResolved = false;
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005448 RID: 21576 RVA: 0x00198C00 File Offset: 0x00196E00
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaMeshTransform_gRef", ref this.lavaMeshTransform_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaSurfacePlaneTransform_gRef", ref this.lavaSurfacePlaneTransform_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaVolume_gRef", ref this.lavaVolume_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationRenderer_gRef", ref this.lavaActivationRenderer_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationStartPos_gRef", ref this.lavaActivationStartPos_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationEndPos_gRef", ref this.lavaActivationEndPos_gRef);
			GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationProjectileHitNotifier_gRef", ref this.lavaActivationProjectileHitNotifier_gRef);
			GuidedRefHub.RegisterReceiverArray<InfectionLavaController, VolcanoEffects>(this, "volcanoEffects_gRefs", ref this.volcanoEffects, ref this.volcanoEffects_gRefs);
			GuidedRefHub.ReceiverFullyRegistered<InfectionLavaController>(this);
		}

		// Token: 0x06005449 RID: 21577 RVA: 0x00198CA4 File Offset: 0x00196EA4
		bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
		{
			return GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaMeshTransform, this.lavaMeshTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaSurfacePlaneTransform, this.lavaSurfacePlaneTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, WaterVolume>(this, ref this.lavaVolume, this.lavaVolume_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, MeshRenderer>(this, ref this.lavaActivationRenderer, this.lavaActivationRenderer_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationStartPos, this.lavaActivationStartPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationEndPos, this.lavaActivationEndPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, SlingshotProjectileHitNotifier>(this, ref this.lavaActivationProjectileHitNotifier, this.lavaActivationProjectileHitNotifier_gRef, target) || GuidedRefHub.TryResolveArrayItem<InfectionLavaController, VolcanoEffects>(this, this.volcanoEffects, this.volcanoEffects_gRefs, target);
		}

		// Token: 0x0600544C RID: 21580 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0600544D RID: 21581 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x0400571F RID: 22303
		[OnEnterPlay_SetNull]
		private static InfectionLavaController instance;

		// Token: 0x04005720 RID: 22304
		[SerializeField]
		private float lavaMeshMinScale = 3.17f;

		// Token: 0x04005721 RID: 22305
		[Tooltip("If you throw rocks into the volcano quickly enough, then it will raise to this height.")]
		[SerializeField]
		private float lavaMeshMaxScale = 8.941086f;

		// Token: 0x04005722 RID: 22306
		[SerializeField]
		private float eruptTime = 3f;

		// Token: 0x04005723 RID: 22307
		[SerializeField]
		private float riseTime = 10f;

		// Token: 0x04005724 RID: 22308
		[SerializeField]
		private float fullTime = 240f;

		// Token: 0x04005725 RID: 22309
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x04005726 RID: 22310
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x04005727 RID: 22311
		[SerializeField]
		private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005728 RID: 22312
		[Header("Volcano Activation")]
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageDefaultQueue = 0.42f;

		// Token: 0x04005729 RID: 22313
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageCompetitiveQueue = 0.6f;

		// Token: 0x0400572A RID: 22314
		[SerializeField]
		private Gradient lavaActivationGradient;

		// Token: 0x0400572B RID: 22315
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400572C RID: 22316
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400572D RID: 22317
		[SerializeField]
		private float lavaActivationVisualMovementProgressPerSecond = 1f;

		// Token: 0x0400572E RID: 22318
		[SerializeField]
		private bool debugLavaActivationVotes;

		// Token: 0x0400572F RID: 22319
		[Header("Scene References")]
		[SerializeField]
		private Transform lavaMeshTransform;

		// Token: 0x04005730 RID: 22320
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaMeshTransform_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04005731 RID: 22321
		[SerializeField]
		private Transform lavaSurfacePlaneTransform;

		// Token: 0x04005732 RID: 22322
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaSurfacePlaneTransform_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04005733 RID: 22323
		[SerializeField]
		private WaterVolume lavaVolume;

		// Token: 0x04005734 RID: 22324
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaVolume_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04005735 RID: 22325
		[SerializeField]
		private MeshRenderer lavaActivationRenderer;

		// Token: 0x04005736 RID: 22326
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationRenderer_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04005737 RID: 22327
		[SerializeField]
		private Transform lavaActivationStartPos;

		// Token: 0x04005738 RID: 22328
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationStartPos_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x04005739 RID: 22329
		[SerializeField]
		private Transform lavaActivationEndPos;

		// Token: 0x0400573A RID: 22330
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationEndPos_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x0400573B RID: 22331
		[SerializeField]
		private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

		// Token: 0x0400573C RID: 22332
		[SerializeField]
		private GuidedRefReceiverFieldInfo lavaActivationProjectileHitNotifier_gRef = new GuidedRefReceiverFieldInfo(true);

		// Token: 0x0400573D RID: 22333
		[SerializeField]
		private VolcanoEffects[] volcanoEffects;

		// Token: 0x0400573E RID: 22334
		[SerializeField]
		private GuidedRefReceiverArrayInfo volcanoEffects_gRefs = new GuidedRefReceiverArrayInfo(true);

		// Token: 0x0400573F RID: 22335
		[DebugReadout]
		private InfectionLavaController.LavaSyncData reliableState;

		// Token: 0x04005740 RID: 22336
		private int[] lavaActivationVotePlayerIds = new int[10];

		// Token: 0x04005741 RID: 22337
		private int lavaActivationVoteCount;

		// Token: 0x04005742 RID: 22338
		private float localLagLavaProgressOffset;

		// Token: 0x04005743 RID: 22339
		[DebugReadout]
		private float lavaProgressLinear;

		// Token: 0x04005744 RID: 22340
		[DebugReadout]
		private float lavaProgressSmooth;

		// Token: 0x04005745 RID: 22341
		private double lastTagSelfRPCTime;

		// Token: 0x04005746 RID: 22342
		private const string lavaRockProjectileTag = "LavaRockProjectile";

		// Token: 0x04005747 RID: 22343
		private double currentTime;

		// Token: 0x04005748 RID: 22344
		private double prevTime;

		// Token: 0x04005749 RID: 22345
		private float activationProgessSmooth;

		// Token: 0x0400574A RID: 22346
		private float lavaScale;

		// Token: 0x0400574B RID: 22347
		private static readonly int shaderProp_BaseColor = Shader.PropertyToID("_BaseColor");

		// Token: 0x0400574C RID: 22348
		private GorillaSerializerScene networkObject;

		// Token: 0x0400574E RID: 22350
		private bool guidedRefsFullyResolved;

		// Token: 0x02000D28 RID: 3368
		public enum RisingLavaState
		{
			// Token: 0x04005751 RID: 22353
			Drained,
			// Token: 0x04005752 RID: 22354
			Erupting,
			// Token: 0x04005753 RID: 22355
			Rising,
			// Token: 0x04005754 RID: 22356
			Full,
			// Token: 0x04005755 RID: 22357
			Draining
		}

		// Token: 0x02000D29 RID: 3369
		private struct LavaSyncData
		{
			// Token: 0x04005756 RID: 22358
			public InfectionLavaController.RisingLavaState state;

			// Token: 0x04005757 RID: 22359
			public double stateStartTime;

			// Token: 0x04005758 RID: 22360
			public double activationProgress;
		}
	}
}
