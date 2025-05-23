using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020008E9 RID: 2281
internal class OwnershipGaurdHandler : IPunOwnershipCallbacks
{
	// Token: 0x06003767 RID: 14183 RVA: 0x0010BD5D File Offset: 0x00109F5D
	static OwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(OwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x0010BD7D File Offset: 0x00109F7D
	internal static void RegisterView(PhotonView view)
	{
		if (view == null || OwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		OwnershipGaurdHandler.gaurdedViews.Add(view);
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x0010BDA4 File Offset: 0x00109FA4
	internal static void RegisterViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGaurdHandler.RegisterView(photonViews[i]);
		}
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x0010BDC9 File Offset: 0x00109FC9
	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		OwnershipGaurdHandler.gaurdedViews.Remove(view);
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x0010BDE4 File Offset: 0x00109FE4
	internal static void RemoveViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGaurdHandler.RemoveView(photonViews[i]);
		}
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x0010BE0C File Offset: 0x0010A00C
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (!OwnershipGaurdHandler.gaurdedViews.Contains(targetView))
		{
			return;
		}
		if (targetView.IsRoomView)
		{
			if (targetView.Owner != PhotonNetwork.MasterClient)
			{
				targetView.OwnerActorNr = 0;
				targetView.ControllerActorNr = 0;
				return;
			}
		}
		else if (targetView.OwnerActorNr != targetView.CreatorActorNr || targetView.ControllerActorNr != targetView.CreatorActorNr)
		{
			targetView.OwnerActorNr = targetView.CreatorActorNr;
			targetView.ControllerActorNr = targetView.CreatorActorNr;
		}
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x000023F4 File Offset: 0x000005F4
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x000023F4 File Offset: 0x000005F4
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x04003CFD RID: 15613
	private static HashSet<PhotonView> gaurdedViews = new HashSet<PhotonView>();

	// Token: 0x04003CFE RID: 15614
	private static readonly OwnershipGaurdHandler callbackInstance = new OwnershipGaurdHandler();
}
