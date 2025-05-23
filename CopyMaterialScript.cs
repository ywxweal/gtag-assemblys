using System;
using UnityEngine;

// Token: 0x02000922 RID: 2338
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x060038F7 RID: 14583 RVA: 0x00112972 File Offset: 0x00110B72
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060038F8 RID: 14584 RVA: 0x00112989 File Offset: 0x00110B89
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x04003E2C RID: 15916
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x04003E2D RID: 15917
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
