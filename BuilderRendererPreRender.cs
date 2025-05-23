using System;
using UnityEngine;

// Token: 0x020004D6 RID: 1238
public class BuilderRendererPreRender : MonoBehaviour
{
	// Token: 0x06001DFC RID: 7676 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Awake()
	{
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x00091C71 File Offset: 0x0008FE71
	private void LateUpdate()
	{
		if (this.builderRenderer != null)
		{
			this.builderRenderer.PreRenderIndirect();
		}
	}

	// Token: 0x0400212A RID: 8490
	public BuilderRenderer builderRenderer;
}
