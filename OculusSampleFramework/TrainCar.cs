using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BF6 RID: 3062
	public class TrainCar : TrainCarBase
	{
		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06004BA6 RID: 19366 RVA: 0x00166CA5 File Offset: 0x00164EA5
		public float DistanceBehindParentScaled
		{
			get
			{
				return this.scale * this._distanceBehindParent;
			}
		}

		// Token: 0x06004BA7 RID: 19367 RVA: 0x00166CB4 File Offset: 0x00164EB4
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x06004BA8 RID: 19368 RVA: 0x00166CBC File Offset: 0x00164EBC
		public override void UpdatePosition()
		{
			base.Distance = this._parentLocomotive.Distance - this.DistanceBehindParentScaled;
			base.UpdateCarPosition();
			base.RotateCarWheels();
		}

		// Token: 0x04004E40 RID: 20032
		[SerializeField]
		private TrainCarBase _parentLocomotive;

		// Token: 0x04004E41 RID: 20033
		[SerializeField]
		protected float _distanceBehindParent = 0.1f;
	}
}
