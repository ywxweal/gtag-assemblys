using System;

// Token: 0x0200066F RID: 1647
public interface IGorillaSliceableSimple
{
	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x06002930 RID: 10544
	bool isActiveAndEnabled { get; }

	// Token: 0x06002931 RID: 10545
	void SliceUpdate();

	// Token: 0x06002932 RID: 10546
	void OnEnable();

	// Token: 0x06002933 RID: 10547
	void OnDisable();
}
