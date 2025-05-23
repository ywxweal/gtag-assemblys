using System;
using UnityEngine;

// Token: 0x020004AE RID: 1198
[CreateAssetMenu(fileName = "New AudioMixVarPool", menuName = "ScriptableObjects/AudioMixVarPool", order = 0)]
public class AudioMixVarPool : ScriptableObject
{
	// Token: 0x06001CE7 RID: 7399 RVA: 0x0008C284 File Offset: 0x0008A484
	public bool Rent(out AudioMixVar mixVar)
	{
		for (int i = 0; i < this._vars.Length; i++)
		{
			if (!this._vars[i].taken)
			{
				this._vars[i].taken = true;
				mixVar = this._vars[i];
				return true;
			}
		}
		mixVar = null;
		return false;
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x0008C2D4 File Offset: 0x0008A4D4
	public void Return(AudioMixVar mixVar)
	{
		if (mixVar == null)
		{
			return;
		}
		int num = this._vars.IndexOfRef(mixVar);
		if (num == -1)
		{
			return;
		}
		this._vars[num].taken = false;
	}

	// Token: 0x0400201C RID: 8220
	[SerializeField]
	private AudioMixVar[] _vars = new AudioMixVar[0];
}
