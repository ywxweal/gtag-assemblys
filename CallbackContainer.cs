using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200097F RID: 2431
internal class CallbackContainer<T> where T : ICallBack
{
	// Token: 0x170005C9 RID: 1481
	// (get) Token: 0x06003A6C RID: 14956 RVA: 0x00117FF5 File Offset: 0x001161F5
	public int Count
	{
		get
		{
			return this.callbackList.Count;
		}
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x00118002 File Offset: 0x00116202
	public CallbackContainer()
	{
		this.callbackList = new List<T>(100);
		this.currentIndex = -1;
		this.callbackCount = -1;
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x00118025 File Offset: 0x00116225
	public CallbackContainer(int capacity)
	{
		this.callbackList = new List<T>(capacity);
		this.currentIndex = -1;
		this.callbackCount = -1;
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x00118047 File Offset: 0x00116247
	public virtual void Add(T inCallback)
	{
		this.callbackCount++;
		this.callbackList.Add(inCallback);
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x00118064 File Offset: 0x00116264
	public virtual void Remove(T inCallback)
	{
		int num = this.callbackList.IndexOf(inCallback);
		if (num > -1)
		{
			if (num <= this.currentIndex)
			{
				this.currentIndex--;
			}
			this.callbackCount--;
			this.callbackList.RemoveAt(num);
		}
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x001180B4 File Offset: 0x001162B4
	public virtual void TryRunCallbacks()
	{
		this.callbackCount = this.callbackList.Count;
		this.currentIndex = 0;
		while (this.currentIndex < this.callbackCount)
		{
			try
			{
				T t = this.callbackList[this.currentIndex];
				t.CallBack();
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
			this.currentIndex++;
		}
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x00118138 File Offset: 0x00116338
	public virtual void Clear()
	{
		this.callbackList.Clear();
	}

	// Token: 0x04003F6A RID: 16234
	protected List<T> callbackList;

	// Token: 0x04003F6B RID: 16235
	protected int currentIndex;

	// Token: 0x04003F6C RID: 16236
	protected int callbackCount;
}
