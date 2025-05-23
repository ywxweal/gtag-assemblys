using System;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class CosmeticRefRegistry : MonoBehaviour
{
	// Token: 0x06000901 RID: 2305 RVA: 0x00030C2C File Offset: 0x0002EE2C
	private void Awake()
	{
		foreach (CosmeticRefTarget cosmeticRefTarget in this.builtInRefTargets)
		{
			this.Register(cosmeticRefTarget.id, cosmeticRefTarget.gameObject);
		}
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00030C64 File Offset: 0x0002EE64
	public void Register(CosmeticRefID partID, GameObject part)
	{
		this.partsTable[(int)partID] = part;
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00030C6F File Offset: 0x0002EE6F
	public GameObject Get(CosmeticRefID partID)
	{
		return this.partsTable[(int)partID];
	}

	// Token: 0x04000AB1 RID: 2737
	private GameObject[] partsTable = new GameObject[6];

	// Token: 0x04000AB2 RID: 2738
	[SerializeField]
	private CosmeticRefTarget[] builtInRefTargets;
}
