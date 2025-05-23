﻿using System;
using System.Text;
using Steamworks;
using UnityEngine;

// Token: 0x020008E4 RID: 2276
public class MothershipSteamAuthenticator : MonoBehaviour
{
	// Token: 0x06003756 RID: 14166 RVA: 0x0010B9B9 File Offset: 0x00109BB9
	public void StartSteamLogIn()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			string nonce = resp.Nonce;
			this.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce, ticket, delegate(LoginResponse successResp)
				{
					Debug.Log("Logged in to Mothership with Steam");
				}, delegate(MothershipError finalError, int finalErrorCode)
				{
					Debug.LogFormat("Could not log into Mothership with Steam {0} {1}", new object[] { finalError, finalErrorCode });
				});
			}, delegate(EResult error)
			{
			});
		}, delegate(MothershipError error, int errorcode)
		{
			Debug.LogFormat("Couldn't start mothership auth for steam {0} {1}", new object[] { error, errorcode });
		});
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x0010B9EC File Offset: 0x00109BEC
	public HAuthTicket GetAuthTicket(Action<string> successCallback, Action<EResult> failureCallback)
	{
		HAuthTicket ticketHandle = HAuthTicket.Invalid;
		Callback<GetAuthSessionTicketResponse_t> ticketCallback = null;
		byte[] ticketBlob = new byte[1024];
		uint ticketSize = 0U;
		ticketCallback = Callback<GetAuthSessionTicketResponse_t>.Create(delegate(GetAuthSessionTicketResponse_t response)
		{
			if (response.m_hAuthTicket != ticketHandle)
			{
				return;
			}
			ticketCallback.Dispose();
			ticketCallback = null;
			if (response.m_eResult != EResult.k_EResultOK)
			{
				Action<EResult> failureCallback3 = failureCallback;
				if (failureCallback3 == null)
				{
					return;
				}
				failureCallback3(response.m_eResult);
				return;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (uint num = 0U; num < ticketSize; num += 1U)
				{
					stringBuilder.AppendFormat("{0:x2}", ticketBlob[(int)num]);
				}
				Action<string> successCallback2 = successCallback;
				if (successCallback2 == null)
				{
					return;
				}
				successCallback2(stringBuilder.ToString());
				return;
			}
		});
		SteamNetworkingIdentity steamNetworkingIdentity = default(SteamNetworkingIdentity);
		ticketHandle = SteamUser.GetAuthSessionTicket(ticketBlob, ticketBlob.Length, out ticketSize, ref steamNetworkingIdentity);
		if (ticketHandle == HAuthTicket.Invalid)
		{
			Action<EResult> failureCallback2 = failureCallback;
			if (failureCallback2 != null)
			{
				failureCallback2(EResult.k_EResultFail);
			}
		}
		return ticketHandle;
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x0010BAA0 File Offset: 0x00109CA0
	public HAuthTicket GetAuthTicketForWebApi(string authenticatorId, Action<string> successCallback, Action<EResult> failureCallback)
	{
		HAuthTicket ticketHandle = HAuthTicket.Invalid;
		Callback<GetTicketForWebApiResponse_t> ticketCallback = null;
		ticketCallback = Callback<GetTicketForWebApiResponse_t>.Create(delegate(GetTicketForWebApiResponse_t response)
		{
			if (response.m_hAuthTicket != ticketHandle)
			{
				return;
			}
			ticketCallback.Dispose();
			ticketCallback = null;
			if (response.m_eResult != EResult.k_EResultOK)
			{
				Action<EResult> failureCallback3 = failureCallback;
				if (failureCallback3 == null)
				{
					return;
				}
				failureCallback3(response.m_eResult);
				return;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < response.m_cubTicket; i++)
				{
					stringBuilder.AppendFormat("{0:x2}", response.m_rgubTicket[i]);
				}
				Action<string> successCallback2 = successCallback;
				if (successCallback2 == null)
				{
					return;
				}
				successCallback2(stringBuilder.ToString());
				return;
			}
		});
		ticketHandle = SteamUser.GetAuthTicketForWebApi(authenticatorId);
		if (ticketHandle == HAuthTicket.Invalid)
		{
			Action<EResult> failureCallback2 = failureCallback;
			if (failureCallback2 != null)
			{
				failureCallback2(EResult.k_EResultFail);
			}
		}
		return ticketHandle;
	}
}
