using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006B0 RID: 1712
public class SnapXformToLine : MonoBehaviour
{
	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x06002ABD RID: 10941 RVA: 0x000D1F6F File Offset: 0x000D016F
	public Vector3 linePoint
	{
		get
		{
			return this._closest;
		}
	}

	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x06002ABE RID: 10942 RVA: 0x000D1F77 File Offset: 0x000D0177
	public float linearDistance
	{
		get
		{
			return this._linear;
		}
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000D1F7F File Offset: 0x000D017F
	public void SnapTarget(bool applyToXform = true)
	{
		this.Snap(this.target, true);
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x000D1F8E File Offset: 0x000D018E
	public void SnapTarget(Vector3 point)
	{
		if (this.target)
		{
			this.target.position = this.GetSnappedPoint(this.target.position);
		}
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x000D1FBC File Offset: 0x000D01BC
	public void SnapTargetLinear(float t)
	{
		if (this.target && this.from && this.to)
		{
			this.target.position = Vector3.Lerp(this.from.position, this.to.position, t);
		}
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x000D2017 File Offset: 0x000D0217
	public Vector3 GetSnappedPoint(Transform t)
	{
		return this.GetSnappedPoint(t.position);
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x000D2028 File Offset: 0x000D0228
	public Vector3 GetSnappedPoint(Vector3 point)
	{
		if (!this.apply)
		{
			return point;
		}
		if (!this.from || !this.to)
		{
			return point;
		}
		return SnapXformToLine.GetClosestPointOnLine(point, this.from.position, this.to.position);
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x000D2078 File Offset: 0x000D0278
	public void Snap(Transform xform, bool applyToXform = true)
	{
		if (!this.apply || !xform || !this.from || !this.to)
		{
			return;
		}
		Vector3 position = xform.position;
		Vector3 position2 = this.from.position;
		Vector3 position3 = this.to.position;
		Vector3 closestPointOnLine = SnapXformToLine.GetClosestPointOnLine(position, position2, position3);
		float num = Vector3.Distance(position2, position3);
		float num2 = Vector3.Distance(closestPointOnLine, position2);
		Vector3 closest = this._closest;
		Vector3 vector = closestPointOnLine;
		float linear = this._linear;
		float num3 = (Mathf.Approximately(num, 0f) ? 0f : (num2 / (num + Mathf.Epsilon)));
		this._closest = vector;
		this._linear = num3;
		if (this.output)
		{
			IRangedVariable<float> asT = this.output.AsT;
			asT.Set(asT.Min + this._linear * asT.Range);
		}
		if (applyToXform)
		{
			xform.position = this._closest;
			if (!Mathf.Approximately(closest.x, vector.x) || !Mathf.Approximately(closest.y, vector.y) || !Mathf.Approximately(closest.z, vector.z))
			{
				UnityEvent<Vector3> unityEvent = this.onPositionChanged;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this._closest);
				}
			}
			if (!Mathf.Approximately(linear, num3))
			{
				UnityEvent<float> unityEvent2 = this.onLinearDistanceChanged;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this._linear);
				}
			}
			if (this.snapOrientation)
			{
				xform.forward = (position3 - position2).normalized;
				xform.up = Vector3.Lerp(this.from.up.normalized, this.to.up.normalized, this._linear);
			}
		}
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x000D223E File Offset: 0x000D043E
	private void OnDisable()
	{
		if (this.resetOnDisable)
		{
			this.SnapTargetLinear(0f);
		}
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x000D2253 File Offset: 0x000D0453
	private void LateUpdate()
	{
		this.SnapTarget(true);
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x000D225C File Offset: 0x000D045C
	private static Vector3 GetClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
	{
		Vector3 vector = p - a;
		Vector3 vector2 = b - a;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num = Mathf.Clamp(Vector3.Dot(vector, vector2) / sqrMagnitude, 0f, 1f);
		return a + vector2 * num;
	}

	// Token: 0x04002FA0 RID: 12192
	public bool apply = true;

	// Token: 0x04002FA1 RID: 12193
	public bool snapOrientation = true;

	// Token: 0x04002FA2 RID: 12194
	public bool resetOnDisable = true;

	// Token: 0x04002FA3 RID: 12195
	[Space]
	public Transform target;

	// Token: 0x04002FA4 RID: 12196
	[Space]
	public Transform from;

	// Token: 0x04002FA5 RID: 12197
	public Transform to;

	// Token: 0x04002FA6 RID: 12198
	private Vector3 _closest;

	// Token: 0x04002FA7 RID: 12199
	private float _linear;

	// Token: 0x04002FA8 RID: 12200
	public Ref<IRangedVariable<float>> output;

	// Token: 0x04002FA9 RID: 12201
	public UnityEvent<float> onLinearDistanceChanged;

	// Token: 0x04002FAA RID: 12202
	public UnityEvent<Vector3> onPositionChanged;
}
