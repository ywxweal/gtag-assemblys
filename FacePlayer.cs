using System;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public class FacePlayer : MonoBehaviour
{
	// Token: 0x06001C3D RID: 7229 RVA: 0x0008AAB4 File Offset: 0x00088CB4
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
