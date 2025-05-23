using System;
using UnityEngine;

// Token: 0x020003CF RID: 975
public class PuppetFollow : MonoBehaviour
{
	// Token: 0x060016B9 RID: 5817 RVA: 0x0006D484 File Offset: 0x0006B684
	private void FixedUpdate()
	{
		base.transform.position = this.sourceTarget.position - this.sourceBase.position + this.puppetBase.position;
		base.transform.localRotation = this.sourceTarget.localRotation;
	}

	// Token: 0x04001926 RID: 6438
	public Transform sourceTarget;

	// Token: 0x04001927 RID: 6439
	public Transform sourceBase;

	// Token: 0x04001928 RID: 6440
	public Transform puppetBase;
}
