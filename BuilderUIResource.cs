using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x02000520 RID: 1312
public class BuilderUIResource : MonoBehaviour
{
	// Token: 0x06001FC3 RID: 8131 RVA: 0x000A0750 File Offset: 0x0009E950
	public void SetResourceCost(BuilderResourceQuantity resourceCost, BuilderTable table)
	{
		BuilderResourceType type = resourceCost.type;
		int count = resourceCost.count;
		int availableResources = table.GetAvailableResources(type);
		if (this.resourceNameLabel != null)
		{
			this.resourceNameLabel.text = this.GetResourceName(type);
		}
		if (this.costLabel != null)
		{
			this.costLabel.text = count.ToString();
		}
		if (this.availableLabel != null)
		{
			this.availableLabel.text = availableResources.ToString();
		}
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000A07D3 File Offset: 0x0009E9D3
	private string GetResourceName(BuilderResourceType type)
	{
		switch (type)
		{
		case BuilderResourceType.Basic:
			return "Basic";
		case BuilderResourceType.Decorative:
			return "Decorative";
		case BuilderResourceType.Functional:
			return "Functional";
		default:
			return "Resource Needs Name";
		}
	}

	// Token: 0x040023AA RID: 9130
	public TextMeshPro resourceNameLabel;

	// Token: 0x040023AB RID: 9131
	public TextMeshPro costLabel;

	// Token: 0x040023AC RID: 9132
	public TextMeshPro availableLabel;
}
