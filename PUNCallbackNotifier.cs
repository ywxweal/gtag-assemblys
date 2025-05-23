using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class PUNCallbackNotifier : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x0600114D RID: 4429 RVA: 0x00053CDE File Offset: 0x00051EDE
	private void Start()
	{
		this.parentSystem = base.GetComponent<NetworkSystemPUN>();
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Update()
	{
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x00053CEC File Offset: 0x00051EEC
	public override void OnConnectedToMaster()
	{
		this.parentSystem.OnConnectedtoMaster();
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x00053CF9 File Offset: 0x00051EF9
	public override void OnJoinedRoom()
	{
		this.parentSystem.OnJoinedRoom();
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x00053D06 File Offset: 0x00051F06
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x00053D06 File Offset: 0x00051F06
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x00053D15 File Offset: 0x00051F15
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnCreateRoomFailed(returnCode, message);
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x00053D24 File Offset: 0x00051F24
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.parentSystem.OnPlayerEnteredRoom(newPlayer);
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x00053D32 File Offset: 0x00051F32
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.parentSystem.OnPlayerLeftRoom(otherPlayer);
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x00053D40 File Offset: 0x00051F40
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnect callback, cause:" + cause.ToString());
		this.parentSystem.OnDisconnected(cause);
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x00053D6A File Offset: 0x00051F6A
	public void OnEvent(EventData photonEvent)
	{
		this.parentSystem.RaiseEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x00053D89 File Offset: 0x00051F89
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		this.parentSystem.OnMasterClientSwitched(newMasterClient);
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x00053D97 File Offset: 0x00051F97
	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		base.OnCustomAuthenticationResponse(data);
		NetworkSystem.Instance.CustomAuthenticationResponse(data);
	}

	// Token: 0x0400139C RID: 5020
	private NetworkSystemPUN parentSystem;
}
