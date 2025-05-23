using System;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using UnityEngine;

// Token: 0x0200091A RID: 2330
public class PUNErrorLogging : MonoBehaviour
{
	// Token: 0x060038DA RID: 14554 RVA: 0x0011235C File Offset: 0x0011055C
	private void Start()
	{
		PhotonNetwork.InternalEventError = (Action<EventData, Exception>)Delegate.Combine(PhotonNetwork.InternalEventError, new Action<EventData, Exception>(this.PUNError));
		PlayFabTitleDataCache.Instance.GetTitleData("PUNErrorLogging", delegate(string data)
		{
			int num;
			if (!int.TryParse(data, out num))
			{
				return;
			}
			PUNErrorLogging.LogFlags logFlags = (PUNErrorLogging.LogFlags)num;
			this.m_logSerializeView = logFlags.HasFlag(PUNErrorLogging.LogFlags.SerializeView);
			this.m_logOwnershipTransfer = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipTransfer);
			this.m_logOwnershipRequest = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipRequest);
			this.m_logOwnershipUpdate = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipUpdate);
			this.m_logRPC = logFlags.HasFlag(PUNErrorLogging.LogFlags.RPC);
			this.m_logInstantiate = logFlags.HasFlag(PUNErrorLogging.LogFlags.Instantiate);
			this.m_logDestroy = logFlags.HasFlag(PUNErrorLogging.LogFlags.Destroy);
			this.m_logDestroyPlayer = logFlags.HasFlag(PUNErrorLogging.LogFlags.DestroyPlayer);
		}, delegate(PlayFabError error)
		{
		});
	}

	// Token: 0x060038DB RID: 14555 RVA: 0x001123C4 File Offset: 0x001105C4
	private void PUNError(EventData data, Exception exception)
	{
		NetworkSystem.Instance.GetPlayer(data.Sender);
		byte code = data.Code;
		switch (code)
		{
		case 200:
			this.PrintException(exception, this.m_logRPC);
			return;
		case 201:
		case 206:
			this.PrintException(exception, this.m_logSerializeView);
			return;
		case 202:
			this.PrintException(exception, this.m_logInstantiate);
			return;
		case 203:
		case 205:
		case 208:
		case 211:
			break;
		case 204:
			this.PrintException(exception, this.m_logDestroy);
			return;
		case 207:
			this.PrintException(exception, this.m_logDestroyPlayer);
			return;
		case 209:
			this.PrintException(exception, this.m_logOwnershipRequest);
			return;
		case 210:
			this.PrintException(exception, this.m_logOwnershipTransfer);
			return;
		case 212:
			this.PrintException(exception, this.m_logOwnershipUpdate);
			return;
		default:
			if (code == 254)
			{
				this.PrintException(exception, true);
				return;
			}
			break;
		}
		this.PrintException(exception, true);
	}

	// Token: 0x060038DC RID: 14556 RVA: 0x001124B2 File Offset: 0x001106B2
	private void PrintException(Exception e, bool print)
	{
		if (print)
		{
			Debug.LogException(e);
		}
	}

	// Token: 0x04003E06 RID: 15878
	[SerializeField]
	private bool m_logSerializeView = true;

	// Token: 0x04003E07 RID: 15879
	[SerializeField]
	private bool m_logOwnershipTransfer = true;

	// Token: 0x04003E08 RID: 15880
	[SerializeField]
	private bool m_logOwnershipRequest = true;

	// Token: 0x04003E09 RID: 15881
	[SerializeField]
	private bool m_logOwnershipUpdate = true;

	// Token: 0x04003E0A RID: 15882
	[SerializeField]
	private bool m_logRPC = true;

	// Token: 0x04003E0B RID: 15883
	[SerializeField]
	private bool m_logInstantiate = true;

	// Token: 0x04003E0C RID: 15884
	[SerializeField]
	private bool m_logDestroy = true;

	// Token: 0x04003E0D RID: 15885
	[SerializeField]
	private bool m_logDestroyPlayer = true;

	// Token: 0x0200091B RID: 2331
	[Flags]
	private enum LogFlags
	{
		// Token: 0x04003E0F RID: 15887
		SerializeView = 1,
		// Token: 0x04003E10 RID: 15888
		OwnershipTransfer = 2,
		// Token: 0x04003E11 RID: 15889
		OwnershipRequest = 4,
		// Token: 0x04003E12 RID: 15890
		OwnershipUpdate = 8,
		// Token: 0x04003E13 RID: 15891
		RPC = 16,
		// Token: 0x04003E14 RID: 15892
		Instantiate = 32,
		// Token: 0x04003E15 RID: 15893
		Destroy = 64,
		// Token: 0x04003E16 RID: 15894
		DestroyPlayer = 128
	}
}
