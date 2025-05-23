using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
public class GRUIScoreboard : MonoBehaviour
{
	// Token: 0x0600244D RID: 9293 RVA: 0x000B6800 File Offset: 0x000B4A00
	public void Refresh(List<VRRig> vrRigs)
	{
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (!(this.entries[i] == null))
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.entries[i].gameObject.SetActive(true);
					this.entries[i].Setup(vrRigs[i], vrRigs[i].OwningNetPlayer.ActorNumber);
				}
				else
				{
					this.entries[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x04002967 RID: 10599
	public List<GRUIScoreboardEntry> entries;
}
