using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020004AF RID: 1199
[Serializable]
public class AudioMixVar
{
	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001CEA RID: 7402 RVA: 0x0008C33C File Offset: 0x0008A53C
	// (set) Token: 0x06001CEB RID: 7403 RVA: 0x0008C38B File Offset: 0x0008A58B
	public float value
	{
		get
		{
			if (!this.group)
			{
				return 0f;
			}
			if (!this.mixer)
			{
				return 0f;
			}
			float num;
			if (!this.mixer.GetFloat(this.name, out num))
			{
				return 0f;
			}
			return num;
		}
		set
		{
			if (this.mixer)
			{
				this.mixer.SetFloat(this.name, value);
			}
		}
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x0008C3AD File Offset: 0x0008A5AD
	public void ReturnToPool()
	{
		if (this._pool != null)
		{
			this._pool.Return(this);
		}
	}

	// Token: 0x0400201D RID: 8221
	public AudioMixerGroup group;

	// Token: 0x0400201E RID: 8222
	public AudioMixer mixer;

	// Token: 0x0400201F RID: 8223
	public string name;

	// Token: 0x04002020 RID: 8224
	[NonSerialized]
	public bool taken;

	// Token: 0x04002021 RID: 8225
	[SerializeField]
	private AudioMixVarPool _pool;
}
