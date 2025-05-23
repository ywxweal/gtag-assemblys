using System;

// Token: 0x0200077C RID: 1916
[Serializable]
public struct StateHash
{
	// Token: 0x06002FE1 RID: 12257 RVA: 0x000ED8C5 File Offset: 0x000EBAC5
	public override int GetHashCode()
	{
		return HashCode.Combine<int, int>(this.last, this.next);
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x000ED8D8 File Offset: 0x000EBAD8
	public override string ToString()
	{
		return this.GetHashCode().ToString();
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x000ED8F9 File Offset: 0x000EBAF9
	public bool Changed()
	{
		if (this.last == this.next)
		{
			return false;
		}
		this.last = this.next;
		return true;
	}

	// Token: 0x06002FE4 RID: 12260 RVA: 0x000ED918 File Offset: 0x000EBB18
	public void Poll<T0>(T0 v0)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T0>(v0);
	}

	// Token: 0x06002FE5 RID: 12261 RVA: 0x000ED932 File Offset: 0x000EBB32
	public void Poll<T1, T2>(T1 v1, T2 v2)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2>(v1, v2);
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x000ED94D File Offset: 0x000EBB4D
	public void Poll<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3>(v1, v2, v3);
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x000ED969 File Offset: 0x000EBB69
	public void Poll<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4>(v1, v2, v3, v4);
	}

	// Token: 0x06002FE8 RID: 12264 RVA: 0x000ED987 File Offset: 0x000EBB87
	public void Poll<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5>(v1, v2, v3, v4, v5);
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x000ED9A7 File Offset: 0x000EBBA7
	public void Poll<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6>(v1, v2, v3, v4, v5, v6);
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x000ED9C9 File Offset: 0x000EBBC9
	public void Poll<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7>(v1, v2, v3, v4, v5, v6, v7);
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x000ED9F0 File Offset: 0x000EBBF0
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x000EDA24 File Offset: 0x000EBC24
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9>(num, v9);
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x000EDA60 File Offset: 0x000EBC60
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10>(num, v9, v10);
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x000EDA9C File Offset: 0x000EBC9C
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11>(num, v9, v10, v11);
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x000EDADC File Offset: 0x000EBCDC
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12>(num, v9, v10, v11, v12);
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x000EDB1C File Offset: 0x000EBD1C
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13>(num, v9, v10, v11, v12, v13);
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x000EDB60 File Offset: 0x000EBD60
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13, T14>(num, v9, v10, v11, v12, v13, v14);
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x000EDBA4 File Offset: 0x000EBDA4
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14, T15 v15)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13, T14, T15>(num, v9, v10, v11, v12, v13, v14, v15);
	}

	// Token: 0x06002FF3 RID: 12275 RVA: 0x000EDBEC File Offset: 0x000EBDEC
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14, T15 v15, T16 v16)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		int num2 = HashCode.Combine<int, T9, T10, T11, T12, T13, T14, T15>(num, v9, v10, v11, v12, v13, v14, v15);
		this.next = HashCode.Combine<int, int, T16>(num, num2, v16);
	}

	// Token: 0x04003634 RID: 13876
	public int last;

	// Token: 0x04003635 RID: 13877
	public int next;
}
