using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200051C RID: 1308
public class BuilderSetSelector : MonoBehaviour
{
	// Token: 0x06001FAA RID: 8106 RVA: 0x0009F508 File Offset: 0x0009D708
	private void Start()
	{
		this.zoneRenderers.Clear();
		foreach (GorillaPressableButton gorillaPressableButton in this.setButtons)
		{
			this.zoneRenderers.Add(gorillaPressableButton.buttonRenderer);
			TMP_Text myTmpText = gorillaPressableButton.myTmpText;
			Renderer renderer = ((myTmpText != null) ? myTmpText.GetComponent<Renderer>() : null);
			if (renderer != null)
			{
				this.zoneRenderers.Add(renderer);
			}
		}
		this.zoneRenderers.Add(this.previousPageButton.buttonRenderer);
		this.zoneRenderers.Add(this.nextPageButton.buttonRenderer);
		TMP_Text myTmpText2 = this.previousPageButton.myTmpText;
		Renderer renderer2 = ((myTmpText2 != null) ? myTmpText2.GetComponent<Renderer>() : null);
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		TMP_Text myTmpText3 = this.nextPageButton.myTmpText;
		renderer2 = ((myTmpText3 != null) ? myTmpText3.GetComponent<Renderer>() : null);
		if (renderer2 != null)
		{
			this.zoneRenderers.Add(renderer2);
		}
		this.inBuilderZone = true;
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.OnZoneChanged();
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x0009F630 File Offset: 0x0009D830
	public void Setup(List<BuilderPieceSet.BuilderPieceCategory> categories)
	{
		List<BuilderPieceSet> livePieceSets = BuilderSetManager.instance.GetLivePieceSets();
		this.numLivePieceSets = livePieceSets.Count;
		this.pieceSets = new List<BuilderPieceSet>(livePieceSets.Count);
		this._includedCategories = categories;
		foreach (BuilderPieceSet builderPieceSet in livePieceSets)
		{
			if (!builderPieceSet.setName.Equals("HIDDEN") && this.DoesSetHaveIncludedCategories(builderPieceSet))
			{
				this.pieceSets.Add(builderPieceSet);
			}
		}
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedSets));
		BuilderSetManager.instance.OnLiveSetsUpdated.AddListener(new UnityAction(this.RefreshUnlockedSets));
		this.setsPerPage = this.setButtons.Length;
		this.totalPages = this.pieceSets.Count / this.setsPerPage;
		if (this.pieceSets.Count % this.setsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = this.totalPages > 1;
		this.nextPageButton.myTmpText.enabled = this.totalPages > 1;
		this.pageIndex = 0;
		this.currentSet = this.pieceSets[this.setIndex];
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.setButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += this.OnSetButtonPressed;
		}
		this.UpdateLabels();
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x0009F840 File Offset: 0x0009DA40
	private void OnDestroy()
	{
		if (this.previousPageButton != null)
		{
			this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
		}
		if (this.nextPageButton != null)
		{
			this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedSets));
			BuilderSetManager.instance.OnLiveSetsUpdated.RemoveListener(new UnityAction(this.RefreshUnlockedSets));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.setButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnSetButtonPressed;
			}
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x0009F954 File Offset: 0x0009DB54
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Renderer renderer = enumerator.Current;
					renderer.enabled = true;
				}
				goto IL_008B;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			foreach (Renderer renderer2 in this.zoneRenderers)
			{
				renderer2.enabled = false;
			}
		}
		IL_008B:
		this.inBuilderZone = flag;
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x0009FA10 File Offset: 0x0009DC10
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		int num = 0;
		for (int i = 0; i < this.setButtons.Length; i++)
		{
			if (button.Equals(this.setButtons[i]))
			{
				num = i;
				break;
			}
		}
		int num2 = this.pageIndex * this.setsPerPage + num;
		if (num2 < this.pieceSets.Count)
		{
			BuilderPieceSet builderPieceSet = this.pieceSets[num2];
			if (this.currentSet == null || builderPieceSet.setName != this.currentSet.setName)
			{
				UnityEvent<int> onSelectedSet = this.OnSelectedSet;
				if (onSelectedSet == null)
				{
					return;
				}
				onSelectedSet.Invoke(builderPieceSet.GetIntIdentifier());
			}
		}
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x0009FAB0 File Offset: 0x0009DCB0
	private void RefreshUnlockedSets()
	{
		List<BuilderPieceSet> livePieceSets = BuilderSetManager.instance.GetLivePieceSets();
		if (livePieceSets.Count != this.numLivePieceSets)
		{
			string text = ((this.currentSet != null) ? this.currentSet.setName : "");
			this.numLivePieceSets = livePieceSets.Count;
			this.pieceSets.EnsureCapacity(this.numLivePieceSets);
			this.pieceSets.Clear();
			int num = 0;
			foreach (BuilderPieceSet builderPieceSet in livePieceSets)
			{
				if (!builderPieceSet.setName.Equals("HIDDEN") && this.DoesSetHaveIncludedCategories(builderPieceSet))
				{
					if (builderPieceSet.setName.Equals(text))
					{
						num = this.pieceSets.Count;
					}
					this.pieceSets.Add(builderPieceSet);
				}
			}
			if (this.pieceSets.Count < 1)
			{
				this.currentSet = null;
			}
			else
			{
				this.setIndex = num;
				this.currentSet = this.pieceSets[this.setIndex];
			}
			this.totalPages = this.pieceSets.Count / this.setsPerPage;
			if (this.pieceSets.Count % this.setsPerPage > 0)
			{
				this.totalPages++;
			}
			this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
			this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
			this.previousPageButton.myTmpText.enabled = this.totalPages > 1;
			this.nextPageButton.myTmpText.enabled = this.totalPages > 1;
		}
		this.UpdateLabels();
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x0009FC80 File Offset: 0x0009DE80
	private void OnPreviousPageClicked()
	{
		this.RefreshUnlockedSets();
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x0009FCC0 File Offset: 0x0009DEC0
	private void OnNextPageClicked()
	{
		this.RefreshUnlockedSets();
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x0009FD00 File Offset: 0x0009DF00
	public void SetSelection(int setID)
	{
		if (BuilderSetManager.instance == null)
		{
			return;
		}
		BuilderPieceSet pieceSetFromID = BuilderSetManager.instance.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return;
		}
		this.currentSet = pieceSetFromID;
		this.UpdateLabels();
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x0009FD44 File Offset: 0x0009DF44
	private void UpdateLabels()
	{
		for (int i = 0; i < this.setLabels.Length; i++)
		{
			int num = this.pageIndex * this.setsPerPage + i;
			if (num < this.pieceSets.Count && this.pieceSets[num] != null)
			{
				if (!this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(true);
					this.setButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.setButtons[i].myTmpText.text != this.pieceSets[num].setName.ToUpper())
				{
					this.setButtons[i].myTmpText.text = this.pieceSets[num].setName.ToUpper();
				}
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.pieceSets[num].GetIntIdentifier()))
				{
					bool flag = this.currentSet != null && this.pieceSets[num].setName == this.currentSet.setName;
					if (flag != this.setButtons[i].isOn || !this.setButtons[i].enabled)
					{
						this.setButtons[i].isOn = flag;
						this.setButtons[i].buttonRenderer.material = (flag ? this.setButtons[i].pressedMaterial : this.setButtons[i].unpressedMaterial);
					}
					this.setButtons[i].enabled = true;
				}
				else
				{
					if (this.setButtons[i].enabled)
					{
						this.setButtons[i].buttonRenderer.material = this.disabledMaterial;
					}
					this.setButtons[i].enabled = false;
				}
			}
			else
			{
				if (this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(false);
					this.setButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.setButtons[i].isOn || this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = false;
					this.setButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x000A0030 File Offset: 0x0009E230
	public bool DoesSetHaveIncludedCategories(BuilderPieceSet set)
	{
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in set.subsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000A0098 File Offset: 0x0009E298
	public BuilderPieceSet GetSelectedSet()
	{
		return this.currentSet;
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x000A00A0 File Offset: 0x0009E2A0
	public int GetDefaultSetID()
	{
		if (this.pieceSets == null || this.pieceSets.Count < 1)
		{
			return -1;
		}
		BuilderPieceSet builderPieceSet = this.pieceSets[0];
		if (!BuilderSetManager.instance.IsPieceSetOwnedLocally(builderPieceSet.GetIntIdentifier()))
		{
			foreach (BuilderPieceSet builderPieceSet2 in this.pieceSets)
			{
				if (BuilderSetManager.instance.IsPieceSetOwnedLocally(builderPieceSet2.GetIntIdentifier()))
				{
					return builderPieceSet2.GetIntIdentifier();
				}
			}
			Debug.LogWarning("No default set available for shelf");
			return -1;
		}
		return builderPieceSet.GetIntIdentifier();
	}

	// Token: 0x04002386 RID: 9094
	private List<BuilderPieceSet> pieceSets;

	// Token: 0x04002387 RID: 9095
	private int numLivePieceSets;

	// Token: 0x04002388 RID: 9096
	[SerializeField]
	private Material disabledMaterial;

	// Token: 0x04002389 RID: 9097
	[Header("UI")]
	[SerializeField]
	private Text[] setLabels;

	// Token: 0x0400238A RID: 9098
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableButton[] setButtons;

	// Token: 0x0400238B RID: 9099
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x0400238C RID: 9100
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x0400238D RID: 9101
	private List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x0400238E RID: 9102
	private int setIndex;

	// Token: 0x0400238F RID: 9103
	private BuilderPieceSet currentSet;

	// Token: 0x04002390 RID: 9104
	private int pageIndex;

	// Token: 0x04002391 RID: 9105
	private int setsPerPage = 3;

	// Token: 0x04002392 RID: 9106
	private int totalPages = 1;

	// Token: 0x04002393 RID: 9107
	private List<Renderer> zoneRenderers = new List<Renderer>(10);

	// Token: 0x04002394 RID: 9108
	private bool inBuilderZone;

	// Token: 0x04002395 RID: 9109
	[HideInInspector]
	public UnityEvent<int> OnSelectedSet;
}
