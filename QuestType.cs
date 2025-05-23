using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x02000145 RID: 325
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestType
{
	// Token: 0x04000A08 RID: 2568
	none,
	// Token: 0x04000A09 RID: 2569
	gameModeObjective,
	// Token: 0x04000A0A RID: 2570
	gameModeRound,
	// Token: 0x04000A0B RID: 2571
	grabObject,
	// Token: 0x04000A0C RID: 2572
	dropObject,
	// Token: 0x04000A0D RID: 2573
	eatObject,
	// Token: 0x04000A0E RID: 2574
	tapObject,
	// Token: 0x04000A0F RID: 2575
	launchedProjectile,
	// Token: 0x04000A10 RID: 2576
	moveDistance,
	// Token: 0x04000A11 RID: 2577
	swimDistance,
	// Token: 0x04000A12 RID: 2578
	triggerHandEffect,
	// Token: 0x04000A13 RID: 2579
	enterLocation,
	// Token: 0x04000A14 RID: 2580
	misc,
	// Token: 0x04000A15 RID: 2581
	critter
}
