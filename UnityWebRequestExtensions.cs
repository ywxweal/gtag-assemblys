﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000833 RID: 2099
public static class UnityWebRequestExtensions
{
	// Token: 0x0600335E RID: 13150 RVA: 0x000FD4CC File Offset: 0x000FB6CC
	public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		TaskCompletionSource<UnityWebRequest> tcs = new TaskCompletionSource<UnityWebRequest>();
		asyncOp.completed += delegate(AsyncOperation operation)
		{
			tcs.TrySetResult(asyncOp.webRequest);
		};
		return tcs.Task.GetAwaiter();
	}
}
