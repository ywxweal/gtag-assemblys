using System;

// Token: 0x0200022B RID: 555
public enum UnityTag
{
	// Token: 0x04000FAA RID: 4010
	Invalid = -1,
	// Token: 0x04000FAB RID: 4011
	Untagged,
	// Token: 0x04000FAC RID: 4012
	Respawn,
	// Token: 0x04000FAD RID: 4013
	Finish,
	// Token: 0x04000FAE RID: 4014
	EditorOnly,
	// Token: 0x04000FAF RID: 4015
	MainCamera,
	// Token: 0x04000FB0 RID: 4016
	Player,
	// Token: 0x04000FB1 RID: 4017
	GameController,
	// Token: 0x04000FB2 RID: 4018
	SceneChanger,
	// Token: 0x04000FB3 RID: 4019
	PlayerOffset,
	// Token: 0x04000FB4 RID: 4020
	GorillaTagManager,
	// Token: 0x04000FB5 RID: 4021
	GorillaTagCollider,
	// Token: 0x04000FB6 RID: 4022
	GorillaPlayer,
	// Token: 0x04000FB7 RID: 4023
	GorillaObject,
	// Token: 0x04000FB8 RID: 4024
	GorillaGameManager,
	// Token: 0x04000FB9 RID: 4025
	GorillaCosmetic,
	// Token: 0x04000FBA RID: 4026
	projectile,
	// Token: 0x04000FBB RID: 4027
	FxTemporaire,
	// Token: 0x04000FBC RID: 4028
	SlingshotProjectile,
	// Token: 0x04000FBD RID: 4029
	SlingshotProjectileTrail,
	// Token: 0x04000FBE RID: 4030
	SlingshotProjectilePlayerImpactFX,
	// Token: 0x04000FBF RID: 4031
	SlingshotProjectileSurfaceImpactFX,
	// Token: 0x04000FC0 RID: 4032
	BalloonPopFX,
	// Token: 0x04000FC1 RID: 4033
	WorldShareableItem,
	// Token: 0x04000FC2 RID: 4034
	HornsSlingshotProjectile,
	// Token: 0x04000FC3 RID: 4035
	HornsSlingshotProjectileTrail,
	// Token: 0x04000FC4 RID: 4036
	HornsSlingshotProjectilePlayerImpactFX,
	// Token: 0x04000FC5 RID: 4037
	HornsSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04000FC6 RID: 4038
	FryingPan,
	// Token: 0x04000FC7 RID: 4039
	LeafPileImpactFX,
	// Token: 0x04000FC8 RID: 4040
	BalloonPopFx,
	// Token: 0x04000FC9 RID: 4041
	CloudSlingshotProjectile,
	// Token: 0x04000FCA RID: 4042
	CloudSlingshotProjectileTrail,
	// Token: 0x04000FCB RID: 4043
	CloudSlingshotProjectilePlayerImpactFX,
	// Token: 0x04000FCC RID: 4044
	CloudSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04000FCD RID: 4045
	SnowballProjectile,
	// Token: 0x04000FCE RID: 4046
	SnowballProjectileImpactFX,
	// Token: 0x04000FCF RID: 4047
	CupidBowProjectile,
	// Token: 0x04000FD0 RID: 4048
	CupidBowProjectileTrail,
	// Token: 0x04000FD1 RID: 4049
	CupidBowProjectileSurfaceImpactFX,
	// Token: 0x04000FD2 RID: 4050
	NoCrazyCheck,
	// Token: 0x04000FD3 RID: 4051
	IceSlingshotProjectile,
	// Token: 0x04000FD4 RID: 4052
	IceSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04000FD5 RID: 4053
	IceSlingshotProjectileTrail,
	// Token: 0x04000FD6 RID: 4054
	ElfBowProjectile,
	// Token: 0x04000FD7 RID: 4055
	ElfBowProjectileSurfaceImpactFX,
	// Token: 0x04000FD8 RID: 4056
	ElfBowProjectileTrail,
	// Token: 0x04000FD9 RID: 4057
	RenderIfSmall,
	// Token: 0x04000FDA RID: 4058
	DeleteOnNonBetaBuild,
	// Token: 0x04000FDB RID: 4059
	DeleteOnNonDebugBuild,
	// Token: 0x04000FDC RID: 4060
	FlagColoringCauldon,
	// Token: 0x04000FDD RID: 4061
	WaterRippleEffect,
	// Token: 0x04000FDE RID: 4062
	WaterSplashEffect,
	// Token: 0x04000FDF RID: 4063
	FireworkMortarProjectile,
	// Token: 0x04000FE0 RID: 4064
	FireworkMortarProjectileImpactFX,
	// Token: 0x04000FE1 RID: 4065
	WaterBalloonProjectile,
	// Token: 0x04000FE2 RID: 4066
	WaterBalloonProjectileImpactFX,
	// Token: 0x04000FE3 RID: 4067
	PlayerHeadTrigger,
	// Token: 0x04000FE4 RID: 4068
	WizardStaff,
	// Token: 0x04000FE5 RID: 4069
	LurkerGhost,
	// Token: 0x04000FE6 RID: 4070
	HauntedObject,
	// Token: 0x04000FE7 RID: 4071
	WanderingGhost,
	// Token: 0x04000FE8 RID: 4072
	LavaSurfaceRock,
	// Token: 0x04000FE9 RID: 4073
	LavaRockProjectile,
	// Token: 0x04000FEA RID: 4074
	LavaRockProjectileImpactFX,
	// Token: 0x04000FEB RID: 4075
	MoltenSlingshotProjectile,
	// Token: 0x04000FEC RID: 4076
	MoltenSlingshotProjectileTrail,
	// Token: 0x04000FED RID: 4077
	MoltenSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04000FEE RID: 4078
	MoltenSlingshotProjectilePlayerImpactFX,
	// Token: 0x04000FEF RID: 4079
	SpiderBowProjectile,
	// Token: 0x04000FF0 RID: 4080
	SpiderBowProjectileTrail,
	// Token: 0x04000FF1 RID: 4081
	SpiderBowProjectileSurfaceImpactFX,
	// Token: 0x04000FF2 RID: 4082
	SpiderBowProjectilePlayerImpactFX,
	// Token: 0x04000FF3 RID: 4083
	ZoneRoot,
	// Token: 0x04000FF4 RID: 4084
	DontProcessMaterials,
	// Token: 0x04000FF5 RID: 4085
	OrnamentProjectileSurfaceImpactFX,
	// Token: 0x04000FF6 RID: 4086
	BucketGiftCane,
	// Token: 0x04000FF7 RID: 4087
	BucketGiftCoal,
	// Token: 0x04000FF8 RID: 4088
	BucketGiftRoll,
	// Token: 0x04000FF9 RID: 4089
	BucketGiftRound,
	// Token: 0x04000FFA RID: 4090
	BucketGiftSquare,
	// Token: 0x04000FFB RID: 4091
	OrnamentProjectile,
	// Token: 0x04000FFC RID: 4092
	OrnamentShatterFX,
	// Token: 0x04000FFD RID: 4093
	ScienceCandyProjectile,
	// Token: 0x04000FFE RID: 4094
	ScienceCandyImpactFX,
	// Token: 0x04000FFF RID: 4095
	PaperAirplaneProjectile,
	// Token: 0x04001000 RID: 4096
	DevilBowProjectile,
	// Token: 0x04001001 RID: 4097
	DevilBowProjectileTrail,
	// Token: 0x04001002 RID: 4098
	DevilBowProjectileSurfaceImpactFX,
	// Token: 0x04001003 RID: 4099
	DevilBowProjectilePlayerImpactFX,
	// Token: 0x04001004 RID: 4100
	FireFX,
	// Token: 0x04001005 RID: 4101
	FishFood,
	// Token: 0x04001006 RID: 4102
	FishFoodImpactFX,
	// Token: 0x04001007 RID: 4103
	LeafNinjaStarProjectile,
	// Token: 0x04001008 RID: 4104
	LeafNinjaStarProjectileC1,
	// Token: 0x04001009 RID: 4105
	LeafNinjaStarProjectileC2,
	// Token: 0x0400100A RID: 4106
	SamuraiBowProjectile,
	// Token: 0x0400100B RID: 4107
	SamuraiBowProjectileTrail,
	// Token: 0x0400100C RID: 4108
	SamuraiBowProjectileSurfaceImpactFX,
	// Token: 0x0400100D RID: 4109
	SamuraiBowProjectilePlayerImpactFX,
	// Token: 0x0400100E RID: 4110
	DragonSlingProjectile,
	// Token: 0x0400100F RID: 4111
	DragonSlingProjectileTrail,
	// Token: 0x04001010 RID: 4112
	DragonSlingProjectileSurfaceImpactFX,
	// Token: 0x04001011 RID: 4113
	DragonSlingProjectilePlayerImpactFX,
	// Token: 0x04001012 RID: 4114
	FireballProjectile,
	// Token: 0x04001013 RID: 4115
	StealthHandTapFX,
	// Token: 0x04001014 RID: 4116
	EnvPieceTree01,
	// Token: 0x04001015 RID: 4117
	FxSnapPiecePlaced,
	// Token: 0x04001016 RID: 4118
	FxSnapPieceDisconnected,
	// Token: 0x04001017 RID: 4119
	FxSnapPieceGrabbed,
	// Token: 0x04001018 RID: 4120
	FxSnapPieceLocationLock,
	// Token: 0x04001019 RID: 4121
	CyberNinjaStarProjectile,
	// Token: 0x0400101A RID: 4122
	RoomLight,
	// Token: 0x0400101B RID: 4123
	SamplesInfoPanel,
	// Token: 0x0400101C RID: 4124
	GorillaHandLeft,
	// Token: 0x0400101D RID: 4125
	GorillaHandRight,
	// Token: 0x0400101E RID: 4126
	GorillaHandSocket,
	// Token: 0x0400101F RID: 4127
	PlayingCardProjectile,
	// Token: 0x04001020 RID: 4128
	RottenPumpkinProjectile,
	// Token: 0x04001021 RID: 4129
	FxSnapPieceRecycle,
	// Token: 0x04001022 RID: 4130
	FxSnapPieceDispenser,
	// Token: 0x04001023 RID: 4131
	AppleProjectile,
	// Token: 0x04001024 RID: 4132
	AppleProjectileSurfaceImpactFX,
	// Token: 0x04001025 RID: 4133
	RecyclerForceVolumeFX,
	// Token: 0x04001026 RID: 4134
	FxSnapPieceTooHeavy,
	// Token: 0x04001027 RID: 4135
	FxBuilderPrivatePlotClaimed,
	// Token: 0x04001028 RID: 4136
	TrickTreatCandy,
	// Token: 0x04001029 RID: 4137
	TrickTreatEyeball,
	// Token: 0x0400102A RID: 4138
	TrickTreatBat,
	// Token: 0x0400102B RID: 4139
	TrickTreatBomb,
	// Token: 0x0400102C RID: 4140
	TrickTreatSurfaceImpact,
	// Token: 0x0400102D RID: 4141
	TrickTreatBatImpact,
	// Token: 0x0400102E RID: 4142
	TrickTreatBombImpact,
	// Token: 0x0400102F RID: 4143
	GuardianSlapFX,
	// Token: 0x04001030 RID: 4144
	GuardianSlamFX,
	// Token: 0x04001031 RID: 4145
	GuardianIdolLandedFX,
	// Token: 0x04001032 RID: 4146
	GuardianIdolFallFX,
	// Token: 0x04001033 RID: 4147
	GuardianIdolTappedFX,
	// Token: 0x04001034 RID: 4148
	VotingRockProjectile,
	// Token: 0x04001035 RID: 4149
	LeafPileImpactFXMedium,
	// Token: 0x04001036 RID: 4150
	LeafPileImpactFXSmall,
	// Token: 0x04001037 RID: 4151
	WoodenSword,
	// Token: 0x04001038 RID: 4152
	WoodenShield,
	// Token: 0x04001039 RID: 4153
	FxBuilderShrink,
	// Token: 0x0400103A RID: 4154
	FxBuilderGrow,
	// Token: 0x0400103B RID: 4155
	FxSnapPieceWreathJump,
	// Token: 0x0400103C RID: 4156
	ElfLauncherElf,
	// Token: 0x0400103D RID: 4157
	RubberBandCar,
	// Token: 0x0400103E RID: 4158
	SnowPileImpactFX,
	// Token: 0x0400103F RID: 4159
	FirecrackersProjectile,
	// Token: 0x04001040 RID: 4160
	PaperAirplaneSquareProjectile,
	// Token: 0x04001041 RID: 4161
	SmokeBombProjectile,
	// Token: 0x04001042 RID: 4162
	ThrowableHeartProjectile,
	// Token: 0x04001043 RID: 4163
	SunFlowers,
	// Token: 0x04001044 RID: 4164
	RobotCannonProjectile,
	// Token: 0x04001045 RID: 4165
	RobotCannonProjectileImpact,
	// Token: 0x04001046 RID: 4166
	SmokeBombExplosionEffect,
	// Token: 0x04001047 RID: 4167
	FireCrackerExplosionEffect,
	// Token: 0x04001048 RID: 4168
	GorillaMouth
}
