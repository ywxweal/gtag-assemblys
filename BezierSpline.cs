using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020009C0 RID: 2496
public class BezierSpline : MonoBehaviour
{
	// Token: 0x06003BAA RID: 15274 RVA: 0x0011D130 File Offset: 0x0011B330
	private void Awake()
	{
		float num = 0f;
		for (int i = 1; i < this.points.Length; i++)
		{
			num += (this.points[i] - this.points[i - 1]).magnitude;
		}
		int num2 = Mathf.RoundToInt(num / 0.1f);
		this.buildTimesLenghtsTables(num2);
	}

	// Token: 0x06003BAB RID: 15275 RVA: 0x0011D194 File Offset: 0x0011B394
	private void buildTimesLenghtsTables(int subdivisions)
	{
		this._totalArcLength = 0f;
		float num = 1f / (float)subdivisions;
		this._timesTable = new float[subdivisions];
		this._lengthsTable = new float[subdivisions];
		Vector3 vector = this.GetPoint(0f);
		for (int i = 1; i <= subdivisions; i++)
		{
			float num2 = num * (float)i;
			Vector3 point = this.GetPoint(num2);
			this._totalArcLength += Vector3.Distance(point, vector);
			vector = point;
			this._timesTable[i - 1] = num2;
			this._lengthsTable[i - 1] = this._totalArcLength;
		}
	}

	// Token: 0x06003BAC RID: 15276 RVA: 0x0011D22C File Offset: 0x0011B42C
	private float getPathFromTime(float t)
	{
		if (float.IsNaN(this._totalArcLength) || this._totalArcLength == 0f)
		{
			return t;
		}
		if (t > 0f && t < 1f)
		{
			float num = this._totalArcLength * t;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			int num6 = this._lengthsTable.Length;
			int i = 0;
			while (i < num6)
			{
				if (this._lengthsTable[i] > num)
				{
					num4 = this._timesTable[i];
					num5 = this._lengthsTable[i];
					if (i > 0)
					{
						num3 = this._lengthsTable[i - 1];
						break;
					}
					break;
				}
				else
				{
					num2 = this._timesTable[i];
					i++;
				}
			}
			t = num2 + (num - num3) / (num5 - num3) * (num4 - num2);
		}
		if (t > 1f)
		{
			t = 1f;
		}
		else if (t < 0f)
		{
			t = 0f;
		}
		return t;
	}

	// Token: 0x170005E0 RID: 1504
	// (get) Token: 0x06003BAD RID: 15277 RVA: 0x0011D317 File Offset: 0x0011B517
	// (set) Token: 0x06003BAE RID: 15278 RVA: 0x0011D31F File Offset: 0x0011B51F
	public bool Loop
	{
		get
		{
			return this.loop;
		}
		set
		{
			this.loop = value;
			if (value)
			{
				this.modes[this.modes.Length - 1] = this.modes[0];
				this.SetControlPoint(0, this.points[0]);
			}
		}
	}

	// Token: 0x170005E1 RID: 1505
	// (get) Token: 0x06003BAF RID: 15279 RVA: 0x0011D357 File Offset: 0x0011B557
	public int ControlPointCount
	{
		get
		{
			return this.points.Length;
		}
	}

	// Token: 0x06003BB0 RID: 15280 RVA: 0x0011D361 File Offset: 0x0011B561
	public Vector3 GetControlPoint(int index)
	{
		return this.points[index];
	}

	// Token: 0x06003BB1 RID: 15281 RVA: 0x0011D370 File Offset: 0x0011B570
	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 vector = point - this.points[index];
			if (this.loop)
			{
				if (index == 0)
				{
					this.points[1] += vector;
					this.points[this.points.Length - 2] += vector;
					this.points[this.points.Length - 1] = point;
				}
				else if (index == this.points.Length - 1)
				{
					this.points[0] = point;
					this.points[1] += vector;
					this.points[index - 1] += vector;
				}
				else
				{
					this.points[index - 1] += vector;
					this.points[index + 1] += vector;
				}
			}
			else
			{
				if (index > 0)
				{
					this.points[index - 1] += vector;
				}
				if (index + 1 < this.points.Length)
				{
					this.points[index + 1] += vector;
				}
			}
		}
		this.points[index] = point;
		this.EnforceMode(index);
	}

	// Token: 0x06003BB2 RID: 15282 RVA: 0x0011D502 File Offset: 0x0011B702
	public BezierControlPointMode GetControlPointMode(int index)
	{
		return this.modes[(index + 1) / 3];
	}

	// Token: 0x06003BB3 RID: 15283 RVA: 0x0011D510 File Offset: 0x0011B710
	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int num = (index + 1) / 3;
		this.modes[num] = mode;
		if (this.loop)
		{
			if (num == 0)
			{
				this.modes[this.modes.Length - 1] = mode;
			}
			else if (num == this.modes.Length - 1)
			{
				this.modes[0] = mode;
			}
		}
		this.EnforceMode(index);
	}

	// Token: 0x06003BB4 RID: 15284 RVA: 0x0011D568 File Offset: 0x0011B768
	private void EnforceMode(int index)
	{
		int num = (index + 1) / 3;
		BezierControlPointMode bezierControlPointMode = this.modes[num];
		if (bezierControlPointMode == BezierControlPointMode.Free || (!this.loop && (num == 0 || num == this.modes.Length - 1)))
		{
			return;
		}
		int num2 = num * 3;
		int num3;
		int num4;
		if (index <= num2)
		{
			num3 = num2 - 1;
			if (num3 < 0)
			{
				num3 = this.points.Length - 2;
			}
			num4 = num2 + 1;
			if (num4 >= this.points.Length)
			{
				num4 = 1;
			}
		}
		else
		{
			num3 = num2 + 1;
			if (num3 >= this.points.Length)
			{
				num3 = 1;
			}
			num4 = num2 - 1;
			if (num4 < 0)
			{
				num4 = this.points.Length - 2;
			}
		}
		Vector3 vector = this.points[num2];
		Vector3 vector2 = vector - this.points[num3];
		if (bezierControlPointMode == BezierControlPointMode.Aligned)
		{
			vector2 = vector2.normalized * Vector3.Distance(vector, this.points[num4]);
		}
		this.points[num4] = vector + vector2;
	}

	// Token: 0x170005E2 RID: 1506
	// (get) Token: 0x06003BB5 RID: 15285 RVA: 0x0011D657 File Offset: 0x0011B857
	public int CurveCount
	{
		get
		{
			return (this.points.Length - 1) / 3;
		}
	}

	// Token: 0x06003BB6 RID: 15286 RVA: 0x0011D665 File Offset: 0x0011B865
	public Vector3 GetPoint(float t, bool ConstantVelocity)
	{
		if (ConstantVelocity)
		{
			return this.GetPoint(this.getPathFromTime(t));
		}
		return this.GetPoint(t);
	}

	// Token: 0x06003BB7 RID: 15287 RVA: 0x0011D680 File Offset: 0x0011B880
	public Vector3 GetPoint(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t));
	}

	// Token: 0x06003BB8 RID: 15288 RVA: 0x0011D710 File Offset: 0x0011B910
	public Vector3 GetPointLocal(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t);
	}

	// Token: 0x06003BB9 RID: 15289 RVA: 0x0011D794 File Offset: 0x0011B994
	public Vector3 GetVelocity(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t)) - base.transform.position;
	}

	// Token: 0x06003BBA RID: 15290 RVA: 0x0011D831 File Offset: 0x0011BA31
	public Vector3 GetDirection(float t, bool ConstantVelocity)
	{
		if (ConstantVelocity)
		{
			return this.GetDirection(this.getPathFromTime(t));
		}
		return this.GetDirection(t);
	}

	// Token: 0x06003BBB RID: 15291 RVA: 0x0011D84C File Offset: 0x0011BA4C
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06003BBC RID: 15292 RVA: 0x0011D868 File Offset: 0x0011BA68
	public void AddCurve()
	{
		Vector3 vector = this.points[this.points.Length - 1];
		Array.Resize<Vector3>(ref this.points, this.points.Length + 3);
		vector.x += 1f;
		this.points[this.points.Length - 3] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 2] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 1] = vector;
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length + 1);
		this.modes[this.modes.Length - 1] = this.modes[this.modes.Length - 2];
		this.EnforceMode(this.points.Length - 4);
		if (this.loop)
		{
			this.points[this.points.Length - 1] = this.points[0];
			this.modes[this.modes.Length - 1] = this.modes[0];
			this.EnforceMode(0);
		}
	}

	// Token: 0x06003BBD RID: 15293 RVA: 0x0011D9A2 File Offset: 0x0011BBA2
	public void RemoveLastCurve()
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		Array.Resize<Vector3>(ref this.points, this.points.Length - 3);
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length - 1);
	}

	// Token: 0x06003BBE RID: 15294 RVA: 0x0011D9DC File Offset: 0x0011BBDC
	public void RemoveCurve(int index)
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		List<Vector3> list = this.points.ToList<Vector3>();
		int num = 4;
		while (num < this.points.Length && index - 3 > num)
		{
			num += 3;
		}
		for (int i = 0; i < 3; i++)
		{
			list.RemoveAt(num);
		}
		this.points = list.ToArray();
		int num2 = (num - 4) / 3;
		List<BezierControlPointMode> list2 = this.modes.ToList<BezierControlPointMode>();
		list2.RemoveAt(num2);
		this.modes = list2.ToArray();
	}

	// Token: 0x06003BBF RID: 15295 RVA: 0x0011DA64 File Offset: 0x0011BC64
	public void Reset()
	{
		this.points = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 2f),
			new Vector3(0f, -1f, 4f),
			new Vector3(0f, -1f, 6f)
		};
		this.modes = new BezierControlPointMode[2];
	}

	// Token: 0x04004007 RID: 16391
	[SerializeField]
	private Vector3[] points;

	// Token: 0x04004008 RID: 16392
	[SerializeField]
	private BezierControlPointMode[] modes;

	// Token: 0x04004009 RID: 16393
	[SerializeField]
	private bool loop;

	// Token: 0x0400400A RID: 16394
	private float _totalArcLength;

	// Token: 0x0400400B RID: 16395
	private float[] _timesTable;

	// Token: 0x0400400C RID: 16396
	private float[] _lengthsTable;
}
