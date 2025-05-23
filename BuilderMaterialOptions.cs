using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004ED RID: 1261
[CreateAssetMenu(fileName = "BuilderMaterialOptions01a", menuName = "Gorilla Tag/Builder/Options", order = 0)]
public class BuilderMaterialOptions : ScriptableObject
{
	// Token: 0x06001E8F RID: 7823 RVA: 0x0009555C File Offset: 0x0009375C
	public void GetMaterialFromType(int materialType, out Material material, out int soundIndex)
	{
		if (this.options == null)
		{
			material = null;
			soundIndex = -1;
			return;
		}
		foreach (BuilderMaterialOptions.Options options in this.options)
		{
			if (options.materialId.GetHashCode() == materialType)
			{
				material = options.material;
				soundIndex = options.soundIndex;
				return;
			}
		}
		material = null;
		soundIndex = -1;
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x000955E0 File Offset: 0x000937E0
	public void GetDefaultMaterial(out int materialType, out Material material, out int soundIndex)
	{
		if (this.options.Count > 0)
		{
			materialType = this.options[0].materialId.GetHashCode();
			material = this.options[0].material;
			soundIndex = this.options[0].soundIndex;
			return;
		}
		materialType = -1;
		material = null;
		soundIndex = -1;
	}

	// Token: 0x040021E5 RID: 8677
	public List<BuilderMaterialOptions.Options> options;

	// Token: 0x020004EE RID: 1262
	[Serializable]
	public class Options
	{
		// Token: 0x040021E6 RID: 8678
		public string materialId;

		// Token: 0x040021E7 RID: 8679
		public Material material;

		// Token: 0x040021E8 RID: 8680
		[GorillaSoundLookup]
		public int soundIndex;

		// Token: 0x040021E9 RID: 8681
		[NonSerialized]
		public int materialType;
	}
}
