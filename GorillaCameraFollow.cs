using System;
using Cinemachine;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class GorillaCameraFollow : MonoBehaviour
{
	// Token: 0x06001C8D RID: 7309 RVA: 0x0008B504 File Offset: 0x00089704
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			this.cameraParent.SetActive(false);
		}
		if (this.cinemachineCamera != null)
		{
			this.cinemachineFollow = this.cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			this.baseCameraRadius = this.cinemachineFollow.CameraRadius;
			this.baseFollowDistance = this.cinemachineFollow.CameraDistance;
			this.baseVerticalArmLength = this.cinemachineFollow.VerticalArmLength;
			this.baseShoulderOffset = this.cinemachineFollow.ShoulderOffset;
		}
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x0008B58C File Offset: 0x0008978C
	private void LateUpdate()
	{
		if (this.cinemachineFollow != null)
		{
			float scale = GTPlayer.Instance.scale;
			this.cinemachineFollow.CameraRadius = this.baseCameraRadius * scale;
			this.cinemachineFollow.CameraDistance = this.baseFollowDistance * scale;
			this.cinemachineFollow.VerticalArmLength = this.baseVerticalArmLength * scale;
			this.cinemachineFollow.ShoulderOffset = this.baseShoulderOffset * scale;
		}
	}

	// Token: 0x04001F9A RID: 8090
	public Transform playerHead;

	// Token: 0x04001F9B RID: 8091
	public GameObject cameraParent;

	// Token: 0x04001F9C RID: 8092
	public Vector3 headOffset;

	// Token: 0x04001F9D RID: 8093
	public Vector3 eulerRotationOffset;

	// Token: 0x04001F9E RID: 8094
	public CinemachineVirtualCamera cinemachineCamera;

	// Token: 0x04001F9F RID: 8095
	private Cinemachine3rdPersonFollow cinemachineFollow;

	// Token: 0x04001FA0 RID: 8096
	private float baseCameraRadius = 0.2f;

	// Token: 0x04001FA1 RID: 8097
	private float baseFollowDistance = 2f;

	// Token: 0x04001FA2 RID: 8098
	private float baseVerticalArmLength = 0.4f;

	// Token: 0x04001FA3 RID: 8099
	private Vector3 baseShoulderOffset = new Vector3(0.5f, -0.4f, 0f);
}
