using System;
using UnityEngine;

// Token: 0x02000922 RID: 2338
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x060038F6 RID: 14582 RVA: 0x0011289A File Offset: 0x00110A9A
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060038F7 RID: 14583 RVA: 0x001128B1 File Offset: 0x00110AB1
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x04003E2B RID: 15915
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x04003E2C RID: 15916
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
