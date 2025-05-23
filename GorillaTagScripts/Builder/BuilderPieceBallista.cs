using System;
using System.Collections;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B57 RID: 2903
	public class BuilderPieceBallista : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x060047A6 RID: 18342 RVA: 0x00154AF8 File Offset: 0x00152CF8
		private void Awake()
		{
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.playerPullInRate = Mathf.Exp(this.playerMagnetismStrength);
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnHandTriggerPressed));
			}
			this.hasLaunchParticles = this.launchParticles != null;
		}

		// Token: 0x060047A7 RID: 18343 RVA: 0x00154BBE File Offset: 0x00152DBE
		private void OnDestroy()
		{
			if (this.handTrigger != null)
			{
				this.handTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnHandTriggerPressed));
			}
		}

		// Token: 0x060047A8 RID: 18344 RVA: 0x00154BEA File Offset: 0x00152DEA
		private void OnHandTriggerPressed()
		{
			if (this.autoLaunch)
			{
				return;
			}
			if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 4);
			}
		}

		// Token: 0x060047A9 RID: 18345 RVA: 0x00154C20 File Offset: 0x00152E20
		private void UpdateStateMaster()
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
			switch (this.ballistaState)
			{
			case BuilderPieceBallista.BallistaState.Idle:
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				return;
			case BuilderPieceBallista.BallistaState.Loading:
				if (currentAnimatorStateInfo.shortNameHash == this.loadStateHash && (double)Time.time > this.loadCompleteTime)
				{
					if (this.playerInTrigger && this.playerRigInTrigger != null && (this.launchBigMonkes || (double)this.playerRigInTrigger.scaleFactor < 0.99))
					{
						this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
						return;
					}
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ballistaState = BuilderPieceBallista.BallistaState.WaitingForTrigger;
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.WaitingForTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					return;
				}
				if (this.playerInTrigger)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 3, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PlayerInTrigger:
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 2, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				if (this.autoLaunch && (double)Time.time > this.enteredTriggerTime + (double)this.autoLaunchDelay)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			case BuilderPieceBallista.BallistaState.PrepareForLaunch:
			case BuilderPieceBallista.BallistaState.PrepareForLaunchLocal:
			{
				if (!this.playerInTrigger || this.playerRigInTrigger == null || (!this.launchBigMonkes && this.playerRigInTrigger.scaleFactor >= 0.99f))
				{
					this.playerInTrigger = false;
					this.playerRigInTrigger = null;
					this.ResetFlags();
					this.myPiece.functionalPieceState = 0;
					this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
					return;
				}
				Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(this.playerRigInTrigger.transform, this.playerRigInTrigger.scaleFactor);
				Vector3 vector = Vector3.Dot(playerBodyCenterPosition - this.launchStart.position, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 vector2 = playerBodyCenterPosition - vector;
				if (Vector3.Lerp(Vector3.zero, vector2, Mathf.Exp(-this.playerPullInRate * Time.deltaTime)).sqrMagnitude < this.playerReadyToFireDist * this.myPiece.GetScale() * this.playerReadyToFireDist * this.myPiece.GetScale())
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 6, this.playerRigInTrigger.Creator.GetPlayerRef(), NetworkSystem.Instance.ServerTimestamp);
					return;
				}
				break;
			}
			case BuilderPieceBallista.BallistaState.Launching:
			case BuilderPieceBallista.BallistaState.LaunchingLocal:
				if (currentAnimatorStateInfo.shortNameHash == this.idleStateHash)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060047AA RID: 18346 RVA: 0x0015502F File Offset: 0x0015322F
		private void ResetFlags()
		{
			this.playerLaunched = false;
			this.loadCompleteTime = double.MaxValue;
		}

		// Token: 0x060047AB RID: 18347 RVA: 0x00155048 File Offset: 0x00153248
		private void UpdatePlayerPosition()
		{
			if (this.ballistaState != BuilderPieceBallista.BallistaState.PrepareForLaunchLocal && this.ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			GTPlayer instance = GTPlayer.Instance;
			Vector3 playerBodyCenterPosition = this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale);
			Vector3 vector = playerBodyCenterPosition - this.launchStart.position;
			BuilderPieceBallista.BallistaState ballistaState = this.ballistaState;
			if (ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				Vector3 vector2 = Vector3.Dot(vector, this.launchDirection) * this.launchDirection + this.launchStart.position;
				Vector3 vector3 = playerBodyCenterPosition - vector2;
				Vector3 vector4 = Vector3.Lerp(Vector3.zero, vector3, Mathf.Exp(-this.playerPullInRate * deltaTime));
				instance.transform.position = instance.transform.position + (vector4 - vector3);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				return;
			}
			if (ballistaState != BuilderPieceBallista.BallistaState.LaunchingLocal)
			{
				return;
			}
			if (!this.playerLaunched)
			{
				float num = Vector3.Dot(this.launchBone.position - this.launchStart.position, this.launchDirection) / this.launchRampDistance;
				float num2 = Vector3.Dot(vector, this.launchDirection) / this.launchRampDistance;
				float num3 = 0.25f * this.myPiece.GetScale() / this.launchRampDistance;
				float num4 = Mathf.Max(num + num3, num2);
				float num5 = num4 * this.launchRampDistance;
				Vector3 vector5 = this.launchDirection * num5 + this.launchStart.position;
				instance.transform.position + (vector5 - playerBodyCenterPosition);
				instance.transform.position = instance.transform.position + (vector5 - playerBodyCenterPosition);
				instance.SetPlayerVelocity(Vector3.zero);
				instance.SetMaximumSlipThisFrame();
				if (num4 >= 1f)
				{
					this.playerLaunched = true;
					this.launchedTime = (double)Time.time;
					instance.SetPlayerVelocity(this.launchSpeed * this.myPiece.GetScale() * this.launchDirection);
					instance.SetMaximumSlipThisFrame();
					return;
				}
			}
			else if ((double)Time.time < this.launchedTime + (double)this.slipOverrideDuration)
			{
				instance.SetMaximumSlipThisFrame();
			}
		}

		// Token: 0x060047AC RID: 18348 RVA: 0x00155280 File Offset: 0x00153480
		private Vector3 GetPlayerBodyCenterPosition(Transform headTransform, float playerScale)
		{
			return headTransform.position + Quaternion.Euler(0f, headTransform.rotation.eulerAngles.y, 0f) * new Vector3(0f, 0f, this.playerBodyOffsetFromHead.z * playerScale) + Vector3.down * (this.playerBodyOffsetFromHead.y * playerScale);
		}

		// Token: 0x060047AD RID: 18349 RVA: 0x001552F8 File Offset: 0x001534F8
		private void OnTriggerEnter(Collider other)
		{
			if (this.playerRigInTrigger != null)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.launchBigMonkes && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			this.playerRigInTrigger = vrrig;
			this.playerInTrigger = true;
		}

		// Token: 0x060047AE RID: 18350 RVA: 0x00155398 File Offset: 0x00153598
		private void OnTriggerExit(Collider other)
		{
			if (this.playerRigInTrigger == null || !this.playerInTrigger)
			{
				return;
			}
			if (other.GetComponent<CapsuleCollider>() == null)
			{
				return;
			}
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (this.playerRigInTrigger.Equals(vrrig))
			{
				this.playerInTrigger = false;
				this.playerRigInTrigger = null;
			}
		}

		// Token: 0x060047AF RID: 18351 RVA: 0x00155430 File Offset: 0x00153630
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			if (!this.myPiece.GetTable().isTableMutable)
			{
				this.launchBigMonkes = true;
			}
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.playerLaunched = false;
		}

		// Token: 0x060047B0 RID: 18352 RVA: 0x00155467 File Offset: 0x00153667
		public void OnPieceDestroy()
		{
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
		}

		// Token: 0x060047B1 RID: 18353 RVA: 0x0015547C File Offset: 0x0015367C
		public void OnPiecePlacementDeserialized()
		{
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
		}

		// Token: 0x060047B2 RID: 18354 RVA: 0x001554D4 File Offset: 0x001536D4
		public void OnPieceActivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = true;
			}
			this.animator.SetFloat(this.pitchParamHash, this.pitch);
			this.appliedAnimatorPitch = this.pitch;
			this.launchDirection = this.launchEnd.position - this.launchStart.position;
			this.launchRampDistance = this.launchDirection.magnitude;
			this.launchDirection /= this.launchRampDistance;
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x060047B3 RID: 18355 RVA: 0x001555A4 File Offset: 0x001537A4
		public void OnPieceDeactivate()
		{
			foreach (Collider collider in this.triggers)
			{
				collider.enabled = false;
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Stop();
				this.launchParticles.Clear();
			}
			this.myPiece.functionalPieceState = 0;
			this.ballistaState = BuilderPieceBallista.BallistaState.Idle;
			this.playerInTrigger = false;
			this.playerRigInTrigger = null;
			this.ResetFlags();
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
		}

		// Token: 0x060047B4 RID: 18356 RVA: 0x0015564C File Offset: 0x0015384C
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState) || instigator == null)
			{
				return;
			}
			if ((BuilderPieceBallista.BallistaState)newState == this.ballistaState)
			{
				return;
			}
			if (newState == 4)
			{
				if (this.ballistaState == BuilderPieceBallista.BallistaState.PlayerInTrigger && this.playerInTrigger && this.playerRigInTrigger != null)
				{
					this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 4, this.playerRigInTrigger.Creator.GetPlayerRef(), timeStamp);
					return;
				}
			}
			else
			{
				Debug.LogWarning("BuilderPiece Ballista unexpected state request for " + newState.ToString());
			}
		}

		// Token: 0x060047B5 RID: 18357 RVA: 0x001556EC File Offset: 0x001538EC
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			BuilderPieceBallista.BallistaState ballistaState = (BuilderPieceBallista.BallistaState)newState;
			if (ballistaState == this.ballistaState)
			{
				return;
			}
			switch (newState)
			{
			case 0:
				this.ResetFlags();
				goto IL_02C2;
			case 1:
				this.ResetFlags();
				foreach (Collider collider in this.disableWhileLaunching)
				{
					collider.enabled = true;
				}
				if (this.ballistaState == BuilderPieceBallista.BallistaState.Launching || this.ballistaState == BuilderPieceBallista.BallistaState.LaunchingLocal)
				{
					this.loadCompleteTime = (double)(Time.time + this.reloadDelay);
					if (this.loadSFX != null)
					{
						this.loadSFX.Play();
					}
				}
				else
				{
					this.loadCompleteTime = (double)(Time.time + this.loadTime);
				}
				this.animator.SetTrigger(this.loadTriggerHash);
				goto IL_02C2;
			case 2:
			case 5:
				goto IL_02C2;
			case 3:
				this.enteredTriggerTime = (double)Time.time;
				if (this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
					goto IL_02C2;
				}
				goto IL_02C2;
			case 4:
			{
				this.playerLaunched = false;
				if (!this.autoLaunch && this.cockSFX != null)
				{
					this.cockSFX.Play();
				}
				if (!instigator.IsLocal)
				{
					goto IL_02C2;
				}
				GTPlayer instance = GTPlayer.Instance;
				if (Vector3.Distance(this.GetPlayerBodyCenterPosition(instance.headCollider.transform, instance.scale), this.launchStart.position) > this.prepareForLaunchDistance * this.myPiece.GetScale() || (!this.launchBigMonkes && (double)GorillaTagger.Instance.offlineVRRig.scaleFactor >= 0.99))
				{
					goto IL_02C2;
				}
				ballistaState = BuilderPieceBallista.BallistaState.PrepareForLaunchLocal;
				using (List<Collider>.Enumerator enumerator = this.disableWhileLaunching.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider2 = enumerator.Current;
						collider2.enabled = false;
					}
					goto IL_02C2;
				}
				break;
			}
			case 6:
				break;
			default:
				goto IL_02C2;
			}
			this.playerLaunched = false;
			this.animator.SetTrigger(this.fireTriggerHash);
			if (this.launchSFX != null)
			{
				this.launchSFX.Play();
			}
			if (this.hasLaunchParticles)
			{
				this.launchParticles.Play();
			}
			if (this.debugDrawTrajectoryOnLaunch)
			{
				base.StartCoroutine(this.DebugDrawTrajectory(8f));
			}
			if (instigator.IsLocal && this.ballistaState == BuilderPieceBallista.BallistaState.PrepareForLaunchLocal)
			{
				ballistaState = BuilderPieceBallista.BallistaState.LaunchingLocal;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 4f);
			}
			IL_02C2:
			this.ballistaState = ballistaState;
		}

		// Token: 0x060047B6 RID: 18358 RVA: 0x001559E0 File Offset: 0x00153BE0
		public bool IsStateValid(byte state)
		{
			return state < 8;
		}

		// Token: 0x060047B7 RID: 18359 RVA: 0x001559E6 File Offset: 0x00153BE6
		public void FunctionalPieceUpdate()
		{
			if (this.myPiece == null || this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.UpdateStateMaster();
			}
			this.UpdatePlayerPosition();
		}

		// Token: 0x060047B8 RID: 18360 RVA: 0x00155A1C File Offset: 0x00153C1C
		private void UpdatePredictionLine()
		{
			float num = 0.033333335f;
			Vector3 vector = this.launchEnd.position;
			Vector3 vector2 = (this.launchEnd.position - this.launchStart.position).normalized * this.launchSpeed;
			for (int i = 0; i < 240; i++)
			{
				this.predictionLinePoints[i] = vector;
				vector += vector2 * num;
				vector2 += Vector3.down * 9.8f * num;
			}
		}

		// Token: 0x060047B9 RID: 18361 RVA: 0x00155AB6 File Offset: 0x00153CB6
		private IEnumerator DebugDrawTrajectory(float duration)
		{
			this.UpdatePredictionLine();
			float startTime = Time.time;
			while (Time.time < startTime + duration)
			{
				DebugUtil.DrawLine(this.launchStart.position, this.launchEnd.position, Color.yellow, true);
				DebugUtil.DrawLines(this.predictionLinePoints, Color.yellow, true);
				yield return null;
			}
			yield break;
		}

		// Token: 0x040049F9 RID: 18937
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040049FA RID: 18938
		[SerializeField]
		private List<Collider> triggers;

		// Token: 0x040049FB RID: 18939
		[SerializeField]
		private List<Collider> disableWhileLaunching;

		// Token: 0x040049FC RID: 18940
		[SerializeField]
		private BuilderSmallHandTrigger handTrigger;

		// Token: 0x040049FD RID: 18941
		[SerializeField]
		private bool autoLaunch;

		// Token: 0x040049FE RID: 18942
		[SerializeField]
		private float autoLaunchDelay = 0.75f;

		// Token: 0x040049FF RID: 18943
		private double enteredTriggerTime;

		// Token: 0x04004A00 RID: 18944
		public Animator animator;

		// Token: 0x04004A01 RID: 18945
		public Transform launchStart;

		// Token: 0x04004A02 RID: 18946
		public Transform launchEnd;

		// Token: 0x04004A03 RID: 18947
		public Transform launchBone;

		// Token: 0x04004A04 RID: 18948
		[SerializeField]
		private SoundBankPlayer loadSFX;

		// Token: 0x04004A05 RID: 18949
		[SerializeField]
		private SoundBankPlayer launchSFX;

		// Token: 0x04004A06 RID: 18950
		[SerializeField]
		private SoundBankPlayer cockSFX;

		// Token: 0x04004A07 RID: 18951
		[SerializeField]
		private ParticleSystem launchParticles;

		// Token: 0x04004A08 RID: 18952
		private bool hasLaunchParticles;

		// Token: 0x04004A09 RID: 18953
		public float reloadDelay = 1f;

		// Token: 0x04004A0A RID: 18954
		public float loadTime = 1.933f;

		// Token: 0x04004A0B RID: 18955
		public float slipOverrideDuration = 0.1f;

		// Token: 0x04004A0C RID: 18956
		private double launchedTime;

		// Token: 0x04004A0D RID: 18957
		public float playerMagnetismStrength = 3f;

		// Token: 0x04004A0E RID: 18958
		[Tooltip("Speed will be scaled by piece scale")]
		public float launchSpeed = 20f;

		// Token: 0x04004A0F RID: 18959
		[Range(0f, 1f)]
		public float pitch;

		// Token: 0x04004A10 RID: 18960
		private bool debugDrawTrajectoryOnLaunch;

		// Token: 0x04004A11 RID: 18961
		private int loadTriggerHash = Animator.StringToHash("Load");

		// Token: 0x04004A12 RID: 18962
		private int fireTriggerHash = Animator.StringToHash("Fire");

		// Token: 0x04004A13 RID: 18963
		private int pitchParamHash = Animator.StringToHash("Pitch");

		// Token: 0x04004A14 RID: 18964
		private int idleStateHash = Animator.StringToHash("Idle");

		// Token: 0x04004A15 RID: 18965
		private int loadStateHash = Animator.StringToHash("Load");

		// Token: 0x04004A16 RID: 18966
		private int fireStateHash = Animator.StringToHash("Fire");

		// Token: 0x04004A17 RID: 18967
		private bool playerInTrigger;

		// Token: 0x04004A18 RID: 18968
		private VRRig playerRigInTrigger;

		// Token: 0x04004A19 RID: 18969
		private bool playerLaunched;

		// Token: 0x04004A1A RID: 18970
		private float playerReadyToFireDist = 1.6667f;

		// Token: 0x04004A1B RID: 18971
		private float prepareForLaunchDistance = 2.5f;

		// Token: 0x04004A1C RID: 18972
		private Vector3 launchDirection;

		// Token: 0x04004A1D RID: 18973
		private float launchRampDistance;

		// Token: 0x04004A1E RID: 18974
		private float playerPullInRate;

		// Token: 0x04004A1F RID: 18975
		private float appliedAnimatorPitch;

		// Token: 0x04004A20 RID: 18976
		private bool launchBigMonkes;

		// Token: 0x04004A21 RID: 18977
		private Vector3 playerBodyOffsetFromHead = new Vector3(0f, -0.4f, -0.15f);

		// Token: 0x04004A22 RID: 18978
		private double loadCompleteTime;

		// Token: 0x04004A23 RID: 18979
		private BuilderPieceBallista.BallistaState ballistaState;

		// Token: 0x04004A24 RID: 18980
		private const int predictionLineSamples = 240;

		// Token: 0x04004A25 RID: 18981
		private Vector3[] predictionLinePoints = new Vector3[240];

		// Token: 0x02000B58 RID: 2904
		private enum BallistaState
		{
			// Token: 0x04004A27 RID: 18983
			Idle,
			// Token: 0x04004A28 RID: 18984
			Loading,
			// Token: 0x04004A29 RID: 18985
			WaitingForTrigger,
			// Token: 0x04004A2A RID: 18986
			PlayerInTrigger,
			// Token: 0x04004A2B RID: 18987
			PrepareForLaunch,
			// Token: 0x04004A2C RID: 18988
			PrepareForLaunchLocal,
			// Token: 0x04004A2D RID: 18989
			Launching,
			// Token: 0x04004A2E RID: 18990
			LaunchingLocal,
			// Token: 0x04004A2F RID: 18991
			Count
		}
	}
}
