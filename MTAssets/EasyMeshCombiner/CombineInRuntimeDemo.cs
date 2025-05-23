using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000C9E RID: 3230
	public class CombineInRuntimeDemo : MonoBehaviour
	{
		// Token: 0x06005018 RID: 20504 RVA: 0x0017D930 File Offset: 0x0017BB30
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

		// Token: 0x06005019 RID: 20505 RVA: 0x0017D987 File Offset: 0x0017BB87
		public void CombineMeshes()
		{
			this.runtimeCombiner.CombineMeshes();
		}

		// Token: 0x0600501A RID: 20506 RVA: 0x0017D995 File Offset: 0x0017BB95
		public void UndoMerge()
		{
			this.runtimeCombiner.UndoMerge();
		}

		// Token: 0x0400530B RID: 21259
		public GameObject combineButton;

		// Token: 0x0400530C RID: 21260
		public GameObject undoButton;

		// Token: 0x0400530D RID: 21261
		public RuntimeMeshCombiner runtimeCombiner;
	}
}
