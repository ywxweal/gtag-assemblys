using System;
using UnityEngine;

// Token: 0x020006D4 RID: 1748
public class DayNightResetForBaking : MonoBehaviour
{
	// Token: 0x06002B8F RID: 11151 RVA: 0x000D6E14 File Offset: 0x000D5014
	public void SetMaterialsForBaking()
	{
		foreach (Material material in this.dayNightManager.dayNightSupportedMaterials)
		{
			if (material != null)
			{
				material.shader = this.dayNightManager.standard;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
		foreach (Material material2 in this.dayNightManager.dayNightSupportedMaterialsCutout)
		{
			if (material2 != null)
			{
				material2.shader = this.dayNightManager.standardCutout;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials cutout in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
	}

	// Token: 0x06002B90 RID: 11152 RVA: 0x000D6EB8 File Offset: 0x000D50B8
	public void SetMaterialsForGame()
	{
		foreach (Material material in this.dayNightManager.dayNightSupportedMaterials)
		{
			if (material != null)
			{
				material.shader = this.dayNightManager.gorillaUnlit;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materials in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
		foreach (Material material2 in this.dayNightManager.dayNightSupportedMaterialsCutout)
		{
			if (material2 != null)
			{
				material2.shader = this.dayNightManager.gorillaUnlitCutout;
			}
			else
			{
				Debug.LogError("a material is missing from day night supported materialsc cutout in the daynightmanager! something might have gotten deleted inappropriately, or an entry should be manually removed.", base.gameObject);
			}
		}
	}

	// Token: 0x0400318E RID: 12686
	public BetterDayNightManager dayNightManager;
}
