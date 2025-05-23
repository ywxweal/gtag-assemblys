using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000CF0 RID: 3312
	public class GorillaVelocityTracker : MonoBehaviour
	{
		// Token: 0x0600522F RID: 21039 RVA: 0x0018FEB8 File Offset: 0x0018E0B8
		public void ResetState()
		{
			this.trans = base.transform;
			this.localSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|10_0(this.localSpaceData);
			this.worldSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|10_0(this.worldSpaceData);
			this.isRelativeTo = this.relativeTo != null;
			this.lastLocalSpacePos = this.GetPosition(false);
			this.lastWorldSpacePos = this.GetPosition(true);
		}

		// Token: 0x06005230 RID: 21040 RVA: 0x0018FF37 File Offset: 0x0018E137
		private void Awake()
		{
			this.ResetState();
		}

		// Token: 0x06005231 RID: 21041 RVA: 0x0018FF37 File Offset: 0x0018E137
		private void OnDisable()
		{
			this.ResetState();
		}

		// Token: 0x06005232 RID: 21042 RVA: 0x0018FF3F File Offset: 0x0018E13F
		private Vector3 GetPosition(bool worldSpace)
		{
			if (worldSpace)
			{
				return this.trans.position;
			}
			if (this.isRelativeTo)
			{
				return this.relativeTo.InverseTransformPoint(this.trans.position);
			}
			return this.trans.localPosition;
		}

		// Token: 0x06005233 RID: 21043 RVA: 0x0018FF7A File Offset: 0x0018E17A
		private void Update()
		{
			this.Tick();
		}

		// Token: 0x06005234 RID: 21044 RVA: 0x0018FF84 File Offset: 0x0018E184
		public void Tick()
		{
			if (Time.frameCount <= this.lastTickedFrame)
			{
				return;
			}
			Vector3 position = this.GetPosition(false);
			Vector3 position2 = this.GetPosition(true);
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = this.localSpaceData[this.currentDataPointIndex];
			velocityDataPoint.delta = (position - this.lastLocalSpacePos) / Time.deltaTime;
			velocityDataPoint.time = Time.time;
			this.localSpaceData[this.currentDataPointIndex] = velocityDataPoint;
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint2 = this.worldSpaceData[this.currentDataPointIndex];
			velocityDataPoint2.delta = (position2 - this.lastWorldSpacePos) / Time.deltaTime;
			velocityDataPoint2.time = Time.time;
			this.worldSpaceData[this.currentDataPointIndex] = velocityDataPoint2;
			this.lastLocalSpacePos = position;
			this.lastWorldSpacePos = position2;
			this.currentDataPointIndex++;
			if (this.currentDataPointIndex >= this.maxDataPoints)
			{
				this.currentDataPointIndex = 0;
			}
			this.lastTickedFrame = Time.frameCount;
		}

		// Token: 0x06005235 RID: 21045 RVA: 0x00190071 File Offset: 0x0018E271
		private void AddToQueue(ref List<GorillaVelocityTracker.VelocityDataPoint> dataPoints, GorillaVelocityTracker.VelocityDataPoint newData)
		{
			dataPoints.Add(newData);
			if (dataPoints.Count >= this.maxDataPoints)
			{
				dataPoints.RemoveAt(0);
			}
		}

		// Token: 0x06005236 RID: 21046 RVA: 0x00190094 File Offset: 0x0018E294
		public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
		{
			float num = maxTimeFromPast / 2f;
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return Vector3.zero;
			}
			GorillaVelocityTracker.<>c__DisplayClass17_0 CS$<>8__locals1;
			CS$<>8__locals1.total = Vector3.zero;
			CS$<>8__locals1.totalMag = 0f;
			CS$<>8__locals1.added = 0;
			float num2 = Time.time - maxTimeFromPast;
			float num3 = Time.time - num;
			int i = 0;
			int num4 = this.currentDataPointIndex;
			while (i < this.maxDataPoints)
			{
				GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = array[num4];
				if (doMagnitudeCheck && CS$<>8__locals1.added > 1 && velocityDataPoint.time >= num3)
				{
					if (velocityDataPoint.delta.magnitude >= CS$<>8__locals1.totalMag / (float)CS$<>8__locals1.added)
					{
						GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|17_0(velocityDataPoint, ref CS$<>8__locals1);
					}
				}
				else if (velocityDataPoint.time >= num2)
				{
					GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|17_0(velocityDataPoint, ref CS$<>8__locals1);
				}
				num4++;
				if (num4 >= this.maxDataPoints)
				{
					num4 = 0;
				}
				i++;
			}
			if (CS$<>8__locals1.added > 0)
			{
				return CS$<>8__locals1.total / (float)CS$<>8__locals1.added;
			}
			return Vector3.zero;
		}

		// Token: 0x06005237 RID: 21047 RVA: 0x001901A4 File Offset: 0x0018E3A4
		public Vector3 GetLatestVelocity(bool worldSpace = false)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			return array[this.currentDataPointIndex].delta;
		}

		// Token: 0x06005238 RID: 21048 RVA: 0x001901D4 File Offset: 0x0018E3D4
		public float GetAverageSpeedChangeMagnitudeInDirection(Vector3 dir, bool worldSpace = false, float maxTimeFromPast = 0.05f)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = 0;
			float num3 = Time.time - maxTimeFromPast;
			bool flag = false;
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].time >= num3)
				{
					if (!flag)
					{
						vector = array[i].delta;
						flag = true;
					}
					else
					{
						num += Mathf.Abs(Vector3.Dot(array[i].delta - vector, dir));
						num2++;
					}
				}
			}
			if (num2 <= 0)
			{
				return 0f;
			}
			return num / (float)num2;
		}

		// Token: 0x0600523A RID: 21050 RVA: 0x00190294 File Offset: 0x0018E494
		[CompilerGenerated]
		private void <ResetState>g__PopulateArray|10_0(GorillaVelocityTracker.VelocityDataPoint[] array)
		{
			for (int i = 0; i < this.maxDataPoints; i++)
			{
				array[i] = new GorillaVelocityTracker.VelocityDataPoint();
			}
		}

		// Token: 0x0600523B RID: 21051 RVA: 0x001902BC File Offset: 0x0018E4BC
		[CompilerGenerated]
		internal static void <GetAverageVelocity>g__AddPoint|17_0(GorillaVelocityTracker.VelocityDataPoint point, ref GorillaVelocityTracker.<>c__DisplayClass17_0 A_1)
		{
			A_1.total += point.delta;
			A_1.totalMag += point.delta.magnitude;
			int added = A_1.added;
			A_1.added = added + 1;
		}

		// Token: 0x04005668 RID: 22120
		[SerializeField]
		private int maxDataPoints = 20;

		// Token: 0x04005669 RID: 22121
		[SerializeField]
		private Transform relativeTo;

		// Token: 0x0400566A RID: 22122
		private int currentDataPointIndex;

		// Token: 0x0400566B RID: 22123
		private GorillaVelocityTracker.VelocityDataPoint[] localSpaceData;

		// Token: 0x0400566C RID: 22124
		private GorillaVelocityTracker.VelocityDataPoint[] worldSpaceData;

		// Token: 0x0400566D RID: 22125
		private Transform trans;

		// Token: 0x0400566E RID: 22126
		private Vector3 lastWorldSpacePos;

		// Token: 0x0400566F RID: 22127
		private Vector3 lastLocalSpacePos;

		// Token: 0x04005670 RID: 22128
		private bool isRelativeTo;

		// Token: 0x04005671 RID: 22129
		private int lastTickedFrame = -1;

		// Token: 0x02000CF1 RID: 3313
		public class VelocityDataPoint
		{
			// Token: 0x04005672 RID: 22130
			public Vector3 delta;

			// Token: 0x04005673 RID: 22131
			public float time = -1f;
		}
	}
}
