using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000D08 RID: 3336
	[Serializable]
	public struct XformOffset
	{
		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x060053B2 RID: 21426 RVA: 0x00196335 File Offset: 0x00194535
		// (set) Token: 0x060053B3 RID: 21427 RVA: 0x0019633D File Offset: 0x0019453D
		[Tooltip("The rotation of the cosmetic relative to the parent bone.")]
		public Quaternion rot
		{
			get
			{
				return this._rotQuat;
			}
			set
			{
				this._rotQuat = value;
			}
		}

		// Token: 0x060053B4 RID: 21428 RVA: 0x00196346 File Offset: 0x00194546
		public XformOffset(int thisIsAnUnusedDummyValue)
		{
			this.pos = Vector3.zero;
			this._rotQuat = Quaternion.identity;
			this._rotEulerAngles = Vector3.zero;
			this.scale = Vector3.one;
		}

		// Token: 0x060053B5 RID: 21429 RVA: 0x00196374 File Offset: 0x00194574
		public XformOffset(Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = scale;
		}

		// Token: 0x060053B6 RID: 21430 RVA: 0x00196398 File Offset: 0x00194598
		public XformOffset(Vector3 pos, Vector3 rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = scale;
		}

		// Token: 0x060053B7 RID: 21431 RVA: 0x001963BB File Offset: 0x001945BB
		public XformOffset(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = Vector3.one;
		}

		// Token: 0x060053B8 RID: 21432 RVA: 0x001963E3 File Offset: 0x001945E3
		public XformOffset(Vector3 pos, Vector3 rot)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = Vector3.one;
		}

		// Token: 0x060053B9 RID: 21433 RVA: 0x0019640C File Offset: 0x0019460C
		public XformOffset(Transform boneXform, Transform cosmeticXform)
		{
			this.pos = boneXform.InverseTransformPoint(cosmeticXform.position);
			this._rotQuat = Quaternion.Inverse(boneXform.rotation) * cosmeticXform.rotation;
			this._rotEulerAngles = this._rotQuat.eulerAngles;
			Vector3 lossyScale = boneXform.lossyScale;
			Vector3 lossyScale2 = cosmeticXform.lossyScale;
			this.scale = new Vector3(lossyScale2.x / lossyScale.x, lossyScale2.y / lossyScale.y, lossyScale2.z / lossyScale.z);
		}

		// Token: 0x060053BA RID: 21434 RVA: 0x00196498 File Offset: 0x00194698
		public XformOffset(Matrix4x4 matrix)
		{
			this.pos = matrix.GetPosition();
			this.scale = matrix.lossyScale;
			if (Vector3.Dot(Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)), matrix.GetColumn(2)) < 0f)
			{
				this.scale = -this.scale;
			}
			Matrix4x4 matrix4x = matrix;
			matrix4x.SetColumn(0, matrix4x.GetColumn(0) / this.scale.x);
			matrix4x.SetColumn(1, matrix4x.GetColumn(1) / this.scale.y);
			matrix4x.SetColumn(2, matrix4x.GetColumn(2) / this.scale.z);
			this._rotQuat = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
			this._rotEulerAngles = this._rotQuat.eulerAngles;
		}

		// Token: 0x060053BB RID: 21435 RVA: 0x001965A0 File Offset: 0x001947A0
		public bool Approx(XformOffset other)
		{
			return this.pos.Approx(other.pos, 1E-05f) && this._rotQuat.Approx(other._rotQuat, 1E-06f) && this.scale.Approx(other.scale, 1E-05f);
		}

		// Token: 0x040056CB RID: 22219
		[Tooltip("The position of the cosmetic relative to the parent bone.")]
		public Vector3 pos;

		// Token: 0x040056CC RID: 22220
		[FormerlySerializedAs("_edRotQuat")]
		[FormerlySerializedAs("rot")]
		[HideInInspector]
		[SerializeField]
		private Quaternion _rotQuat;

		// Token: 0x040056CD RID: 22221
		[FormerlySerializedAs("_edRotEulerAngles")]
		[FormerlySerializedAs("_edRotEuler")]
		[HideInInspector]
		[SerializeField]
		private Vector3 _rotEulerAngles;

		// Token: 0x040056CE RID: 22222
		[Tooltip("The scale of the cosmetic relative to the parent bone.")]
		public Vector3 scale;

		// Token: 0x040056CF RID: 22223
		public static readonly XformOffset Identity = new XformOffset
		{
			_rotQuat = Quaternion.identity,
			scale = Vector3.one
		};
	}
}
