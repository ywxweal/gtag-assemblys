using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E86 RID: 3718
	public struct Aabb
	{
		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x06005CE7 RID: 23783 RVA: 0x001CABDB File Offset: 0x001C8DDB
		// (set) Token: 0x06005CE8 RID: 23784 RVA: 0x001CABE8 File Offset: 0x001C8DE8
		public float MinX
		{
			get
			{
				return this.Min.x;
			}
			set
			{
				this.Min.x = value;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x06005CE9 RID: 23785 RVA: 0x001CABF6 File Offset: 0x001C8DF6
		// (set) Token: 0x06005CEA RID: 23786 RVA: 0x001CAC03 File Offset: 0x001C8E03
		public float MinY
		{
			get
			{
				return this.Min.y;
			}
			set
			{
				this.Min.y = value;
			}
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06005CEB RID: 23787 RVA: 0x001CAC11 File Offset: 0x001C8E11
		// (set) Token: 0x06005CEC RID: 23788 RVA: 0x001CAC1E File Offset: 0x001C8E1E
		public float MinZ
		{
			get
			{
				return this.Min.z;
			}
			set
			{
				this.Min.z = value;
			}
		}

		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x06005CED RID: 23789 RVA: 0x001CAC2C File Offset: 0x001C8E2C
		// (set) Token: 0x06005CEE RID: 23790 RVA: 0x001CAC39 File Offset: 0x001C8E39
		public float MaxX
		{
			get
			{
				return this.Max.x;
			}
			set
			{
				this.Max.x = value;
			}
		}

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x06005CEF RID: 23791 RVA: 0x001CAC47 File Offset: 0x001C8E47
		// (set) Token: 0x06005CF0 RID: 23792 RVA: 0x001CAC54 File Offset: 0x001C8E54
		public float MaxY
		{
			get
			{
				return this.Max.y;
			}
			set
			{
				this.Max.y = value;
			}
		}

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06005CF1 RID: 23793 RVA: 0x001CAC62 File Offset: 0x001C8E62
		// (set) Token: 0x06005CF2 RID: 23794 RVA: 0x001CAC6F File Offset: 0x001C8E6F
		public float MaxZ
		{
			get
			{
				return this.Max.z;
			}
			set
			{
				this.Max.z = value;
			}
		}

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x06005CF3 RID: 23795 RVA: 0x001CAC7D File Offset: 0x001C8E7D
		public Vector3 Center
		{
			get
			{
				return 0.5f * (this.Min + this.Max);
			}
		}

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x06005CF4 RID: 23796 RVA: 0x001CAC9C File Offset: 0x001C8E9C
		public Vector3 Size
		{
			get
			{
				Vector3 vector = this.Max - this.Min;
				vector.x = Mathf.Max(0f, vector.x);
				vector.y = Mathf.Max(0f, vector.y);
				vector.z = Mathf.Max(0f, vector.z);
				return vector;
			}
		}

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06005CF5 RID: 23797 RVA: 0x001CAD01 File Offset: 0x001C8F01
		public static Aabb Empty
		{
			get
			{
				return new Aabb(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			}
		}

		// Token: 0x06005CF6 RID: 23798 RVA: 0x001CAD30 File Offset: 0x001C8F30
		public static Aabb FromPoint(Vector3 p)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(p);
			return empty;
		}

		// Token: 0x06005CF7 RID: 23799 RVA: 0x001CAD4C File Offset: 0x001C8F4C
		public static Aabb FromPoints(Vector3 a, Vector3 b)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(a);
			empty.Include(b);
			return empty;
		}

		// Token: 0x06005CF8 RID: 23800 RVA: 0x001CAD70 File Offset: 0x001C8F70
		public Aabb(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x06005CF9 RID: 23801 RVA: 0x001CAD80 File Offset: 0x001C8F80
		public void Include(Vector3 p)
		{
			this.MinX = Mathf.Min(this.MinX, p.x);
			this.MinY = Mathf.Min(this.MinY, p.y);
			this.MinZ = Mathf.Min(this.MinZ, p.z);
			this.MaxX = Mathf.Max(this.MaxX, p.x);
			this.MaxY = Mathf.Max(this.MaxY, p.y);
			this.MaxZ = Mathf.Max(this.MaxZ, p.z);
		}

		// Token: 0x06005CFA RID: 23802 RVA: 0x001CAE18 File Offset: 0x001C9018
		public bool Contains(Vector3 p)
		{
			return this.MinX <= p.x && this.MinY <= p.y && this.MinZ <= p.z && this.MaxX >= p.x && this.MaxY >= p.y && this.MaxZ >= p.z;
		}

		// Token: 0x06005CFB RID: 23803 RVA: 0x001CAE7E File Offset: 0x001C907E
		public bool ContainsX(Vector3 p)
		{
			return this.MinX <= p.x && this.MaxX >= p.x;
		}

		// Token: 0x06005CFC RID: 23804 RVA: 0x001CAEA1 File Offset: 0x001C90A1
		public bool ContainsY(Vector3 p)
		{
			return this.MinY <= p.y && this.MaxY >= p.y;
		}

		// Token: 0x06005CFD RID: 23805 RVA: 0x001CAEC4 File Offset: 0x001C90C4
		public bool ContainsZ(Vector3 p)
		{
			return this.MinZ <= p.z && this.MaxZ >= p.z;
		}

		// Token: 0x06005CFE RID: 23806 RVA: 0x001CAEE8 File Offset: 0x001C90E8
		public bool Intersects(Aabb rhs)
		{
			return this.MinX <= rhs.MaxX && this.MinY <= rhs.MaxY && this.MinZ <= rhs.MaxZ && this.MaxX >= rhs.MinX && this.MaxY >= rhs.MinY && this.MaxZ >= rhs.MinZ;
		}

		// Token: 0x06005CFF RID: 23807 RVA: 0x001CAF54 File Offset: 0x001C9154
		public bool Intersects(ref BoingEffector.Params effector)
		{
			if (!effector.Bits.IsBitSet(0))
			{
				return this.Intersects(Aabb.FromPoint(effector.CurrPosition).Expand(effector.Radius));
			}
			return this.Intersects(Aabb.FromPoints(effector.PrevPosition, effector.CurrPosition).Expand(effector.Radius));
		}

		// Token: 0x06005D00 RID: 23808 RVA: 0x001CAFB4 File Offset: 0x001C91B4
		public Aabb Expand(float amount)
		{
			this.MinX -= amount;
			this.MinY -= amount;
			this.MinZ -= amount;
			this.MaxX += amount;
			this.MaxY += amount;
			this.MaxZ += amount;
			return this;
		}

		// Token: 0x0400611D RID: 24861
		public Vector3 Min;

		// Token: 0x0400611E RID: 24862
		public Vector3 Max;
	}
}
