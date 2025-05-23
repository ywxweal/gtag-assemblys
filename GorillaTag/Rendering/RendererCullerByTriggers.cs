using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000D99 RID: 3481
	public class RendererCullerByTriggers : MonoBehaviour, IBuildValidation
	{
		// Token: 0x06005666 RID: 22118 RVA: 0x001A4BEC File Offset: 0x001A2DEC
		protected void OnEnable()
		{
			this.camWasTouching = false;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
		}

		// Token: 0x06005667 RID: 22119 RVA: 0x001A4C48 File Offset: 0x001A2E48
		protected void LateUpdate()
		{
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
			Vector3 position = this.mainCameraTransform.position;
			bool flag = false;
			foreach (Collider collider in this.colliders)
			{
				if (!(collider == null) && (collider.ClosestPoint(position) - position).sqrMagnitude < 0.010000001f)
				{
					flag = true;
					break;
				}
			}
			if (this.camWasTouching == flag)
			{
				return;
			}
			this.camWasTouching = flag;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = flag;
				}
			}
		}

		// Token: 0x06005668 RID: 22120 RVA: 0x001A4D08 File Offset: 0x001A2F08
		public bool BuildValidationCheck()
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				if (this.renderers[i] == null)
				{
					Debug.LogError("rendererculllerbytriggers has null renderer", base.gameObject);
					return false;
				}
			}
			for (int j = 0; j < this.colliders.Length; j++)
			{
				if (this.colliders[j] == null)
				{
					Debug.LogError("rendererculllerbytriggers has null collider", base.gameObject);
					return false;
				}
			}
			return true;
		}

		// Token: 0x04005A2E RID: 23086
		[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
		public Renderer[] renderers;

		// Token: 0x04005A2F RID: 23087
		public Collider[] colliders;

		// Token: 0x04005A30 RID: 23088
		private bool camWasTouching;

		// Token: 0x04005A31 RID: 23089
		private const float cameraRadiusSq = 0.010000001f;

		// Token: 0x04005A32 RID: 23090
		private Transform mainCameraTransform;
	}
}
