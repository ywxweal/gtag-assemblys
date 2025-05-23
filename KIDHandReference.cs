using System;
using UnityEngine;

// Token: 0x020007C0 RID: 1984
public class KIDHandReference : MonoBehaviour
{
	// Token: 0x170004F5 RID: 1269
	// (get) Token: 0x060030FD RID: 12541 RVA: 0x000F0ECA File Offset: 0x000EF0CA
	public static GameObject LeftHand
	{
		get
		{
			return KIDHandReference._leftHandRef;
		}
	}

	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x060030FE RID: 12542 RVA: 0x000F0ED1 File Offset: 0x000EF0D1
	public static GameObject RightHand
	{
		get
		{
			return KIDHandReference._rightHandRef;
		}
	}

	// Token: 0x060030FF RID: 12543 RVA: 0x000F0ED8 File Offset: 0x000EF0D8
	private void Awake()
	{
		KIDHandReference._leftHandRef = this._leftHand;
		KIDHandReference._rightHandRef = this._rightHand;
	}

	// Token: 0x06003100 RID: 12544 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Update()
	{
	}

	// Token: 0x0400376C RID: 14188
	[SerializeField]
	private GameObject _leftHand;

	// Token: 0x0400376D RID: 14189
	[SerializeField]
	private GameObject _rightHand;

	// Token: 0x0400376E RID: 14190
	private static GameObject _leftHandRef;

	// Token: 0x0400376F RID: 14191
	private static GameObject _rightHandRef;
}
