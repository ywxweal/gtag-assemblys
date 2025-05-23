using System;
using UnityEngine;

// Token: 0x0200047F RID: 1151
public static class DeepLinkSender
{
	// Token: 0x06001C3C RID: 7228 RVA: 0x0008AAA4 File Offset: 0x00088CA4
	public static bool SendDeepLink(ulong deepLinkAppID, string deepLinkMessage, Action<string> onSent)
	{
		Debug.LogError("[DeepLinkSender::SendDeepLink] Called on non-oculus platform!");
		return false;
	}

	// Token: 0x04001F6A RID: 8042
	private static Action<string> currentDeepLinkSentCallback;
}
