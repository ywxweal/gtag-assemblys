using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE6 RID: 3302
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x060051F6 RID: 20982 RVA: 0x0018DF70 File Offset: 0x0018C170
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x04005618 RID: 22040
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x04005619 RID: 22041
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x0400561A RID: 22042
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
