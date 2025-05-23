using System;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class SpinRotation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000A26 RID: 2598 RVA: 0x0003556C File Offset: 0x0003376C
	// (set) Token: 0x06000A27 RID: 2599 RVA: 0x00035574 File Offset: 0x00033774
	public bool TickRunning { get; set; }

	// Token: 0x06000A28 RID: 2600 RVA: 0x0003557D File Offset: 0x0003377D
	public void Tick()
	{
		base.transform.localRotation = Quaternion.Euler(this.rotationPerSecondEuler * (Time.time - this.baseTime)) * this.baseRotation;
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x000355B1 File Offset: 0x000337B1
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x000355C4 File Offset: 0x000337C4
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.baseTime = Time.time;
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0002E4E5 File Offset: 0x0002C6E5
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x04000C43 RID: 3139
	[SerializeField]
	private Vector3 rotationPerSecondEuler;

	// Token: 0x04000C44 RID: 3140
	private Quaternion baseRotation;

	// Token: 0x04000C45 RID: 3141
	private float baseTime;
}
