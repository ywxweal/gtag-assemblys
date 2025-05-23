using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CB6 RID: 3254
	[CreateAssetMenu(fileName = "New Tag Effect Pack", menuName = "Tag Effect Pack")]
	public class TagEffectPack : ScriptableObject
	{
		// Token: 0x04005395 RID: 21397
		public GameObject thirdPerson;

		// Token: 0x04005396 RID: 21398
		public bool thirdPersonParentEffect = true;

		// Token: 0x04005397 RID: 21399
		public GameObject firstPerson;

		// Token: 0x04005398 RID: 21400
		public bool firstPersonParentEffect = true;

		// Token: 0x04005399 RID: 21401
		public GameObject highFive;

		// Token: 0x0400539A RID: 21402
		public bool highFiveParentEffect;

		// Token: 0x0400539B RID: 21403
		public GameObject fistBump;

		// Token: 0x0400539C RID: 21404
		public bool fistBumpParentEffect;

		// Token: 0x0400539D RID: 21405
		public bool shouldFaceTagger;
	}
}
