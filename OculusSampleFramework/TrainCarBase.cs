using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BF7 RID: 3063
	public abstract class TrainCarBase : MonoBehaviour
	{
		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06004BAA RID: 19370 RVA: 0x00166CF5 File Offset: 0x00164EF5
		// (set) Token: 0x06004BAB RID: 19371 RVA: 0x00166CFD File Offset: 0x00164EFD
		public float Distance { get; protected set; }

		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06004BAC RID: 19372 RVA: 0x00166D06 File Offset: 0x00164F06
		// (set) Token: 0x06004BAD RID: 19373 RVA: 0x00166D0E File Offset: 0x00164F0E
		public float Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		// Token: 0x06004BAE RID: 19374 RVA: 0x000023F4 File Offset: 0x000005F4
		protected virtual void Awake()
		{
		}

		// Token: 0x06004BAF RID: 19375 RVA: 0x00166D18 File Offset: 0x00164F18
		public void UpdatePose(float distance, TrainCarBase train, Pose pose)
		{
			distance = (train._trainTrack.TrackLength + distance) % train._trainTrack.TrackLength;
			if (distance < 0f)
			{
				distance += train._trainTrack.TrackLength;
			}
			TrackSegment segment = train._trainTrack.GetSegment(distance);
			float num = distance - segment.StartDistance;
			segment.UpdatePose(num, pose);
		}

		// Token: 0x06004BB0 RID: 19376 RVA: 0x00166D78 File Offset: 0x00164F78
		protected void UpdateCarPosition()
		{
			this.UpdatePose(this.Distance + this._frontWheels.transform.localPosition.z * this.scale, this, this._frontPose);
			this.UpdatePose(this.Distance + this._rearWheels.transform.localPosition.z * this.scale, this, this._rearPose);
			Vector3 vector = 0.5f * (this._frontPose.Position + this._rearPose.Position);
			Vector3 vector2 = this._frontPose.Position - this._rearPose.Position;
			base.transform.position = vector + TrainCarBase.OFFSET;
			base.transform.rotation = Quaternion.LookRotation(vector2, base.transform.up);
			this._frontWheels.transform.rotation = this._frontPose.Rotation;
			this._rearWheels.transform.rotation = this._rearPose.Rotation;
		}

		// Token: 0x06004BB1 RID: 19377 RVA: 0x00166E90 File Offset: 0x00165090
		protected void RotateCarWheels()
		{
			float num = this.Distance / 0.027f % 6.2831855f;
			Transform[] individualWheels = this._individualWheels;
			for (int i = 0; i < individualWheels.Length; i++)
			{
				individualWheels[i].localRotation = Quaternion.AngleAxis(57.29578f * num, Vector3.right);
			}
		}

		// Token: 0x06004BB2 RID: 19378
		public abstract void UpdatePosition();

		// Token: 0x04004E42 RID: 20034
		private static Vector3 OFFSET = new Vector3(0f, 0.0195f, 0f);

		// Token: 0x04004E43 RID: 20035
		private const float WHEEL_RADIUS = 0.027f;

		// Token: 0x04004E44 RID: 20036
		private const float TWO_PI = 6.2831855f;

		// Token: 0x04004E45 RID: 20037
		[SerializeField]
		protected Transform _frontWheels;

		// Token: 0x04004E46 RID: 20038
		[SerializeField]
		protected Transform _rearWheels;

		// Token: 0x04004E47 RID: 20039
		[SerializeField]
		protected TrainTrack _trainTrack;

		// Token: 0x04004E48 RID: 20040
		[SerializeField]
		protected Transform[] _individualWheels;

		// Token: 0x04004E4A RID: 20042
		protected float scale = 1f;

		// Token: 0x04004E4B RID: 20043
		private Pose _frontPose = new Pose();

		// Token: 0x04004E4C RID: 20044
		private Pose _rearPose = new Pose();
	}
}
