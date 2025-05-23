using System;
using UnityEngine;

// Token: 0x02000967 RID: 2407
public class TrickTreatItem : RandomComponent<MeshRenderer>
{
	// Token: 0x06003A13 RID: 14867 RVA: 0x00116BF4 File Offset: 0x00114DF4
	protected override void OnNextItem(MeshRenderer item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			MeshRenderer meshRenderer = this.items[i];
			meshRenderer.enabled = meshRenderer == item;
		}
	}

	// Token: 0x06003A14 RID: 14868 RVA: 0x00116C28 File Offset: 0x00114E28
	public void Randomize()
	{
		this.NextItem();
	}
}
