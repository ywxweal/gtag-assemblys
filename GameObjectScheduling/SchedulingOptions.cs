using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000E2D RID: 3629
	[CreateAssetMenu(fileName = "New Options", menuName = "Game Object Scheduling/Options", order = 0)]
	public class SchedulingOptions : ScriptableObject
	{
		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06005AC4 RID: 23236 RVA: 0x001B9E8D File Offset: 0x001B808D
		public DateTime DtDebugServerTime
		{
			get
			{
				return this.dtDebugServerTime.AddSeconds((double)(Time.time * this.timescale));
			}
		}

		// Token: 0x04005ECB RID: 24267
		[SerializeField]
		private string debugServerTime;

		// Token: 0x04005ECC RID: 24268
		[SerializeField]
		private DateTime dtDebugServerTime;

		// Token: 0x04005ECD RID: 24269
		[SerializeField]
		[Range(-60f, 3660f)]
		private float timescale = 1f;
	}
}
