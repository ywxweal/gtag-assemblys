using System;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x020001DF RID: 479
[NetworkBehaviourWeaved(1)]
public class GTSplineAnimateFixedUpdater : NetworkComponent
{
	// Token: 0x06000B31 RID: 2865 RVA: 0x0003C327 File Offset: 0x0003A527
	protected override void Awake()
	{
		base.Awake();
		this.splineAnimateRef.AddCallbackOnLoad(new Action(this.InitSplineAnimate));
		this.splineAnimateRef.AddCallbackOnUnload(new Action(this.ClearSplineAnimate));
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0003C35D File Offset: 0x0003A55D
	private void InitSplineAnimate()
	{
		this.isSplineLoaded = this.splineAnimateRef.TryResolve<SplineAnimate>(out this.splineAnimate);
		if (this.isSplineLoaded)
		{
			this.splineAnimate.enabled = false;
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0003C38A File Offset: 0x0003A58A
	private void ClearSplineAnimate()
	{
		this.splineAnimate = null;
		this.isSplineLoaded = false;
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0003C39C File Offset: 0x0003A59C
	private void FixedUpdate()
	{
		if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
		{
			if (this.isSplineLoaded)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f) % this.Duration;
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
				return;
			}
		}
		else
		{
			this.progress = (this.progress + Time.fixedDeltaTime) % this.Duration;
			if (this.isSplineLoaded)
			{
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
			}
		}
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000B35 RID: 2869 RVA: 0x0003C451 File Offset: 0x0003A651
	// (set) Token: 0x06000B36 RID: 2870 RVA: 0x0003C477 File Offset: 0x0003A677
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe float Netdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(float*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(float*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0003C49E File Offset: 0x0003A69E
	public override void WriteDataFusion()
	{
		this.Netdata = this.progress + 1f;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0003C4B2 File Offset: 0x0003A6B2
	public override void ReadDataFusion()
	{
		this.SharedReadData(this.Netdata);
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0003C4C0 File Offset: 0x0003A6C0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.progress + 1f);
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0003C4E8 File Offset: 0x0003A6E8
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float num = (float)stream.ReceiveNext();
		this.SharedReadData(num);
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0003C518 File Offset: 0x0003A718
	private void SharedReadData(float incomingValue)
	{
		if (float.IsNaN(incomingValue) || incomingValue > this.Duration + 1f || incomingValue < 0f)
		{
			return;
		}
		this.progressLerpEnd = incomingValue;
		if (this.progressLerpEnd < this.progress)
		{
			if (this.progress < this.Duration)
			{
				this.progressLerpEnd += this.Duration;
			}
			else
			{
				this.progress -= this.Duration;
			}
		}
		this.progressLerpStart = this.progress;
		this.progressLerpStartTime = Time.time;
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0003C5A7 File Offset: 0x0003A7A7
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Netdata = this._Netdata;
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0003C5BF File Offset: 0x0003A7BF
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Netdata = this.Netdata;
	}

	// Token: 0x04000DAA RID: 3498
	[SerializeField]
	private XSceneRef splineAnimateRef;

	// Token: 0x04000DAB RID: 3499
	[SerializeField]
	private float Duration;

	// Token: 0x04000DAC RID: 3500
	private SplineAnimate splineAnimate;

	// Token: 0x04000DAD RID: 3501
	private bool isSplineLoaded;

	// Token: 0x04000DAE RID: 3502
	private float progress;

	// Token: 0x04000DAF RID: 3503
	private float progressLerpStart;

	// Token: 0x04000DB0 RID: 3504
	private float progressLerpEnd;

	// Token: 0x04000DB1 RID: 3505
	private const float progressLerpDuration = 1f;

	// Token: 0x04000DB2 RID: 3506
	private float progressLerpStartTime;

	// Token: 0x04000DB3 RID: 3507
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Netdata", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private float _Netdata;
}
