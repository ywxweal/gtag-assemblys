using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200041C RID: 1052
[Serializable]
public class AdvancedItemState
{
	// Token: 0x060019BD RID: 6589 RVA: 0x0007D932 File Offset: 0x0007BB32
	public void Encode()
	{
		this._encodedValue = this.EncodeData();
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x0007D940 File Offset: 0x0007BB40
	public void Decode()
	{
		AdvancedItemState advancedItemState = this.DecodeData(this._encodedValue);
		this.index = advancedItemState.index;
		this.preData = advancedItemState.preData;
		this.limitAxis = advancedItemState.limitAxis;
		this.reverseGrip = advancedItemState.reverseGrip;
		this.angle = advancedItemState.angle;
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x0007D998 File Offset: 0x0007BB98
	public Quaternion GetQuaternion()
	{
		Vector3 one = Vector3.one;
		if (this.reverseGrip)
		{
			switch (this.limitAxis)
			{
			case LimitAxis.NoMovement:
				return Quaternion.identity;
			case LimitAxis.YAxis:
				return Quaternion.identity;
			case LimitAxis.XAxis:
			case LimitAxis.ZAxis:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Quaternion.identity;
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x0007D9EC File Offset: 0x0007BBEC
	[return: TupleElementNames(new string[] { "grabPointIndex", "YRotation", "XRotation", "ZRotation" })]
	public ValueTuple<int, float, float, float> DecodeAdvancedItemState(int encodedValue)
	{
		int num = (encodedValue >> 21) & 255;
		float num2 = (float)((encodedValue >> 14) & 127) / 128f * 360f;
		float num3 = (float)((encodedValue >> 7) & 127) / 128f * 360f;
		float num4 = (float)(encodedValue & 127) / 128f * 360f;
		return new ValueTuple<int, float, float, float>(num, num2, num3, num4);
	}

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x060019C1 RID: 6593 RVA: 0x0007DA46 File Offset: 0x0007BC46
	private float EncodedDeltaRotation
	{
		get
		{
			return this.GetEncodedDeltaRotation();
		}
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x0007DA4E File Offset: 0x0007BC4E
	public float GetEncodedDeltaRotation()
	{
		return Mathf.Abs(Mathf.Atan2(this.angleVectorWhereUpIsStandard.x, this.angleVectorWhereUpIsStandard.y)) / 3.1415927f;
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x0007DA78 File Offset: 0x0007BC78
	public void DecodeDeltaRotation(float encodedDelta, bool isFlipped)
	{
		float num = encodedDelta * 3.1415927f;
		if (isFlipped)
		{
			this.angleVectorWhereUpIsStandard = new Vector2(-Mathf.Sin(num), Mathf.Cos(num));
		}
		else
		{
			this.angleVectorWhereUpIsStandard = new Vector2(Mathf.Sin(num), Mathf.Cos(num));
		}
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
		case LimitAxis.XAxis:
		case LimitAxis.ZAxis:
			return;
		case LimitAxis.YAxis:
		{
			Vector3 vector = new Vector3(this.angleVectorWhereUpIsStandard.x, 0f, this.angleVectorWhereUpIsStandard.y);
			Vector3 vector2 = (this.reverseGrip ? Vector3.down : Vector3.up);
			this.deltaRotation = Quaternion.LookRotation(vector, vector2);
			return;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x0007DB2C File Offset: 0x0007BD2C
	public int EncodeData()
	{
		int num = 0;
		if ((this.index >= 32) | (this.index < 0))
		{
			throw new ArgumentOutOfRangeException(string.Format("Index is invalid {0}", this.index));
		}
		num |= this.index << 25;
		AdvancedItemState.PointType pointType = this.preData.pointType;
		num |= (int)((int)(pointType & (AdvancedItemState.PointType)7) << 22);
		num |= (int)((int)this.limitAxis << 19);
		num |= (this.reverseGrip ? 1 : 0) << 18;
		bool flag = this.angleVectorWhereUpIsStandard.x < 0f;
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num2 = (int)(this.GetEncodedDeltaRotation() * 512f) & 511;
			num |= (flag ? 1 : 0) << 17;
			num |= num2 << 9;
			int num3 = (int)(this.preData.distAlongLine * 256f) & 255;
			num |= num3;
		}
		else
		{
			int num4 = (int)(this.GetEncodedDeltaRotation() * 65536f) & 65535;
			num |= (flag ? 1 : 0) << 17;
			num |= num4 << 1;
		}
		return num;
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x0007DC48 File Offset: 0x0007BE48
	public AdvancedItemState DecodeData(int encoded)
	{
		AdvancedItemState advancedItemState = new AdvancedItemState();
		advancedItemState.index = (encoded >> 25) & 31;
		advancedItemState.limitAxis = (LimitAxis)((encoded >> 19) & 7);
		advancedItemState.reverseGrip = ((encoded >> 18) & 1) == 1;
		AdvancedItemState.PointType pointType = (AdvancedItemState.PointType)((encoded >> 22) & 7);
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType,
				distAlongLine = (float)(encoded & 255) / 256f
			};
			this.DecodeDeltaRotation((float)((encoded >> 9) & 511) / 512f, ((encoded >> 17) & 1) > 0);
		}
		else
		{
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType
			};
			this.DecodeDeltaRotation((float)((encoded >> 1) & 65535) / 65536f, ((encoded >> 17) & 1) > 0);
		}
		return advancedItemState;
	}

	// Token: 0x04001CC6 RID: 7366
	private int _encodedValue;

	// Token: 0x04001CC7 RID: 7367
	public Vector2 angleVectorWhereUpIsStandard;

	// Token: 0x04001CC8 RID: 7368
	public Quaternion deltaRotation;

	// Token: 0x04001CC9 RID: 7369
	public int index;

	// Token: 0x04001CCA RID: 7370
	public AdvancedItemState.PreData preData;

	// Token: 0x04001CCB RID: 7371
	public LimitAxis limitAxis;

	// Token: 0x04001CCC RID: 7372
	public bool reverseGrip;

	// Token: 0x04001CCD RID: 7373
	public float angle;

	// Token: 0x0200041D RID: 1053
	[Serializable]
	public class PreData
	{
		// Token: 0x04001CCE RID: 7374
		public float distAlongLine;

		// Token: 0x04001CCF RID: 7375
		public AdvancedItemState.PointType pointType;
	}

	// Token: 0x0200041E RID: 1054
	public enum PointType
	{
		// Token: 0x04001CD1 RID: 7377
		Standard,
		// Token: 0x04001CD2 RID: 7378
		DistanceBased
	}
}
