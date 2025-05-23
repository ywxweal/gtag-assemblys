using System;
using Steamworks;
using UnityEngine;

// Token: 0x020008F2 RID: 2290
public class SteamAuthTicket : IDisposable
{
	// Token: 0x0600378E RID: 14222 RVA: 0x0010C000 File Offset: 0x0010A200
	private SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		this.m_hAuthTicket = hAuthTicket;
	}

	// Token: 0x0600378F RID: 14223 RVA: 0x0010C00F File Offset: 0x0010A20F
	public static implicit operator SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		return new SteamAuthTicket(hAuthTicket);
	}

	// Token: 0x06003790 RID: 14224 RVA: 0x0010C018 File Offset: 0x0010A218
	~SteamAuthTicket()
	{
		this.Dispose();
	}

	// Token: 0x06003791 RID: 14225 RVA: 0x0010C044 File Offset: 0x0010A244
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		if (this.m_hAuthTicket != HAuthTicket.Invalid)
		{
			try
			{
				SteamUser.CancelAuthTicket(this.m_hAuthTicket);
			}
			catch (InvalidOperationException)
			{
				Debug.LogWarning("Failed to invalidate a Steam auth ticket because the Steam API was shut down. Was it supposed to be disposed of sooner?");
			}
			this.m_hAuthTicket = HAuthTicket.Invalid;
		}
	}

	// Token: 0x04003D19 RID: 15641
	private HAuthTicket m_hAuthTicket;
}
