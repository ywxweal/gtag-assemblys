using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class GorillaVelocityEstimator : MonoBehaviour
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060005A2 RID: 1442 RVA: 0x000209B9 File Offset: 0x0001EBB9
	// (set) Token: 0x060005A3 RID: 1443 RVA: 0x000209C1 File Offset: 0x0001EBC1
	public Vector3 linearVelocity { get; private set; }

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060005A4 RID: 1444 RVA: 0x000209CA File Offset: 0x0001EBCA
	// (set) Token: 0x060005A5 RID: 1445 RVA: 0x000209D2 File Offset: 0x0001EBD2
	public Vector3 angularVelocity { get; private set; }

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060005A6 RID: 1446 RVA: 0x000209DB File Offset: 0x0001EBDB
	// (set) Token: 0x060005A7 RID: 1447 RVA: 0x000209E3 File Offset: 0x0001EBE3
	public Vector3 handPos { get; private set; }

	// Token: 0x060005A8 RID: 1448 RVA: 0x000209EC File Offset: 0x0001EBEC
	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00020A00 File Offset: 0x0001EC00
	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
		GorillaVelocityEstimatorManager.Register(this);
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00020A61 File Offset: 0x0001EC61
	private void OnDisable()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00020A61 File Offset: 0x0001EC61
	private void OnDestroy()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00020A6C File Offset: 0x0001EC6C
	public void TriggeredLateUpdate()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = Vector3.zero;
		if (!this.useGlobalSpace)
		{
			vector = GTPlayer.Instance.InstantaneousVelocity;
		}
		Vector3 vector2 = (position - this.lastPos) / Time.deltaTime - vector;
		Quaternion rotation = base.transform.rotation;
		Vector3 vector3 = (rotation * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector3.x > 180f)
		{
			vector3.x -= 360f;
		}
		if (vector3.y > 180f)
		{
			vector3.y -= 360f;
		}
		if (vector3.z > 180f)
		{
			vector3.z -= 360f;
		}
		vector3 *= 0.017453292f / Time.fixedDeltaTime;
		this.history[this.currentFrame % this.numFrames] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = vector2,
			angular = vector3
		};
		this.linearVelocity = this.history[0].linear;
		this.angularVelocity = this.history[0].angular;
		for (int i = 1; i < this.numFrames; i++)
		{
			this.linearVelocity += this.history[i].linear;
			this.angularVelocity += this.history[i].angular;
		}
		this.linearVelocity /= (float)this.numFrames;
		this.angularVelocity /= (float)this.numFrames;
		this.handPos = position;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = position;
		this.lastRotation = rotation;
	}

	// Token: 0x040006AD RID: 1709
	private int numFrames = 8;

	// Token: 0x040006B1 RID: 1713
	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	// Token: 0x040006B2 RID: 1714
	private int currentFrame;

	// Token: 0x040006B3 RID: 1715
	private Vector3 lastPos;

	// Token: 0x040006B4 RID: 1716
	private Quaternion lastRotation;

	// Token: 0x040006B5 RID: 1717
	private Vector3 lastRotationVec;

	// Token: 0x040006B6 RID: 1718
	public bool useGlobalSpace;

	// Token: 0x020000E3 RID: 227
	public struct VelocityHistorySample
	{
		// Token: 0x040006B7 RID: 1719
		public Vector3 linear;

		// Token: 0x040006B8 RID: 1720
		public Vector3 angular;
	}
}
