using System;

// Token: 0x020005E0 RID: 1504
internal interface IGorillaSerializeableScene : IGorillaSerializeable
{
	// Token: 0x060024CE RID: 9422
	void OnSceneLinking(GorillaSerializerScene serializer);

	// Token: 0x060024CF RID: 9423
	void OnNetworkObjectDisable();

	// Token: 0x060024D0 RID: 9424
	void OnNetworkObjectEnable();
}
