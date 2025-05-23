using System;
using UnityEngine;

// Token: 0x02000620 RID: 1568
public class GorillaLightmapData : MonoBehaviour
{
	// Token: 0x060026DF RID: 9951 RVA: 0x000C1140 File Offset: 0x000BF340
	public void Awake()
	{
		this.lights = new Color[this.lightTextures.Length][];
		this.dirs = new Color[this.dirTextures.Length][];
		for (int i = 0; i < this.dirTextures.Length; i++)
		{
			float value = Random.value;
			Debug.Log(value.ToString() + " before load " + Time.realtimeSinceStartup.ToString());
			this.dirs[i] = this.dirTextures[i].GetPixels();
			this.lights[i] = this.lightTextures[i].GetPixels();
			Debug.Log(value.ToString() + " after load " + Time.realtimeSinceStartup.ToString());
		}
	}

	// Token: 0x04002B5E RID: 11102
	[SerializeField]
	public Texture2D[] dirTextures;

	// Token: 0x04002B5F RID: 11103
	[SerializeField]
	public Texture2D[] lightTextures;

	// Token: 0x04002B60 RID: 11104
	public Color[][] lights;

	// Token: 0x04002B61 RID: 11105
	public Color[][] dirs;

	// Token: 0x04002B62 RID: 11106
	public bool done;
}
