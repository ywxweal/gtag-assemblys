using System;

// Token: 0x02000654 RID: 1620
[Serializable]
public struct GTSignalID : IEquatable<GTSignalID>, IEquatable<int>
{
	// Token: 0x06002872 RID: 10354 RVA: 0x000C97E0 File Offset: 0x000C79E0
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

	// Token: 0x06002873 RID: 10355 RVA: 0x000C981C File Offset: 0x000C7A1C
	public bool Equals(GTSignalID other)
	{
		return this._id == other._id;
	}

	// Token: 0x06002874 RID: 10356 RVA: 0x000C982C File Offset: 0x000C7A2C
	public bool Equals(int other)
	{
		return this._id == other;
	}

	// Token: 0x06002875 RID: 10357 RVA: 0x000C9837 File Offset: 0x000C7A37
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06002876 RID: 10358 RVA: 0x000C983F File Offset: 0x000C7A3F
	public static bool operator ==(GTSignalID x, GTSignalID y)
	{
		return x.Equals(y);
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000C9849 File Offset: 0x000C7A49
	public static bool operator !=(GTSignalID x, GTSignalID y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000C9837 File Offset: 0x000C7A37
	public static implicit operator int(GTSignalID sid)
	{
		return sid._id;
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x000C9858 File Offset: 0x000C7A58
	public static implicit operator GTSignalID(string s)
	{
		return new GTSignalID
		{
			_id = GTSignal.ComputeID(s)
		};
	}

	// Token: 0x04002D4D RID: 11597
	private int _id;
}
