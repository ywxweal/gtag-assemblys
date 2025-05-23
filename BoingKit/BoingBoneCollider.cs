using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E4E RID: 3662
	public class BoingBoneCollider : MonoBehaviour
	{
		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06005BB8 RID: 23480 RVA: 0x001C2AAC File Offset: 0x001C0CAC
		public Bounds Bounds
		{
			get
			{
				switch (this.Shape)
				{
				case BoingBoneCollider.Type.Sphere:
				{
					float num = VectorUtil.MinComponent(base.transform.localScale);
					return new Bounds(base.transform.position, 2f * num * this.Radius * Vector3.one);
				}
				case BoingBoneCollider.Type.Capsule:
				{
					float num2 = VectorUtil.MinComponent(base.transform.localScale);
					return new Bounds(base.transform.position, 2f * num2 * this.Radius * Vector3.one + this.Height * VectorUtil.ComponentWiseAbs(base.transform.rotation * Vector3.up));
				}
				case BoingBoneCollider.Type.Box:
					return new Bounds(base.transform.position, VectorUtil.ComponentWiseMult(base.transform.localScale, VectorUtil.ComponentWiseAbs(base.transform.rotation * this.Dimensions)));
				default:
					return default(Bounds);
				}
			}
		}

		// Token: 0x06005BB9 RID: 23481 RVA: 0x001C2BBC File Offset: 0x001C0DBC
		public bool Collide(Vector3 boneCenter, float boneRadius, out Vector3 push)
		{
			switch (this.Shape)
			{
			case BoingBoneCollider.Type.Sphere:
			{
				float num = VectorUtil.MinComponent(base.transform.localScale);
				return Collision.SphereSphere(boneCenter, boneRadius, base.transform.position, num * this.Radius, out push);
			}
			case BoingBoneCollider.Type.Capsule:
			{
				float num2 = VectorUtil.MinComponent(base.transform.localScale);
				Vector3 vector = base.transform.TransformPoint(0.5f * this.Height * Vector3.up);
				Vector3 vector2 = base.transform.TransformPoint(0.5f * this.Height * Vector3.down);
				return Collision.SphereCapsule(boneCenter, boneRadius, vector, vector2, num2 * this.Radius, out push);
			}
			case BoingBoneCollider.Type.Box:
			{
				Vector3 vector3 = base.transform.InverseTransformPoint(boneCenter);
				Vector3 vector4 = 0.5f * VectorUtil.ComponentWiseMult(base.transform.localScale, this.Dimensions);
				if (!Collision.SphereBox(vector3, boneRadius, vector4, out push))
				{
					return false;
				}
				push = base.transform.TransformVector(push);
				return true;
			}
			default:
				push = Vector3.zero;
				return false;
			}
		}

		// Token: 0x06005BBA RID: 23482 RVA: 0x001C2CE0 File Offset: 0x001C0EE0
		public void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.Dimensions.x = Mathf.Max(0f, this.Dimensions.x);
			this.Dimensions.y = Mathf.Max(0f, this.Dimensions.y);
			this.Dimensions.z = Mathf.Max(0f, this.Dimensions.z);
		}

		// Token: 0x06005BBB RID: 23483 RVA: 0x001C2D63 File Offset: 0x001C0F63
		public void OnDrawGizmos()
		{
			this.DrawGizmos();
		}

		// Token: 0x06005BBC RID: 23484 RVA: 0x001C2D6C File Offset: 0x001C0F6C
		public void DrawGizmos()
		{
			switch (this.Shape)
			{
			case BoingBoneCollider.Type.Sphere:
			{
				float num = VectorUtil.MinComponent(base.transform.localScale) * this.Radius;
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
				if (this.Shape == BoingBoneCollider.Type.Sphere)
				{
					Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
					Gizmos.DrawSphere(Vector3.zero, num);
				}
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(Vector3.zero, num);
				Gizmos.matrix = Matrix4x4.identity;
				return;
			}
			case BoingBoneCollider.Type.Capsule:
			{
				float num2 = VectorUtil.MinComponent(base.transform.localScale);
				float num3 = num2 * this.Radius;
				float num4 = 0.5f * num2 * this.Height;
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
				if (this.Shape == BoingBoneCollider.Type.Capsule)
				{
					Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
					Gizmos.DrawSphere(num4 * Vector3.up, num3);
					Gizmos.DrawSphere(num4 * Vector3.down, num3);
				}
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(num4 * Vector3.up, num3);
				Gizmos.DrawWireSphere(num4 * Vector3.down, num3);
				for (int i = 0; i < 4; i++)
				{
					float num5 = (float)i * MathUtil.HalfPi;
					Vector3 vector = new Vector3(num3 * Mathf.Cos(num5), 0f, num3 * Mathf.Sin(num5));
					Gizmos.DrawLine(vector + num4 * Vector3.up, vector + num4 * Vector3.down);
				}
				Gizmos.matrix = Matrix4x4.identity;
				return;
			}
			case BoingBoneCollider.Type.Box:
			{
				Vector3 vector2 = VectorUtil.ComponentWiseMult(base.transform.localScale, this.Dimensions);
				Gizmos.matrix = base.transform.localToWorldMatrix;
				if (this.Shape == BoingBoneCollider.Type.Box)
				{
					Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
					Gizmos.DrawCube(Vector3.zero, vector2);
				}
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(Vector3.zero, vector2);
				Gizmos.matrix = Matrix4x4.identity;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x04005F9D RID: 24477
		public BoingBoneCollider.Type Shape;

		// Token: 0x04005F9E RID: 24478
		public float Radius = 0.1f;

		// Token: 0x04005F9F RID: 24479
		public float Height = 0.25f;

		// Token: 0x04005FA0 RID: 24480
		public Vector3 Dimensions = new Vector3(0.1f, 0.1f, 0.1f);

		// Token: 0x02000E4F RID: 3663
		public enum Type
		{
			// Token: 0x04005FA2 RID: 24482
			Sphere,
			// Token: 0x04005FA3 RID: 24483
			Capsule,
			// Token: 0x04005FA4 RID: 24484
			Box
		}
	}
}
