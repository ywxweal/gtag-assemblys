using System;

// Token: 0x02000671 RID: 1649
public interface IVariable<T> : IVariable
{
	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x06002939 RID: 10553 RVA: 0x000CD009 File Offset: 0x000CB209
	// (set) Token: 0x0600293A RID: 10554 RVA: 0x000CD011 File Offset: 0x000CB211
	T Value
	{
		get
		{
			return this.Get();
		}
		set
		{
			this.Set(value);
		}
	}

	// Token: 0x0600293B RID: 10555
	T Get();

	// Token: 0x0600293C RID: 10556
	void Set(T value);

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x0600293D RID: 10557 RVA: 0x000CD01A File Offset: 0x000CB21A
	Type IVariable.ValueType
	{
		get
		{
			return typeof(T);
		}
	}
}
