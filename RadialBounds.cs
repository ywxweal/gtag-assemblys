using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006A2 RID: 1698
public class RadialBounds : MonoBehaviour
{
	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x06002A79 RID: 10873 RVA: 0x000D13F4 File Offset: 0x000CF5F4
	// (set) Token: 0x06002A7A RID: 10874 RVA: 0x000D13FC File Offset: 0x000CF5FC
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
	// (get) Token: 0x06002A7B RID: 10875 RVA: 0x000D1405 File Offset: 0x000CF605
	// (set) Token: 0x06002A7C RID: 10876 RVA: 0x000D140D File Offset: 0x000CF60D
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
	// (get) Token: 0x06002A7D RID: 10877 RVA: 0x000D1416 File Offset: 0x000CF616
	public Vector3 center
	{
		get
		{
			return base.transform.TransformPoint(this._localCenter);
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x06002A7E RID: 10878 RVA: 0x000D1429 File Offset: 0x000CF629
	public float radius
	{
		get
		{
			return MathUtils.GetScaledRadius(this._localRadius, base.transform.lossyScale);
		}
	}

	// Token: 0x04002F60 RID: 12128
	[SerializeField]
	private Vector3 _localCenter;

	// Token: 0x04002F61 RID: 12129
	[SerializeField]
	private float _localRadius = 1f;

	// Token: 0x04002F62 RID: 12130
	[Space]
	public UnityEvent<RadialBounds> onOverlapEnter;

	// Token: 0x04002F63 RID: 12131
	public UnityEvent<RadialBounds> onOverlapExit;

	// Token: 0x04002F64 RID: 12132
	public UnityEvent<RadialBounds, float> onOverlapStay;
}
