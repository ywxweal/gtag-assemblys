using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E75 RID: 3701
	public class BoingReactorFieldCPUSampler : MonoBehaviour
	{
		// Token: 0x06005C8F RID: 23695 RVA: 0x001C7D2E File Offset: 0x001C5F2E
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06005C90 RID: 23696 RVA: 0x001C7D36 File Offset: 0x001C5F36
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06005C91 RID: 23697 RVA: 0x001C7D40 File Offset: 0x001C5F40
		public void SampleFromField()
		{
			this.m_objPosition = base.transform.position;
			this.m_objRotation = base.transform.rotation;
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				return;
			}
			Vector3 vector;
			Vector4 vector2;
			if (!component.SampleCpuGrid(base.transform.position, out vector, out vector2))
			{
				return;
			}
			base.transform.position = this.m_objPosition + vector * this.PositionSampleMultiplier;
			base.transform.rotation = QuaternionUtil.Pow(QuaternionUtil.FromVector4(vector2, true), this.RotationSampleMultiplier) * this.m_objRotation;
		}

		// Token: 0x06005C92 RID: 23698 RVA: 0x001C7DFF File Offset: 0x001C5FFF
		public void Restore()
		{
			base.transform.position = this.m_objPosition;
			base.transform.rotation = this.m_objRotation;
		}

		// Token: 0x040060AA RID: 24746
		public BoingReactorField ReactorField;

		// Token: 0x040060AB RID: 24747
		[Tooltip("Match this mode with how you update your object's transform.\n\nUpdate - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\nFixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates.")]
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x040060AC RID: 24748
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x040060AD RID: 24749
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x040060AE RID: 24750
		private Vector3 m_objPosition;

		// Token: 0x040060AF RID: 24751
		private Quaternion m_objRotation;
	}
}
