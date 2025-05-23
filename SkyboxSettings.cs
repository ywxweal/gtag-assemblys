using System;
using UnityEngine;

// Token: 0x02000217 RID: 535
[ExecuteInEditMode]
public class SkyboxSettings : MonoBehaviour
{
	// Token: 0x06000C81 RID: 3201 RVA: 0x000418C6 File Offset: 0x0003FAC6
	private void OnEnable()
	{
		if (this._skyMaterial)
		{
			RenderSettings.skybox = this._skyMaterial;
		}
	}

	// Token: 0x04000F0C RID: 3852
	[SerializeField]
	private Material _skyMaterial;
}
