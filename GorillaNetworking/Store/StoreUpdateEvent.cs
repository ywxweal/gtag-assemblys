using System;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C94 RID: 3220
	public class StoreUpdateEvent
	{
		// Token: 0x06004FCE RID: 20430 RVA: 0x00002050 File Offset: 0x00000250
		public StoreUpdateEvent()
		{
		}

		// Token: 0x06004FCF RID: 20431 RVA: 0x0017C49C File Offset: 0x0017A69C
		public StoreUpdateEvent(string pedestalID, string itemName, DateTime startTimeUTC, DateTime endTimeUTC)
		{
			this.PedestalID = pedestalID;
			this.ItemName = itemName;
			this.StartTimeUTC = startTimeUTC;
			this.EndTimeUTC = endTimeUTC;
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x0017C4C1 File Offset: 0x0017A6C1
		public static string SerializeAsJSon(StoreUpdateEvent storeEvent)
		{
			return JsonUtility.ToJson(storeEvent);
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x0017C4C9 File Offset: 0x0017A6C9
		public static string SerializeArrayAsJSon(StoreUpdateEvent[] storeEvents)
		{
			return JsonConvert.SerializeObject(storeEvents);
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x0017C4D1 File Offset: 0x0017A6D1
		public static StoreUpdateEvent DeserializeFromJSon(string json)
		{
			return JsonUtility.FromJson<StoreUpdateEvent>(json);
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x0017C4D9 File Offset: 0x0017A6D9
		public static StoreUpdateEvent[] DeserializeFromJSonArray(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list.ToArray();
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x0017C50B File Offset: 0x0017A70B
		public static List<StoreUpdateEvent> DeserializeFromJSonList(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list;
		}

		// Token: 0x040052E3 RID: 21219
		public string PedestalID;

		// Token: 0x040052E4 RID: 21220
		public string ItemName;

		// Token: 0x040052E5 RID: 21221
		public DateTime StartTimeUTC;

		// Token: 0x040052E6 RID: 21222
		public DateTime EndTimeUTC;
	}
}
