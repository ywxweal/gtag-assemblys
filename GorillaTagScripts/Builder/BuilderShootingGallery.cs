using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B6A RID: 2922
	public class BuilderShootingGallery : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004851 RID: 18513 RVA: 0x00159198 File Offset: 0x00157398
		private void Awake()
		{
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
			this.wheelHitNotifier.OnProjectileHit += this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit += this.OnCowboyHit;
		}

		// Token: 0x06004852 RID: 18514 RVA: 0x0015921C File Offset: 0x0015741C
		private void OnDestroy()
		{
			this.wheelHitNotifier.OnProjectileHit -= this.OnWheelHit;
			this.cowboyHitNotifier.OnProjectileHit -= this.OnCowboyHit;
		}

		// Token: 0x06004853 RID: 18515 RVA: 0x0015924C File Offset: 0x0015744C
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

		// Token: 0x06004854 RID: 18516 RVA: 0x001592BC File Offset: 0x001574BC
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

		// Token: 0x06004855 RID: 18517 RVA: 0x0015932C File Offset: 0x0015752C
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

		// Token: 0x06004856 RID: 18518 RVA: 0x00159380 File Offset: 0x00157580
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

		// Token: 0x06004857 RID: 18519 RVA: 0x001593D4 File Offset: 0x001575D4
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

		// Token: 0x06004858 RID: 18520 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004859 RID: 18521 RVA: 0x0015947C File Offset: 0x0015767C
		public void OnPiecePlacementDeserialized()
		{
			if (!this.activated && this.myPiece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				this.myPiece.GetTable().RegisterFunctionalPieceFixedUpdate(this);
				this.activated = true;
			}
		}

		// Token: 0x0600485A RID: 18522 RVA: 0x001594AC File Offset: 0x001576AC
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

		// Token: 0x0600485B RID: 18523 RVA: 0x0015950C File Offset: 0x0015770C
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

		// Token: 0x0600485C RID: 18524 RVA: 0x001595A8 File Offset: 0x001577A8
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

		// Token: 0x0600485D RID: 18525 RVA: 0x0015962C File Offset: 0x0015782C
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

		// Token: 0x0600485E RID: 18526 RVA: 0x00159691 File Offset: 0x00157891
		public bool IsStateValid(byte state)
		{
			return state <= 2;
		}

		// Token: 0x0600485F RID: 18527 RVA: 0x0015969C File Offset: 0x0015789C
		public void FunctionalPieceUpdate()
		{
			if (this.lastHitTime + (double)this.hitCooldown < (double)Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06004860 RID: 18528 RVA: 0x001596F0 File Offset: 0x001578F0
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

		// Token: 0x06004861 RID: 18529 RVA: 0x00159797 File Offset: 0x00157997
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06004862 RID: 18530 RVA: 0x001597B9 File Offset: 0x001579B9
		private long CowboyCycleLengthMs()
		{
			return (long)(this.cowboyCycleDuration * 1000f);
		}

		// Token: 0x06004863 RID: 18531 RVA: 0x001597C8 File Offset: 0x001579C8
		private long WheelCycleLengthMs()
		{
			return (long)(this.wheelCycleDuration * 1000f);
		}

		// Token: 0x06004864 RID: 18532 RVA: 0x001597D8 File Offset: 0x001579D8
		public double CowboyPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CowboyCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004865 RID: 18533 RVA: 0x00159804 File Offset: 0x00157A04
		public double WheelPlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.WheelCycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004866 RID: 18534 RVA: 0x0015982F File Offset: 0x00157A2F
		public int CowboyCycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CowboyCycleLengthMs());
		}

		// Token: 0x06004867 RID: 18535 RVA: 0x0015983F File Offset: 0x00157A3F
		public float CowboyCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.CowboyPlatformTime() / (double)this.cowboyCycleDuration), 0f, 1f);
		}

		// Token: 0x06004868 RID: 18536 RVA: 0x0015985F File Offset: 0x00157A5F
		public float WheelCycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.WheelPlatformTime() / (double)this.wheelCycleDuration), 0f, 1f);
		}

		// Token: 0x06004869 RID: 18537 RVA: 0x0015987F File Offset: 0x00157A7F
		public bool IsEvenCycle()
		{
			return this.CowboyCycleCount() % 2 == 0;
		}

		// Token: 0x04004AE1 RID: 19169
		public BuilderPiece myPiece;

		// Token: 0x04004AE2 RID: 19170
		[SerializeField]
		private Transform wheelTransform;

		// Token: 0x04004AE3 RID: 19171
		[SerializeField]
		private Transform cowboyTransform;

		// Token: 0x04004AE4 RID: 19172
		[SerializeField]
		private SlingshotProjectileHitNotifier wheelHitNotifier;

		// Token: 0x04004AE5 RID: 19173
		[SerializeField]
		private SlingshotProjectileHitNotifier cowboyHitNotifier;

		// Token: 0x04004AE6 RID: 19174
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04004AE7 RID: 19175
		[SerializeField]
		protected SoundBankPlayer wheelHitSound;

		// Token: 0x04004AE8 RID: 19176
		[SerializeField]
		protected Animation wheelHitAnimation;

		// Token: 0x04004AE9 RID: 19177
		[SerializeField]
		protected SoundBankPlayer cowboyHitSound;

		// Token: 0x04004AEA RID: 19178
		[SerializeField]
		private Animation cowboyHitAnimation;

		// Token: 0x04004AEB RID: 19179
		[SerializeField]
		private float hitCooldown = 1f;

		// Token: 0x04004AEC RID: 19180
		private double lastHitTime;

		// Token: 0x04004AED RID: 19181
		private BuilderShootingGallery.FunctionalState currentState;

		// Token: 0x04004AEE RID: 19182
		private bool activated;

		// Token: 0x04004AEF RID: 19183
		[SerializeField]
		private float cowboyVelocity;

		// Token: 0x04004AF0 RID: 19184
		[SerializeField]
		private Transform cowboyStart;

		// Token: 0x04004AF1 RID: 19185
		[SerializeField]
		private Transform cowboyEnd;

		// Token: 0x04004AF2 RID: 19186
		[SerializeField]
		private AnimationCurve cowboyCurve;

		// Token: 0x04004AF3 RID: 19187
		[SerializeField]
		private float wheelVelocity;

		// Token: 0x04004AF4 RID: 19188
		private Quaternion cowboyInitLocalRotation = Quaternion.identity;

		// Token: 0x04004AF5 RID: 19189
		private Vector3 cowboyInitLocalPos = Vector3.zero;

		// Token: 0x04004AF6 RID: 19190
		private Quaternion wheelInitLocalRot = Quaternion.identity;

		// Token: 0x04004AF7 RID: 19191
		private float cowboyCycleDuration;

		// Token: 0x04004AF8 RID: 19192
		private float wheelCycleDuration;

		// Token: 0x04004AF9 RID: 19193
		private float distance;

		// Token: 0x04004AFA RID: 19194
		private float currT;

		// Token: 0x04004AFB RID: 19195
		private bool currForward;

		// Token: 0x04004AFC RID: 19196
		private float dtSinceServerUpdate;

		// Token: 0x04004AFD RID: 19197
		private int lastServerTimeStamp;

		// Token: 0x04004AFE RID: 19198
		private float rotateStartAmt;

		// Token: 0x04004AFF RID: 19199
		private float rotateAmt;

		// Token: 0x02000B6B RID: 2923
		private enum FunctionalState
		{
			// Token: 0x04004B01 RID: 19201
			Idle,
			// Token: 0x04004B02 RID: 19202
			HitWheel,
			// Token: 0x04004B03 RID: 19203
			HitCowboy
		}
	}
}
