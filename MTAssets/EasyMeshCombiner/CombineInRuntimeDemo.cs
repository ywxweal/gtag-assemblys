using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000C9E RID: 3230
	public class CombineInRuntimeDemo : MonoBehaviour
	{
		// Token: 0x06005017 RID: 20503 RVA: 0x0017D858 File Offset: 0x0017BA58
		private void Update()
		{
			if (!this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(true);
				this.undoButton.SetActive(false);
			}
			if (this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(false);
				this.undoButton.SetActive(true);
			}
		}

		// Token: 0x06005018 RID: 20504 RVA: 0x0017D8AF File Offset: 0x0017BAAF
		public void CombineMeshes()
		{
			this.runtimeCombiner.CombineMeshes();
		}

		// Token: 0x06005019 RID: 20505 RVA: 0x0017D8BD File Offset: 0x0017BABD
		public void UndoMerge()
		{
			this.runtimeCombiner.UndoMerge();
		}

		// Token: 0x0400530A RID: 21258
		public GameObject combineButton;

		// Token: 0x0400530B RID: 21259
		public GameObject undoButton;

		// Token: 0x0400530C RID: 21260
		public RuntimeMeshCombiner runtimeCombiner;
	}
}
