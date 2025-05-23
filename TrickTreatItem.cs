using System;
using UnityEngine;

// Token: 0x02000967 RID: 2407
public class TrickTreatItem : RandomComponent<MeshRenderer>
{
	// Token: 0x06003A14 RID: 14868 RVA: 0x00116CCC File Offset: 0x00114ECC
	protected override void OnNextItem(MeshRenderer item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			MeshRenderer meshRenderer = this.items[i];
			meshRenderer.enabled = meshRenderer == item;
		}
	}

	// Token: 0x06003A15 RID: 14869 RVA: 0x00116D00 File Offset: 0x00114F00
	public void Randomize()
	{
		this.NextItem();
	}
}
