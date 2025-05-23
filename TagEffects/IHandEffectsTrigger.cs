using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CB4 RID: 3252
	public interface IHandEffectsTrigger
	{
		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x0600506F RID: 20591
		IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x06005070 RID: 20592
		Transform Transform { get; }

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x06005071 RID: 20593
		VRRig Rig { get; }

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x06005072 RID: 20594
		bool FingersDown { get; }

		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x06005073 RID: 20595
		bool FingersUp { get; }

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x06005074 RID: 20596
		Vector3 Velocity { get; }

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x06005075 RID: 20597
		bool RightHand { get; }

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x06005076 RID: 20598
		TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x06005077 RID: 20599
		bool Static { get; }

		// Token: 0x06005078 RID: 20600
		void OnTriggerEntered(IHandEffectsTrigger other);

		// Token: 0x06005079 RID: 20601
		bool InTriggerZone(IHandEffectsTrigger t);

		// Token: 0x02000CB5 RID: 3253
		public enum Mode
		{
			// Token: 0x04005390 RID: 21392
			HighFive,
			// Token: 0x04005391 RID: 21393
			FistBump,
			// Token: 0x04005392 RID: 21394
			Tag3P,
			// Token: 0x04005393 RID: 21395
			Tag1P,
			// Token: 0x04005394 RID: 21396
			HighFive_And_FistBump
		}
	}
}
