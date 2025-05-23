using System;
using BoingKit;
using UnityEngine;

// Token: 0x020006BA RID: 1722
public class SyncRigidBodyToMovement : MonoBehaviour
{
	// Token: 0x06002AFA RID: 11002 RVA: 0x000D382A File Offset: 0x000D1A2A
	private void Awake()
	{
		this.targetParent = this.targetRigidbody.transform.parent;
		this.targetRigidbody.transform.parent = null;
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x000D3864 File Offset: 0x000D1A64
	private void OnEnable()
	{
		this.targetRigidbody.gameObject.SetActive(true);
		this.targetRigidbody.transform.position = base.transform.position;
		this.targetRigidbody.transform.rotation = base.transform.rotation;
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x000D38B8 File Offset: 0x000D1AB8
	private void OnDisable()
	{
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x000D38CC File Offset: 0x000D1ACC
	private void FixedUpdate()
	{
		this.targetRigidbody.velocity = (base.transform.position - this.targetRigidbody.position) / Time.fixedDeltaTime;
		this.targetRigidbody.angularVelocity = QuaternionUtil.ToAngularVector(Quaternion.Inverse(this.targetRigidbody.rotation) * base.transform.rotation) / Time.fixedDeltaTime;
	}

	// Token: 0x04002FF2 RID: 12274
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x04002FF3 RID: 12275
	private Transform targetParent;
}
