using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BFD RID: 3069
	public class TrainTrack : MonoBehaviour
	{
		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06004BDA RID: 19418 RVA: 0x00167938 File Offset: 0x00165B38
		// (set) Token: 0x06004BDB RID: 19419 RVA: 0x00167940 File Offset: 0x00165B40
		public float TrackLength
		{
			get
			{
				return this._trainLength;
			}
			private set
			{
				this._trainLength = value;
			}
		}

		// Token: 0x06004BDC RID: 19420 RVA: 0x00167949 File Offset: 0x00165B49
		private void Awake()
		{
			this.Regenerate();
		}

		// Token: 0x06004BDD RID: 19421 RVA: 0x00167954 File Offset: 0x00165B54
		public TrackSegment GetSegment(float distance)
		{
			int childCount = this._segmentParent.childCount;
			for (int i = 0; i < childCount; i++)
			{
				TrackSegment trackSegment = this._trackSegments[i];
				TrackSegment trackSegment2 = this._trackSegments[(i + 1) % childCount];
				if (distance >= trackSegment.StartDistance && (distance < trackSegment2.StartDistance || i == childCount - 1))
				{
					return trackSegment;
				}
			}
			return null;
		}

		// Token: 0x06004BDE RID: 19422 RVA: 0x001679AC File Offset: 0x00165BAC
		public void Regenerate()
		{
			this._trackSegments = this._segmentParent.GetComponentsInChildren<TrackSegment>();
			this.TrackLength = 0f;
			int childCount = this._segmentParent.childCount;
			TrackSegment trackSegment = null;
			float num = 0f;
			for (int i = 0; i < childCount; i++)
			{
				TrackSegment trackSegment2 = this._trackSegments[i];
				trackSegment2.SubDivCount = this._subDivCount;
				num = trackSegment2.setGridSize(this._gridSize);
				if (trackSegment != null)
				{
					Pose endPose = trackSegment.EndPose;
					trackSegment2.transform.position = endPose.Position;
					trackSegment2.transform.rotation = endPose.Rotation;
					trackSegment2.StartDistance = this.TrackLength;
				}
				if (this._regnerateTrackMeshOnAwake)
				{
					trackSegment2.RegenerateTrackAndMesh();
				}
				this.TrackLength += trackSegment2.SegmentLength;
				trackSegment = trackSegment2;
			}
			this.SetScale(num);
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x00167A94 File Offset: 0x00165C94
		private void SetScale(float ratio)
		{
			this._trainParent.localScale = new Vector3(ratio, ratio, ratio);
			TrainCar[] componentsInChildren = this._trainParent.GetComponentsInChildren<TrainCar>();
			this._trainParent.GetComponentInChildren<TrainLocomotive>().Scale = ratio;
			TrainCar[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Scale = ratio;
			}
		}

		// Token: 0x04004E8E RID: 20110
		[SerializeField]
		private float _gridSize = 0.5f;

		// Token: 0x04004E8F RID: 20111
		[SerializeField]
		private int _subDivCount = 20;

		// Token: 0x04004E90 RID: 20112
		[SerializeField]
		private Transform _segmentParent;

		// Token: 0x04004E91 RID: 20113
		[SerializeField]
		private Transform _trainParent;

		// Token: 0x04004E92 RID: 20114
		[SerializeField]
		private bool _regnerateTrackMeshOnAwake;

		// Token: 0x04004E93 RID: 20115
		private float _trainLength = -1f;

		// Token: 0x04004E94 RID: 20116
		private TrackSegment[] _trackSegments;
	}
}
