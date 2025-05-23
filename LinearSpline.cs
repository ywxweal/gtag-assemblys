﻿using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009C3 RID: 2499
public class LinearSpline : MonoBehaviour
{
	// Token: 0x06003BCF RID: 15311 RVA: 0x0011E230 File Offset: 0x0011C430
	private void RefreshControlPoints()
	{
		this.controlPoints.Clear();
		for (int i = 0; i < this.controlPointTransforms.Length; i++)
		{
			this.controlPoints.Add(this.controlPointTransforms[i].position);
		}
		this.totalDistance = 0f;
		this.distances.Clear();
		for (int j = 1; j < this.controlPoints.Count; j++)
		{
			float num = Vector3.Distance(this.controlPoints[j - 1], this.controlPoints[j]);
			this.distances.Add(num);
			this.totalDistance += num;
		}
		float num2 = Vector3.Distance(this.controlPoints[this.controlPoints.Count - 1], this.controlPoints[0]);
		this.distances.Add(num2);
		if (this.looping)
		{
			this.totalDistance += num2;
		}
		this.curveBoundaries.Clear();
		if (this.roundCorners)
		{
			for (int k = 0; k < this.controlPoints.Count; k++)
			{
				int num3 = ((k > 0) ? (k - 1) : (this.controlPoints.Count - 1));
				int num4 = (k + 1) % this.controlPoints.Count;
				float num5 = Mathf.Min(Mathf.Min(this.cornerRadius, this.distances[num3 % this.distances.Count] * 0.5f), this.distances[k % this.distances.Count] * 0.5f);
				this.curveBoundaries.Add(new LinearSpline.CurveBoundary
				{
					start = Vector3.Lerp(this.controlPoints[num3], this.controlPoints[k], 1f - num5 / this.distances[num3 % this.distances.Count]),
					end = Vector3.Lerp(this.controlPoints[k], this.controlPoints[num4], num5 / this.distances[k])
				});
			}
		}
	}

	// Token: 0x06003BD0 RID: 15312 RVA: 0x0011E46A File Offset: 0x0011C66A
	private void Awake()
	{
		this.RefreshControlPoints();
	}

	// Token: 0x06003BD1 RID: 15313 RVA: 0x0011E474 File Offset: 0x0011C674
	public Vector3 Evaluate(float t)
	{
		if (this.controlPoints.Count < 1)
		{
			return Vector3.zero;
		}
		if (this.controlPoints.Count < 2)
		{
			return this.controlPoints[0];
		}
		if (this.controlPoints.Count < 3)
		{
			return Vector3.Lerp(this.controlPoints[0], this.controlPoints[1], t);
		}
		float num = Mathf.Clamp01(t) * this.totalDistance;
		int num2 = 0;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < this.distances.Count; i++)
		{
			if (this.looping || i != this.distances.Count - 1)
			{
				num2 = i;
				if (num - num4 <= this.distances[i])
				{
					num3 = Mathf.Clamp01((num - num4) / this.distances[i]);
					break;
				}
				num3 = 1f;
				num4 += this.distances[i];
			}
		}
		num2 %= this.controlPoints.Count;
		int num5 = (num2 + 1) % this.controlPoints.Count;
		if (this.roundCorners)
		{
			if (num3 > 0.5f && (this.looping || num2 < this.controlPoints.Count - 2))
			{
				int num6 = (num5 + 1) % this.controlPoints.Count;
				float num7 = Mathf.Min(Mathf.Min(this.cornerRadius, this.distances[num2] * 0.5f), this.distances[num5 % this.distances.Count] * 0.5f);
				float num8 = 1f - num7 / this.distances[num2];
				if (num3 > num8)
				{
					Vector3 start = this.curveBoundaries[num5].start;
					Vector3 end = this.curveBoundaries[num5].end;
					float num9 = 0.5f * Mathf.Clamp01((num3 - num8) / (1f - num8));
					Vector3 vector = Vector3.Lerp(start, this.controlPoints[num5], num9);
					Vector3 vector2 = Vector3.Lerp(this.controlPoints[num5], end, num9);
					return Vector3.Lerp(vector, vector2, num9);
				}
			}
			else if (num3 <= 0.5f && (this.looping || num2 > 0))
			{
				int num10 = ((num2 > 0) ? (num2 - 1) : (this.controlPoints.Count - 1));
				float num11 = Mathf.Min(Mathf.Min(this.cornerRadius, this.distances[num2] * 0.5f), this.distances[num10 % this.distances.Count] * 0.5f) / this.distances[num2];
				if (num3 < num11)
				{
					Vector3 start2 = this.curveBoundaries[num2].start;
					Vector3 end2 = this.curveBoundaries[num2].end;
					float num12 = 0.5f + 0.5f * Mathf.Clamp01(num3 / num11);
					Vector3 vector3 = Vector3.Lerp(start2, this.controlPoints[num2], num12);
					Vector3 vector4 = Vector3.Lerp(this.controlPoints[num2], end2, num12);
					return Vector3.Lerp(vector3, vector4, num12);
				}
			}
		}
		return Vector3.Lerp(this.controlPoints[num2], this.controlPoints[num5], num3);
	}

	// Token: 0x06003BD2 RID: 15314 RVA: 0x0011E7C0 File Offset: 0x0011C9C0
	public Vector3 GetForwardTangent(float t, float step = 0.01f)
	{
		t = Mathf.Clamp(t, 0f, 1f - step - Mathf.Epsilon);
		Vector3 vector = this.Evaluate(t);
		return (this.Evaluate(t + step) - vector).normalized;
	}

	// Token: 0x06003BD3 RID: 15315 RVA: 0x0011E808 File Offset: 0x0011CA08
	private void OnDrawGizmosSelected()
	{
		this.RefreshControlPoints();
		Gizmos.color = Color.yellow;
		int num = this.gizmoResolution;
		Vector3 vector = this.Evaluate(0f);
		for (int i = 1; i <= num; i++)
		{
			float num2 = (float)i / (float)num;
			Vector3 vector2 = this.Evaluate(num2);
			Gizmos.DrawLine(vector, vector2);
			vector = vector2;
		}
		Vector3 vector3 = this.Evaluate(1f);
		Gizmos.DrawLine(vector, vector3);
	}

	// Token: 0x04004014 RID: 16404
	public Transform[] controlPointTransforms = new Transform[0];

	// Token: 0x04004015 RID: 16405
	public Transform debugTransform;

	// Token: 0x04004016 RID: 16406
	public List<Vector3> controlPoints = new List<Vector3>();

	// Token: 0x04004017 RID: 16407
	public List<float> distances = new List<float>();

	// Token: 0x04004018 RID: 16408
	public List<LinearSpline.CurveBoundary> curveBoundaries = new List<LinearSpline.CurveBoundary>();

	// Token: 0x04004019 RID: 16409
	public bool roundCorners;

	// Token: 0x0400401A RID: 16410
	public float cornerRadius = 1f;

	// Token: 0x0400401B RID: 16411
	public bool looping;

	// Token: 0x0400401C RID: 16412
	public float testFloat;

	// Token: 0x0400401D RID: 16413
	public int gizmoResolution = 128;

	// Token: 0x0400401E RID: 16414
	public float totalDistance;

	// Token: 0x020009C4 RID: 2500
	public struct CurveBoundary
	{
		// Token: 0x0400401F RID: 16415
		public Vector3 start;

		// Token: 0x04004020 RID: 16416
		public Vector3 end;
	}
}
