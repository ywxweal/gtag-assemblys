using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B6A RID: 2922
	public class BuilderShootingGallery : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004852 RID: 18514 RVA: 0x00159270 File Offset: 0x00157470
		private void Awake()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
			this.wheelHitNotifier.OnProjectileHit += this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit += this.OnCowboyHit;
		}

		// Token: 0x06004853 RID: 18515 RVA: 0x001592F4 File Offset: 0x001574F4
		private void OnDestroy()
		{
			this.wheelHitNotifier.OnProjectileHit -= this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit -= this.OnCowboyHit;
		}

		// Token: 0x06004854 RID: 18516 RVA: 0x00159324 File Offset: 0x00157524
		private void OnWheelHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x06004855 RID: 18517 RVA: 0x00159394 File Offset: 0x00157594
		private void OnCowboyHit(SlingshotProjectile projectile, Collision collision)
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (projectile.projectileOwner == null || projectile.projectileOwner != NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 2);
			}
		}

		// Token: 0x06004856 RID: 18518 RVA: 0x00159404 File Offset: 0x00157604
		private void CowboyHitEffects()
		{
			if (this.cowboyHitSound != null)
			{
				this.cowboyHitSound.Play();
			}
			if (this.cowboyHitAnimation != null && this.cowboyHitAnimation.clip != null)
			{
				this.cowboyHitAnimation.Play();
			}
		}

		// Token: 0x06004857 RID: 18519 RVA: 0x00159458 File Offset: 0x00157658
		private void WheelHitEffects()
		{
			if (this.wheelHitSound != null)
			{
				this.wheelHitSound.Play();
			}
			if (this.wheelHitAnimation != null && this.wheelHitAnimation.clip != null)
			{
				this.wheelHitAnimation.Play();
			}
		}

		// Token: 0x06004858 RID: 18520 RVA: 0x001594AC File Offset: 0x001576AC
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderShootingGallery.FunctionalState.Idle;
			this.cowboyInitLocalPos = this.cowboyTransform.transform.localPosition;
			this.cowboyInitLocalRotation = this.cowboyTransform.transform.localRotation;
			this.wheelInitLocalRot = this.wheelTransform.transform.localRotation;
			this.distance = Vector3.Distance(this.cowboyStart.position, this.cowboyEnd.position);
			this.cowboyCycleDuration = this.distance / (this.cowboyVelocity * this.myPiece.GetScale());
			this.wheelCycleDuration = 1f / this.wheelVelocity;
		}

		// Token: 0x06004859 RID: 18521 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x0600485A RID: 18522 RVA: 0x00159554 File Offset: 0x00157754
		public void OnPiecePlacementDeserialized()
		{
			if (!this.activated && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x0600485B RID: 18523 RVA: 0x00159584 File Offset: 0x00157784
		public void OnPieceActivate()
		{
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
			if (!this.activated)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x0600485C RID: 18524 RVA: 0x001595E4 File Offset: 0x001577E4
		public void OnPieceDeactivate()
		{
			if (this.currentState != BuilderShootingGallery.FunctionalState.Idle)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
			if (this.activated)
			{
				this.myPiece.GetTable().UnregisterFunctionalPieceFixedUpdate(this);
				this.activated = false;
			}
			this.cowboyTransform.SetLocalPositionAndRotation(this.cowboyInitLocalPos, this.cowboyInitLocalRotation);
			this.wheelTransform.SetLocalPositionAndRotation(this.wheelTransform.localPosition, this.wheelInitLocalRot);
		}

		// Token: 0x0600485D RID: 18525 RVA: 0x00159680 File Offset: 0x00157880
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (instigator == null)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.WheelHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			else if (newState == 2 && this.currentState == BuilderShootingGallery.FunctionalState.Idle)
			{
				this.lastHitTime = (double)Time.time;
				this.CowboyHitEffects();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderShootingGallery.FunctionalState)newState;
		}

		// Token: 0x0600485E RID: 18526 RVA: 0x00159704 File Offset: 0x00157904
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
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x0600485F RID: 18527 RVA: 0x00159769 File Offset: 0x00157969
		public bool IsStateValid(byte state)
		{
			return state <= 2;
		}

		// Token: 0x06004860 RID: 18528 RVA: 0x00159774 File Offset: 0x00157974
		public void FunctionalPieceUpdate()
		{
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06004861 RID: 18529 RVA: 0x001597C8 File Offset: 0x001579C8
		public void FunctionalPieceFixedUpdate()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			this.currT = this.CowboyCycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			float num = (this.currForward ? this.currT : (1f - this.currT));
			float num2 = this.WheelCycleCompletionPercent();
			float num3 = this.cowboyCurve.Evaluate(num);
			this.cowboyTransform.localPosition = Vector3.Lerp(this.cowboyStart.localPosition, this.cowboyEnd.localPosition, num3);
			Quaternion quaternion = Quaternion.AngleAxis(num2 * 360f, Vector3.right);
			this.wheelTransform.localRotation = quaternion;
		}

		// Token: 0x06004862 RID: 18530 RVA: 0x0015986F File Offset: 0x00157A6F
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06004863 RID: 18531 RVA: 0x00159891 File Offset: 0x00157A91
		private long CowboyCycleLengthMs()
		{
			return (long)(this.cowboyCycleDuration * 1000f);
		}

		// Token: 0x06004864 RID: 18532 RVA: 0x001598A0 File Offset: 0x00157AA0
		private long WheelCycleLengthMs()
		{
			return (long)(this.wheelCycleDuration * 1000f);
		}

		// Token: 0x06004865 RID: 18533 RVA: 0x001598B0 File Offset: 0x00157AB0
		public double CowboyPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CowboyCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004866 RID: 18534 RVA: 0x001598DC File Offset: 0x00157ADC
		public double WheelPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.WheelCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004867 RID: 18535 RVA: 0x00159907 File Offset: 0x00157B07
		public int CowboyCycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CowboyCycleLengthMs());
		}

		// Token: 0x06004868 RID: 18536 RVA: 0x00159917 File Offset: 0x00157B17
		public float CowboyCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.CowboyPlatformTime() / (double)this.cowboyCycleDuration), 0f, 1f);
		}

		// Token: 0x06004869 RID: 18537 RVA: 0x00159937 File Offset: 0x00157B37
		public float WheelCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.WheelPlatformTime() / (double)this.wheelCycleDuration), 0f, 1f);
		}

		// Token: 0x0600486A RID: 18538 RVA: 0x00159957 File Offset: 0x00157B57
		public bool IsEvenCycle()
		{
			return this.CowboyCycleCount() % 2 == 0;
		}

		// Token: 0x04004AE2 RID: 19170
		public BuilderPiece myPiece;

		// Token: 0x04004AE3 RID: 19171
		[SerializeField]
		private Transform wheelTransform;

		// Token: 0x04004AE4 RID: 19172
		[SerializeField]
		private Transform cowboyTransform;

		// Token: 0x04004AE5 RID: 19173
		[SerializeField]
		private SlingshotProjectileHitNotifier wheelHitNotifier;

		// Token: 0x04004AE6 RID: 19174
		[SerializeField]
		private SlingshotProjectileHitNotifier cowboyHitNotifier;

		// Token: 0x04004AE7 RID: 19175
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04004AE8 RID: 19176
		[SerializeField]
		protected SoundBankPlayer wheelHitSound;

		// Token: 0x04004AE9 RID: 19177
		[SerializeField]
		protected Animation wheelHitAnimation;

		// Token: 0x04004AEA RID: 19178
		[SerializeField]
		protected SoundBankPlayer cowboyHitSound;

		// Token: 0x04004AEB RID: 19179
		[SerializeField]
		private Animation cowboyHitAnimation;

		// Token: 0x04004AEC RID: 19180
		[SerializeField]
		private float hitCooldown = 1f;

		// Token: 0x04004AED RID: 19181
		private double lastHitTime;

		// Token: 0x04004AEE RID: 19182
		private BuilderShootingGallery.FunctionalState currentState;

		// Token: 0x04004AEF RID: 19183
		private bool activated;

		// Token: 0x04004AF0 RID: 19184
		[SerializeField]
		private float cowboyVelocity;

		// Token: 0x04004AF1 RID: 19185
		[SerializeField]
		private Transform cowboyStart;

		// Token: 0x04004AF2 RID: 19186
		[SerializeField]
		private Transform cowboyEnd;

		// Token: 0x04004AF3 RID: 19187
		[SerializeField]
		private AnimationCurve cowboyCurve;

		// Token: 0x04004AF4 RID: 19188
		[SerializeField]
		private float wheelVelocity;

		// Token: 0x04004AF5 RID: 19189
		private Quaternion cowboyInitLocalRotation = Quaternion.identity;

		// Token: 0x04004AF6 RID: 19190
		private Vector3 cowboyInitLocalPos = Vector3.zero;

		// Token: 0x04004AF7 RID: 19191
		private Quaternion wheelInitLocalRot = Quaternion.identity;

		// Token: 0x04004AF8 RID: 19192
		private float cowboyCycleDuration;

		// Token: 0x04004AF9 RID: 19193
		private float wheelCycleDuration;

		// Token: 0x04004AFA RID: 19194
		private float distance;

		// Token: 0x04004AFB RID: 19195
		private float currT;

		// Token: 0x04004AFC RID: 19196
		private bool currForward;

		// Token: 0x04004AFD RID: 19197
		private float dtSinceServerUpdate;

		// Token: 0x04004AFE RID: 19198
		private int lastServerTimeStamp;

		// Token: 0x04004AFF RID: 19199
		private float rotateStartAmt;

		// Token: 0x04004B00 RID: 19200
		private float rotateAmt;

		// Token: 0x02000B6B RID: 2923
		private enum FunctionalState
		{
			// Token: 0x04004B02 RID: 19202
			Idle,
			// Token: 0x04004B03 RID: 19203
			HitWheel,
			// Token: 0x04004B04 RID: 19204
			HitCowboy
		}
	}
}
