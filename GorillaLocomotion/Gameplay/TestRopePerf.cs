using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE6 RID: 3302
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x060051F7 RID: 20983 RVA: 0x0018E048 File Offset: 0x0018C248
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x04005619 RID: 22041
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x0400561A RID: 22042
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x0400561B RID: 22043
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
