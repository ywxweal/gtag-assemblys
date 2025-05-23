using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B65 RID: 2917
	public class BuilderProjectileLauncher : MonoBehaviour, IBuilderPieceFunctional, IBuilderPieceComponent
	{
		// Token: 0x0600482B RID: 18475 RVA: 0x0015839C File Offset: 0x0015659C
		private void LaunchProjectile(int timeStamp)
		{
			if (Time.time > this.lastFireTime + this.fireCooldown)
			{
				this.lastFireTime = Time.time;
				int num = PoolUtils.GameObjHashCode(this.projectilePrefab);
				try
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(num, true);
					this.projectileScale = this.myPiece.GetScale();
					gameObject.transform.localScale = Vector3.one * this.projectileScale;
					BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
					int num2 = HashCode.Combine<int, int>(this.myPiece.pieceId, timeStamp);
					if (this.allProjectiles.ContainsKey(num2))
					{
						this.allProjectiles.Remove(num2);
					}
					this.allProjectiles.Add(num2, component);
					SlingshotProjectile.AOEKnockbackConfig aoeknockbackConfig = new SlingshotProjectile.AOEKnockbackConfig
					{
						aeoOuterRadius = this.knockbackConfig.aeoOuterRadius * this.projectileScale,
						aeoInnerRadius = this.knockbackConfig.aeoInnerRadius * this.projectileScale,
						applyAOEKnockback = this.knockbackConfig.applyAOEKnockback,
						impactVelocityThreshold = this.knockbackConfig.impactVelocityThreshold * this.projectileScale,
						knockbackVelocity = this.knockbackConfig.knockbackVelocity * this.projectileScale,
						playerProximityEffect = this.knockbackConfig.playerProximityEffect
					};
					component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeknockbackConfig);
					component.gravityMultiplier = this.gravityMultiplier;
					component.Launch(this.launchPosition.position, this.launchVelocity * this.projectileScale * this.launchPosition.up, this, num2, this.projectileScale, timeStamp);
					if (this.launchSound != null && this.launchSound.clip != null)
					{
						this.launchSound.Play();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					throw;
				}
			}
		}

		// Token: 0x0600482C RID: 18476 RVA: 0x00158584 File Offset: 0x00156784
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if ((BuilderProjectileLauncher.FunctionalState)newState == this.currentState)
			{
				return;
			}
			this.currentState = (BuilderProjectileLauncher.FunctionalState)newState;
			if (newState == 1)
			{
				this.LaunchProjectile(timeStamp);
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x0600482D RID: 18477 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnStateRequest(byte newState, NetPlayer instigator, int timeStamp)
		{
		}

		// Token: 0x0600482E RID: 18478 RVA: 0x00153493 File Offset: 0x00151693
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x0600482F RID: 18479 RVA: 0x001585D8 File Offset: 0x001567D8
		public void FunctionalPieceUpdate()
		{
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].UpdateProjectile();
			}
			if (PhotonNetwork.IsMasterClient && this.lastFireTime + this.fireCooldown < Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, 1, PhotonNetwork.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
			}
		}

		// Token: 0x06004830 RID: 18480 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06004831 RID: 18481 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004832 RID: 18482 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004833 RID: 18483 RVA: 0x00158659 File Offset: 0x00156859
		public void OnPieceActivate()
		{
			this.myPiece.GetTable().RegisterFunctionalPiece(this);
		}

		// Token: 0x06004834 RID: 18484 RVA: 0x0015866C File Offset: 0x0015686C
		public void OnPieceDeactivate()
		{
			this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			for (int i = this.launchedProjectiles.Count - 1; i >= 0; i--)
			{
				this.launchedProjectiles[i].Deactivate();
			}
		}

		// Token: 0x06004835 RID: 18485 RVA: 0x001586B3 File Offset: 0x001568B3
		public void RegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Add(projectile);
		}

		// Token: 0x06004836 RID: 18486 RVA: 0x001586C1 File Offset: 0x001568C1
		public void UnRegisterProjectile(BuilderProjectile projectile)
		{
			this.launchedProjectiles.Remove(projectile);
			this.allProjectiles.Remove(projectile.projectileId);
		}

		// Token: 0x04004AA1 RID: 19105
		private List<BuilderProjectile> launchedProjectiles = new List<BuilderProjectile>();

		// Token: 0x04004AA2 RID: 19106
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x04004AA3 RID: 19107
		[SerializeField]
		protected float fireCooldown = 2f;

		// Token: 0x04004AA4 RID: 19108
		[Tooltip("launch in Y direction")]
		[SerializeField]
		private Transform launchPosition;

		// Token: 0x04004AA5 RID: 19109
		[SerializeField]
		private float launchVelocity;

		// Token: 0x04004AA6 RID: 19110
		[SerializeField]
		private AudioSource launchSound;

		// Token: 0x04004AA7 RID: 19111
		[SerializeField]
		protected GameObject projectilePrefab;

		// Token: 0x04004AA8 RID: 19112
		protected float projectileScale = 0.06f;

		// Token: 0x04004AA9 RID: 19113
		[SerializeField]
		protected float gravityMultiplier = 1f;

		// Token: 0x04004AAA RID: 19114
		public SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

		// Token: 0x04004AAB RID: 19115
		private float lastFireTime;

		// Token: 0x04004AAC RID: 19116
		private BuilderProjectileLauncher.FunctionalState currentState;

		// Token: 0x04004AAD RID: 19117
		private Dictionary<int, BuilderProjectile> allProjectiles = new Dictionary<int, BuilderProjectile>();

		// Token: 0x02000B66 RID: 2918
		private enum FunctionalState
		{
			// Token: 0x04004AAF RID: 19119
			Idle,
			// Token: 0x04004AB0 RID: 19120
			Fire
		}
	}
}
