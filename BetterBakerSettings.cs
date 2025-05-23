using System;
using UnityEngine;

// Token: 0x02000978 RID: 2424
public class BetterBakerSettings : MonoBehaviour
{
	// Token: 0x04003F51 RID: 16209
	[SerializeField]
	public GameObject[] lightMapMaps = new GameObject[9];

	// Token: 0x02000979 RID: 2425
	[Serializable]
	public struct LightMapMap
	{
		// Token: 0x04003F52 RID: 16210
		[SerializeField]
		public string timeOfDayName;

		// Token: 0x04003F53 RID: 16211
		[SerializeField]
		public GameObject sceneLightObject;
	}
}
