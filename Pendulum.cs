using System;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class Pendulum : MonoBehaviour
{
	// Token: 0x06000458 RID: 1112 RVA: 0x0001942C File Offset: 0x0001762C
	private void Start()
	{
		this.pendulum = (this.ClockPendulum = base.gameObject.GetComponent<Transform>());
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00019454 File Offset: 0x00017654
	private void Update()
	{
		if (this.pendulum)
		{
			float num = this.MaxAngleDeflection * Mathf.Sin(Time.time * this.SpeedOfPendulum);
			this.pendulum.localRotation = Quaternion.Euler(0f, 0f, num);
			return;
		}
	}

	// Token: 0x04000503 RID: 1283
	public float MaxAngleDeflection = 10f;

	// Token: 0x04000504 RID: 1284
	public float SpeedOfPendulum = 1f;

	// Token: 0x04000505 RID: 1285
	public Transform ClockPendulum;

	// Token: 0x04000506 RID: 1286
	private Transform pendulum;
}
