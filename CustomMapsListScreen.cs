using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GorillaNetworking;
using ModIO;
using PlayFab;
using TMPro;
using UnityEngine;

// Token: 0x0200072F RID: 1839
public class CustomMapsListScreen : CustomMapsTerminalScreen
{
	// Token: 0x17000480 RID: 1152
	// (get) Token: 0x06002DD1 RID: 11729 RVA: 0x000E3D11 File Offset: 0x000E1F11
	public int CurrentModPage
	{
		get
		{
			return this.currentModPage;
		}
	}

	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x06002DD2 RID: 11730 RVA: 0x000E3D19 File Offset: 0x000E1F19
	public int SelectedModIndex
	{
		get
		{
			return this.selectedModIndex;
		}
	}

	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x06002DD3 RID: 11731 RVA: 0x000E3D21 File Offset: 0x000E1F21
	public int ModsPerPage
	{
		get
		{
			return this.modsPerPage;
		}
	}

	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x06002DD4 RID: 11732 RVA: 0x000E3D29 File Offset: 0x000E1F29
	// (set) Token: 0x06002DD5 RID: 11733 RVA: 0x000E3D34 File Offset: 0x000E1F34
	public SortModsBy SortType
	{
		get
		{
			return this.sortType;
		}
		set
		{
			if (this.sortType != value)
			{
				this.currentAvailableModsRequestPage = 0;
			}
			this.sortType = value;
			switch (this.sortType)
			{
			case SortModsBy.Name:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Price:
				break;
			case SortModsBy.Rating:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Popular:
				this.isAscendingOrder = false;
				return;
			case SortModsBy.Downloads:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.Subscribers:
				this.isAscendingOrder = true;
				return;
			case SortModsBy.DateSubmitted:
				this.isAscendingOrder = true;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void Initialize()
	{
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x000E3DB4 File Offset: 0x000E1FB4
	public override void Show()
	{
		base.Show();
		if (this.featuredMods.IsNullOrEmpty<ModProfile>())
		{
			this.RetrieveFeaturedMods();
		}
		if (this.availableMods.IsNullOrEmpty<ModProfile>())
		{
			this.RetrieveAvailableMods();
		}
		if (this.subscribedMods.IsNullOrEmpty<SubscribedMod>())
		{
			this.RetrieveSubscribedMods();
		}
		this.RefreshScreenState();
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x000E3E08 File Offset: 0x000E2008
	protected override void PressButton(CustomMapsTerminalButton.ModIOKeyboardBindings buttonPressed)
	{
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.goback)
		{
			return;
		}
		if (this.loadingText.gameObject.activeSelf)
		{
			return;
		}
		if (this.CheckTags(buttonPressed))
		{
			this.Refresh(null);
			return;
		}
		if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.option3)
		{
			ModIOManager.Refresh(delegate(bool result)
			{
				if (result)
				{
					this.Refresh(null);
				}
			}, false);
			return;
		}
		if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.up)
		{
			this.selectedModIndex--;
			if ((this.IsOnFirstPage() && this.selectedModIndex < 0) || (!this.IsOnFirstPage() && this.selectedModIndex < -1))
			{
				this.selectedModIndex = (this.IsOnLastPage() ? (this.displayedModProfiles.Count - 1) : this.displayedModProfiles.Count);
			}
			this.UpdateModListSelection();
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.down)
		{
			this.selectedModIndex++;
			if ((this.IsOnLastPage() && this.selectedModIndex >= this.displayedModProfiles.Count) || (!this.IsOnLastPage() && this.selectedModIndex > this.displayedModProfiles.Count))
			{
				this.selectedModIndex = (this.IsOnFirstPage() ? 0 : (-1));
			}
			this.UpdateModListSelection();
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.enter)
		{
			if (!this.IsOnFirstPage() && this.selectedModIndex == -1)
			{
				this.currentModPage--;
				this.selectedModIndex = 0;
				CustomMapsTerminal.SendTerminalStatus(false, false);
				this.RefreshScreenState();
				return;
			}
			if (!this.IsOnLastPage() && this.selectedModIndex == this.displayedModProfiles.Count)
			{
				this.currentModPage++;
				this.selectedModIndex = 0;
				CustomMapsTerminal.SendTerminalStatus(false, false);
				this.RefreshScreenState();
				return;
			}
			if (this.selectedModIndex >= 0 && this.selectedModIndex < this.displayedModProfiles.Count)
			{
				CustomMapsTerminal.ShowDetailsScreen(this.displayedModProfiles[this.selectedModIndex]);
				return;
			}
		}
		else
		{
			if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.sub)
			{
				this.SwapListDisplay();
				return;
			}
			if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.sort)
			{
				this.SetSortType();
				this.Refresh(null);
			}
		}
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x000E3FFE File Offset: 0x000E21FE
	public void ClearTags(bool clearModLists = false)
	{
		this.searchTags.Clear();
		if (clearModLists)
		{
			this.featuredMods.Clear();
			this.availableMods.Clear();
		}
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x000E4024 File Offset: 0x000E2224
	public void UpdateTagsFromDriver(List<string> tags)
	{
		this.currentAvailableModsRequestPage = 0;
		this.searchTags.Clear();
		if (tags.IsNullOrEmpty<string>())
		{
			return;
		}
		this.searchTags.AddRange(tags);
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x000E4050 File Offset: 0x000E2250
	private bool CheckTags(CustomMapsTerminalButton.ModIOKeyboardBindings buttonPressed)
	{
		bool flag = false;
		short num = (short)buttonPressed;
		if (num > 0 && num < 10)
		{
			flag = true;
			string text;
			if (CustomMapsTerminal.SetTagButtonStatus(num, out text))
			{
				if (!this.searchTags.Contains(text))
				{
					this.searchTags.Add(text);
				}
			}
			else if (this.searchTags.Contains(text))
			{
				this.searchTags.Remove(text);
			}
		}
		return flag;
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x000E40B0 File Offset: 0x000E22B0
	private void SetSortType()
	{
		this.currentAvailableModsRequestPage = 0;
		this.sortTypeIndex++;
		if (this.sortTypeIndex >= 6)
		{
			this.sortTypeIndex = 0;
		}
		switch (this.sortTypeIndex)
		{
		case 0:
			this.sortType = SortModsBy.Popular;
			this.isAscendingOrder = false;
			return;
		case 1:
			this.sortType = SortModsBy.Name;
			this.isAscendingOrder = true;
			return;
		case 2:
			this.sortType = SortModsBy.Rating;
			this.isAscendingOrder = true;
			return;
		case 3:
			this.sortType = SortModsBy.Downloads;
			this.isAscendingOrder = true;
			return;
		case 4:
			this.sortType = SortModsBy.Subscribers;
			this.isAscendingOrder = true;
			return;
		case 5:
			this.sortType = SortModsBy.DateSubmitted;
			this.isAscendingOrder = true;
			return;
		default:
			this.sortTypeIndex = 0;
			this.sortType = SortModsBy.Popular;
			this.isAscendingOrder = false;
			return;
		}
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x000E4178 File Offset: 0x000E2378
	private void SwapListDisplay()
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			this.currentState = CustomMapsListScreen.ListScreenState.SubscribedMods;
		}
		else if (this.currentState == CustomMapsListScreen.ListScreenState.SubscribedMods)
		{
			this.currentState = CustomMapsListScreen.ListScreenState.AvailableMods;
		}
		this.selectedModIndex = 0;
		this.currentModPage = 0;
		CustomMapsTerminal.UpdateListScreenState(this.currentState);
		CustomMapsTerminal.SendTerminalStatus(this.currentState == CustomMapsListScreen.ListScreenState.SubscribedMods, false);
		this.RefreshScreenState();
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x000E41D4 File Offset: 0x000E23D4
	public void Refresh(long[] customModListIds = null)
	{
		if (this.loadingAvailableMods || this.loadingFeaturedMods || this.loadingCustomModList)
		{
			return;
		}
		this.currentModPage = 0;
		this.selectedModIndex = 0;
		CustomMapsTerminal.SendTerminalStatus(false, true);
		this.featuredMods.Clear();
		this.availableMods.Clear();
		this.filteredAvailableMods.Clear();
		this.currentAvailableModsRequestPage = 0;
		this.errorLoadingAvailableMods = false;
		this.totalAvailableMods = 0;
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
		if (customModListIds != null && customModListIds.Length != 0)
		{
			this.customModListModIds.Clear();
			this.customModListModIds.AddRange(customModListIds);
		}
		this.customModList.Clear();
		this.RetrieveFeaturedMods();
		this.RetrieveAvailableMods();
		this.RetrieveSubscribedMods();
		this.RetrieveCustomModList();
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x000E42A0 File Offset: 0x000E24A0
	private void RetrieveFeaturedMods()
	{
		if (this.loadingFeaturedMods || this.featuredMods.Count > 0)
		{
			return;
		}
		this.loadingFeaturedMods = true;
		PlayFabTitleDataCache.Instance.GetTitleData(this.featuredModsPlayFabKey, new Action<string>(this.OnGetFeaturedModsTitleData), delegate(PlayFabError error)
		{
			this.loadingFeaturedMods = false;
			this.RefreshScreenState();
		});
	}

	// Token: 0x06002DE0 RID: 11744 RVA: 0x000E42F4 File Offset: 0x000E24F4
	private async void OnGetFeaturedModsTitleData(string data)
	{
		if (data.IsNullOrEmpty())
		{
			this.RefreshScreenState();
		}
		else
		{
			this.featuredModIds.Clear();
			this.featuredMods.Clear();
			if (data[0] == '"' && data[data.Length - 1] == '"')
			{
				data = data.Substring(1, data.Length - 2);
			}
			string[] array = data.Split(',', StringSplitOptions.None);
			foreach (string text in array)
			{
				if (!text.IsNullOrEmpty())
				{
					long featuredModId;
					try
					{
						featuredModId = long.Parse(text);
					}
					catch (Exception)
					{
						goto IL_0178;
					}
					ResultAnd<ModProfile> resultAnd = await ModIOUnityAsync.GetMod(new ModId(featuredModId));
					if (resultAnd.result.Succeeded())
					{
						this.featuredModIds.Add(featuredModId);
						this.featuredMods.Add(resultAnd.value);
					}
				}
				IL_0178:;
			}
			string[] array2 = null;
			this.totalFeaturedMods = this.featuredMods.Count;
			this.FilterAvailableMods();
			this.loadingFeaturedMods = false;
			this.RefreshScreenState();
		}
	}

	// Token: 0x06002DE1 RID: 11745 RVA: 0x000E4334 File Offset: 0x000E2534
	private void RetrieveAvailableMods()
	{
		if (this.loadingAvailableMods)
		{
			return;
		}
		if (this.retrieveRetryCount > 10)
		{
			this.retrieveRetryCount = 0;
			return;
		}
		this.retrieveRetryCount++;
		this.loadingAvailableMods = true;
		int num = this.currentAvailableModsRequestPage;
		this.currentAvailableModsRequestPage = num + 1;
		SearchFilter searchFilter = new SearchFilter(num, this.numModsPerRequest);
		searchFilter.SetSortBy(this.sortType);
		if (!this.searchTags.IsNullOrEmpty<string>())
		{
			searchFilter.AddTags(this.searchTags);
		}
		searchFilter.SetToAscending(this.isAscendingOrder);
		ModIOUnity.GetMods(searchFilter, new Action<ResultAnd<ModPage>>(this.OnAvailableModsRetrieved));
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000E43D4 File Offset: 0x000E25D4
	private void OnAvailableModsRetrieved(ResultAnd<ModPage> result)
	{
		if (result.result.Succeeded())
		{
			this.totalAvailableMods = (int)result.value.totalSearchResultsFound;
			if (this.totalAvailableMods == 0 && this.retrieveRetryCount < 10)
			{
				this.loadingAvailableMods = false;
				this.currentAvailableModsRequestPage = Mathf.Max(0, this.currentAvailableModsRequestPage - 1);
				this.RetrieveAvailableMods();
				return;
			}
			this.availableMods.AddRange(result.value.modProfiles);
			this.FilterAvailableMods();
		}
		else
		{
			this.errorLoadingAvailableMods = true;
		}
		this.retrieveRetryCount = 0;
		this.loadingAvailableMods = false;
		this.RefreshScreenState();
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000E4470 File Offset: 0x000E2670
	private void FilterAvailableMods()
	{
		if (this.availableMods.IsNullOrEmpty<ModProfile>())
		{
			return;
		}
		this.filteredAvailableMods.Clear();
		if (this.searchTags.IsNullOrEmpty<string>())
		{
			this.totalAvailableMods = Mathf.Max(0, this.totalAvailableMods - 1);
		}
		foreach (ModProfile modProfile in this.availableMods)
		{
			if (!(modProfile.id == ModIOManager.GetNewMapsModId()) && (this.featuredModIds.IsNullOrEmpty<long>() || !this.featuredModIds.Contains(modProfile.id.id)))
			{
				this.filteredAvailableMods.Add(modProfile);
			}
		}
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000E453C File Offset: 0x000E273C
	private void RetrieveSubscribedMods()
	{
		this.subscribedMods = ModIOManager.GetSubscribedMods();
		this.FilterSubscribedMods();
		this.totalSubscribedMods = this.filteredSubscribedMods.Count;
		this.RefreshScreenState();
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000E4568 File Offset: 0x000E2768
	private void FilterSubscribedMods()
	{
		if (this.subscribedMods.IsNullOrEmpty<SubscribedMod>())
		{
			return;
		}
		this.filteredSubscribedMods.Clear();
		foreach (SubscribedMod subscribedMod in this.subscribedMods)
		{
			if (!(subscribedMod.modProfile.id == ModIOManager.GetNewMapsModId()))
			{
				this.filteredSubscribedMods.Add(subscribedMod);
			}
		}
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000E45D0 File Offset: 0x000E27D0
	public void ShowCustomModList(long[] modIds)
	{
		if (modIds == null || modIds.Length == 0)
		{
			return;
		}
		if (this.customModListModIds.Count > 0 && this.customModListModIds.Count == modIds.Length)
		{
			bool flag = true;
			foreach (long num in this.customModListModIds)
			{
				if (!modIds.Contains(num))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.currentState = CustomMapsListScreen.ListScreenState.CustomModList;
				this.currentModPage = 0;
				this.selectedModIndex = 0;
				this.RefreshScreenState();
				return;
			}
		}
		this.customModListModIds.Clear();
		this.customModList.Clear();
		this.customModListModIds.AddRange(modIds);
		if (this.loadingCustomModList)
		{
			this.restartCustomModListRetrieval = true;
		}
		this.currentState = CustomMapsListScreen.ListScreenState.CustomModList;
		this.loadingCustomModList = true;
		this.RefreshScreenState();
		this.RetrieveCustomModList();
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x000E46BC File Offset: 0x000E28BC
	private void RetrieveCustomModList()
	{
		if (this.restartCustomModListRetrieval || this.customModListModIds.Count == 0)
		{
			return;
		}
		ModIOManager.GetModProfile(new ModId(this.customModListModIds[this.customModList.Count]), new Action<ModIORequestResultAnd<ModProfile>>(this.OnModProfileReceived));
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x000E470C File Offset: 0x000E290C
	private void OnModProfileReceived(ModIORequestResultAnd<ModProfile> requestResult)
	{
		if (this.restartCustomModListRetrieval)
		{
			this.restartCustomModListRetrieval = false;
			this.RetrieveCustomModList();
			return;
		}
		if (!requestResult.result.success)
		{
			this.loadingCustomModList = false;
			this.errorLoadingCustomModList = true;
			this.RefreshScreenState();
			return;
		}
		this.customModList.Add(requestResult.data);
		if (this.customModList.Count < this.customModListModIds.Count)
		{
			ModIOManager.GetModProfile(new ModId(this.customModListModIds[this.customModList.Count]), new Action<ModIORequestResultAnd<ModProfile>>(this.OnModProfileReceived));
			return;
		}
		this.loadingCustomModList = false;
		this.RefreshScreenState();
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x000E47B4 File Offset: 0x000E29B4
	public bool DoesModListMatchDisplay(long[] modIds)
	{
		if (this.displayedModProfiles.IsNullOrEmpty<ModProfile>() || this.displayedModProfiles.Count != modIds.Length)
		{
			return false;
		}
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			if (modIds[i] != this.displayedModProfiles[i].id.id)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x000E4814 File Offset: 0x000E2A14
	public ModProfile GetProfile()
	{
		if (this.displayedModProfiles == null)
		{
			return default(ModProfile);
		}
		if (this.selectedModIndex < 0 || this.selectedModIndex >= this.displayedModProfiles.Count)
		{
			return default(ModProfile);
		}
		return this.displayedModProfiles[this.selectedModIndex];
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x000E486C File Offset: 0x000E2A6C
	public void GetModList(out long[] modList)
	{
		if (this.displayedModProfiles == null)
		{
			modList = Array.Empty<long>();
			return;
		}
		modList = new long[this.displayedModProfiles.Count];
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			modList[i] = this.displayedModProfiles[i].id.id;
		}
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000E48CC File Offset: 0x000E2ACC
	private void RefreshScreenState()
	{
		this.displayedModProfiles.Clear();
		this.modListText.text = "";
		this.modListText.gameObject.SetActive(false);
		this.modPageLabel.gameObject.SetActive(false);
		this.modPageText.gameObject.SetActive(false);
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.createdByText.gameObject.SetActive(false);
		this.tagText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		for (int i = 0; i < this.buttonsToShow.Length; i++)
		{
			this.buttonsToShow[i].SetActive(true);
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
		{
			this.titleText.text = this.browseModsTitle + string.Format(" (SORTED BY {0})", this.sortType);
			for (int j = 0; j < this.buttonsToHideForAvailableMaps.Length; j++)
			{
				this.buttonsToHideForAvailableMaps[j].SetActive(false);
			}
			if (this.loadingFeaturedMods || this.loadingAvailableMods)
			{
				return;
			}
			if (this.errorLoadingAvailableMods)
			{
				this.modListText.text = this.failedToRetrieveModsString;
				this.loadingText.gameObject.SetActive(false);
				this.modListText.gameObject.SetActive(true);
				return;
			}
			this.UpdatePageCount(this.totalAvailableMods);
			int num = 0;
			int num2 = this.modsPerPage - 1;
			if (this.IsOnFirstPage())
			{
				this.displayedModProfiles.AddRange(this.featuredMods);
				num2 -= this.totalFeaturedMods;
			}
			else
			{
				num = this.currentModPage * this.modsPerPage - this.totalFeaturedMods;
				num2 = num + this.modsPerPage - 1;
			}
			if (this.filteredAvailableMods.Count <= num2 && this.totalAvailableMods > this.availableMods.Count)
			{
				this.displayedModProfiles.Clear();
				this.RetrieveAvailableMods();
				return;
			}
			int num3 = num;
			while (num3 <= num2 && this.filteredAvailableMods.Count > num3)
			{
				this.displayedModProfiles.Add(this.filteredAvailableMods[num3]);
				num3++;
			}
			this.UpdateModListSelection();
			this.loadingText.gameObject.SetActive(false);
			this.modListText.gameObject.SetActive(true);
			return;
		}
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
		{
			this.titleText.text = this.subscribedOnlyTitle;
			for (int k = 0; k < this.buttonsToHideForSubscribedMaps.Length; k++)
			{
				this.buttonsToHideForSubscribedMaps[k].SetActive(false);
			}
			this.UpdatePageCount(this.totalSubscribedMods);
			int num4 = this.currentModPage * this.modsPerPage;
			int num5 = num4;
			while (num5 < num4 + this.modsPerPage && this.filteredSubscribedMods.Count > num5)
			{
				this.displayedModProfiles.Add(this.filteredSubscribedMods[num5].modProfile);
				num5++;
			}
			this.UpdateModListSelection();
			this.loadingText.gameObject.SetActive(false);
			this.modListText.gameObject.SetActive(true);
			CustomMapsTerminal.SendTerminalStatus(true, false);
			return;
		}
		case CustomMapsListScreen.ListScreenState.CustomModList:
		{
			this.titleText.text = CustomMapsTerminal.GetDriverNickname() + "'s " + this.subscribedOnlyTitle;
			for (int l = 0; l < this.buttonsToHideForSubscribedMaps.Length; l++)
			{
				this.buttonsToHideForSubscribedMaps[l].SetActive(false);
			}
			if (this.loadingCustomModList)
			{
				return;
			}
			if (this.errorLoadingCustomModList)
			{
				this.modPageLabel.gameObject.SetActive(false);
				this.modPageText.gameObject.SetActive(false);
				if (CustomMapsTerminal.IsDriver)
				{
					this.currentModPage = -1;
				}
				this.modListText.text = this.failedToRetrieveModsString;
				this.loadingText.gameObject.SetActive(false);
				this.modListText.gameObject.SetActive(true);
				return;
			}
			this.UpdatePageCount(this.customModList.Count);
			this.displayedModProfiles.AddRange(this.customModList);
			this.UpdateModListSelection();
			this.loadingText.gameObject.SetActive(false);
			this.modListText.gameObject.SetActive(true);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x000E4D00 File Offset: 0x000E2F00
	public void UpdateModListSelection()
	{
		if (this.displayedModProfiles.IsNullOrEmpty<ModProfile>())
		{
			return;
		}
		if (this.selectedModIndex < -1 || this.selectedModIndex > this.displayedModProfiles.Count)
		{
			this.mapScreenshotImage.gameObject.SetActive(false);
			this.createdByText.gameObject.SetActive(false);
			this.tagText.gameObject.SetActive(false);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (this.selectedModIndex == -1 || this.selectedModIndex == this.displayedModProfiles.Count)
		{
			this.mapScreenshotImage.gameObject.SetActive(false);
			this.createdByText.gameObject.SetActive(false);
			this.tagText.gameObject.SetActive(false);
		}
		else
		{
			this.DownloadThumbnail(this.displayedModProfiles[this.selectedModIndex].logoImage320x180);
			this.createdByText.gameObject.SetActive(true);
			this.tagText.gameObject.SetActive(true);
			this.createdByText.text = this.creatorString + this.displayedModProfiles[this.selectedModIndex].creator.username;
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < this.displayedModProfiles[this.selectedModIndex].tags.Length; i++)
			{
				stringBuilder2.Append((i == 0) ? (this.displayedModProfiles[this.selectedModIndex].tags[i] ?? "") : (", " + this.displayedModProfiles[this.selectedModIndex].tags[i]));
			}
			this.tagText.text = this.tagString + stringBuilder2.ToString();
		}
		if (!this.IsOnFirstPage())
		{
			stringBuilder.Append((this.selectedModIndex == -1) ? ("> " + this.prevPageString + "\n") : ("  " + this.prevPageString + "\n"));
		}
		long num = (long)(this.currentModPage * this.modsPerPage + 1);
		bool flag = false;
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			if (this.IsOnFirstPage())
			{
				for (int j = 0; j < this.displayedModProfiles.Count; j++)
				{
					if (j < this.totalFeaturedMods)
					{
						stringBuilder.Append((j == this.selectedModIndex) ? this.TruncateModName("> FEATURED: " + this.displayedModProfiles[j].name) : this.TruncateModName("  FEATURED: " + this.displayedModProfiles[j].name));
					}
					else
					{
						StringBuilder stringBuilder3 = stringBuilder;
						string text2;
						if (j != this.selectedModIndex)
						{
							string text = "  {0}. {1}";
							long num2 = num;
							num = num2 + 1L;
							text2 = this.TruncateModName(string.Format(text, num2, this.displayedModProfiles[j].name));
						}
						else
						{
							string text3 = "> {0}. {1}";
							long num3 = num;
							num = num3 + 1L;
							text2 = this.TruncateModName(string.Format(text3, num3, this.displayedModProfiles[j].name));
						}
						stringBuilder3.Append(text2);
					}
				}
				flag = true;
			}
			else
			{
				num -= (long)this.totalFeaturedMods;
			}
		}
		if (!flag)
		{
			for (int k = 0; k < this.displayedModProfiles.Count; k++)
			{
				stringBuilder.Append((k == this.selectedModIndex) ? this.TruncateModName(string.Format("> {0}. {1}", num + (long)k, this.displayedModProfiles[k].name)) : this.TruncateModName(string.Format("  {0}. {1}", num + (long)k, this.displayedModProfiles[k].name)));
			}
		}
		if (!this.IsOnLastPage())
		{
			stringBuilder.Append((this.selectedModIndex == this.displayedModProfiles.Count) ? ("> " + this.nextPageString + "\n") : ("  " + this.nextPageString + "\n"));
		}
		this.modListText.text = stringBuilder.ToString();
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x000E5124 File Offset: 0x000E3324
	private string TruncateModName(string modname)
	{
		if (modname.Length <= this.maxModListItemLength)
		{
			return modname + "\n";
		}
		return modname.Substring(0, this.maxModListItemLength) + "\n";
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x000E5158 File Offset: 0x000E3358
	private void DownloadThumbnail(DownloadReference thumbnail)
	{
		this.mapScreenshotImage.gameObject.SetActive(false);
		if (this.isDownloadingThumbnail)
		{
			this.newDownloadRequest = true;
			this.currentThumbnail = thumbnail;
			return;
		}
		this.isDownloadingThumbnail = true;
		ModIOUnity.DownloadTexture(thumbnail, new Action<ResultAnd<Texture2D>>(this.OnTextureDownloaded));
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x000E51A8 File Offset: 0x000E33A8
	private void OnTextureDownloaded(ResultAnd<Texture2D> resultAnd)
	{
		this.isDownloadingThumbnail = false;
		if (this.newDownloadRequest)
		{
			this.newDownloadRequest = false;
			this.mapScreenshotImage.gameObject.SetActive(false);
			ModIOUnity.DownloadTexture(this.currentThumbnail, new Action<ResultAnd<Texture2D>>(this.OnTextureDownloaded));
			return;
		}
		if (!resultAnd.result.Succeeded())
		{
			return;
		}
		Texture2D value = resultAnd.value;
		this.mapScreenshotImage.sprite = Sprite.Create(value, new Rect(0f, 0f, (float)value.width, (float)value.height), new Vector2(0.5f, 0.5f));
		this.mapScreenshotImage.gameObject.SetActive(true);
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x000E5258 File Offset: 0x000E3458
	private void UpdatePageCount(int totalMods)
	{
		this.totalModCount = totalMods;
		this.modPageText.gameObject.SetActive(false);
		this.modPageLabel.gameObject.SetActive(false);
		if (this.totalModCount == 0)
		{
			this.modListText.text = ((this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods) ? this.noModsAvailableString : this.noSubscribedModsString);
			return;
		}
		int numPages = this.GetNumPages();
		if (numPages > 1)
		{
			this.modPageText.text = string.Format("{0} / {1}", this.currentModPage + 1, numPages);
			this.modPageText.gameObject.SetActive(true);
			this.modPageLabel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x000E5310 File Offset: 0x000E3510
	public int GetNumPages()
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			return this.customModListPageCount;
		}
		int num = this.totalModCount % this.modsPerPage;
		int num2 = this.totalModCount / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x000E5350 File Offset: 0x000E3550
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x000E535C File Offset: 0x000E355C
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x000E5380 File Offset: 0x000E3580
	public void UpdateFromTerminalStatus(CustomMapsTerminal.TerminalStatus localStatus)
	{
		switch (localStatus.currentScreen)
		{
		case CustomMapsTerminal.ScreenType.AvailableMods:
			this.currentState = CustomMapsListScreen.ListScreenState.AvailableMods;
			break;
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList && CustomMapsTerminal.IsDriver)
			{
				this.currentState = CustomMapsListScreen.ListScreenState.SubscribedMods;
				this.currentModPage = 0;
				this.selectedModIndex = 0;
				this.customModListPageCount = -1;
				return;
			}
			this.currentState = (CustomMapsTerminal.IsDriver ? CustomMapsListScreen.ListScreenState.SubscribedMods : CustomMapsListScreen.ListScreenState.CustomModList);
			break;
		}
		this.currentModPage = localStatus.pageIndex;
		this.selectedModIndex = localStatus.modIndex;
		this.customModListPageCount = localStatus.numModPages;
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000E5421 File Offset: 0x000E3621
	public void RefreshDriverNickname(string driverNickname)
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			this.titleText.text = driverNickname + "'s " + this.subscribedOnlyTitle;
		}
	}

	// Token: 0x04003423 RID: 13347
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04003424 RID: 13348
	[SerializeField]
	private TMP_Text modListText;

	// Token: 0x04003425 RID: 13349
	[SerializeField]
	private TMP_Text modPageLabel;

	// Token: 0x04003426 RID: 13350
	[SerializeField]
	private TMP_Text modPageText;

	// Token: 0x04003427 RID: 13351
	[SerializeField]
	private TMP_Text titleText;

	// Token: 0x04003428 RID: 13352
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x04003429 RID: 13353
	[SerializeField]
	private TMP_Text createdByText;

	// Token: 0x0400342A RID: 13354
	[SerializeField]
	private TMP_Text tagText;

	// Token: 0x0400342B RID: 13355
	[SerializeField]
	private GameObject[] buttonsToHideForAvailableMaps;

	// Token: 0x0400342C RID: 13356
	[SerializeField]
	private GameObject[] buttonsToShow;

	// Token: 0x0400342D RID: 13357
	[SerializeField]
	private GameObject[] buttonsToHideForSubscribedMaps;

	// Token: 0x0400342E RID: 13358
	[SerializeField]
	private string browseModsTitle = "AVAILABLE MODS";

	// Token: 0x0400342F RID: 13359
	[SerializeField]
	private string subscribedOnlyTitle = "SUBSCRIBED MODS";

	// Token: 0x04003430 RID: 13360
	[SerializeField]
	private string nextPageString = "NEXT PAGE";

	// Token: 0x04003431 RID: 13361
	[SerializeField]
	private string prevPageString = "PREVIOUS PAGE";

	// Token: 0x04003432 RID: 13362
	[SerializeField]
	private string noModsAvailableString = "NO MODS AVAILABLE";

	// Token: 0x04003433 RID: 13363
	[SerializeField]
	private string noSubscribedModsString = "NOT SUBSCRIBED TO ANY MODS";

	// Token: 0x04003434 RID: 13364
	[SerializeField]
	private string failedToRetrieveModsString = "FAILED TO RETRIEVE MODS FROM MOD.IO \nPRESS THE 'REFRESH' BUTTON TO RETRY";

	// Token: 0x04003435 RID: 13365
	[SerializeField]
	private string creatorString = "CREATED BY:\n";

	// Token: 0x04003436 RID: 13366
	[SerializeField]
	private string tagString = "MAP TAGS:\n";

	// Token: 0x04003437 RID: 13367
	[SerializeField]
	private int modsPerPage = 10;

	// Token: 0x04003438 RID: 13368
	[SerializeField]
	private int numModsPerRequest = 50;

	// Token: 0x04003439 RID: 13369
	[SerializeField]
	private int maxModListItemLength = 25;

	// Token: 0x0400343A RID: 13370
	[SerializeField]
	private string featuredModsPlayFabKey = "VStumpFeaturedMaps";

	// Token: 0x0400343B RID: 13371
	private bool loadingFeaturedMods;

	// Token: 0x0400343C RID: 13372
	private int totalFeaturedMods;

	// Token: 0x0400343D RID: 13373
	private List<long> featuredModIds = new List<long>();

	// Token: 0x0400343E RID: 13374
	private List<ModProfile> featuredMods = new List<ModProfile>();

	// Token: 0x0400343F RID: 13375
	private int currentAvailableModsRequestPage;

	// Token: 0x04003440 RID: 13376
	private bool loadingAvailableMods;

	// Token: 0x04003441 RID: 13377
	private int totalAvailableMods;

	// Token: 0x04003442 RID: 13378
	private bool errorLoadingAvailableMods;

	// Token: 0x04003443 RID: 13379
	private List<ModProfile> availableMods = new List<ModProfile>();

	// Token: 0x04003444 RID: 13380
	private List<ModProfile> filteredAvailableMods = new List<ModProfile>();

	// Token: 0x04003445 RID: 13381
	private int totalSubscribedMods;

	// Token: 0x04003446 RID: 13382
	private SubscribedMod[] subscribedMods;

	// Token: 0x04003447 RID: 13383
	private List<SubscribedMod> filteredSubscribedMods = new List<SubscribedMod>();

	// Token: 0x04003448 RID: 13384
	private bool loadingCustomModList;

	// Token: 0x04003449 RID: 13385
	private bool errorLoadingCustomModList;

	// Token: 0x0400344A RID: 13386
	private int customModListPageCount = -1;

	// Token: 0x0400344B RID: 13387
	private List<long> customModListModIds = new List<long>();

	// Token: 0x0400344C RID: 13388
	private List<ModProfile> customModList = new List<ModProfile>();

	// Token: 0x0400344D RID: 13389
	private int selectedModIndex;

	// Token: 0x0400344E RID: 13390
	private int currentModPage;

	// Token: 0x0400344F RID: 13391
	private int totalModCount;

	// Token: 0x04003450 RID: 13392
	private List<ModProfile> displayedModProfiles = new List<ModProfile>();

	// Token: 0x04003451 RID: 13393
	private int sortTypeIndex;

	// Token: 0x04003452 RID: 13394
	private SortModsBy sortType = SortModsBy.Popular;

	// Token: 0x04003453 RID: 13395
	private const int MAX_SORT_TYPES = 6;

	// Token: 0x04003454 RID: 13396
	private List<string> searchTags = new List<string>();

	// Token: 0x04003455 RID: 13397
	private bool isAscendingOrder = true;

	// Token: 0x04003456 RID: 13398
	private bool isDownloadingThumbnail;

	// Token: 0x04003457 RID: 13399
	private bool newDownloadRequest;

	// Token: 0x04003458 RID: 13400
	private DownloadReference currentThumbnail;

	// Token: 0x04003459 RID: 13401
	private bool restartCustomModListRetrieval;

	// Token: 0x0400345A RID: 13402
	public CustomMapsListScreen.ListScreenState currentState;

	// Token: 0x0400345B RID: 13403
	private int retrieveRetryCount;

	// Token: 0x0400345C RID: 13404
	private const int maxRetryCount = 10;

	// Token: 0x02000730 RID: 1840
	public enum ListScreenState
	{
		// Token: 0x0400345E RID: 13406
		AvailableMods,
		// Token: 0x0400345F RID: 13407
		SubscribedMods,
		// Token: 0x04003460 RID: 13408
		CustomModList
	}
}
