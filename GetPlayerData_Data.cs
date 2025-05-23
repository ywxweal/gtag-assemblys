using System;
using KID.Model;
using UnityEngine;

// Token: 0x02000791 RID: 1937
public class GetPlayerData_Data
{
	// Token: 0x06003075 RID: 12405 RVA: 0x000EF408 File Offset: 0x000ED608
	public GetPlayerData_Data(GetSessionResponseType type, GetPlayerDataResponse response)
	{
		this.responseType = type;
		if (response == null)
		{
			if (this.responseType == GetSessionResponseType.OK)
			{
				this.responseType = GetSessionResponseType.ERROR;
				Debug.LogError("[KID::GET_PLAYER_DATA_DATA] Incoming [GetPlayerDataResponse] is NULL");
			}
			return;
		}
		this.AgeStatus = response.AgeStatus;
		this.status = response.Status;
		if (this.status != null)
		{
			this.session = new TMPSession(response.Session, response.DefaultSession, this.status.Value);
		}
	}

	// Token: 0x040036B3 RID: 14003
	public readonly AgeStatusType? AgeStatus;

	// Token: 0x040036B4 RID: 14004
	public readonly GetSessionResponseType responseType;

	// Token: 0x040036B5 RID: 14005
	public readonly SessionStatus? status;

	// Token: 0x040036B6 RID: 14006
	public readonly TMPSession session;
}
