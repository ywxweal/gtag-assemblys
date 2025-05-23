using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class D20_ShaderManager : MonoBehaviour
{
	// Token: 0x060006D2 RID: 1746 RVA: 0x00026A20 File Offset: 0x00024C20
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.material;
		this.material.SetVector("_Velocity", this.velocity);
		base.StartCoroutine(this.UpdateVelocityCoroutine());
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00026A85 File Offset: 0x00024C85
	private IEnumerator UpdateVelocityCoroutine()
	{
		for (;;)
		{
			Vector3 position = base.transform.position;
			this.velocity = (position - this.lastPosition) / this.updateInterval;
			this.lastPosition = position;
			this.material.SetVector("_Velocity", this.velocity);
			yield return new WaitForSeconds(this.updateInterval);
		}
		yield break;
	}

	// Token: 0x0400081D RID: 2077
	private Rigidbody rb;

	// Token: 0x0400081E RID: 2078
	private Vector3 lastPosition;

	// Token: 0x0400081F RID: 2079
	public float updateInterval = 0.1f;

	// Token: 0x04000820 RID: 2080
	public Vector3 velocity;

	// Token: 0x04000821 RID: 2081
	private Material material;
}
