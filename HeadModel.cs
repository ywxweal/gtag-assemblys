using System;
using System.Collections.Generic;
using Cysharp.Text;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x020003F4 RID: 1012
public class HeadModel : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06001849 RID: 6217 RVA: 0x00075D7C File Offset: 0x00073F7C
	protected void Awake()
	{
		this._mannequinRenderer = base.GetComponentInChildren<Renderer>(true);
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x00075D8B File Offset: 0x00073F8B
	public void SetCosmeticActive(string playFabId, bool forRightSide = false)
	{
		this._ClearCurrent();
		this._AddPreviewCosmetic(playFabId, forRightSide);
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x00075D9C File Offset: 0x00073F9C
	public void SetCosmeticActiveArray(string[] playFabIds, bool[] forRightSideArray)
	{
		this._ClearCurrent();
		for (int i = 0; i < playFabIds.Length; i++)
		{
			this._AddPreviewCosmetic(playFabIds[i], forRightSideArray[i]);
		}
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x00075DCC File Offset: 0x00073FCC
	private void _AddPreviewCosmetic(string playFabId, bool forRightSide)
	{
		CosmeticInfoV2 cosmeticInfoV;
		if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfoV))
		{
			if (!(playFabId == "null") && !(playFabId == "NOTHING") && !(playFabId == "Slingshot"))
			{
				Debug.LogError(ZString.Concat<string, string, string>("HeadModel._AddPreviewCosmetic: Cosmetic id \"", playFabId, "\" not found in `CosmeticsController`."), this);
			}
			return;
		}
		if (cosmeticInfoV.hideWardrobeMannequin)
		{
			this._mannequinRenderer.enabled = false;
		}
		foreach (CosmeticPart cosmeticPart in cosmeticInfoV.wardrobeParts)
		{
			foreach (CosmeticAttachInfo cosmeticAttachInfo in cosmeticPart.attachAnchors)
			{
				if ((!forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Left)) && (forRightSide || !(cosmeticAttachInfo.selectSide == ECosmeticSelectSide.Right)))
				{
					HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo
					{
						playFabId = playFabId,
						prefabAssetRef = cosmeticPart.prefabAssetRef,
						attachInfo = cosmeticAttachInfo,
						loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(base.transform, false),
						xform = null
					};
					cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadOpOnCompleted;
					this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
					this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
				}
			}
		}
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x00075F54 File Offset: 0x00074154
	private void _HandleLoadOpOnCompleted(AsyncOperationHandle<GameObject> loadOp)
	{
		int num;
		if (!this._loadOp_to_partInfoIndex.TryGetValue(loadOp, out num))
		{
			if (loadOp.Status == AsyncOperationStatus.Succeeded && loadOp.Result)
			{
				Object.Destroy(loadOp.Result);
			}
			return;
		}
		HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[num];
		if (loadOp.Status == AsyncOperationStatus.Failed)
		{
			Debug.Log("HeadModel: Failed to load a part for cosmetic \"" + cosmeticPartLoadInfo.playFabId + "\"! Waiting for 10 seconds before trying again.", this);
			GTDelayedExec.Add(this, 10f, num);
			return;
		}
		cosmeticPartLoadInfo.xform = loadOp.Result.transform;
		cosmeticPartLoadInfo.xform.localPosition = cosmeticPartLoadInfo.attachInfo.offset.pos;
		cosmeticPartLoadInfo.xform.localRotation = cosmeticPartLoadInfo.attachInfo.offset.rot;
		cosmeticPartLoadInfo.xform.localScale = cosmeticPartLoadInfo.attachInfo.offset.scale;
		cosmeticPartLoadInfo.xform.gameObject.SetActive(true);
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x00076050 File Offset: 0x00074250
	void IDelayedExecListener.OnDelayedAction(int partLoadInfosIndex)
	{
		if (partLoadInfosIndex < 0 || partLoadInfosIndex >= this._currentPartLoadInfos.Count)
		{
			return;
		}
		HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = this._currentPartLoadInfos[partLoadInfosIndex];
		if (cosmeticPartLoadInfo.loadOp.Status != AsyncOperationStatus.Failed)
		{
			return;
		}
		cosmeticPartLoadInfo.loadOp.Completed += this._HandleLoadOpOnCompleted;
		cosmeticPartLoadInfo.loadOp = cosmeticPartLoadInfo.prefabAssetRef.InstantiateAsync(base.transform, false);
		this._loadOp_to_partInfoIndex[cosmeticPartLoadInfo.loadOp] = partLoadInfosIndex;
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x000760D8 File Offset: 0x000742D8
	protected void _ClearCurrent()
	{
		for (int i = 0; i < this._currentPartLoadInfos.Count; i++)
		{
			Object.Destroy(this._currentPartLoadInfos[i].loadOp.Result);
		}
		this._EnsureCapacityAndClear<AsyncOperationHandle, int>(this._loadOp_to_partInfoIndex);
		this._EnsureCapacityAndClear<HeadModel._CosmeticPartLoadInfo>(this._currentPartLoadInfos);
		this._mannequinRenderer.enabled = true;
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x0007613D File Offset: 0x0007433D
	private void _EnsureCapacityAndClear<T>(List<T> list)
	{
		if (list.Count > list.Capacity)
		{
			list.Capacity = list.Count;
		}
		list.Clear();
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x0007615F File Offset: 0x0007435F
	private void _EnsureCapacityAndClear<T1, T2>(Dictionary<T1, T2> dict)
	{
		dict.EnsureCapacity(dict.Count);
		dict.Clear();
	}

	// Token: 0x04001B2C RID: 6956
	[DebugReadout]
	protected readonly List<HeadModel._CosmeticPartLoadInfo> _currentPartLoadInfos = new List<HeadModel._CosmeticPartLoadInfo>(1);

	// Token: 0x04001B2D RID: 6957
	[DebugReadout]
	private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

	// Token: 0x04001B2E RID: 6958
	private Renderer _mannequinRenderer;

	// Token: 0x04001B2F RID: 6959
	public GameObject[] cosmetics;

	// Token: 0x020003F5 RID: 1013
	protected struct _CosmeticPartLoadInfo
	{
		// Token: 0x04001B30 RID: 6960
		public string playFabId;

		// Token: 0x04001B31 RID: 6961
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x04001B32 RID: 6962
		public CosmeticAttachInfo attachInfo;

		// Token: 0x04001B33 RID: 6963
		public AsyncOperationHandle<GameObject> loadOp;

		// Token: 0x04001B34 RID: 6964
		public Transform xform;
	}
}
