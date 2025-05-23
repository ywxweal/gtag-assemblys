using System;
using Newtonsoft.Json;

// Token: 0x02000997 RID: 2455
public static class JsonUtils
{
	// Token: 0x06003ACC RID: 15052 RVA: 0x001193F3 File Offset: 0x001175F3
	public static string ToJson<T>(this T obj, bool indent = true)
	{
		return JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None);
	}

	// Token: 0x06003ACD RID: 15053 RVA: 0x00119407 File Offset: 0x00117607
	public static T FromJson<T>(this string s)
	{
		return JsonConvert.DeserializeObject<T>(s);
	}

	// Token: 0x06003ACE RID: 15054 RVA: 0x00119410 File Offset: 0x00117610
	public static string JsonSerializeEventData<T>(this T obj)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All,
			CheckAdditionalContent = true,
			Formatting = Formatting.None
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
	}

	// Token: 0x06003ACF RID: 15055 RVA: 0x00119454 File Offset: 0x00117654
	public static T JsonDeserializeEventData<T>(this string s)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.DeserializeObject<T>(s, jsonSerializerSettings);
	}
}
