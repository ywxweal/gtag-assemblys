﻿using System;
using UnityEngine;

// Token: 0x02000934 RID: 2356
internal abstract class RPCNetworkBase : MonoBehaviour
{
	// Token: 0x06003935 RID: 14645
	public abstract void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler);
}
