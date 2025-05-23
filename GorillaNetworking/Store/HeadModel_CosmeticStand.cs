using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C88 RID: 3208
	public class HeadModel_CosmeticStand : HeadModel
	{
		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x06004F8E RID: 20366 RVA: 0x0017B246 File Offset: 0x00179446
		private string mountID
		{
			get
			{
				return "Mount_" + this.bustType.ToString();
			}
		}

		// Token: 0x06004F8F RID: 20367 RVA: 0x0017B264 File Offset: 0x00179464
		public void LoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide = false)
		{
			this.ClearManuallySpawnedCosmeticParts();
			this.ClearCosmetics();
			if (cosmeticInfo == null)
			{
				Debug.LogWarning("Dynamic Cosmetics - LoadWardRobeParts -  No Cosmetic Info");
				return;
			}
			Debug.Log("Dynamic Cosmetics - Loading Wardrobe Parts for " + cosmeticInfo.info.playFabID);
			this.HandleLoadCosmeticParts(cosmeticInfo, forRightSide);
		}

		// Token: 0x06004F90 RID: 20368 RVA: 0x0017B2B4 File Offset: 0x001794B4
		private void HandleLoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide)
		{
			if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Set)
			{
				foreach (CosmeticSO cosmeticSO in cosmeticInfo.info.setCosmetics)
				{
					this.HandleLoadCosmeticParts(cosmeticSO, forRightSide);
				}
				return;
			}
			if (cosmeticInfo.info.category == CosmeticsController.CosmeticCategory.Fur)
			{
				CosmeticPart[] array = cosmeticInfo.info.functionalParts;
				int i = 0;
				if (i < array.Length)
				{
					CosmeticPart cosmeticPart = array[i];
					GameObject gameObject = this.LoadAndInstantiatePrefab(cosmeticPart.prefabAssetRef, base.transform);
					gameObject.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin);
					Object.DestroyImmediate(gameObject);
					return;
				}
			}
			CosmeticPart[] array2;
			if (cosmeticInfo.info.storeParts.Length != 0)
			{
				array2 = cosmeticInfo.info.storeParts;
			}
			else
			{
				array2 = cosmeticInfo.info.wardrobeParts;
			}
			foreach (CosmeticPart cosmeticPart2 in array2)
			{
				foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart2.attachAnchors)
				{
					if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
					{
						HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
						{
							playFabId = cosmeticInfo.info.playFabID,
							prefabAssetRef = cosmeticPart2.prefabAssetRef,
							attachInfo = cosmeticAttachInfo,
							xform = null
						};
						GameObject gameObject2 = this.LoadAndInstantiatePrefab(cosmeticPart2.prefabAssetRef, base.transform);
						cosmeticPartLoadInfo.xform = gameObject2.transform;
						this._manuallySpawnedCosmeticParts.Add(gameObject2);
						gameObject2.SetActive(true);
						switch (this.bustType)
						{
						case HeadModel_CosmeticStand.BustType.Disabled:
							this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaHead:
							this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaTorso:
							this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
							this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.GorillaMannequin:
							this._manuallySpawnedCosmeticParts.Remove(gameObject2);
							Object.DestroyImmediate(gameObject2);
							break;
						case HeadModel_CosmeticStand.BustType.GuitarStand:
							this.PositionWardRobeItems(gameObject2, cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.JewelryBox:
							this.PositionWardRobeItems(gameObject2, cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.Table:
							this.PositionWardRobeItems(gameObject2, cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.PinDisplay:
							this.PositionWardRobeItems(gameObject2, cosmeticPartLoadInfo);
							break;
						case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
							this.PositionWardRobeItems(gameObject2, cosmeticPartLoadInfo);
							break;
						default:
							this.PositionWithWardRobeOffsets(cosmeticPartLoadInfo);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06004F91 RID: 20369 RVA: 0x0017B541 File Offset: 0x00179741
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.bustType = newBustType;
		}

		// Token: 0x06004F92 RID: 20370 RVA: 0x0017B54C File Offset: 0x0017974C
		private void PositionWardRobeItems(GameObject instantiateEdObject, HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Transform transform = instantiateEdObject.transform.FindChildRecursive(this.mountID);
			if (transform != null)
			{
				Debug.Log("Dynamic Cosmetics - Mount Found: " + this.mountID);
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				instantiateEdObject.transform.localPosition = transform.localPosition;
				instantiateEdObject.transform.localRotation = transform.localRotation;
				return;
			}
			HeadModel_CosmeticStand.BustType bustType = this.bustType;
			if (bustType - HeadModel_CosmeticStand.BustType.GuitarStand <= 2 || bustType == HeadModel_CosmeticStand.BustType.TagEffectDisplay)
			{
				instantiateEdObject.transform.position = base.transform.position;
				instantiateEdObject.transform.rotation = base.transform.rotation;
				return;
			}
			this.PositionWithWardRobeOffsets(partLoadInfo);
		}

		// Token: 0x06004F93 RID: 20371 RVA: 0x0017B620 File Offset: 0x00179820
		private void PositionWithWardRobeOffsets(HeadModel._CosmeticPartLoadInfo partLoadInfo)
		{
			Debug.Log("Dynamic Cosmetics - Mount Not Found: " + this.mountID);
			partLoadInfo.xform.localPosition = partLoadInfo.attachInfo.offset.pos;
			partLoadInfo.xform.localRotation = partLoadInfo.attachInfo.offset.rot;
			partLoadInfo.xform.localScale = partLoadInfo.attachInfo.offset.scale;
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x0017B694 File Offset: 0x00179894
		public void ClearManuallySpawnedCosmeticParts()
		{
			foreach (GameObject gameObject in this._manuallySpawnedCosmeticParts)
			{
				Object.DestroyImmediate(gameObject);
			}
			this._manuallySpawnedCosmeticParts.Clear();
		}

		// Token: 0x06004F95 RID: 20373 RVA: 0x0017B6F0 File Offset: 0x001798F0
		public void ClearCosmetics()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x06004F96 RID: 20374 RVA: 0x00045F91 File Offset: 0x00044191
		private GameObject LoadAndInstantiatePrefab(GTAssetRef<GameObject> prefabAssetRef, Transform parent)
		{
			return null;
		}

		// Token: 0x06004F97 RID: 20375 RVA: 0x000023F4 File Offset: 0x000005F4
		public void UpdateCosmeticsMountPositions(CosmeticSO findCosmeticInAllCosmeticsArraySO)
		{
		}

		// Token: 0x040052A1 RID: 21153
		public HeadModel_CosmeticStand.BustType bustType = HeadModel_CosmeticStand.BustType.JewelryBox;

		// Token: 0x040052A2 RID: 21154
		[SerializeField]
		private List<GameObject> _manuallySpawnedCosmeticParts = new List<GameObject>();

		// Token: 0x040052A3 RID: 21155
		public GameObject mannequin;

		// Token: 0x02000C89 RID: 3209
		public enum BustType
		{
			// Token: 0x040052A5 RID: 21157
			Disabled,
			// Token: 0x040052A6 RID: 21158
			GorillaHead,
			// Token: 0x040052A7 RID: 21159
			GorillaTorso,
			// Token: 0x040052A8 RID: 21160
			GorillaTorsoPost,
			// Token: 0x040052A9 RID: 21161
			GorillaMannequin,
			// Token: 0x040052AA RID: 21162
			GuitarStand,
			// Token: 0x040052AB RID: 21163
			JewelryBox,
			// Token: 0x040052AC RID: 21164
			Table,
			// Token: 0x040052AD RID: 21165
			PinDisplay,
			// Token: 0x040052AE RID: 21166
			TagEffectDisplay
		}
	}
}
