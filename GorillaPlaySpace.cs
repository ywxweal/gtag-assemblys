using System;
using UnityEngine;

// Token: 0x02000497 RID: 1175
public class GorillaPlaySpace : MonoBehaviour
{
	// Token: 0x1700031E RID: 798
	// (get) Token: 0x06001CA0 RID: 7328 RVA: 0x0008B794 File Offset: 0x00089994
	public static GorillaPlaySpace Instance
	{
		get
		{
			return GorillaPlaySpace._instance;
		}
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x0008B79B File Offset: 0x0008999B
	private void Awake()
	{
		if (GorillaPlaySpace._instance != null && GorillaPlaySpace._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaPlaySpace._instance = this;
	}

	// Token: 0x04001FB1 RID: 8113
	[OnEnterPlay_SetNull]
	private static GorillaPlaySpace _instance;

	// Token: 0x04001FB2 RID: 8114
	public Collider headCollider;

	// Token: 0x04001FB3 RID: 8115
	public Collider bodyCollider;

	// Token: 0x04001FB4 RID: 8116
	public Transform rightHandTransform;

	// Token: 0x04001FB5 RID: 8117
	public Transform leftHandTransform;

	// Token: 0x04001FB6 RID: 8118
	public Vector3 headColliderOffset;

	// Token: 0x04001FB7 RID: 8119
	public Vector3 bodyColliderOffset;

	// Token: 0x04001FB8 RID: 8120
	private Vector3 lastLeftHandPosition;

	// Token: 0x04001FB9 RID: 8121
	private Vector3 lastRightHandPosition;

	// Token: 0x04001FBA RID: 8122
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04001FBB RID: 8123
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04001FBC RID: 8124
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04001FBD RID: 8125
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04001FBE RID: 8126
	private Rigidbody playspaceRigidbody;

	// Token: 0x04001FBF RID: 8127
	public Transform headsetTransform;

	// Token: 0x04001FC0 RID: 8128
	public Vector3 rightHandOffset;

	// Token: 0x04001FC1 RID: 8129
	public Vector3 leftHandOffset;

	// Token: 0x04001FC2 RID: 8130
	public VRRig vrRig;

	// Token: 0x04001FC3 RID: 8131
	public VRRig offlineVRRig;

	// Token: 0x04001FC4 RID: 8132
	public float vibrationCooldown = 0.1f;

	// Token: 0x04001FC5 RID: 8133
	public float vibrationDuration = 0.05f;

	// Token: 0x04001FC6 RID: 8134
	private float leftLastTouchedSurface;

	// Token: 0x04001FC7 RID: 8135
	private float rightLastTouchedSurface;

	// Token: 0x04001FC8 RID: 8136
	public VRRig myVRRig;

	// Token: 0x04001FC9 RID: 8137
	private float bodyHeight;

	// Token: 0x04001FCA RID: 8138
	public float tagCooldown;

	// Token: 0x04001FCB RID: 8139
	public float taggedTime;

	// Token: 0x04001FCC RID: 8140
	public float disconnectTime = 60f;

	// Token: 0x04001FCD RID: 8141
	public float maxStepVelocity = 2f;

	// Token: 0x04001FCE RID: 8142
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x04001FCF RID: 8143
	public float tapHapticDuration = 0.05f;

	// Token: 0x04001FD0 RID: 8144
	public float tapHapticStrength = 0.5f;

	// Token: 0x04001FD1 RID: 8145
	public float tagHapticDuration = 0.15f;

	// Token: 0x04001FD2 RID: 8146
	public float tagHapticStrength = 1f;

	// Token: 0x04001FD3 RID: 8147
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04001FD4 RID: 8148
	public float taggedHapticStrength = 1f;
}
