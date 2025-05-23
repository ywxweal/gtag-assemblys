using System;

// Token: 0x0200055B RID: 1371
[Serializable]
public struct GroupJoinZoneAB
{
	// Token: 0x06002124 RID: 8484 RVA: 0x000A64A8 File Offset: 0x000A46A8
	public static GroupJoinZoneAB operator &(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a & two.a),
			b = (one.b & two.b)
		};
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x000A64E8 File Offset: 0x000A46E8
	public static GroupJoinZoneAB operator |(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a | two.a),
			b = (one.b | two.b)
		};
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000A6528 File Offset: 0x000A4728
	public static GroupJoinZoneAB operator ~(GroupJoinZoneAB z)
	{
		return new GroupJoinZoneAB
		{
			a = ~z.a,
			b = ~z.b
		};
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000A655A File Offset: 0x000A475A
	public static bool operator ==(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a == two.a && one.b == two.b;
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000A657A File Offset: 0x000A477A
	public static bool operator !=(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a != two.a || one.b != two.b;
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000A659D File Offset: 0x000A479D
	public override bool Equals(object other)
	{
		return this == (GroupJoinZoneAB)other;
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000A65B0 File Offset: 0x000A47B0
	public override int GetHashCode()
	{
		return this.a.GetHashCode() ^ this.b.GetHashCode();
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000A65D8 File Offset: 0x000A47D8
	public static implicit operator GroupJoinZoneAB(int d)
	{
		return new GroupJoinZoneAB
		{
			a = (GroupJoinZoneA)d
		};
	}

	// Token: 0x0600212C RID: 8492 RVA: 0x000A65F8 File Offset: 0x000A47F8
	public override string ToString()
	{
		if (this.b == (GroupJoinZoneB)0)
		{
			return this.a.ToString();
		}
		if (this.a != (GroupJoinZoneA)0)
		{
			return this.a.ToString() + "," + this.b.ToString();
		}
		return this.b.ToString();
	}

	// Token: 0x04002579 RID: 9593
	public GroupJoinZoneA a;

	// Token: 0x0400257A RID: 9594
	public GroupJoinZoneB b;
}
