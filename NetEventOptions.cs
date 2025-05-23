using System;
using Photon.Realtime;

// Token: 0x020002B5 RID: 693
public class NetEventOptions
{
	// Token: 0x170001CC RID: 460
	// (get) Token: 0x0600109D RID: 4253 RVA: 0x0005013B File Offset: 0x0004E33B
	public bool HasWebHooks
	{
		get
		{
			return this.Flags != WebFlags.Default;
		}
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x0005014D File Offset: 0x0004E34D
	public NetEventOptions()
	{
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x00050160 File Offset: 0x0004E360
	public NetEventOptions(int reciever, int[] actors, byte flags)
	{
		this.Reciever = (NetEventOptions.RecieverTarget)reciever;
		this.TargetActors = actors;
		this.Flags = new WebFlags(flags);
	}

	// Token: 0x040012EE RID: 4846
	public NetEventOptions.RecieverTarget Reciever;

	// Token: 0x040012EF RID: 4847
	public int[] TargetActors;

	// Token: 0x040012F0 RID: 4848
	public WebFlags Flags = WebFlags.Default;

	// Token: 0x020002B6 RID: 694
	public enum RecieverTarget
	{
		// Token: 0x040012F2 RID: 4850
		others,
		// Token: 0x040012F3 RID: 4851
		all,
		// Token: 0x040012F4 RID: 4852
		master
	}
}
