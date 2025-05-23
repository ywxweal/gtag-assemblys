using System;
using BoingKit;
using UnityEngine;

// Token: 0x020006BA RID: 1722
public class SyncRigidBodyToMovement : MonoBehaviour
{
	// Token: 0x06002AFB RID: 11003 RVA: 0x000D38CE File Offset: 0x000D1ACE
	private void Awake()
	{
		this.targetParent = this.targetRigidbody.transform.parent;
		this.targetRigidbody.transform.parent = null;
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x000D3908 File Offset: 0x000D1B08
	private void OnEnable()
	{
		this.targetRigidbody.gameObject.SetActive(true);
		this.targetRigidbody.transform.position = base.transform.position;
		this.targetRigidbody.transform.rotation = base.transform.rotation;
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x000D395C File Offset: 0x000D1B5C
	private void OnDisable()
	{
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x000D3970 File Offset: 0x000D1B70
	private void FixedUpdate()
	{
		this.targetRigidbody.velocity = (base.transform.position - this.targetRigidbody.position) / Time.fixedDeltaTime;
		this.targetRigidbody.angularVelocity = QuaternionUtil.ToAngularVector(Quaternion.Inverse(this.targetRigidbody.rotation) * base.transform.rotation) / Time.fixedDeltaTime;
	}

	// Token: 0x04002FF4 RID: 12276
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x04002FF5 RID: 12277
	private Transform targetParent;
}
