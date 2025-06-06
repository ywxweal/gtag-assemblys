﻿using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B4F RID: 2895
	public class BuilderAnimatePieceOnTriggerEnter : MonoBehaviour, IBuilderPieceComponent, IBuilderPieceFunctional
	{
		// Token: 0x06004760 RID: 18272 RVA: 0x001533F8 File Offset: 0x001515F8
		private void OnTriggerEnter(Collider other)
		{
			if (this.lastAnimateTime + this.animateCooldown < Time.time)
			{
				this.myPiece.GetTable().builderNetworking.RequestFunctionalPieceStateChange(this.myPiece.pieceId, 1);
			}
		}

		// Token: 0x06004761 RID: 18273 RVA: 0x0015342F File Offset: 0x0015162F
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.currentState = BuilderAnimatePieceOnTriggerEnter.FunctionalState.Idle;
			this.trigger.enabled = false;
		}

		// Token: 0x06004762 RID: 18274 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06004763 RID: 18275 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06004764 RID: 18276 RVA: 0x00153444 File Offset: 0x00151644
		public void OnPieceActivate()
		{
			this.trigger.enabled = true;
		}

		// Token: 0x06004765 RID: 18277 RVA: 0x00153454 File Offset: 0x00151654
		public void OnPieceDeactivate()
		{
			this.trigger.enabled = false;
			if (this.currentState == BuilderAnimatePieceOnTriggerEnter.FunctionalState.Animating)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x06004766 RID: 18278 RVA: 0x001534A8 File Offset: 0x001516A8
		public void OnStateChanged(byte newState, NetPlayer instigator, int timeStamp)
		{
			if (!this.IsStateValid(newState))
			{
				return;
			}
			if (newState == 1 && this.currentState != BuilderAnimatePieceOnTriggerEnter.FunctionalState.Animating)
			{
				this.lastAnimateTime = Time.time;
				this.anim.Rewind();
				this.anim.Play();
				this.myPiece.GetTable().RegisterFunctionalPiece(this);
			}
			this.currentState = (BuilderAnimatePieceOnTriggerEnter.FunctionalState)newState;
		}

		// Token: 0x06004767 RID: 18279 RVA: 0x00153508 File Offset: 0x00151708
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
			if (this.lastAnimateTime + this.animateCooldown < Time.time)
			{
				this.myPiece.GetTable().builderNetworking.FunctionalPieceStateChangeMaster(this.myPiece.pieceId, newState, instigator.GetPlayerRef(), timeStamp);
			}
		}

		// Token: 0x06004768 RID: 18280 RVA: 0x0015356B File Offset: 0x0015176B
		public bool IsStateValid(byte state)
		{
			return state <= 1;
		}

		// Token: 0x06004769 RID: 18281 RVA: 0x00153574 File Offset: 0x00151774
		public void FunctionalPieceUpdate()
		{
			if (this.lastAnimateTime + this.animateCooldown < Time.time)
			{
				this.myPiece.SetFunctionalPieceState(0, NetworkSystem.Instance.LocalPlayer, NetworkSystem.Instance.ServerTimestamp);
				this.myPiece.GetTable().UnregisterFunctionalPiece(this);
			}
		}

		// Token: 0x040049B1 RID: 18865
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040049B2 RID: 18866
		[SerializeField]
		private Collider trigger;

		// Token: 0x040049B3 RID: 18867
		[SerializeField]
		private Animation anim;

		// Token: 0x040049B4 RID: 18868
		[SerializeField]
		private float animateCooldown = 0.5f;

		// Token: 0x040049B5 RID: 18869
		private float lastAnimateTime;

		// Token: 0x040049B6 RID: 18870
		private BuilderAnimatePieceOnTriggerEnter.FunctionalState currentState;

		// Token: 0x02000B50 RID: 2896
		private enum FunctionalState
		{
			// Token: 0x040049B8 RID: 18872
			Idle,
			// Token: 0x040049B9 RID: 18873
			Animating
		}
	}
}
