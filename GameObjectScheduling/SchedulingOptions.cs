using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000E2D RID: 3629
	[CreateAssetMenu(fileName = "New Options", menuName = "Game Object Scheduling/Options", order = 0)]
	public class SchedulingOptions : ScriptableObject
	{
		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06005AC3 RID: 23235 RVA: 0x001B9DB5 File Offset: 0x001B7FB5
		public DateTime DtDebugServerTime
		{
			get
			{
				return this.dtDebugServerTime.AddSeconds((double)(Time.time * this.timescale));
			}
		}

		// Token: 0x04005ECA RID: 24266
		[SerializeField]
		private string debugServerTime;

		// Token: 0x04005ECB RID: 24267
		[SerializeField]
		private DateTime dtDebugServerTime;

		// Token: 0x04005ECC RID: 24268
		[SerializeField]
		[Range(-60f, 3660f)]
		private float timescale = 1f;
	}
}
