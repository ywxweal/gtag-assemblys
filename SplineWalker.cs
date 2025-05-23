using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009C6 RID: 2502
public class SplineWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x06003BD7 RID: 15319 RVA: 0x0011E9CC File Offset: 0x0011CBCC
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06003BD8 RID: 15320 RVA: 0x0011E9DC File Offset: 0x0011CBDC
	private void Update()
	{
		if (this.goingForward)
		{
			this.progress += Time.deltaTime / this.duration;
			if (this.progress > 1f)
			{
				if (this.mode == SplineWalkerMode.Once)
				{
					this.progress = 1f;
				}
				else if (this.mode == SplineWalkerMode.Loop)
				{
					this.progress -= 1f;
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
			this.progress -= Time.deltaTime / this.duration;
			if (this.progress < 0f)
			{
				this.progress = -this.progress;
				this.goingForward = true;
			}
		}
		if (this.linearSpline != null && this.walkLinearPath)
		{
			Vector3 vector = this.linearSpline.Evaluate(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = vector;
			}
			else
			{
				base.transform.localPosition = vector;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(vector + this.linearSpline.GetForwardTangent(this.progress, 0.01f));
				return;
			}
		}
		else if (this.spline != null)
		{
			Vector3 point = this.spline.GetPoint(this.progress);
			if (this.useWorldPosition)
			{
				base.transform.position = point;
			}
			else
			{
				base.transform.localPosition = point;
			}
			if (this.lookForward)
			{
				base.transform.LookAt(point + this.spline.GetDirection(this.progress));
			}
		}
	}

	// Token: 0x06003BD9 RID: 15321 RVA: 0x0011EB8A File Offset: 0x0011CD8A
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	// Token: 0x04004025 RID: 16421
	public BezierSpline spline;

	// Token: 0x04004026 RID: 16422
	public LinearSpline linearSpline;

	// Token: 0x04004027 RID: 16423
	public float duration;

	// Token: 0x04004028 RID: 16424
	public bool lookForward;

	// Token: 0x04004029 RID: 16425
	public SplineWalkerMode mode;

	// Token: 0x0400402A RID: 16426
	public bool walkLinearPath;

	// Token: 0x0400402B RID: 16427
	public bool useWorldPosition;

	// Token: 0x0400402C RID: 16428
	public float progress;

	// Token: 0x0400402D RID: 16429
	private bool goingForward = true;

	// Token: 0x0400402E RID: 16430
	public bool DoNetworkSync = true;

	// Token: 0x0400402F RID: 16431
	private PhotonView _view;
}
