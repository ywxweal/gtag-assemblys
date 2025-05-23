using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000D25 RID: 3365
	public class DayNightWatchWearable : MonoBehaviour
	{
		// Token: 0x06005417 RID: 21527 RVA: 0x00197794 File Offset: 0x00195994
		private void Start()
		{
			if (!this.dayNightManager)
			{
				this.dayNightManager = BetterDayNightManager.instance;
			}
			this.rotationDegree = 0f;
			if (this.clockNeedle)
			{
				this.initialRotation = this.clockNeedle.localRotation;
			}
		}

		// Token: 0x06005418 RID: 21528 RVA: 0x001977E4 File Offset: 0x001959E4
		private void Update()
		{
			this.currentTimeOfDay = this.dayNightManager.currentTimeOfDay;
			double currentTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).currentTimeInSeconds;
			double totalTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).totalTimeInSeconds;
			this.rotationDegree = (float)(360.0 * currentTimeInSeconds / totalTimeInSeconds);
			this.rotationDegree = Mathf.Floor(this.rotationDegree);
			if (this.clockNeedle)
			{
				this.clockNeedle.localRotation = this.initialRotation * Quaternion.AngleAxis(this.rotationDegree, this.needleRotationAxis);
			}
		}

		// Token: 0x0400570F RID: 22287
		[Tooltip("The transform that will be rotated to indicate the current time.")]
		public Transform clockNeedle;

		// Token: 0x04005710 RID: 22288
		[FormerlySerializedAs("dialRotationAxis")]
		[Tooltip("The axis that the needle will rotate around.")]
		public Vector3 needleRotationAxis = Vector3.right;

		// Token: 0x04005711 RID: 22289
		private BetterDayNightManager dayNightManager;

		// Token: 0x04005712 RID: 22290
		[DebugOption]
		private float rotationDegree;

		// Token: 0x04005713 RID: 22291
		private string currentTimeOfDay;

		// Token: 0x04005714 RID: 22292
		private Quaternion initialRotation;
	}
}
