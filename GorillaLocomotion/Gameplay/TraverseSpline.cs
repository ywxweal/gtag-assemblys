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
		// Token: 0x060051FE RID: 20990 RVA: 0x0018DFB1 File Offset: 0x0018C1B1
		protected override void Awake()
		{
			base.Awake();
			this.progress = this.SplineProgressOffet % 1f;
		}

		// Token: 0x060051FF RID: 20991 RVA: 0x0018DFCC File Offset: 0x0018C1CC
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
		// (get) Token: 0x06005200 RID: 20992 RVA: 0x0018E195 File Offset: 0x0018C395
		// (set) Token: 0x06005201 RID: 20993 RVA: 0x0018E1BB File Offset: 0x0018C3BB
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

		// Token: 0x06005202 RID: 20994 RVA: 0x0018E1E2 File Offset: 0x0018C3E2
		public override void WriteDataFusion()
		{
			this.Data = this.progress + this.currentSpeedMultiplier * 1f / this.duration;
		}

		// Token: 0x06005203 RID: 20995 RVA: 0x0018E204 File Offset: 0x0018C404
		public override void ReadDataFusion()
		{
			this.progressLerpEnd = this.Data;
			this.ReadDataShared();
		}

		// Token: 0x06005204 RID: 20996 RVA: 0x0018E218 File Offset: 0x0018C418
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.progress + this.currentSpeedMultiplier * 1f / this.duration);
		}

		// Token: 0x06005205 RID: 20997 RVA: 0x0018E23F File Offset: 0x0018C43F
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			this.progressLerpEnd = (float)stream.ReceiveNext();
			this.ReadDataShared();
		}

		// Token: 0x06005206 RID: 20998 RVA: 0x0018E258 File Offset: 0x0018C458
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

		// Token: 0x06005207 RID: 20999 RVA: 0x0018E313 File Offset: 0x0018C513
		protected float GetProgress()
		{
			return this.progress;
		}

		// Token: 0x06005208 RID: 21000 RVA: 0x0018E31B File Offset: 0x0018C51B
		public float GetCurrentSpeed()
		{
			return this.currentSpeedMultiplier;
		}

		// Token: 0x0600520A RID: 21002 RVA: 0x0018E371 File Offset: 0x0018C571
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600520B RID: 21003 RVA: 0x0018E389 File Offset: 0x0018C589
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0400561D RID: 22045
		public BezierSpline spline;

		// Token: 0x0400561E RID: 22046
		public float duration = 30f;

		// Token: 0x0400561F RID: 22047
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x04005620 RID: 22048
		private float currentSpeedMultiplier;

		// Token: 0x04005621 RID: 22049
		public float acceleration = 1f;

		// Token: 0x04005622 RID: 22050
		public float deceleration = 1f;

		// Token: 0x04005623 RID: 22051
		private bool isHeldByLocalPlayer;

		// Token: 0x04005624 RID: 22052
		public bool lookForward = true;

		// Token: 0x04005625 RID: 22053
		public SplineWalkerMode mode;

		// Token: 0x04005626 RID: 22054
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x04005627 RID: 22055
		private float progress;

		// Token: 0x04005628 RID: 22056
		private float progressLerpStart;

		// Token: 0x04005629 RID: 22057
		private float progressLerpEnd;

		// Token: 0x0400562A RID: 22058
		private const float progressLerpDuration = 1f;

		// Token: 0x0400562B RID: 22059
		private float progressLerpStartTime;

		// Token: 0x0400562C RID: 22060
		private bool goingForward = true;

		// Token: 0x0400562D RID: 22061
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x0400562E RID: 22062
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private float _Data;
	}
}
