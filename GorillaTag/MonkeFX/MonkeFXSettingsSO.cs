using System;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x02000D58 RID: 3416
	[CreateAssetMenu(fileName = "MeshGenerator", menuName = "ScriptableObjects/MeshGenerator", order = 1)]
	public class MonkeFXSettingsSO : ScriptableObject
	{
		// Token: 0x0600556B RID: 21867 RVA: 0x001A0376 File Offset: 0x0019E576
		protected void Awake()
		{
			MonkeFX.Register(this);
		}

		// Token: 0x040058DA RID: 22746
		public GTDirectAssetRef<Mesh>[] sourceMeshes;

		// Token: 0x040058DB RID: 22747
		[HideInInspector]
		public Mesh combinedMesh;
	}
}
