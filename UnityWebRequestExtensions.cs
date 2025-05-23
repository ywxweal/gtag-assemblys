using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000833 RID: 2099
public static class UnityWebRequestExtensions
{
	// Token: 0x0600335D RID: 13149 RVA: 0x000FD3F4 File Offset: 0x000FB5F4
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
