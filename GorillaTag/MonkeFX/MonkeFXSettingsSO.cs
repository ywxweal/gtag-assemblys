using System;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x02000D58 RID: 3416
	[CreateAssetMenu(fileName = "MeshGenerator", menuName = "ScriptableObjects/MeshGenerator", order = 1)]
	public class MonkeFXSettingsSO : ScriptableObject
	{
		// Token: 0x0600556C RID: 21868 RVA: 0x001A044E File Offset: 0x0019E64E
		protected void Awake()
		{
			MonkeFX.Register(this);
		}

		// Token: 0x040058DB RID: 22747
		public GTDirectAssetRef<Mesh>[] sourceMeshes;

		// Token: 0x040058DC RID: 22748
		[HideInInspector]
		public Mesh combinedMesh;
	}
}
