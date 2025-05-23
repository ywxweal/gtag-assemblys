using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CDE RID: 3294
	[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
	public class GorillaRopeSwingSettings : ScriptableObject
	{
		// Token: 0x040055DD RID: 21981
		public float inheritVelocityMultiplier = 1f;

		// Token: 0x040055DE RID: 21982
		public float frictionWhenNotHeld = 0.25f;
	}
}
