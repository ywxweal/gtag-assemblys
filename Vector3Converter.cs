﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000998 RID: 2456
public class Vector3Converter : JsonConverter
{
	// Token: 0x06003AD1 RID: 15057 RVA: 0x00119560 File Offset: 0x00117760
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Vector3 vector = (Vector3)value;
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(vector.x);
		writer.WritePropertyName("y");
		writer.WriteValue(vector.y);
		writer.WritePropertyName("z");
		writer.WriteValue(vector.z);
		writer.WriteEndObject();
	}

	// Token: 0x06003AD2 RID: 15058 RVA: 0x001195C8 File Offset: 0x001177C8
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject jobject = JObject.Load(reader);
		return new Vector3((float)jobject["x"], (float)jobject["y"], (float)jobject["z"]);
	}

	// Token: 0x06003AD3 RID: 15059 RVA: 0x00119619 File Offset: 0x00117819
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector3);
	}
}
