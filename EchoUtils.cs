using System;
using UnityEngine;

// Token: 0x0200098E RID: 2446
public static class EchoUtils
{
	// Token: 0x06003ABF RID: 15039 RVA: 0x00119244 File Offset: 0x00117444
	[HideInCallstack]
	public static T Echo<T>(this T message)
	{
		Debug.Log(message);
		return message;
	}

	// Token: 0x06003AC0 RID: 15040 RVA: 0x00119252 File Offset: 0x00117452
	[HideInCallstack]
	public static T Echo<T>(this T message, Object context)
	{
		Debug.Log(message, context);
		return message;
	}
}
