using System;
using GorillaTagScripts.Builder;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD5 RID: 2773
	public class SurfaceMover : MonoBehaviour
	{
		// Token: 0x0600430F RID: 17167 RVA: 0x00135B05 File Offset: 0x00133D05
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterSurfaceMover(this);
		}

		// Token: 0x06004310 RID: 17168 RVA: 0x00135B1E File Offset: 0x00133D1E
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterSurfaceMover(this);
			}
		}

		// Token: 0x06004311 RID: 17169 RVA: 0x00135B38 File Offset: 0x00133D38
		public void InitMovingSurface()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / this.velocity;
				this.cycleDuration = num + this.cycleDelay;
				float num2 = this.cycleDelay / this.cycleDuration;
				Vector2 vector = new Vector2(num2 / 2f, 0f);
				Vector2 vector2 = new Vector2(1f - num2 / 2f, 1f);
				float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
				this.lerpAlpha = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(num2 / 2f, 0f, 0f, num3),
					new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
				});
			}
			else
			{
				this.cycleDuration = 1f / this.velocity;
			}
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x06004312 RID: 17170 RVA: 0x00135C9F File Offset: 0x00133E9F
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06004313 RID: 17171 RVA: 0x00135CC8 File Offset: 0x00133EC8
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06004314 RID: 17172 RVA: 0x00135CD8 File Offset: 0x00133ED8
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004315 RID: 17173 RVA: 0x00135D03 File Offset: 0x00133F03
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06004316 RID: 17174 RVA: 0x00135D13 File Offset: 0x00133F13
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06004317 RID: 17175 RVA: 0x00135D33 File Offset: 0x00133F33
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06004318 RID: 17176 RVA: 0x00135D40 File Offset: 0x00133F40
		public void Move()
		{
			this.Progress();
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x06004319 RID: 17177 RVA: 0x00135D88 File Offset: 0x00133F88
		private Vector3 UpdatePointToPoint(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, num);
		}

		// Token: 0x0600431A RID: 17178 RVA: 0x00135DC0 File Offset: 0x00133FC0
		private void UpdateRotation(float perc)
		{
			Quaternion quaternion = Quaternion.AngleAxis(perc * 360f, Vector3.up);
			base.transform.localRotation = quaternion;
		}

		// Token: 0x0600431B RID: 17179 RVA: 0x00135DEC File Offset: 0x00133FEC
		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		// Token: 0x04004588 RID: 17800
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x04004589 RID: 17801
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x0400458A RID: 17802
		[SerializeField]
		private float velocity;

		// Token: 0x0400458B RID: 17803
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x0400458C RID: 17804
		[SerializeField]
		private bool reverseDir;

		// Token: 0x0400458D RID: 17805
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x0400458E RID: 17806
		[SerializeField]
		protected Transform startXf;

		// Token: 0x0400458F RID: 17807
		[SerializeField]
		protected Transform endXf;

		// Token: 0x04004590 RID: 17808
		private AnimationCurve lerpAlpha;

		// Token: 0x04004591 RID: 17809
		private float cycleDuration;

		// Token: 0x04004592 RID: 17810
		private float distance;

		// Token: 0x04004593 RID: 17811
		private float currT;

		// Token: 0x04004594 RID: 17812
		private float percent;

		// Token: 0x04004595 RID: 17813
		private bool currForward;

		// Token: 0x04004596 RID: 17814
		private float dtSinceServerUpdate;

		// Token: 0x04004597 RID: 17815
		private int lastServerTimeStamp;

		// Token: 0x04004598 RID: 17816
		private float rotateStartAmt;

		// Token: 0x04004599 RID: 17817
		private float rotateAmt;

		// Token: 0x0400459A RID: 17818
		private uint startPercentageCycleOffset;
	}
}
