using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D24 RID: 3364
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticTryOnNotifier : MonoBehaviour
	{
		// Token: 0x06005412 RID: 21522 RVA: 0x00197634 File Offset: 0x00195834
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(out this.m_vrrigCollection))
			{
				this.m_vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this.m_vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this.m_vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x06005413 RID: 21523 RVA: 0x001976A9 File Offset: 0x001958A9
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			PlayerCosmeticsSystem.SetRigTryOn(true, playerRig);
		}

		// Token: 0x06005414 RID: 21524 RVA: 0x001976B2 File Offset: 0x001958B2
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			PlayerCosmeticsSystem.SetRigTryOn(false, playerRig);
		}

		// Token: 0x0400570D RID: 22285
		private VRRigCollection m_vrrigCollection;
	}
}
