using System;
using UnityEngine;

// Token: 0x02000653 RID: 1619
public class GTSignalEmitter : MonoBehaviour
{
	// Token: 0x0600286E RID: 10350 RVA: 0x000C978F File Offset: 0x000C798F
	public virtual void Emit()
	{
		GTSignal.Emit(this.emitMode, this.signal, Array.Empty<object>());
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000C97AC File Offset: 0x000C79AC
	public virtual void Emit(int targetActor)
	{
		GTSignal.Emit(targetActor, this.signal, Array.Empty<object>());
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x000C97C4 File Offset: 0x000C79C4
	public virtual void Emit(params object[] data)
	{
		GTSignal.Emit(this.emitMode, this.signal, data);
	}

	// Token: 0x04002D4B RID: 11595
	[Space]
	public GTSignalID signal;

	// Token: 0x04002D4C RID: 11596
	public GTSignal.EmitMode emitMode;
}
