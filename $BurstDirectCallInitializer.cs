using System;
using UnityEngine;

// Token: 0x02000EDD RID: 3805
internal static class $BurstDirectCallInitializer
{
	// Token: 0x06005E02 RID: 24066 RVA: 0x001CD6F8 File Offset: 0x001CB8F8
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		Bindings.Vec3Functions.New_00003402$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Add_00003403$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Sub_00003404$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Mul_00003405$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Div_00003406$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Unm_00003407$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Eq_00003408$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Dot_0000340A$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Cross_0000340B$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Project_0000340C$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Length_0000340D$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Normalize_0000340E$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.SafeNormal_0000340F$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Distance_00003410$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Lerp_00003411$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.Rotate_00003412$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.ZeroVector_00003413$BurstDirectCall.Initialize();
		Bindings.Vec3Functions.OneVector_00003414$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.New_00003415$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.Mul_00003416$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.Eq_00003417$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.FromEuler_00003419$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.FromDirection_0000341A$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.GetUpVector_0000341B$BurstDirectCall.Initialize();
		Bindings.QuatFunctions.Euler_0000341C$BurstDirectCall.Initialize();
		BurstClassInfo.Index_0000341F$BurstDirectCall.Initialize();
		BurstClassInfo.NewIndex_00003420$BurstDirectCall.Initialize();
		BurstClassInfo.NameCall_00003421$BurstDirectCall.Initialize();
	}
}
