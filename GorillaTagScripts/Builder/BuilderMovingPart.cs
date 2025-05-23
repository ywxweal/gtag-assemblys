using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B53 RID: 2899
	public class BuilderMovingPart : MonoBehaviour
	{
		// Token: 0x0600477C RID: 18300 RVA: 0x00153D30 File Offset: 0x00151F30
		private void Awake()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				builderAttachGridPlane.movesOnPlace = true;
				builderAttachGridPlane.movingPart = this;
			}
			this.initLocalPos = base.transform.localPosition;
			this.initLocalRotation = base.transform.localRotation;
		}

		// Token: 0x0600477D RID: 18301 RVA: 0x00153D84 File Offset: 0x00151F84
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x0600477E RID: 18302 RVA: 0x00153DB9 File Offset: 0x00151FB9
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x0600477F RID: 18303 RVA: 0x00153DC8 File Offset: 0x00151FC8
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06004780 RID: 18304 RVA: 0x00153DF3 File Offset: 0x00151FF3
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06004781 RID: 18305 RVA: 0x00153E03 File Offset: 0x00152003
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06004782 RID: 18306 RVA: 0x00153E23 File Offset: 0x00152023
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06004783 RID: 18307 RVA: 0x00153E30 File Offset: 0x00152030
		public void ActivateAtNode(byte node, int timestamp)
		{
			float num = (float)node;
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (num >= this.startPercentage)
			{
				int num2 = (int)((num - this.startPercentage) * (float)this.CycleLengthMs());
				int num3 = timestamp - num2;
				if (flag)
				{
					num3 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = num3;
			}
			else
			{
				int num4 = (int)((num + 2f - this.startPercentage) * (float)this.CycleLengthMs());
				if (flag)
				{
					num4 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = timestamp - num4;
			}
			this.SetMoving(true);
		}

		// Token: 0x06004784 RID: 18308 RVA: 0x00153EE8 File Offset: 0x001520E8
		public int GetTimeOffsetMS()
		{
			int num = PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp;
			uint num2 = (uint)(this.CycleLengthMs() * 2L);
			return num % (int)num2;
		}

		// Token: 0x06004785 RID: 18309 RVA: 0x00153F14 File Offset: 0x00152114
		public byte GetNearestNode()
		{
			int num = Mathf.RoundToInt(this.currT * (float)BuilderMovingPart.NUM_PAUSE_NODES);
			if (!this.IsEvenCycle())
			{
				num += BuilderMovingPart.NUM_PAUSE_NODES;
			}
			return (byte)num;
		}

		// Token: 0x06004786 RID: 18310 RVA: 0x00153F46 File Offset: 0x00152146
		public byte GetStartNode()
		{
			return (byte)Mathf.RoundToInt(this.startPercentage * (float)BuilderMovingPart.NUM_PAUSE_NODES);
		}

		// Token: 0x06004787 RID: 18311 RVA: 0x00153F5C File Offset: 0x0015215C
		public void PauseMovement(byte node)
		{
			this.SetMoving(false);
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			float num = (float)node;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (this.reverseDirOnCycle)
			{
				num = (flag ? (1f - num) : num);
			}
			if (this.reverseDir)
			{
				num = 1f - num;
			}
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(num);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(num);
		}

		// Token: 0x06004788 RID: 18312 RVA: 0x00153FF4 File Offset: 0x001521F4
		public void SetMoving(bool isMoving)
		{
			this.isMoving = isMoving;
			BuilderAttachGridPlane[] array = this.myGridPlanes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].isMoving = isMoving;
			}
			if (!isMoving)
			{
				this.ResetMovingGrid();
			}
		}

		// Token: 0x06004789 RID: 18313 RVA: 0x00154030 File Offset: 0x00152230
		public void InitMovingGrid()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / (this.velocity * this.myPiece.GetScale());
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

		// Token: 0x0600478A RID: 18314 RVA: 0x001541A4 File Offset: 0x001523A4
		public void UpdateMovingGrid()
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
				throw new ArgumentOutOfRangeException();
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x0600478B RID: 18315 RVA: 0x001541F4 File Offset: 0x001523F4
		private Vector3 UpdatePointToPoint(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, num);
		}

		// Token: 0x0600478C RID: 18316 RVA: 0x0015422C File Offset: 0x0015242C
		private void UpdateRotation(float perc)
		{
			Quaternion quaternion = Quaternion.AngleAxis(perc * 360f, Vector3.up);
			base.transform.localRotation = quaternion;
		}

		// Token: 0x0600478D RID: 18317 RVA: 0x00154257 File Offset: 0x00152457
		private void ResetMovingGrid()
		{
			base.transform.SetLocalPositionAndRotation(this.initLocalPos, this.initLocalRotation);
		}

		// Token: 0x0600478E RID: 18318 RVA: 0x00154270 File Offset: 0x00152470
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

		// Token: 0x0600478F RID: 18319 RVA: 0x001542E8 File Offset: 0x001524E8
		public bool IsAnchoredToTable()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				if (builderAttachGridPlane.attachIndex == builderAttachGridPlane.piece.attachIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004790 RID: 18320 RVA: 0x00154324 File Offset: 0x00152524
		public void OnPieceDestroy()
		{
			this.ResetMovingGrid();
		}

		// Token: 0x040049CD RID: 18893
		public BuilderPiece myPiece;

		// Token: 0x040049CE RID: 18894
		public BuilderAttachGridPlane[] myGridPlanes;

		// Token: 0x040049CF RID: 18895
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x040049D0 RID: 18896
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x040049D1 RID: 18897
		[SerializeField]
		private float velocity;

		// Token: 0x040049D2 RID: 18898
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x040049D3 RID: 18899
		[SerializeField]
		private bool reverseDir;

		// Token: 0x040049D4 RID: 18900
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x040049D5 RID: 18901
		[SerializeField]
		protected Transform startXf;

		// Token: 0x040049D6 RID: 18902
		[SerializeField]
		protected Transform endXf;

		// Token: 0x040049D7 RID: 18903
		public static int NUM_PAUSE_NODES = 32;

		// Token: 0x040049D8 RID: 18904
		private AnimationCurve lerpAlpha;

		// Token: 0x040049D9 RID: 18905
		public bool isMoving;

		// Token: 0x040049DA RID: 18906
		private Quaternion initLocalRotation = Quaternion.identity;

		// Token: 0x040049DB RID: 18907
		private Vector3 initLocalPos = Vector3.zero;

		// Token: 0x040049DC RID: 18908
		private float cycleDuration;

		// Token: 0x040049DD RID: 18909
		private float distance;

		// Token: 0x040049DE RID: 18910
		private float currT;

		// Token: 0x040049DF RID: 18911
		private float percent;

		// Token: 0x040049E0 RID: 18912
		private bool currForward;

		// Token: 0x040049E1 RID: 18913
		private float dtSinceServerUpdate;

		// Token: 0x040049E2 RID: 18914
		private int lastServerTimeStamp;

		// Token: 0x040049E3 RID: 18915
		private float rotateStartAmt;

		// Token: 0x040049E4 RID: 18916
		private float rotateAmt;

		// Token: 0x040049E5 RID: 18917
		private uint startPercentageCycleOffset;

		// Token: 0x02000B54 RID: 2900
		public enum BuilderMovingPartType
		{
			// Token: 0x040049E7 RID: 18919
			Translation,
			// Token: 0x040049E8 RID: 18920
			Rotation
		}
	}
}
