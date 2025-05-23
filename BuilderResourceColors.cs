using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004D8 RID: 1240
[CreateAssetMenu(fileName = "BuilderMaterialResourceColors", menuName = "Gorilla Tag/Builder/ResourceColors", order = 0)]
public class BuilderResourceColors : ScriptableObject
{
	// Token: 0x06001DFF RID: 7679 RVA: 0x00091CAC File Offset: 0x0008FEAC
	public Color GetColorForType(BuilderResourceType type)
	{
		foreach (BuilderResourceColor builderResourceColor in this.colors)
		{
			if (builderResourceColor.type == type)
			{
				return builderResourceColor.color;
			}
		}
		return Color.black;
	}

	// Token: 0x0400212D RID: 8493
	public List<BuilderResourceColor> colors;
}
