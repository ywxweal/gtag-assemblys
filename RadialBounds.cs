using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006A2 RID: 1698
public class RadialBounds : MonoBehaviour
{
	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x06002A7A RID: 10874 RVA: 0x000D1498 File Offset: 0x000CF698
	// (set) Token: 0x06002A7B RID: 10875 RVA: 0x000D14A0 File Offset: 0x000CF6A0
	public Vector3 localCenter
	{
		get
		{
			return this._localCenter;
		}
		set
		{
			this._localCenter = value;
		}
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06002A7C RID: 10876 RVA: 0x000D14A9 File Offset: 0x000CF6A9
	// (set) Token: 0x06002A7D RID: 10877 RVA: 0x000D14B1 File Offset: 0x000CF6B1
	public float localRadius
	{
		get
		{
			return this._localRadius;
		}
		set
		{
			this._localRadius = value;
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06002A7E RID: 10878 RVA: 0x000D14BA File Offset: 0x000CF6BA
	public Vector3 center
	{
		get
		{
			return base.transform.TransformPoint(this._localCenter);
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x06002A7F RID: 10879 RVA: 0x000D14CD File Offset: 0x000CF6CD
	public float radius
	{
		get
		{
			return MathUtils.GetScaledRadius(this._localRadius, base.transform.lossyScale);
		}
	}

	// Token: 0x04002F62 RID: 12130
	[SerializeField]
	private Vector3 _localCenter;

	// Token: 0x04002F63 RID: 12131
	[SerializeField]
	private float _localRadius = 1f;

	// Token: 0x04002F64 RID: 12132
	[Space]
	public UnityEvent<RadialBounds> onOverlapEnter;

	// Token: 0x04002F65 RID: 12133
	public UnityEvent<RadialBounds> onOverlapExit;

	// Token: 0x04002F66 RID: 12134
	public UnityEvent<RadialBounds, float> onOverlapStay;
}
