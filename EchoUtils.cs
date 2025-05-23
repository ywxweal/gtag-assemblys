using System;
using UnityEngine;

// Token: 0x0200098E RID: 2446
public static class EchoUtils
{
	// Token: 0x06003AC0 RID: 15040 RVA: 0x0011931C File Offset: 0x0011751C
	[HideInCallstack]
	public static T Echo<T>(this T message)
	{
		Debug.Log(message);
		return message;
	}

	// Token: 0x06003AC1 RID: 15041 RVA: 0x0011932A File Offset: 0x0011752A
	[HideInCallstack]
	public static T Echo<T>(this T message, Object context)
	{
		Debug.Log(message, context);
		return message;
	}
}
