using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE8 RID: 3304
	[NetworkBehaviourWeaved(1)]
	public class TraverseSpline : NetworkComponent
	{
		// Token: 0x060051FF RID: 20991 RVA: 0x0018E089 File Offset: 0x0018C289
		protected override void Awake()
		{
			base.Awake();
			this.progress = this.SplineProgressOffet % 1f;
		}

		// Token: 0x06005200 RID: 20992 RVA: 0x0018E0A4 File Offset: 0x0018C2A4
		protected virtual void FixedUpdate()
		{
			if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f);
			}
			else
			{
				if (this.isHeldByLocalPlayer)
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, this.speedMultiplierWhileHeld, this.acceleration * Time.deltaTime);
				}
				else
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, 1f, this.deceleration * Time.deltaTime);
				}
				if (this.goingForward)
				{
					this.progress += Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress > 1f)
					{
						if (this.mode == SplineWalkerMode.Once)
						{
							this.progress = 1f;
						}
						else if (this.mode == SplineWalkerMode.Loop)
						{
							this.progress %= 1f;
						}
						else
						{
							this.progress = 2f - this.progress;
							this.goingForward = false;
						}
					}
				}
				else
				{
					this.progress -= Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress < 0f)
					{
						this.progress = -this.progress;
						this.goingForward = true;
					}
				}
			}
			Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
			base.transform.position = point;
			if (this.lookForward)
			{
				base.transform.LookAt(base.transform.position + this.spline.GetDirection(this.progress, this.constantVelocity));
			}
		}

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x06005201 RID: 20993 RVA: 0x0018E26D File Offset: 0x0018C46D
		// (set) Token: 0x06005202 RID: 20994 RVA: 0x0018E293 File Offset: 0x0018C493
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe float Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(float*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(float*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06005203 RID: 20995 RVA: 0x0018E2BA File Offset: 0x0018C4BA
		public override void WriteDataFusion()
		{
			this.Data = this.progress + this.currentSpeedMultiplier * 1f / this.duration;
		}

		// Token: 0x06005204 RID: 20996 RVA: 0x0018E2DC File Offset: 0x0018C4DC
		public override void ReadDataFusion()
		{
			this.progressLerpEnd = this.Data;
			this.ReadDataShared();
		}

		// Token: 0x06005205 RID: 20997 RVA: 0x0018E2F0 File Offset: 0x0018C4F0
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.progress + this.currentSpeedMultiplier * 1f / this.duration);
		}

		// Token: 0x06005206 RID: 20998 RVA: 0x0018E317 File Offset: 0x0018C517
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			this.progressLerpEnd = (float)stream.ReceiveNext();
			this.ReadDataShared();
		}

		// Token: 0x06005207 RID: 20999 RVA: 0x0018E330 File Offset: 0x0018C530
		private void ReadDataShared()
		{
			if (float.IsNaN(this.progressLerpEnd) || float.IsInfinity(this.progressLerpEnd))
			{
				this.progressLerpEnd = 1f;
			}
			else
			{
				this.progressLerpEnd = Mathf.Abs(this.progressLerpEnd);
				if (this.progressLerpEnd > 1f)
				{
					this.progressLerpEnd = (float)((double)this.progressLerpEnd % 1.0);
				}
			}
			this.progressLerpStart = ((Mathf.Abs(this.progressLerpEnd - this.progress) > Mathf.Abs(this.progressLerpEnd - (this.progress - 1f))) ? (this.progress - 1f) : this.progress);
			this.progressLerpStartTime = Time.time;
		}

		// Token: 0x06005208 RID: 21000 RVA: 0x0018E3EB File Offset: 0x0018C5EB
		protected float GetProgress()
		{
			return this.progress;
		}

		// Token: 0x06005209 RID: 21001 RVA: 0x0018E3F3 File Offset: 0x0018C5F3
		public float GetCurrentSpeed()
		{
			return this.currentSpeedMultiplier;
		}

		// Token: 0x0600520B RID: 21003 RVA: 0x0018E449 File Offset: 0x0018C649
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600520C RID: 21004 RVA: 0x0018E461 File Offset: 0x0018C661
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400561E RID: 22046
		public BezierSpline spline;

		// Token: 0x0400561F RID: 22047
		public float duration = 30f;

		// Token: 0x04005620 RID: 22048
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x04005621 RID: 22049
		private float currentSpeedMultiplier;

		// Token: 0x04005622 RID: 22050
		public float acceleration = 1f;

		// Token: 0x04005623 RID: 22051
		public float deceleration = 1f;

		// Token: 0x04005624 RID: 22052
		private bool isHeldByLocalPlayer;

		// Token: 0x04005625 RID: 22053
		public bool lookForward = true;

		// Token: 0x04005626 RID: 22054
		public SplineWalkerMode mode;

		// Token: 0x04005627 RID: 22055
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x04005628 RID: 22056
		private float progress;

		// Token: 0x04005629 RID: 22057
		private float progressLerpStart;

		// Token: 0x0400562A RID: 22058
		private float progressLerpEnd;

		// Token: 0x0400562B RID: 22059
		private const float progressLerpDuration = 1f;

		// Token: 0x0400562C RID: 22060
		private float progressLerpStartTime;

		// Token: 0x0400562D RID: 22061
		private bool goingForward = true;

		// Token: 0x0400562E RID: 22062
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x0400562F RID: 22063
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private float _Data;
	}
}
