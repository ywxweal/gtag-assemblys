using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ADA RID: 2778
	public class BuilderFactory : MonoBehaviour
	{
		// Token: 0x06004333 RID: 17203 RVA: 0x001368ED File Offset: 0x00134AED
		private void Awake()
		{
			this.InitIfNeeded();
		}

		// Token: 0x06004334 RID: 17204 RVA: 0x001368F8 File Offset: 0x00134AF8
		public void InitIfNeeded()
		{
			if (this.initialized)
			{
				return;
			}
			this.buildItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnBuildItem));
			this.currPieceTypeIndex = 0;
			this.prevItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevItem));
			this.nextItemButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextItem));
			this.currPieceMaterialIndex = 0;
			this.prevMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnPrevMaterial));
			this.nextMaterialButton.Setup(new Action<BuilderOptionButton, bool>(this.OnNextMaterial));
			this.pieceTypeToIndex = new Dictionary<int, int>(256);
			this.initialized = true;
			if (this.resourceCostUIs != null)
			{
				for (int i = 0; i < this.resourceCostUIs.Count; i++)
				{
					if (this.resourceCostUIs[i] != null)
					{
						this.resourceCostUIs[i].gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06004335 RID: 17205 RVA: 0x001369F0 File Offset: 0x00134BF0
		public void Setup(BuilderTable tableOwner)
		{
			this.table = tableOwner;
			this.InitIfNeeded();
			List<BuilderPiece> list = this.pieceList;
			this.pieceTypes = new List<int>(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				string name = list[i].name;
				int staticHash = name.GetStaticHash();
				int num;
				if (this.pieceTypeToIndex.TryAdd(staticHash, i))
				{
					this.pieceTypes.Add(staticHash);
				}
				else if (this.pieceTypeToIndex.TryGetValue(staticHash, out num))
				{
					string text = "BuilderFactory: ERROR!! " + string.Format("Could not add pieceType \"{0}\" with hash {1} ", name, staticHash) + "to 'pieceTypeToIndex' Dictionary because because it was already added!";
					if (num < 0 || num >= list.Count)
					{
						text += " Also the index to the conflicting piece is out of range of the pieceList!";
					}
					else
					{
						BuilderPiece builderPiece = list[num];
						if (builderPiece != null)
						{
							if (name == builderPiece.name)
							{
								text += "The conflicting piece has the same name (as expected).";
							}
							else
							{
								text = text + "Also the conflicting pieceType has the same hash but different name \"" + builderPiece.name + "\"!";
							}
						}
						else
						{
							text += "And (should never happen) the piece at that slot is null!";
						}
					}
					Debug.LogError(text, this);
				}
			}
			int num2 = this.pieceTypes.Count;
			foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.GetAllPieceSets())
			{
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash2 = pieceInfo.piecePrefab.name.GetStaticHash();
						if (!this.pieceTypeToIndex.ContainsKey(staticHash2))
						{
							this.pieceList.Add(pieceInfo.piecePrefab);
							this.pieceTypes.Add(staticHash2);
							this.pieceTypeToIndex.Add(staticHash2, num2);
							num2++;
						}
					}
				}
			}
		}

		// Token: 0x06004336 RID: 17206 RVA: 0x00136C54 File Offset: 0x00134E54
		public void Show()
		{
			this.RefreshUI();
		}

		// Token: 0x06004337 RID: 17207 RVA: 0x00136C5C File Offset: 0x00134E5C
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			int num;
			if (this.pieceTypeToIndex.TryGetValue(pieceType, out num))
			{
				return this.pieceList[num];
			}
			Debug.LogErrorFormat("No Prefab found for type {0}", new object[] { pieceType });
			return null;
		}

		// Token: 0x06004338 RID: 17208 RVA: 0x00136CA0 File Offset: 0x00134EA0
		public void OnBuildItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > this.currPieceTypeIndex)
			{
				int selectedMaterialType = this.GetSelectedMaterialType();
				this.table.RequestCreatePiece(this.pieceTypes[this.currPieceTypeIndex], this.spawnLocation.position, this.spawnLocation.rotation, selectedMaterialType);
				if (this.audioSource != null && this.buildPieceSound != null)
				{
					this.audioSource.GTPlayOneShot(this.buildPieceSound, 1f);
				}
			}
		}

		// Token: 0x06004339 RID: 17209 RVA: 0x00136D34 File Offset: 0x00134F34
		public void OnPrevItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex - 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x0600433A RID: 17210 RVA: 0x00136DB4 File Offset: 0x00134FB4
		public void OnNextItem(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				for (int i = 0; i < this.pieceTypes.Count; i++)
				{
					this.currPieceTypeIndex = (this.currPieceTypeIndex + 1 + this.pieceTypes.Count) % this.pieceTypes.Count;
					if (this.CanBuildPieceType(this.pieceTypes[this.currPieceTypeIndex]))
					{
						break;
					}
				}
				this.RefreshUI();
			}
		}

		// Token: 0x0600433B RID: 17211 RVA: 0x00136E34 File Offset: 0x00135034
		public void OnPrevMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex - 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x0600433C RID: 17212 RVA: 0x00136F04 File Offset: 0x00135104
		public void OnNextMaterial(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.pieceTypes != null && this.pieceTypes.Count > 0)
			{
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
				if (piecePrefab != null)
				{
					BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
					if (materialOptions != null && materialOptions.options.Count > 0)
					{
						for (int i = 0; i < materialOptions.options.Count; i++)
						{
							this.currPieceMaterialIndex = (this.currPieceMaterialIndex + 1 + materialOptions.options.Count) % materialOptions.options.Count;
							if (this.CanUseMaterialType(materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode()))
							{
								break;
							}
						}
					}
					this.RefreshUI();
				}
			}
		}

		// Token: 0x0600433D RID: 17213 RVA: 0x00136FD4 File Offset: 0x001351D4
		private int GetSelectedMaterialType()
		{
			int num = -1;
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					num = materialOptions.options[this.currPieceMaterialIndex].materialId.GetHashCode();
				}
			}
			return num;
		}

		// Token: 0x0600433E RID: 17214 RVA: 0x00137058 File Offset: 0x00135258
		private string GetSelectedMaterialName()
		{
			string text = "DEFAULT";
			BuilderPiece piecePrefab = this.GetPiecePrefab(this.pieceTypes[this.currPieceTypeIndex]);
			if (piecePrefab != null)
			{
				BuilderMaterialOptions materialOptions = piecePrefab.materialOptions;
				if (materialOptions != null && materialOptions.options != null && this.currPieceMaterialIndex >= 0 && this.currPieceMaterialIndex < materialOptions.options.Count)
				{
					text = materialOptions.options[this.currPieceMaterialIndex].materialId;
				}
			}
			return text;
		}

		// Token: 0x0600433F RID: 17215 RVA: 0x001370D8 File Offset: 0x001352D8
		public bool CanBuildPieceType(int pieceType)
		{
			BuilderPiece piecePrefab = this.GetPiecePrefab(pieceType);
			return !(piecePrefab == null) && !piecePrefab.isBuiltIntoTable;
		}

		// Token: 0x06004340 RID: 17216 RVA: 0x00047642 File Offset: 0x00045842
		public bool CanUseMaterialType(int materalType)
		{
			return true;
		}

		// Token: 0x06004341 RID: 17217 RVA: 0x00137104 File Offset: 0x00135304
		public void RefreshUI()
		{
			if (this.pieceList != null && this.pieceList.Count > this.currPieceTypeIndex)
			{
				this.itemLabel.SetText(this.pieceList[this.currPieceTypeIndex].displayName, true);
			}
			else
			{
				this.itemLabel.SetText("No Items", true);
			}
			if (this.previewPiece != null)
			{
				this.table.builderPool.DestroyPiece(this.previewPiece);
				this.previewPiece = null;
			}
			if (this.currPieceTypeIndex < 0 || this.currPieceTypeIndex >= this.pieceTypes.Count)
			{
				return;
			}
			int num = this.pieceTypes[this.currPieceTypeIndex];
			this.previewPiece = this.table.builderPool.CreatePiece(num, false);
			this.previewPiece.SetTable(this.table);
			this.previewPiece.pieceType = num;
			string selectedMaterialName = this.GetSelectedMaterialName();
			this.materialLabel.SetText(selectedMaterialName, true);
			this.previewPiece.SetScale(this.table.pieceScale * 0.75f);
			this.previewPiece.SetupPiece(this.table.gridSize);
			int selectedMaterialType = this.GetSelectedMaterialType();
			this.previewPiece.SetMaterial(selectedMaterialType, true);
			this.previewPiece.transform.SetPositionAndRotation(this.previewMarker.position, this.previewMarker.rotation);
			this.previewPiece.SetState(BuilderPiece.State.Displayed, false);
			this.previewPiece.enabled = false;
			this.RefreshCostUI();
		}

		// Token: 0x06004342 RID: 17218 RVA: 0x00137290 File Offset: 0x00135490
		private void RefreshCostUI()
		{
			List<BuilderResourceQuantity> list = null;
			if (this.previewPiece != null)
			{
				list = this.previewPiece.cost.quantities;
			}
			for (int i = 0; i < this.resourceCostUIs.Count; i++)
			{
				if (!(this.resourceCostUIs[i] == null))
				{
					bool flag = list != null && i < list.Count;
					this.resourceCostUIs[i].gameObject.SetActive(flag);
					if (flag)
					{
						this.resourceCostUIs[i].SetResourceCost(list[i], this.table);
					}
				}
			}
		}

		// Token: 0x06004343 RID: 17219 RVA: 0x00137330 File Offset: 0x00135530
		public void OnAvailableResourcesChange()
		{
			this.RefreshCostUI();
		}

		// Token: 0x06004344 RID: 17220 RVA: 0x00137338 File Offset: 0x00135538
		public void CreateRandomPiece()
		{
			Debug.LogError("Create Random Piece No longer implemented");
		}

		// Token: 0x040045B4 RID: 17844
		public Transform spawnLocation;

		// Token: 0x040045B5 RID: 17845
		private List<int> pieceTypes;

		// Token: 0x040045B6 RID: 17846
		public List<GameObject> itemList;

		// Token: 0x040045B7 RID: 17847
		[HideInInspector]
		public List<BuilderPiece> pieceList;

		// Token: 0x040045B8 RID: 17848
		public BuilderOptionButton buildItemButton;

		// Token: 0x040045B9 RID: 17849
		public TextMeshPro itemLabel;

		// Token: 0x040045BA RID: 17850
		public BuilderOptionButton prevItemButton;

		// Token: 0x040045BB RID: 17851
		public BuilderOptionButton nextItemButton;

		// Token: 0x040045BC RID: 17852
		public TextMeshPro materialLabel;

		// Token: 0x040045BD RID: 17853
		public BuilderOptionButton prevMaterialButton;

		// Token: 0x040045BE RID: 17854
		public BuilderOptionButton nextMaterialButton;

		// Token: 0x040045BF RID: 17855
		public AudioSource audioSource;

		// Token: 0x040045C0 RID: 17856
		public AudioClip buildPieceSound;

		// Token: 0x040045C1 RID: 17857
		public Transform previewMarker;

		// Token: 0x040045C2 RID: 17858
		public List<BuilderUIResource> resourceCostUIs;

		// Token: 0x040045C3 RID: 17859
		private BuilderPiece previewPiece;

		// Token: 0x040045C4 RID: 17860
		private int currPieceTypeIndex;

		// Token: 0x040045C5 RID: 17861
		private int currPieceMaterialIndex;

		// Token: 0x040045C6 RID: 17862
		private Dictionary<int, int> pieceTypeToIndex;

		// Token: 0x040045C7 RID: 17863
		private BuilderTable table;

		// Token: 0x040045C8 RID: 17864
		private bool initialized;
	}
}
