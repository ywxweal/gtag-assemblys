using System;
using UnityEngine;

// Token: 0x02000972 RID: 2418
public abstract class BasePageHandler : MonoBehaviour
{
	// Token: 0x170005C3 RID: 1475
	// (get) Token: 0x06003A42 RID: 14914 RVA: 0x001171AC File Offset: 0x001153AC
	// (set) Token: 0x06003A43 RID: 14915 RVA: 0x001171B4 File Offset: 0x001153B4
	private protected int selectedIndex { protected get; private set; }

	// Token: 0x170005C4 RID: 1476
	// (get) Token: 0x06003A44 RID: 14916 RVA: 0x001171BD File Offset: 0x001153BD
	// (set) Token: 0x06003A45 RID: 14917 RVA: 0x001171C5 File Offset: 0x001153C5
	private protected int currentPage { protected get; private set; }

	// Token: 0x170005C5 RID: 1477
	// (get) Token: 0x06003A46 RID: 14918 RVA: 0x001171CE File Offset: 0x001153CE
	// (set) Token: 0x06003A47 RID: 14919 RVA: 0x001171D6 File Offset: 0x001153D6
	private protected int pages { protected get; private set; }

	// Token: 0x170005C6 RID: 1478
	// (get) Token: 0x06003A48 RID: 14920 RVA: 0x001171DF File Offset: 0x001153DF
	// (set) Token: 0x06003A49 RID: 14921 RVA: 0x001171E7 File Offset: 0x001153E7
	private protected int maxEntires { protected get; private set; }

	// Token: 0x170005C7 RID: 1479
	// (get) Token: 0x06003A4A RID: 14922
	protected abstract int pageSize { get; }

	// Token: 0x170005C8 RID: 1480
	// (get) Token: 0x06003A4B RID: 14923
	protected abstract int entriesCount { get; }

	// Token: 0x06003A4C RID: 14924 RVA: 0x001171F0 File Offset: 0x001153F0
	protected virtual void Start()
	{
		Debug.Log("base page handler " + this.entriesCount.ToString() + " " + this.pageSize.ToString());
		this.pages = this.entriesCount / this.pageSize + 1;
		this.maxEntires = this.pages * this.pageSize;
	}

	// Token: 0x06003A4D RID: 14925 RVA: 0x00117258 File Offset: 0x00115458
	public void SelectEntryOnPage(int entryIndex)
	{
		int num = entryIndex + this.pageSize * this.currentPage;
		if (num > this.entriesCount)
		{
			return;
		}
		this.selectedIndex = num;
		this.PageEntrySelected(entryIndex, this.selectedIndex);
	}

	// Token: 0x06003A4E RID: 14926 RVA: 0x00117294 File Offset: 0x00115494
	public void SelectEntryFromIndex(int index)
	{
		this.selectedIndex = index;
		this.currentPage = this.selectedIndex / this.pageSize;
		int num = index - this.pageSize * this.currentPage;
		this.PageEntrySelected(num, index);
		this.SetPage(this.currentPage);
	}

	// Token: 0x06003A4F RID: 14927 RVA: 0x001172E0 File Offset: 0x001154E0
	public void ChangePage(bool left)
	{
		int num = (left ? (-1) : 1);
		this.SetPage(Mathf.Abs((this.currentPage + num) % this.pages));
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x00117310 File Offset: 0x00115510
	public void SetPage(int page)
	{
		if (page > this.pages)
		{
			return;
		}
		this.currentPage = page;
		int num = this.pageSize * page;
		this.ShowPage(this.currentPage, num, Mathf.Min(num + this.pageSize, this.entriesCount));
	}

	// Token: 0x06003A51 RID: 14929
	protected abstract void ShowPage(int selectedPage, int startIndex, int endIndex);

	// Token: 0x06003A52 RID: 14930
	protected abstract void PageEntrySelected(int pageEntry, int selectionIndex);
}
