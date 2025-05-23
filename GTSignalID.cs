using System;

// Token: 0x02000654 RID: 1620
[Serializable]
public struct GTSignalID : IEquatable<GTSignalID>, IEquatable<int>
{
	// Token: 0x06002871 RID: 10353 RVA: 0x000C973C File Offset: 0x000C793C
	public override bool Equals(object obj)
	{
		if (obj is GTSignalID)
		{
			GTSignalID gtsignalID = (GTSignalID)obj;
			return this.Equals(gtsignalID);
		}
		if (obj is int)
		{
			int num = (int)obj;
			return this.Equals(num);
		}
		return false;
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000C9778 File Offset: 0x000C7978
	public bool Equals(GTSignalID other)
	{
		return this._id == other._id;
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x000C9788 File Offset: 0x000C7988
	public bool Equals(int other)
	{
		return this._id == other;
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x000C9793 File Offset: 0x000C7993
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x000C979B File Offset: 0x000C799B
	public static bool operator ==(GTSignalID x, GTSignalID y)
	{
		return x.Equals(y);
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x000C97A5 File Offset: 0x000C79A5
	public static bool operator !=(GTSignalID x, GTSignalID y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000C9793 File Offset: 0x000C7993
	public static implicit operator int(GTSignalID sid)
	{
		return sid._id;
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000C97B4 File Offset: 0x000C79B4
	public static implicit operator GTSignalID(string s)
	{
		return new GTSignalID
		{
			_id = GTSignal.ComputeID(s)
		};
	}

	// Token: 0x04002D4B RID: 11595
	private int _id;
}
