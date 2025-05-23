using System;
using UnityEngine;

// Token: 0x0200095E RID: 2398
public class FPSController : MonoBehaviour
{
	// Token: 0x1400006A RID: 106
	// (add) Token: 0x06003A00 RID: 14848 RVA: 0x00116988 File Offset: 0x00114B88
	// (remove) Token: 0x06003A01 RID: 14849 RVA: 0x001169C0 File Offset: 0x00114BC0
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	// Token: 0x1400006B RID: 107
	// (add) Token: 0x06003A02 RID: 14850 RVA: 0x001169F8 File Offset: 0x00114BF8
	// (remove) Token: 0x06003A03 RID: 14851 RVA: 0x00116A30 File Offset: 0x00114C30
	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	// Token: 0x04003F12 RID: 16146
	public float baseMoveSpeed = 4f;

	// Token: 0x04003F13 RID: 16147
	public float shiftMoveSpeed = 8f;

	// Token: 0x04003F14 RID: 16148
	public float ctrlMoveSpeed = 1f;

	// Token: 0x04003F15 RID: 16149
	public float lookHorizontal = 0.4f;

	// Token: 0x04003F16 RID: 16150
	public float lookVertical = 0.25f;

	// Token: 0x04003F17 RID: 16151
	[SerializeField]
	private Vector3 leftControllerPosOffset = new Vector3(-0.2f, -0.25f, 0.3f);

	// Token: 0x04003F18 RID: 16152
	[SerializeField]
	private Vector3 leftControllerRotationOffset = new Vector3(265f, -82f, 28f);

	// Token: 0x04003F19 RID: 16153
	[SerializeField]
	private Vector3 rightControllerPosOffset = new Vector3(0.2f, -0.25f, 0.3f);

	// Token: 0x04003F1A RID: 16154
	[SerializeField]
	private Vector3 rightControllerRotationOffset = new Vector3(263f, 318f, 485f);

	// Token: 0x04003F1B RID: 16155
	[SerializeField]
	private bool toggleGrab;

	// Token: 0x04003F1E RID: 16158
	private bool controlRightHand;

	// Token: 0x04003F1F RID: 16159
	public LayerMask HandMask;

	// Token: 0x0200095F RID: 2399
	// (Invoke) Token: 0x06003A06 RID: 14854
	public delegate void OnStateChangeEventHandler();
}
