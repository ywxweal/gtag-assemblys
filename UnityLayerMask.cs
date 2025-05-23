using System;

// Token: 0x02000229 RID: 553
[Flags]
public enum UnityLayerMask
{
	// Token: 0x04000F87 RID: 3975
	Everything = -1,
	// Token: 0x04000F88 RID: 3976
	Nothing = 0,
	// Token: 0x04000F89 RID: 3977
	Default = 1,
	// Token: 0x04000F8A RID: 3978
	TransparentFX = 2,
	// Token: 0x04000F8B RID: 3979
	IgnoreRaycast = 4,
	// Token: 0x04000F8C RID: 3980
	Zone = 8,
	// Token: 0x04000F8D RID: 3981
	Water = 16,
	// Token: 0x04000F8E RID: 3982
	UI = 32,
	// Token: 0x04000F8F RID: 3983
	MeshBakerAtlas = 64,
	// Token: 0x04000F90 RID: 3984
	GorillaEquipment = 128,
	// Token: 0x04000F91 RID: 3985
	GorillaBodyCollider = 256,
	// Token: 0x04000F92 RID: 3986
	GorillaObject = 512,
	// Token: 0x04000F93 RID: 3987
	GorillaHand = 1024,
	// Token: 0x04000F94 RID: 3988
	GorillaTrigger = 2048,
	// Token: 0x04000F95 RID: 3989
	MetaReportScreen = 4096,
	// Token: 0x04000F96 RID: 3990
	GorillaHead = 8192,
	// Token: 0x04000F97 RID: 3991
	GorillaTagCollider = 16384,
	// Token: 0x04000F98 RID: 3992
	GorillaBoundary = 32768,
	// Token: 0x04000F99 RID: 3993
	GorillaEquipmentContainer = 65536,
	// Token: 0x04000F9A RID: 3994
	LCKHide = 131072,
	// Token: 0x04000F9B RID: 3995
	GorillaInteractable = 262144,
	// Token: 0x04000F9C RID: 3996
	FirstPersonOnly = 524288,
	// Token: 0x04000F9D RID: 3997
	GorillaParticle = 1048576,
	// Token: 0x04000F9E RID: 3998
	GorillaCosmetics = 2097152,
	// Token: 0x04000F9F RID: 3999
	MirrorOnly = 4194304,
	// Token: 0x04000FA0 RID: 4000
	GorillaThrowable = 8388608,
	// Token: 0x04000FA1 RID: 4001
	GorillaHandSocket = 16777216,
	// Token: 0x04000FA2 RID: 4002
	GorillaCosmeticParticle = 33554432,
	// Token: 0x04000FA3 RID: 4003
	BuilderProp = 67108864,
	// Token: 0x04000FA4 RID: 4004
	NoMirror = 134217728,
	// Token: 0x04000FA5 RID: 4005
	GorillaSlingshotCollider = 268435456,
	// Token: 0x04000FA6 RID: 4006
	RopeSwing = 536870912,
	// Token: 0x04000FA7 RID: 4007
	Prop = 1073741824,
	// Token: 0x04000FA8 RID: 4008
	Bake = -2147483648
}
