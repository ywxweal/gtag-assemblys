using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BF6 RID: 3062
	public class TrainCar : TrainCarBase
	{
		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06004BA7 RID: 19367 RVA: 0x00166D7D File Offset: 0x00164F7D
		public float DistanceBehindParentScaled
		{
			get
			{
				return this.scale * this._distanceBehindParent;
			}
		}

		// Token: 0x06004BA8 RID: 19368 RVA: 0x00166D8C File Offset: 0x00164F8C
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x06004BA9 RID: 19369 RVA: 0x00166D94 File Offset: 0x00164F94
		public override void UpdatePosition()
		{
			base.Distance = this._parentLocomotive.Distance - this.DistanceBehindParentScaled;
			base.UpdateCarPosition();
			base.RotateCarWheels();
		}

		// Token: 0x04004E41 RID: 20033
		[SerializeField]
		private TrainCarBase _parentLocomotive;

		// Token: 0x04004E42 RID: 20034
		[SerializeField]
		protected float _distanceBehindParent = 0.1f;
	}
}
