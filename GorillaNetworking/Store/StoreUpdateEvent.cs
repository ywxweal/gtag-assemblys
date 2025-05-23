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
		// Token: 0x06004FCF RID: 20431 RVA: 0x00002050 File Offset: 0x00000250
		public StoreUpdateEvent()
		{
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x0017C574 File Offset: 0x0017A774
		public StoreUpdateEvent(string pedestalID, string itemName, DateTime startTimeUTC, DateTime endTimeUTC)
		{
			this.PedestalID = pedestalID;
			this.ItemName = itemName;
			this.StartTimeUTC = startTimeUTC;
			this.EndTimeUTC = endTimeUTC;
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x0017C599 File Offset: 0x0017A799
		public static string SerializeAsJSon(StoreUpdateEvent storeEvent)
		{
			return JsonUtility.ToJson(storeEvent);
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x0017C5A1 File Offset: 0x0017A7A1
		public static string SerializeArrayAsJSon(StoreUpdateEvent[] storeEvents)
		{
			return JsonConvert.SerializeObject(storeEvents);
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x0017C5A9 File Offset: 0x0017A7A9
		public static StoreUpdateEvent DeserializeFromJSon(string json)
		{
			return JsonUtility.FromJson<StoreUpdateEvent>(json);
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x0017C5B1 File Offset: 0x0017A7B1
		public static StoreUpdateEvent[] DeserializeFromJSonArray(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list.ToArray();
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x0017C5E3 File Offset: 0x0017A7E3
		public static List<StoreUpdateEvent> DeserializeFromJSonList(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list;
		}

		// Token: 0x040052E4 RID: 21220
		public string PedestalID;

		// Token: 0x040052E5 RID: 21221
		public string ItemName;

		// Token: 0x040052E6 RID: 21222
		public DateTime StartTimeUTC;

		// Token: 0x040052E7 RID: 21223
		public DateTime EndTimeUTC;
	}
}
