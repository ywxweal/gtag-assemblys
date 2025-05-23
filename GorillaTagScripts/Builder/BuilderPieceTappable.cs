using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B60 RID: 2912
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(GorillaSurfaceOverride))]
	public class BuilderPieceTappable : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x060047FB RID: 18427 RVA: 0x00157616 File Offset: 0x00155816
		public virtual bool CanTap()
		{
			return this.isPieceActive && Time.time > this.lastTapTime + this.tapCooldown;
		}

		// Token: 0x060047FC RID: 18428 RVA: 0x00157636 File Offset: 0x00155836
		public void OnTapLocal(float tapStrength)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			if (!this.CanTap())
			{
				return;
			}
			this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
		}

		// Token: 0x060047FD RID: 18429 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void OnTapReplicated()
		{
		}

		// Token: 0x060047FE RID: 18430 RVA: 0x0015766F File Offset: 0x0015586F
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderPieceTappable.FunctionalState.Idle;
		}

		// Token: 0x060047FF RID: 18431 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004800 RID: 18432 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004801 RID: 18433 RVA: 0x00157678 File Offset: 0x00155878
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
		}

		// Token: 0x06004802 RID: 18434 RVA: 0x00157684 File Offset: 0x00155884
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			if (this.currentState == BuilderPieceTappable.FunctionalState.Tap)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06004803 RID: 18435 RVA: 0x001576D4 File Offset: 0x001558D4
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderPieceTappable.FunctionalState.Tap)
			{
				this.lastTapTime = Time.time;
				this.OnTapReplicated();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderPieceTappable.FunctionalState)newState;
		}

		// Token: 0x06004804 RID: 18436 RVA: 0x00157724 File Offset: 0x00155924
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
			if (newState == 1 && this.CanTap())
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06004805 RID: 18437 RVA: 0x00153493 File Offset: 0x00151693
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06004806 RID: 18438 RVA: 0x00157780 File Offset: 0x00155980
		public void FunctionalPieceUpdate()
		{
			if (this.lastTapTime + this.tapCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x04004A7B RID: 19067
		[SerializeField]
		protected BuilderPiece myPiece;

		// Token: 0x04004A7C RID: 19068
		[SerializeField]
		protected float tapCooldown = 0.5f;

		// Token: 0x04004A7D RID: 19069
		private bool isPieceActive;

		// Token: 0x04004A7E RID: 19070
		private float lastTapTime;

		// Token: 0x04004A7F RID: 19071
		private BuilderPieceTappable.FunctionalState currentState;

		// Token: 0x02000B61 RID: 2913
		private enum FunctionalState
		{
			// Token: 0x04004A81 RID: 19073
			Idle,
			// Token: 0x04004A82 RID: 19074
			Tap
		}
	}
}
