using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
internal class RPCUtil
{
	// Token: 0x06002519 RID: 9497 RVA: 0x000B974C File Offset: 0x000B794C
	public static bool NotSpam(string id, PhotonMessageInfoWrapped info, float delay)
	{
		RPCUtil.RPCCallID rpccallID = new RPCUtil.RPCCallID(id, info.senderID);
		if (!RPCUtil.RPCCallLog.ContainsKey(rpccallID))
		{
			RPCUtil.RPCCallLog.Add(rpccallID, Time.time);
			return true;
		}
		if (Time.time - RPCUtil.RPCCallLog[rpccallID] > delay)
		{
			RPCUtil.RPCCallLog[rpccallID] = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000B97AD File Offset: 0x000B79AD
	public static bool SafeValue(float v)
	{
		return !float.IsNaN(v) && float.IsFinite(v);
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000B97BF File Offset: 0x000B79BF
	public static bool SafeValue(float v, float min, float max)
	{
		return RPCUtil.SafeValue(v) && v <= max && v >= min;
	}

	// Token: 0x040029ED RID: 10733
	private static Dictionary<RPCUtil.RPCCallID, float> RPCCallLog = new Dictionary<RPCUtil.RPCCallID, float>();

	// Token: 0x020005E9 RID: 1513
	private struct RPCCallID : IEquatable<RPCUtil.RPCCallID>
	{
		// Token: 0x0600251E RID: 9502 RVA: 0x000B97E4 File Offset: 0x000B79E4
		public RPCCallID(string nameOfFunction, int senderId)
		{
			this._senderID = senderId;
			this._nameOfFunction = nameOfFunction;
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x0600251F RID: 9503 RVA: 0x000B97F4 File Offset: 0x000B79F4
		public readonly int SenderID
		{
			get
			{
				return this._senderID;
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06002520 RID: 9504 RVA: 0x000B97FC File Offset: 0x000B79FC
		public readonly string NameOfFunction
		{
			get
			{
				return this._nameOfFunction;
			}
		}

		// Token: 0x06002521 RID: 9505 RVA: 0x000B9804 File Offset: 0x000B7A04
		bool IEquatable<RPCUtil.RPCCallID>.Equals(RPCUtil.RPCCallID other)
		{
			return other.NameOfFunction.Equals(this.NameOfFunction) && other.SenderID.Equals(this.SenderID);
		}

		// Token: 0x040029EE RID: 10734
		private int _senderID;

		// Token: 0x040029EF RID: 10735
		private string _nameOfFunction;
	}
}
