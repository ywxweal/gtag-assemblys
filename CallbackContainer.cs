using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200097F RID: 2431
internal class CallbackContainer<T> where T : ICallBack
{
	// Token: 0x170005C9 RID: 1481
	// (get) Token: 0x06003A6B RID: 14955 RVA: 0x00117F1D File Offset: 0x0011611D
	public int Count
	{
		get
		{
			return this.callbackList.Count;
		}
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x00117F2A File Offset: 0x0011612A
	public CallbackContainer()
	{
		this.callbackList = new List<T>(100);
		this.currentIndex = -1;
		this.callbackCount = -1;
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x00117F4D File Offset: 0x0011614D
	public CallbackContainer(int capacity)
	{
		this.callbackList = new List<T>(capacity);
		this.currentIndex = -1;
		this.callbackCount = -1;
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x00117F6F File Offset: 0x0011616F
	public virtual void Add(T inCallback)
	{
		this.callbackCount++;
		this.callbackList.Add(inCallback);
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x00117F8C File Offset: 0x0011618C
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

	// Token: 0x06003A70 RID: 14960 RVA: 0x00117FDC File Offset: 0x001161DC
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

	// Token: 0x06003A71 RID: 14961 RVA: 0x00118060 File Offset: 0x00116260
	public virtual void Clear()
	{
		this.callbackList.Clear();
	}

	// Token: 0x04003F69 RID: 16233
	protected List<T> callbackList;

	// Token: 0x04003F6A RID: 16234
	protected int currentIndex;

	// Token: 0x04003F6B RID: 16235
	protected int callbackCount;
}
