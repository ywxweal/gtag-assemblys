using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000E2C RID: 3628
	[CreateAssetMenu(fileName = "New Mesh Material Replacement", menuName = "Game Object Scheduling/New Mesh Material Replacement", order = 1)]
	public class MeshMaterialReplacement : ScriptableObject
	{
		// Token: 0x04005EC9 RID: 24265
		public Mesh mesh;

		// Token: 0x04005ECA RID: 24266
		public Material[] materials;
	}
}
