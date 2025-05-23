using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E76 RID: 3702
	public class BoingReactorFieldGPUSampler : MonoBehaviour
	{
		// Token: 0x06005C95 RID: 23701 RVA: 0x001C7F20 File Offset: 0x001C6120
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06005C96 RID: 23702 RVA: 0x001C7F28 File Offset: 0x001C6128
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06005C97 RID: 23703 RVA: 0x001C7F30 File Offset: 0x001C6130
		public void Update()
		{
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			if (this.m_fieldResourceSetId != component.GpuResourceSetId)
			{
				if (this.m_matProps == null)
				{
					this.m_matProps = new MaterialPropertyBlock();
				}
				if (component.UpdateShaderConstants(this.m_matProps, this.PositionSampleMultiplier, this.RotationSampleMultiplier))
				{
					this.m_fieldResourceSetId = component.GpuResourceSetId;
					foreach (Renderer renderer in new Renderer[]
					{
						base.GetComponent<MeshRenderer>(),
						base.GetComponent<SkinnedMeshRenderer>()
					})
					{
						if (!(renderer == null))
						{
							renderer.SetPropertyBlock(this.m_matProps);
						}
					}
				}
			}
		}

		// Token: 0x040060B1 RID: 24753
		public BoingReactorField ReactorField;

		// Token: 0x040060B2 RID: 24754
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x040060B3 RID: 24755
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x040060B4 RID: 24756
		private MaterialPropertyBlock m_matProps;

		// Token: 0x040060B5 RID: 24757
		private int m_fieldResourceSetId = -1;
	}
}
