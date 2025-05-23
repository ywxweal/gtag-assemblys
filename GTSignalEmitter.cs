using System;
using UnityEngine;

// Token: 0x02000653 RID: 1619
public class GTSignalEmitter : MonoBehaviour
{
	// Token: 0x0600286D RID: 10349 RVA: 0x000C96EB File Offset: 0x000C78EB
	public virtual void Emit()
	{
		GTSignal.Emit(this.emitMode, this.signal, Array.Empty<object>());
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000C9708 File Offset: 0x000C7908
	public virtual void Emit(int targetActor)
	{
		GTSignal.Emit(targetActor, this.signal, Array.Empty<object>());
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000C9720 File Offset: 0x000C7920
	public virtual void Emit(params object[] data)
	{
		GTSignal.Emit(this.emitMode, this.signal, data);
	}

	// Token: 0x04002D49 RID: 11593
	[Space]
	public GTSignalID signal;

	// Token: 0x04002D4A RID: 11594
	public GTSignal.EmitMode emitMode;
}
