using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009C1 RID: 2497
public class CatmullRomSpline : MonoBehaviour
{
	// Token: 0x06003BC1 RID: 15297 RVA: 0x0011DAF8 File Offset: 0x0011BCF8
	private void RefreshControlPoints()
	{
		this.controlPoints.Clear();
		this.controlPointsTransformationMatricies.Clear();
		for (int i = 0; i < this.controlPointTransforms.Length; i++)
		{
			this.controlPoints.Add(this.controlPointTransforms[i].position);
			this.controlPointsTransformationMatricies.Add(this.controlPointTransforms[i].localToWorldMatrix);
		}
	}

	// Token: 0x06003BC2 RID: 15298 RVA: 0x0011DB5E File Offset: 0x0011BD5E
	private void Awake()
	{
		this.RefreshControlPoints();
	}

	// Token: 0x06003BC3 RID: 15299 RVA: 0x0011DB68 File Offset: 0x0011BD68
	public static Vector3 Evaluate(List<Vector3> controlPoints, float t)
	{
		if (controlPoints.Count < 1)
		{
			return Vector3.zero;
		}
		if (controlPoints.Count < 2)
		{
			return controlPoints[0];
		}
		if (controlPoints.Count < 3)
		{
			return Vector3.Lerp(controlPoints[0], controlPoints[1], t);
		}
		if (controlPoints.Count >= 4)
		{
			float num = t * (float)(controlPoints.Count - 3);
			int num2 = Mathf.FloorToInt(num);
			float num3 = num - (float)num2;
			int num4 = num2;
			if (num4 >= controlPoints.Count - 3)
			{
				num4 = controlPoints.Count - 4;
				num3 = 1f;
			}
			return CatmullRomSpline.CatmullRom(num3, controlPoints[num4], controlPoints[num4 + 1], controlPoints[num4 + 2], controlPoints[num4 + 3]);
		}
		if (t < 0.5f)
		{
			return Vector3.Lerp(controlPoints[0], controlPoints[1], t * 2f);
		}
		return Vector3.Lerp(controlPoints[1], controlPoints[2], (t - 0.5f) * 2f);
	}

	// Token: 0x06003BC4 RID: 15300 RVA: 0x0011DC5A File Offset: 0x0011BE5A
	public Vector3 Evaluate(float t)
	{
		return CatmullRomSpline.Evaluate(this.controlPoints, t);
	}

	// Token: 0x06003BC5 RID: 15301 RVA: 0x0011DC68 File Offset: 0x0011BE68
	public static float GetClosestEvaluationOnSpline(List<Vector3> controlPoints, Vector3 worldPoint, out Vector3 linePoint)
	{
		float num = float.MaxValue;
		float num2 = 0f;
		int num3 = 0;
		linePoint = worldPoint;
		for (int i = 1; i < controlPoints.Count - 2; i++)
		{
			Vector3 vector = controlPoints[i];
			Vector3 vector2 = controlPoints[i + 1];
			Vector3 vector3 = vector2 - vector;
			float magnitude = vector3.magnitude;
			if ((double)magnitude > 1E-05)
			{
				Vector3 vector4 = vector3 / magnitude;
				float num4 = Vector3.Dot(worldPoint - vector, vector4);
				float num5;
				float num6;
				Vector3 vector5;
				if (num4 <= 0f)
				{
					num5 = (worldPoint - vector).sqrMagnitude;
					num6 = 0f;
					vector5 = vector;
				}
				else if (num4 >= magnitude)
				{
					num5 = (worldPoint - vector2).sqrMagnitude;
					num6 = 1f;
					vector5 = vector2;
				}
				else
				{
					num5 = (worldPoint - (vector + vector4 * num4)).sqrMagnitude;
					num6 = num4 / magnitude;
					vector5 = vector + vector4 * num4;
				}
				if (num5 < num)
				{
					num = num5;
					num2 = num6;
					num3 = i;
					linePoint = vector5;
				}
			}
		}
		return Mathf.Clamp01(((float)(num3 - 1) + num2) / (float)(controlPoints.Count - 3));
	}

	// Token: 0x06003BC6 RID: 15302 RVA: 0x0011DDAB File Offset: 0x0011BFAB
	public float GetClosestEvaluationOnSpline(Vector3 worldPoint, out Vector3 linePoint)
	{
		return CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPoints, worldPoint, out linePoint);
	}

	// Token: 0x06003BC7 RID: 15303 RVA: 0x0011DDBC File Offset: 0x0011BFBC
	public static Vector3 GetForwardTangent(List<Vector3> controlPoints, float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 vector = CatmullRomSpline.Evaluate(controlPoints, t);
		return (CatmullRomSpline.Evaluate(controlPoints, t + step) - vector).normalized;
	}

	// Token: 0x06003BC8 RID: 15304 RVA: 0x0011DE04 File Offset: 0x0011C004
	public Vector3 GetForwardTangent(float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 vector = this.Evaluate(t);
		return (this.Evaluate(t + step) - vector).normalized;
	}

	// Token: 0x06003BC9 RID: 15305 RVA: 0x0011DE4C File Offset: 0x0011C04C
	private static Vector3 CatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 vector = 2f * p1;
		Vector3 vector2 = p2 - p0;
		Vector3 vector3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 vector4 = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (vector + vector2 * t + vector3 * t * t + vector4 * t * t * t);
	}

	// Token: 0x06003BCA RID: 15306 RVA: 0x0011DF10 File Offset: 0x0011C110
	private void OnDrawGizmosSelected()
	{
		if (this.testFloat > 0f)
		{
			Vector3 vector = this.Evaluate(this.testFloat);
			Matrix4x4 matrix4x = CatmullRomSpline.Evaluate(this.controlPointsTransformationMatricies, this.testFloat);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.up * 0.2f);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.forward * 0.2f);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(vector, matrix4x.rotation * Vector3.right * 0.2f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(vector, 0.01f);
			Gizmos.DrawRay(vector, this.GetForwardTangent(this.testFloat, 0.01f));
		}
		this.RefreshControlPoints();
		Gizmos.color = Color.yellow;
		int num = 128;
		Vector3 vector2 = this.Evaluate(0f);
		for (int i = 1; i <= num; i++)
		{
			float num2 = (float)i / (float)num;
			Vector3 vector3 = this.Evaluate(num2);
			Gizmos.DrawLine(vector2, vector3);
			vector2 = vector3;
		}
		if (this.debugTransform != null)
		{
			Vector3 vector4;
			float closestEvaluationOnSpline = this.GetClosestEvaluationOnSpline(this.debugTransform.position, out vector4);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.Evaluate(closestEvaluationOnSpline), 0.2f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(vector4, 0.25f);
			if (this.controlPoints.Count > 3)
			{
				Gizmos.color = Color.green;
				vector2 = this.controlPoints[1];
				for (int j = 2; j < this.controlPoints.Count - 2; j++)
				{
					Vector3 vector5 = this.controlPoints[j];
					Gizmos.DrawLine(vector2, vector5);
					vector2 = vector5;
				}
			}
		}
	}

	// Token: 0x06003BCB RID: 15307 RVA: 0x0011E0F4 File Offset: 0x0011C2F4
	public static Matrix4x4 CatmullRom(float t, Matrix4x4 p0, Matrix4x4 p1, Matrix4x4 p2, Matrix4x4 p3)
	{
		Vector3 vector = CatmullRomSpline.CatmullRom(t, p0.GetColumn(3), p1.GetColumn(3), p2.GetColumn(3), p3.GetColumn(3));
		Quaternion quaternion = Quaternion.Slerp(p1.rotation, p2.rotation, t);
		Vector3 vector2 = Vector3.Lerp(p1.lossyScale, p2.lossyScale, t);
		return Matrix4x4.TRS(vector, quaternion, vector2);
	}

	// Token: 0x06003BCC RID: 15308 RVA: 0x0011E16C File Offset: 0x0011C36C
	public static Matrix4x4 Evaluate(List<Matrix4x4> controlPoints, float t)
	{
		if (controlPoints.Count < 1)
		{
			return Matrix4x4.identity;
		}
		if (controlPoints.Count < 2)
		{
			return controlPoints[0];
		}
		if (controlPoints.Count < 4)
		{
			return controlPoints[0];
		}
		float num = t * (float)(controlPoints.Count - 3);
		int num2 = Mathf.FloorToInt(num);
		float num3 = num - (float)num2;
		int num4 = num2;
		if (num4 >= controlPoints.Count - 3)
		{
			num4 = controlPoints.Count - 4;
			num3 = 1f;
		}
		return CatmullRomSpline.CatmullRom(num3, controlPoints[num4], controlPoints[num4 + 1], controlPoints[num4 + 2], controlPoints[num4 + 3]);
	}

	// Token: 0x0400400D RID: 16397
	public Transform[] controlPointTransforms = new Transform[0];

	// Token: 0x0400400E RID: 16398
	public Transform debugTransform;

	// Token: 0x0400400F RID: 16399
	public List<Vector3> controlPoints = new List<Vector3>();

	// Token: 0x04004010 RID: 16400
	public List<Matrix4x4> controlPointsTransformationMatricies = new List<Matrix4x4>();

	// Token: 0x04004011 RID: 16401
	public float testFloat;
}
