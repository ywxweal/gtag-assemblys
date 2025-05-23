using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000496 RID: 1174
public class GorillaIKHandTarget : MonoBehaviour
{
	// Token: 0x06001C9C RID: 7324 RVA: 0x0008B769 File Offset: 0x00089969
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x0008B77C File Offset: 0x0008997C
	private void FixedUpdate()
	{
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x04001FAC RID: 8108
	public GameObject handToStickTo;

	// Token: 0x04001FAD RID: 8109
	public bool isLeftHand;

	// Token: 0x04001FAE RID: 8110
	public float hapticStrength;

	// Token: 0x04001FAF RID: 8111
	private Rigidbody thisRigidbody;

	// Token: 0x04001FB0 RID: 8112
	private XRController controllerReference;
}
