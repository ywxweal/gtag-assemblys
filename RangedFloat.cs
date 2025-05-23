using System;
using UnityEngine;

// Token: 0x020006A5 RID: 1701
public class RangedFloat : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x06002A89 RID: 10889 RVA: 0x000D191B File Offset: 0x000CFB1B
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x06002A8A RID: 10890 RVA: 0x000D1923 File Offset: 0x000CFB23
	public float Range
	{
		get
		{
			return this._max - this._min;
		}
	}

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x06002A8B RID: 10891 RVA: 0x000D1932 File Offset: 0x000CFB32
	// (set) Token: 0x06002A8C RID: 10892 RVA: 0x000D193A File Offset: 0x000CFB3A
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
	// (get) Token: 0x06002A8D RID: 10893 RVA: 0x000D1943 File Offset: 0x000CFB43
	// (set) Token: 0x06002A8E RID: 10894 RVA: 0x000D194B File Offset: 0x000CFB4B
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
	// (get) Token: 0x06002A8F RID: 10895 RVA: 0x000D1954 File Offset: 0x000CFB54
	// (set) Token: 0x06002A90 RID: 10896 RVA: 0x000D1989 File Offset: 0x000CFB89
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
	// (get) Token: 0x06002A91 RID: 10897 RVA: 0x000D19AC File Offset: 0x000CFBAC
	public float curved
	{
		get
		{
			return this._min + this._curve.Evaluate(this.normalized) * (this._max - this._min);
		}
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x000D19D4 File Offset: 0x000CFBD4
	public float Get()
	{
		return this._value;
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x000D19DC File Offset: 0x000CFBDC
	public void Set(float f)
	{
		this._value = Mathf.Clamp(f, this._min, this._max);
	}

	// Token: 0x04002F75 RID: 12149
	[SerializeField]
	private float _value = 0.5f;

	// Token: 0x04002F76 RID: 12150
	[SerializeField]
	private float _min;

	// Token: 0x04002F77 RID: 12151
	[SerializeField]
	private float _max = 1f;

	// Token: 0x04002F78 RID: 12152
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
