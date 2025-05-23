using System;
using UnityEngine;

// Token: 0x020003DD RID: 989
[RequireComponent(typeof(RigContainer))]
public class VRRigEvents : MonoBehaviour, IPreDisable
{
	// Token: 0x060017D2 RID: 6098 RVA: 0x0007424B File Offset: 0x0007244B
	public void PreDisable()
	{
		Action<RigContainer> action = this.disableEvent;
		if (action == null)
		{
			return;
		}
		action(this.rigRef);
	}

	// Token: 0x04001AAB RID: 6827
	[SerializeField]
	private RigContainer rigRef;

	// Token: 0x04001AAC RID: 6828
	public Action<RigContainer> disableEvent;
}
