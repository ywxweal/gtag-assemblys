using System;
using UnityEngine;

// Token: 0x02000546 RID: 1350
[RequireComponent(typeof(LineRenderer))]
public class FixedSizeTrail : MonoBehaviour
{
	// Token: 0x17000349 RID: 841
	// (get) Token: 0x060020B0 RID: 8368 RVA: 0x000A40FA File Offset: 0x000A22FA
	public LineRenderer renderer
	{
		get
		{
			return this._lineRenderer;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x060020B1 RID: 8369 RVA: 0x000A4102 File Offset: 0x000A2302
	// (set) Token: 0x060020B2 RID: 8370 RVA: 0x000A410A File Offset: 0x000A230A
	public float length
	{
		get
		{
			return this._length;
		}
		set
		{
			this._length = Math.Clamp(value, 0f, 128f);
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x060020B3 RID: 8371 RVA: 0x000A4122 File Offset: 0x000A2322
	public Vector3[] points
	{
		get
		{
			return this._points;
		}
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x000A412A File Offset: 0x000A232A
	private void Reset()
	{
		this.Setup();
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x000A412A File Offset: 0x000A232A
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000A4134 File Offset: 0x000A2334
	private void Setup()
	{
		this._transform = base.transform;
		if (this._lineRenderer == null)
		{
			this._lineRenderer = base.GetComponent<LineRenderer>();
		}
		if (!this._lineRenderer)
		{
			return;
		}
		this._lineRenderer.useWorldSpace = true;
		Vector3 position = this._transform.position;
		Vector3 forward = this._transform.forward;
		int num = this._segments + 1;
		this._points = new Vector3[num];
		float num2 = this._length / (float)this._segments;
		for (int i = 0; i < num; i++)
		{
			this._points[i] = position - forward * num2 * (float)i;
		}
		this._lineRenderer.positionCount = num;
		this._lineRenderer.SetPositions(this._points);
		this.Update();
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000A4212 File Offset: 0x000A2412
	private void Update()
	{
		if (!this.manualUpdate)
		{
			this.Update(Time.deltaTime);
		}
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000A4228 File Offset: 0x000A2428
	private void FixedUpdate()
	{
		if (!this.applyPhysics)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = this._points.Length - 1;
		float num2 = this._length / (float)num;
		for (int i = 1; i < num; i++)
		{
			float num3 = (float)(i - 1) / (float)num;
			float num4 = this.gravityCurve.Evaluate(num3);
			Vector3 vector = this.gravity * (num4 * deltaTime);
			this._points[i] += vector;
			this._points[i + 1] += vector;
		}
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000A42CC File Offset: 0x000A24CC
	public void Update(float dt)
	{
		float num = this._length / (float)(this._segments - 1);
		Vector3 position = this._transform.position;
		this._points[0] = position;
		float num2 = Vector3.Distance(this._points[0], this._points[1]);
		float num3 = num - num2;
		if (num2 > num)
		{
			Array.Copy(this._points, 0, this._points, 1, this._points.Length - 1);
		}
		for (int i = 0; i < this._points.Length - 1; i++)
		{
			Vector3 vector = this._points[i];
			Vector3 vector2 = this._points[i + 1] - vector;
			if (vector2.sqrMagnitude > num * num)
			{
				this._points[i + 1] = vector + vector2.normalized * num;
			}
		}
		if (num3 > 0f)
		{
			int num4 = this._points.Length - 1;
			int num5 = num4 - 1;
			Vector3 vector3 = this._points[num4] - this._points[num5];
			Vector3 vector4 = vector3.normalized;
			if (this.applyPhysics)
			{
				Vector3 normalized = (this._points[num5] - this._points[num5 - 1]).normalized;
				vector4 = Vector3.Lerp(vector4, normalized, 0.5f);
			}
			this._points[num4] = this._points[num5] + vector4 * Math.Min(vector3.magnitude, num3);
		}
		this._lineRenderer.SetPositions(this._points);
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x000A4484 File Offset: 0x000A2684
	private static float CalcLength(in Vector3[] positions)
	{
		float num = 0f;
		for (int i = 0; i < positions.Length - 1; i++)
		{
			num += Vector3.Distance(positions[i], positions[i + 1]);
		}
		return num;
	}

	// Token: 0x040024C9 RID: 9417
	[SerializeField]
	private Transform _transform;

	// Token: 0x040024CA RID: 9418
	[SerializeField]
	private LineRenderer _lineRenderer;

	// Token: 0x040024CB RID: 9419
	[SerializeField]
	[Range(1f, 128f)]
	private int _segments = 8;

	// Token: 0x040024CC RID: 9420
	[SerializeField]
	private float _length = 8f;

	// Token: 0x040024CD RID: 9421
	public bool manualUpdate;

	// Token: 0x040024CE RID: 9422
	[Space]
	public bool applyPhysics;

	// Token: 0x040024CF RID: 9423
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x040024D0 RID: 9424
	public AnimationCurve gravityCurve = AnimationCurves.EaseInCubic;

	// Token: 0x040024D1 RID: 9425
	[Space]
	private Vector3[] _points = new Vector3[8];
}
