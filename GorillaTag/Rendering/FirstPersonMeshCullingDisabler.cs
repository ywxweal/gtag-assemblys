using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000D9A RID: 3482
	public class FirstPersonMeshCullingDisabler : MonoBehaviour
	{
		// Token: 0x0600566A RID: 22122 RVA: 0x001A4D80 File Offset: 0x001A2F80
		protected void Awake()
		{
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
			if (componentsInChildren == null)
			{
				return;
			}
			this.meshes = new Mesh[componentsInChildren.Length];
			this.xforms = new Transform[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.meshes[i] = componentsInChildren[i].mesh;
				this.xforms[i] = componentsInChildren[i].transform;
			}
		}

		// Token: 0x0600566B RID: 22123 RVA: 0x001A4DE4 File Offset: 0x001A2FE4
		protected void OnEnable()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Transform transform = main.transform;
			Vector3 position = transform.position;
			Vector3 vector = Vector3.Normalize(transform.forward);
			float nearClipPlane = main.nearClipPlane;
			float num = (main.farClipPlane - nearClipPlane) / 2f + nearClipPlane;
			Vector3 vector2 = position + vector * num;
			for (int i = 0; i < this.meshes.Length; i++)
			{
				Vector3 vector3 = this.xforms[i].InverseTransformPoint(vector2);
				this.meshes[i].bounds = new Bounds(vector3, Vector3.one);
			}
		}

		// Token: 0x04005A33 RID: 23091
		private Mesh[] meshes;

		// Token: 0x04005A34 RID: 23092
		private Transform[] xforms;
	}
}
