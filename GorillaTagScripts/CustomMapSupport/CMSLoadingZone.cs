using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000B30 RID: 2864
	public class CMSLoadingZone : MonoBehaviour
	{
		// Token: 0x06004680 RID: 18048 RVA: 0x0014F1D4 File Offset: 0x0014D3D4
		private void Start()
		{
			base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
		}

		// Token: 0x06004681 RID: 18049 RVA: 0x0014F1E8 File Offset: 0x0014D3E8
		public void SetupLoadingZone(LoadZoneSettings settings, in string[] assetBundleSceneFilePaths)
		{
			this.scenesToLoad = this.GetSceneIndexes(settings.scenesToLoad, in assetBundleSceneFilePaths);
			this.scenesToUnload = this.CleanSceneUnloadArray(settings.scenesToUnload, settings.scenesToLoad, in assetBundleSceneFilePaths);
			this.triggeredBy = TriggerSource.Body;
			base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x06004682 RID: 18050 RVA: 0x0014F260 File Offset: 0x0014D460
		private int[] GetSceneIndexes(List<string> sceneNames, in string[] assetBundleSceneFilePaths)
		{
			int[] array = new int[sceneNames.Count];
			for (int i = 0; i < sceneNames.Count; i++)
			{
				for (int j = 0; j < assetBundleSceneFilePaths.Length; j++)
				{
					if (string.Equals(sceneNames[i], this.GetSceneNameFromFilePath(assetBundleSceneFilePaths[j])))
					{
						array[i] = j;
						break;
					}
				}
			}
			return array;
		}

		// Token: 0x06004683 RID: 18051 RVA: 0x0014F2B8 File Offset: 0x0014D4B8
		private int[] CleanSceneUnloadArray(List<string> unload, List<string> load, in string[] assetBundleSceneFilePaths)
		{
			for (int i = 0; i < load.Count; i++)
			{
				if (unload.Contains(load[i]))
				{
					unload.Remove(load[i]);
				}
			}
			return this.GetSceneIndexes(unload, in assetBundleSceneFilePaths);
		}

		// Token: 0x06004684 RID: 18052 RVA: 0x0014F2FB File Offset: 0x0014D4FB
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.bodyCollider)
			{
				CustomMapManager.LoadZoneTriggered(this.scenesToLoad, this.scenesToUnload);
			}
		}

		// Token: 0x06004685 RID: 18053 RVA: 0x0014F320 File Offset: 0x0014D520
		private string GetSceneNameFromFilePath(string filePath)
		{
			string[] array = filePath.Split("/", StringSplitOptions.None);
			return array[array.Length - 1].Split(".", StringSplitOptions.None)[0];
		}

		// Token: 0x04004914 RID: 18708
		private int[] scenesToLoad;

		// Token: 0x04004915 RID: 18709
		private int[] scenesToUnload;

		// Token: 0x04004916 RID: 18710
		private TriggerSource triggeredBy = TriggerSource.HeadOrBody;
	}
}
