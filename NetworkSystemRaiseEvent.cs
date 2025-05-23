using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020002B4 RID: 692
public static class NetworkSystemRaiseEvent
{
	// Token: 0x0600109A RID: 4250 RVA: 0x0005007A File Offset: 0x0004E27A
	public static void RaiseEvent(byte code, object data)
	{
		PhotonNetwork.RaiseEvent(code, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x00050090 File Offset: 0x0004E290
	public static void RaiseEvent(byte code, object data, NetEventOptions options, bool reliable)
	{
		PhotonNetwork.RaiseEvent(code, data, new RaiseEventOptions
		{
			TargetActors = options.TargetActors,
			Receivers = (ReceiverGroup)options.Reciever,
			Flags = options.Flags
		}, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
	}

	// Token: 0x040012EA RID: 4842
	public static readonly NetEventOptions neoOthers = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.others
	};

	// Token: 0x040012EB RID: 4843
	public static readonly NetEventOptions neoMaster = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.master
	};

	// Token: 0x040012EC RID: 4844
	public static readonly NetEventOptions neoTarget = new NetEventOptions
	{
		TargetActors = new int[1]
	};

	// Token: 0x040012ED RID: 4845
	public static readonly NetEventOptions newWeb = new NetEventOptions
	{
		Flags = new WebFlags(1)
	};
}
