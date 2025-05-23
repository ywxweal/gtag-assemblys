using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B67 RID: 2919
	public class BuilderProjectileTarget : MonoBehaviour, IBuilderPieceFunctional
	{
		// Token: 0x06004838 RID: 18488 RVA: 0x00158724 File Offset: 0x00156924
		private void Awake()
		{
			this.hitNotifier.OnProjectileHit += this.OnProjectileHit;
			foreach (Collider collider in this.colliders)
			{
				collider.contactOffset = 0.0001f;
			}
		}

		// Token: 0x06004839 RID: 18489 RVA: 0x00158790 File Offset: 0x00156990
		private void OnDestroy()
		{
			this.hitNotifier.OnProjectileHit -= this.OnProjectileHit;
		}

		// Token: 0x0600483A RID: 18490 RVA: 0x001587A9 File Offset: 0x001569A9
		private void OnDisable()
		{
			this.hitCount = 0;
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x0600483B RID: 18491 RVA: 0x001587DC File Offset: 0x001569DC
		private void OnProjectileHit(SlingshotProjectile projectile, Collision collision)
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
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 11);
			}
		}

		// Token: 0x0600483C RID: 18492 RVA: 0x0015884A File Offset: 0x00156A4A
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
			if (newState == 11)
			{
				return;
			}
			this.lastHitTime = (double)Time.time;
			this.hitCount = Mathf.Clamp((int)newState, 0, 10);
			this.PlayHitEffects();
		}

		// Token: 0x0600483D RID: 18493 RVA: 0x00158884 File Offset: 0x00156A84
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (instigator == null)
			{
				return;
			}
			if (newState != 11)
			{
				return;
			}
			this.hitCount++;
			this.hitCount %= 11;
			this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, (byte)this.hitCount, instigator.GetPlayerRef(), timeStamp);
		}

		// Token: 0x0600483E RID: 18494 RVA: 0x001588FD File Offset: 0x00156AFD
		public bool IsStateValid(byte state)
		{
			return state <= 11;
		}

		// Token: 0x0600483F RID: 18495 RVA: 0x00158908 File Offset: 0x00156B08
		private void PlayHitEffects()
		{
			if (this.hitSoundbank != null)
			{
				this.hitSoundbank.Play();
			}
			if (this.hitAnimation != null && this.hitAnimation.clip != null)
			{
				this.hitAnimation.Play();
			}
			if (this.scoreText != null)
			{
				this.scoreText.text = this.hitCount.ToString("D2");
			}
		}

		// Token: 0x06004840 RID: 18496 RVA: 0x000023F4 File Offset: 0x000005F4
		public void FunctionalPieceUpdate()
		{
		}

		// Token: 0x06004841 RID: 18497 RVA: 0x00158984 File Offset: 0x00156B84
		public float GetInteractionDistace()
		{
			return 20f;
		}

		// Token: 0x04004AB1 RID: 19121
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x04004AB2 RID: 19122
		[SerializeField]
		private SlingshotProjectileHitNotifier hitNotifier;

		// Token: 0x04004AB3 RID: 19123
		[SerializeField]
		protected float hitCooldown = 2f;

		// Token: 0x04004AB4 RID: 19124
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected SoundBankPlayer hitSoundbank;

		// Token: 0x04004AB5 RID: 19125
		[Tooltip("Optional Sounds to play on hit")]
		[SerializeField]
		protected Animation hitAnimation;

		// Token: 0x04004AB6 RID: 19126
		[SerializeField]
		protected List<Collider> colliders;

		// Token: 0x04004AB7 RID: 19127
		[SerializeField]
		private TMP_Text scoreText;

		// Token: 0x04004AB8 RID: 19128
		private double lastHitTime;

		// Token: 0x04004AB9 RID: 19129
		private int hitCount;

		// Token: 0x04004ABA RID: 19130
		private const byte MAX_SCORE = 10;

		// Token: 0x04004ABB RID: 19131
		private const byte HIT = 11;
	}
}
