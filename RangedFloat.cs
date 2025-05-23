using System;
using UnityEngine;

// Token: 0x020006A5 RID: 1701
public class RangedFloat : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x06002A88 RID: 10888 RVA: 0x000D1877 File Offset: 0x000CFA77
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x06002A89 RID: 10889 RVA: 0x000D187F File Offset: 0x000CFA7F
	public float Range
	{
		get
		{
			return this._max - this._min;
		}
	}

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x06002A8A RID: 10890 RVA: 0x000D188E File Offset: 0x000CFA8E
	// (set) Token: 0x06002A8B RID: 10891 RVA: 0x000D1896 File Offset: 0x000CFA96
	public float Min
	{
		get
		{
			return this._min;
		}
		set
		{
			this._min = value;
		}
	}

	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x06002A8C RID: 10892 RVA: 0x000D189F File Offset: 0x000CFA9F
	// (set) Token: 0x06002A8D RID: 10893 RVA: 0x000D18A7 File Offset: 0x000CFAA7
	public float Max
	{
		get
		{
			return this._max;
		}
		set
		{
			this._max = value;
		}
	}

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x06002A8E RID: 10894 RVA: 0x000D18B0 File Offset: 0x000CFAB0
	// (set) Token: 0x06002A8F RID: 10895 RVA: 0x000D18E5 File Offset: 0x000CFAE5
	public float normalized
	{
		get
		{
			if (!this.Range.Approx0(1E-06f))
			{
				return (this._value - this._min) / (this._max - this.Min);
			}
			return 0f;
		}
		set
		{
			this._value = this._min + Mathf.Clamp01(value) * (this._max - this._min);
		}
	}

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x06002A90 RID: 10896 RVA: 0x000D1908 File Offset: 0x000CFB08
	public float curved
	{
		get
		{
			return this._min + this._curve.Evaluate(this.normalized) * (this._max - this._min);
		}
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000D1930 File Offset: 0x000CFB30
	public float Get()
	{
		return this._value;
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x000D1938 File Offset: 0x000CFB38
	public void Set(float f)
	{
		this._value = Mathf.Clamp(f, this._min, this._max);
	}

	// Token: 0x04002F73 RID: 12147
	[SerializeField]
	private float _value = 0.5f;

	// Token: 0x04002F74 RID: 12148
	[SerializeField]
	private float _min;

	// Token: 0x04002F75 RID: 12149
	[SerializeField]
	private float _max = 1f;

	// Token: 0x04002F76 RID: 12150
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
