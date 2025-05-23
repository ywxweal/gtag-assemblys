using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E4 RID: 1252
public class BuilderDispenserShelf : MonoBehaviour
{
	// Token: 0x06001E49 RID: 7753 RVA: 0x0009367F File Offset: 0x0009187F
	private void BuildDispenserPool()
	{
		this.dispenserPool = new List<BuilderDispenser>(12);
		this.activeDispensers = new List<BuilderDispenser>(6);
		this.AddToDispenserPool(6);
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x000936A4 File Offset: 0x000918A4
	private void AddToDispenserPool(int count)
	{
		if (this.dispenserPrefab == null)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			BuilderDispenser builderDispenser = Object.Instantiate<BuilderDispenser>(this.dispenserPrefab, this.shelfCenter);
			builderDispenser.gameObject.SetActive(false);
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
			this.dispenserPool.Add(builderDispenser);
		}
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x00093710 File Offset: 0x00091910
	private void ActivateDispensers()
	{
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentSet.subsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		int count = this.piecesInSet.Count;
		if (this.dispenserPool.Count < count)
		{
			this.AddToDispenserPool(count - this.dispenserPool.Count);
		}
		this.activeDispensers.Clear();
		for (int i = 0; i < this.dispenserPool.Count; i++)
		{
			if (i < count)
			{
				BuilderDispenser builderDispenser = this.dispenserPool[i];
				builderDispenser.gameObject.SetActive(true);
				float num = this.shelfWidth / -2f + this.shelfWidth / (float)(count * 2) + this.shelfWidth / (float)count * (float)i;
				builderDispenser.transform.localPosition = new Vector3(num, 0f, 0f);
				builderDispenser.AssignPieceType(this.piecesInSet[i], this.currentSet.materialId.GetHashCode());
				this.activeDispensers.Add(builderDispenser);
			}
			else
			{
				this.dispenserPool[i].ClearDispenser();
				this.dispenserPool[i].gameObject.SetActive(false);
			}
		}
		this.dispenserToUpdate = 0;
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x000938BC File Offset: 0x00091ABC
	public void Setup()
	{
		this.InitIfNeeded();
		foreach (BuilderDispenser builderDispenser in this.dispenserPool)
		{
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
		}
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x00093924 File Offset: 0x00091B24
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.setSelector.Setup(this._includedCategories);
		this.currentSet = this.setSelector.GetSelectedSet();
		this.setSelector.OnSelectedSet.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.BuildDispenserPool();
		this.ActivateDispensers();
		this.initialized = true;
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x0009398B File Offset: 0x00091B8B
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedSet.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
	}

	// Token: 0x06001E4F RID: 7759 RVA: 0x000939B7 File Offset: 0x00091BB7
	public void OnSelectedSetChange(int setId)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, setId, false);
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x000939DC File Offset: 0x00091BDC
	public void SetSelection(int setId)
	{
		this.setSelector.SetSelection(setId);
		BuilderPieceSet selectedSet = this.setSelector.GetSelectedSet();
		if ((this.initialized && this.currentSet == null) || selectedSet.setName != this.currentSet.setName)
		{
			this.currentSet = selectedSet;
			if (this.table.GetTableState() == BuilderTable.TableState.Ready)
			{
				if (!this.animatingShelf)
				{
					this.StartShelfSwap();
					return;
				}
			}
			else
			{
				this.animatingShelf = false;
				this.ImmediateShelfSwap();
			}
		}
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x00093A60 File Offset: 0x00091C60
	public int GetSelectedSetID()
	{
		return this.setSelector.GetSelectedSet().GetIntIdentifier();
	}

	// Token: 0x06001E52 RID: 7762 RVA: 0x00093A74 File Offset: 0x00091C74
	private void ImmediateShelfSwap()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
		this.ActivateDispensers();
	}

	// Token: 0x06001E53 RID: 7763 RVA: 0x00093ACC File Offset: 0x00091CCC
	private void StartShelfSwap()
	{
		this.dispenserToClear = 0;
		this.timeToClearShelf = (double)(Time.time + 0.15f);
		this.resetAnimation.Rewind();
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ParentPieceToShelf(this.resetAnimation.transform);
		}
		this.resetAnimation.Play();
		this.animatingShelf = true;
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x00093B60 File Offset: 0x00091D60
	public void UpdateShelf()
	{
		if (this.animatingShelf && (double)Time.time > this.timeToClearShelf)
		{
			if (this.dispenserToClear < this.activeDispensers.Count)
			{
				if (this.dispenserToClear == 0)
				{
					this.resetSoundBank.Play();
				}
				this.activeDispensers[this.dispenserToClear].ClearDispenser();
				this.dispenserToClear++;
				return;
			}
			if (!this.resetAnimation.isPlaying)
			{
				this.playSpawnSetSound = true;
				this.ActivateDispensers();
				this.animatingShelf = false;
			}
		}
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x00093BF0 File Offset: 0x00091DF0
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (!this.initialized)
		{
			return;
		}
		if (this.animatingShelf)
		{
			return;
		}
		if (this.shouldVerifySetSelection)
		{
			BuilderPieceSet selectedSet = this.setSelector.GetSelectedSet();
			if (selectedSet == null || !BuilderSetManager.instance.DoesAnyPlayerInRoomOwnPieceSet(selectedSet.GetIntIdentifier()))
			{
				int defaultSetID = this.setSelector.GetDefaultSetID();
				if (defaultSetID != -1)
				{
					this.OnSelectedSetChange(defaultSetID);
				}
			}
			this.shouldVerifySetSelection = false;
		}
		if (this.activeDispensers.Count > 0)
		{
			this.activeDispensers[this.dispenserToUpdate].UpdateDispenser();
			this.dispenserToUpdate = (this.dispenserToUpdate + 1) % this.activeDispensers.Count;
		}
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x00093CA9 File Offset: 0x00091EA9
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x06001E57 RID: 7767 RVA: 0x00093CB4 File Offset: 0x00091EB4
	public void OnShelfPieceCreated(BuilderPiece piece, bool playfx)
	{
		if (this.playSpawnSetSound && playfx)
		{
			this.audioSource.GTPlayOneShot(this.spawnNewSetSound, 1f);
			this.playSpawnSetSound = false;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceCreated(piece, playfx);
		}
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x00093D30 File Offset: 0x00091F30
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceRecycled(piece);
		}
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x00093D84 File Offset: 0x00091F84
	public void OnClearTable()
	{
		if (!this.initialized)
		{
			return;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.OnClearTable();
		}
		base.StopAllCoroutines();
		if (this.animatingShelf)
		{
			this.resetAnimation.Rewind();
			this.animatingShelf = false;
		}
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x00093E00 File Offset: 0x00092000
	public void ClearShelf()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
	}

	// Token: 0x04002194 RID: 8596
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x04002195 RID: 8597
	public List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x04002196 RID: 8598
	[Header("Dispenser Shelf Properties")]
	public Transform shelfCenter;

	// Token: 0x04002197 RID: 8599
	public float shelfWidth = 1.4f;

	// Token: 0x04002198 RID: 8600
	public Animation resetAnimation;

	// Token: 0x04002199 RID: 8601
	[SerializeField]
	private SoundBankPlayer resetSoundBank;

	// Token: 0x0400219A RID: 8602
	[SerializeField]
	private AudioClip spawnNewSetSound;

	// Token: 0x0400219B RID: 8603
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400219C RID: 8604
	private bool playSpawnSetSound;

	// Token: 0x0400219D RID: 8605
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x0400219E RID: 8606
	public int shelfID = -1;

	// Token: 0x0400219F RID: 8607
	private BuilderPieceSet currentSet;

	// Token: 0x040021A0 RID: 8608
	private bool initialized;

	// Token: 0x040021A1 RID: 8609
	public BuilderDispenser dispenserPrefab;

	// Token: 0x040021A2 RID: 8610
	private List<BuilderDispenser> dispenserPool;

	// Token: 0x040021A3 RID: 8611
	private List<BuilderDispenser> activeDispensers;

	// Token: 0x040021A4 RID: 8612
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x040021A5 RID: 8613
	private bool animatingShelf;

	// Token: 0x040021A6 RID: 8614
	private double timeToClearShelf = double.MaxValue;

	// Token: 0x040021A7 RID: 8615
	private int dispenserToClear;

	// Token: 0x040021A8 RID: 8616
	private int dispenserToUpdate;

	// Token: 0x040021A9 RID: 8617
	private bool shouldVerifySetSelection;
}
