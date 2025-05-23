using System;

// Token: 0x0200077C RID: 1916
[Serializable]
public struct StateHash
{
	// Token: 0x06002FE2 RID: 12258 RVA: 0x000ED969 File Offset: 0x000EBB69
	public override int GetHashCode()
	{
		return HashCode.Combine<int, int>(this.last, this.next);
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x000ED97C File Offset: 0x000EBB7C
	public override string ToString()
	{
		return this.GetHashCode().ToString();
	}

	// Token: 0x06002FE4 RID: 12260 RVA: 0x000ED99D File Offset: 0x000EBB9D
	public bool Changed()
	{
		if (this.last == this.next)
		{
			return false;
		}
		this.last = this.next;
		return true;
	}

	// Token: 0x06002FE5 RID: 12261 RVA: 0x000ED9BC File Offset: 0x000EBBBC
	public void Poll<T0>(T0 v0)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T0>(v0);
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x000ED9D6 File Offset: 0x000EBBD6
	public void Poll<T1, T2>(T1 v1, T2 v2)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2>(v1, v2);
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x000ED9F1 File Offset: 0x000EBBF1
	public void Poll<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3>(v1, v2, v3);
	}

	// Token: 0x06002FE8 RID: 12264 RVA: 0x000EDA0D File Offset: 0x000EBC0D
	public void Poll<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4>(v1, v2, v3, v4);
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x000EDA2B File Offset: 0x000EBC2B
	public void Poll<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5>(v1, v2, v3, v4, v5);
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x000EDA4B File Offset: 0x000EBC4B
	public void Poll<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6>(v1, v2, v3, v4, v5, v6);
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x000EDA6D File Offset: 0x000EBC6D
	public void Poll<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7>(v1, v2, v3, v4, v5, v6, v7);
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x000EDA94 File Offset: 0x000EBC94
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8)
	{
		this.last = this.next;
		this.next = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x000EDAC8 File Offset: 0x000EBCC8
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9>(num, v9);
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x000EDB04 File Offset: 0x000EBD04
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10>(num, v9, v10);
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x000EDB40 File Offset: 0x000EBD40
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11>(num, v9, v10, v11);
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x000EDB80 File Offset: 0x000EBD80
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12>(num, v9, v10, v11, v12);
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x000EDBC0 File Offset: 0x000EBDC0
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13>(num, v9, v10, v11, v12, v13);
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x000EDC04 File Offset: 0x000EBE04
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13, T14>(num, v9, v10, v11, v12, v13, v14);
	}

	// Token: 0x06002FF3 RID: 12275 RVA: 0x000EDC48 File Offset: 0x000EBE48
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14, T15 v15)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		this.next = HashCode.Combine<int, T9, T10, T11, T12, T13, T14, T15>(num, v9, v10, v11, v12, v13, v14, v15);
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x000EDC90 File Offset: 0x000EBE90
	public void Poll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8, T9 v9, T10 v10, T11 v11, T12 v12, T13 v13, T14 v14, T15 v15, T16 v16)
	{
		this.last = this.next;
		int num = HashCode.Combine<T1, T2, T3, T4, T5, T6, T7, T8>(v1, v2, v3, v4, v5, v6, v7, v8);
		int num2 = HashCode.Combine<int, T9, T10, T11, T12, T13, T14, T15>(num, v9, v10, v11, v12, v13, v14, v15);
		this.next = HashCode.Combine<int, int, T16>(num, num2, v16);
	}

	// Token: 0x04003636 RID: 13878
	public int last;

	// Token: 0x04003637 RID: 13879
	public int next;
}
