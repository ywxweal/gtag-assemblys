using System;
using UnityEngine;

// Token: 0x02000972 RID: 2418
public abstract class BasePageHandler : MonoBehaviour
{
	// Token: 0x170005C3 RID: 1475
	// (get) Token: 0x06003A43 RID: 14915 RVA: 0x00117284 File Offset: 0x00115484
	// (set) Token: 0x06003A44 RID: 14916 RVA: 0x0011728C File Offset: 0x0011548C
	private protected int selectedIndex { protected get; private set; }

	// Token: 0x170005C4 RID: 1476
	// (get) Token: 0x06003A45 RID: 14917 RVA: 0x00117295 File Offset: 0x00115495
	// (set) Token: 0x06003A46 RID: 14918 RVA: 0x0011729D File Offset: 0x0011549D
	private protected int currentPage { protected get; private set; }

	// Token: 0x170005C5 RID: 1477
	// (get) Token: 0x06003A47 RID: 14919 RVA: 0x001172A6 File Offset: 0x001154A6
	// (set) Token: 0x06003A48 RID: 14920 RVA: 0x001172AE File Offset: 0x001154AE
	private protected int pages { protected get; private set; }

	// Token: 0x170005C6 RID: 1478
	// (get) Token: 0x06003A49 RID: 14921 RVA: 0x001172B7 File Offset: 0x001154B7
	// (set) Token: 0x06003A4A RID: 14922 RVA: 0x001172BF File Offset: 0x001154BF
	private protected int maxEntires { protected get; private set; }

	// Token: 0x170005C7 RID: 1479
	// (get) Token: 0x06003A4B RID: 14923
	protected abstract int pageSize { get; }

	// Token: 0x170005C8 RID: 1480
	// (get) Token: 0x06003A4C RID: 14924
	protected abstract int entriesCount { get; }

	// Token: 0x06003A4D RID: 14925 RVA: 0x001172C8 File Offset: 0x001154C8
	protected virtual void Start()
	{
		Debug.Log("base page handler " + this.entriesCount.ToString() + " " + this.pageSize.ToString());
		this.pages = this.entriesCount / this.pageSize + 1;
		this.maxEntires = this.pages * this.pageSize;
	}

	// Token: 0x06003A4E RID: 14926 RVA: 0x00117330 File Offset: 0x00115530
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

	// Token: 0x06003A4F RID: 14927 RVA: 0x0011736C File Offset: 0x0011556C
	public void SelectEntryFromIndex(int index)
	{
		this.selectedIndex = index;
		this.currentPage = this.selectedIndex / this.pageSize;
		int num = index - this.pageSize * this.currentPage;
		this.PageEntrySelected(num, index);
		this.SetPage(this.currentPage);
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x001173B8 File Offset: 0x001155B8
	public void ChangePage(bool left)
	{
		int num = (left ? (-1) : 1);
		this.SetPage(Mathf.Abs((this.currentPage + num) % this.pages));
	}

	// Token: 0x06003A51 RID: 14929 RVA: 0x001173E8 File Offset: 0x001155E8
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

	// Token: 0x06003A52 RID: 14930
	protected abstract void ShowPage(int selectedPage, int startIndex, int endIndex);

	// Token: 0x06003A53 RID: 14931
	protected abstract void PageEntrySelected(int pageEntry, int selectionIndex);
}
