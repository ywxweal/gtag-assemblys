using System;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DD2 RID: 3538
	public class ChickenSword : MonoBehaviour
	{
		// Token: 0x060057C1 RID: 22465 RVA: 0x001AF705 File Offset: 0x001AD905
		private void Awake()
		{
			this.lastHitTime = float.PositiveInfinity;
			this.SwitchState(ChickenSword.SwordState.Ready);
		}

		// Token: 0x060057C2 RID: 22466 RVA: 0x001AF71C File Offset: 0x001AD91C
		private void Update()
		{
			ChickenSword.SwordState swordState = this.currentState;
			if (swordState != ChickenSword.SwordState.Ready)
			{
				if (swordState != ChickenSword.SwordState.Deflated)
				{
					return;
				}
				if (Time.time - this.lastHitTime > this.rechargeCooldown)
				{
					this.lastHitTime = float.PositiveInfinity;
					this.SwitchState(ChickenSword.SwordState.Ready);
					UnityEvent onRechargedShared = this.OnRechargedShared;
					if (onRechargedShared != null)
					{
						onRechargedShared.Invoke();
					}
					if (this.transferrableObject && this.transferrableObject.IsMyItem())
					{
						UnityEvent<bool> onRechargedLocal = this.OnRechargedLocal;
						if (onRechargedLocal == null)
						{
							return;
						}
						onRechargedLocal.Invoke(this.transferrableObject.InLeftHand());
					}
				}
			}
			else if (this.hitReceievd)
			{
				this.hitReceievd = false;
				this.lastHitTime = Time.time;
				this.SwitchState(ChickenSword.SwordState.Deflated);
				UnityEvent onDeflatedShared = this.OnDeflatedShared;
				if (onDeflatedShared != null)
				{
					onDeflatedShared.Invoke();
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					UnityEvent<bool> onDeflatedLocal = this.OnDeflatedLocal;
					if (onDeflatedLocal == null)
					{
						return;
					}
					onDeflatedLocal.Invoke(this.transferrableObject.InLeftHand());
					return;
				}
			}
		}

		// Token: 0x060057C3 RID: 22467 RVA: 0x001AF818 File Offset: 0x001ADA18
		public void OnHitTargetSync(VRRig playerRig)
		{
			if (this.velocityTracker == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (this.currentState == ChickenSword.SwordState.Ready && averageVelocity.magnitude > this.hitVelocityThreshold)
			{
				this.hitReceievd = true;
				UnityEvent<VRRig> onHitTargetShared = this.OnHitTargetShared;
				if (onHitTargetShared != null)
				{
					onHitTargetShared.Invoke(playerRig);
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					bool flag = this.transferrableObject.InLeftHand();
					UnityEvent<bool> onHitTargetLocal = this.OnHitTargetLocal;
					if (onHitTargetLocal == null)
					{
						return;
					}
					onHitTargetLocal.Invoke(flag);
				}
			}
		}

		// Token: 0x060057C4 RID: 22468 RVA: 0x001AF8AE File Offset: 0x001ADAAE
		private void SwitchState(ChickenSword.SwordState newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04005C7F RID: 23679
		[SerializeField]
		private float rechargeCooldown;

		// Token: 0x04005C80 RID: 23680
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04005C81 RID: 23681
		[SerializeField]
		private float hitVelocityThreshold;

		// Token: 0x04005C82 RID: 23682
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04005C83 RID: 23683
		[Space]
		[Space]
		public UnityEvent OnDeflatedShared;

		// Token: 0x04005C84 RID: 23684
		public UnityEvent<bool> OnDeflatedLocal;

		// Token: 0x04005C85 RID: 23685
		public UnityEvent OnRechargedShared;

		// Token: 0x04005C86 RID: 23686
		public UnityEvent<bool> OnRechargedLocal;

		// Token: 0x04005C87 RID: 23687
		public UnityEvent<VRRig> OnHitTargetShared;

		// Token: 0x04005C88 RID: 23688
		public UnityEvent<bool> OnHitTargetLocal;

		// Token: 0x04005C89 RID: 23689
		private float lastHitTime;

		// Token: 0x04005C8A RID: 23690
		private ChickenSword.SwordState currentState;

		// Token: 0x04005C8B RID: 23691
		private bool hitReceievd;

		// Token: 0x02000DD3 RID: 3539
		private enum SwordState
		{
			// Token: 0x04005C8D RID: 23693
			Ready,
			// Token: 0x04005C8E RID: 23694
			Deflated
		}
	}
}
