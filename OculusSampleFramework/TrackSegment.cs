﻿using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BF2 RID: 3058
	public class TrackSegment : MonoBehaviour
	{
		// Token: 0x17000776 RID: 1910
		// (get) Token: 0x06004B84 RID: 19332 RVA: 0x00166227 File Offset: 0x00164427
		// (set) Token: 0x06004B85 RID: 19333 RVA: 0x0016622F File Offset: 0x0016442F
		public float StartDistance { get; set; }

		// Token: 0x17000777 RID: 1911
		// (get) Token: 0x06004B86 RID: 19334 RVA: 0x00166238 File Offset: 0x00164438
		// (set) Token: 0x06004B87 RID: 19335 RVA: 0x00166240 File Offset: 0x00164440
		public float GridSize
		{
			get
			{
				return this._gridSize;
			}
			private set
			{
				this._gridSize = value;
			}
		}

		// Token: 0x17000778 RID: 1912
		// (get) Token: 0x06004B88 RID: 19336 RVA: 0x00166249 File Offset: 0x00164449
		// (set) Token: 0x06004B89 RID: 19337 RVA: 0x00166251 File Offset: 0x00164451
		public int SubDivCount
		{
			get
			{
				return this._subDivCount;
			}
			set
			{
				this._subDivCount = value;
			}
		}

		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x06004B8A RID: 19338 RVA: 0x0016625A File Offset: 0x0016445A
		public TrackSegment.SegmentType Type
		{
			get
			{
				return this._segmentType;
			}
		}

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x06004B8B RID: 19339 RVA: 0x00166262 File Offset: 0x00164462
		public Pose EndPose
		{
			get
			{
				this.UpdatePose(this.SegmentLength, this._endPose);
				return this._endPose;
			}
		}

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x06004B8C RID: 19340 RVA: 0x0016627C File Offset: 0x0016447C
		public float Radius
		{
			get
			{
				return 0.5f * this.GridSize;
			}
		}

		// Token: 0x06004B8D RID: 19341 RVA: 0x0016628A File Offset: 0x0016448A
		public float setGridSize(float size)
		{
			this.GridSize = size;
			return this.GridSize / 0.8f;
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06004B8E RID: 19342 RVA: 0x001662A0 File Offset: 0x001644A0
		public float SegmentLength
		{
			get
			{
				TrackSegment.SegmentType type = this.Type;
				if (type == TrackSegment.SegmentType.Straight)
				{
					return this.GridSize;
				}
				if (type - TrackSegment.SegmentType.LeftTurn > 1)
				{
					return 1f;
				}
				return 1.5707964f * this.Radius;
			}
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Awake()
		{
		}

		// Token: 0x06004B90 RID: 19344 RVA: 0x001662D8 File Offset: 0x001644D8
		public void UpdatePose(float distanceIntoSegment, Pose pose)
		{
			if (this.Type == TrackSegment.SegmentType.Straight)
			{
				pose.Position = base.transform.position + distanceIntoSegment * base.transform.forward;
				pose.Rotation = base.transform.rotation;
				return;
			}
			if (this.Type == TrackSegment.SegmentType.LeftTurn)
			{
				float num = distanceIntoSegment / this.SegmentLength;
				float num2 = 1.5707964f * num;
				Vector3 vector = new Vector3(this.Radius * Mathf.Cos(num2) - this.Radius, 0f, this.Radius * Mathf.Sin(num2));
				Quaternion quaternion = Quaternion.Euler(0f, -num2 * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(vector);
				pose.Rotation = base.transform.rotation * quaternion;
				return;
			}
			if (this.Type == TrackSegment.SegmentType.RightTurn)
			{
				float num3 = 3.1415927f - 1.5707964f * distanceIntoSegment / this.SegmentLength;
				Vector3 vector2 = new Vector3(this.Radius * Mathf.Cos(num3) + this.Radius, 0f, this.Radius * Mathf.Sin(num3));
				Quaternion quaternion2 = Quaternion.Euler(0f, (3.1415927f - num3) * 57.29578f, 0f);
				pose.Position = base.transform.TransformPoint(vector2);
				pose.Rotation = base.transform.rotation * quaternion2;
				return;
			}
			pose.Position = Vector3.zero;
			pose.Rotation = Quaternion.identity;
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Update()
		{
		}

		// Token: 0x06004B92 RID: 19346 RVA: 0x00166464 File Offset: 0x00164664
		private void OnDisable()
		{
			Object.Destroy(this._mesh);
		}

		// Token: 0x06004B93 RID: 19347 RVA: 0x00166474 File Offset: 0x00164674
		private void DrawDebugLines()
		{
			for (int i = 1; i < this.SubDivCount + 1; i++)
			{
				float num = this.SegmentLength / (float)this.SubDivCount;
				this.UpdatePose((float)(i - 1) * num, this._p1);
				this.UpdatePose((float)i * num, this._p2);
				float num2 = 0.075f;
				Debug.DrawLine(this._p1.Position + num2 * (this._p1.Rotation * Vector3.right), this._p2.Position + num2 * (this._p2.Rotation * Vector3.right));
				Debug.DrawLine(this._p1.Position - num2 * (this._p1.Rotation * Vector3.right), this._p2.Position - num2 * (this._p2.Rotation * Vector3.right));
			}
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right, base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position + 0.5f * this.GridSize * base.transform.right, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
			Debug.DrawLine(base.transform.position - 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, base.transform.position + 0.5f * this.GridSize * base.transform.right + this.GridSize * base.transform.forward, Color.yellow);
		}

		// Token: 0x06004B94 RID: 19348 RVA: 0x0016677C File Offset: 0x0016497C
		public void RegenerateTrackAndMesh()
		{
			if (base.transform.childCount > 0 && !this._mesh)
			{
				this._mesh = base.transform.GetChild(0).gameObject;
			}
			if (this._mesh)
			{
				Object.DestroyImmediate(this._mesh);
			}
			if (this._segmentType == TrackSegment.SegmentType.LeftTurn)
			{
				this._mesh = Object.Instantiate<GameObject>(this._leftTurn.gameObject);
			}
			else if (this._segmentType == TrackSegment.SegmentType.RightTurn)
			{
				this._mesh = Object.Instantiate<GameObject>(this._rightTurn.gameObject);
			}
			else
			{
				this._mesh = Object.Instantiate<GameObject>(this._straight.gameObject);
			}
			this._mesh.transform.SetParent(base.transform, false);
			this._mesh.transform.position += this.GridSize / 2f * base.transform.forward;
			this._mesh.transform.localScale = new Vector3(this.GridSize / 0.8f, this.GridSize / 0.8f, this.GridSize / 0.8f);
		}

		// Token: 0x04004E19 RID: 19993
		[SerializeField]
		private TrackSegment.SegmentType _segmentType;

		// Token: 0x04004E1A RID: 19994
		[SerializeField]
		private MeshFilter _straight;

		// Token: 0x04004E1B RID: 19995
		[SerializeField]
		private MeshFilter _leftTurn;

		// Token: 0x04004E1C RID: 19996
		[SerializeField]
		private MeshFilter _rightTurn;

		// Token: 0x04004E1D RID: 19997
		private float _gridSize = 0.8f;

		// Token: 0x04004E1E RID: 19998
		private int _subDivCount = 20;

		// Token: 0x04004E1F RID: 19999
		private const float _originalGridSize = 0.8f;

		// Token: 0x04004E20 RID: 20000
		private const float _trackWidth = 0.15f;

		// Token: 0x04004E21 RID: 20001
		private GameObject _mesh;

		// Token: 0x04004E23 RID: 20003
		private Pose _p1 = new Pose();

		// Token: 0x04004E24 RID: 20004
		private Pose _p2 = new Pose();

		// Token: 0x04004E25 RID: 20005
		private Pose _endPose = new Pose();

		// Token: 0x02000BF3 RID: 3059
		public enum SegmentType
		{
			// Token: 0x04004E27 RID: 20007
			Straight,
			// Token: 0x04004E28 RID: 20008
			LeftTurn,
			// Token: 0x04004E29 RID: 20009
			RightTurn,
			// Token: 0x04004E2A RID: 20010
			Switch
		}
	}
}
