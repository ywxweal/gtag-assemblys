using System;

// Token: 0x02000671 RID: 1649
public interface IVariable<T> : IVariable
{
	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x0600293A RID: 10554 RVA: 0x000CD0AD File Offset: 0x000CB2AD
	// (set) Token: 0x0600293B RID: 10555 RVA: 0x000CD0B5 File Offset: 0x000CB2B5
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

	// Token: 0x0600293C RID: 10556
	T Get();

	// Token: 0x0600293D RID: 10557
	void Set(T value);

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x0600293E RID: 10558 RVA: 0x000CD0BE File Offset: 0x000CB2BE
	Type IVariable.ValueType
	{
		get
		{
			return typeof(T);
		}
	}
}
