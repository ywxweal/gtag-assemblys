using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	// Token: 0x02000A8B RID: 2699
	public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
	{
		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06004082 RID: 16514 RVA: 0x0012B95E File Offset: 0x00129B5E
		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06004083 RID: 16515 RVA: 0x0012B96B File Offset: 0x00129B6B
		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06004084 RID: 16516 RVA: 0x0012B976 File Offset: 0x00129B76
		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06004085 RID: 16517 RVA: 0x0012B981 File Offset: 0x00129B81
		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06004086 RID: 16518 RVA: 0x0012B98C File Offset: 0x00129B8C
		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06004087 RID: 16519 RVA: 0x0012B997 File Offset: 0x00129B97
		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06004088 RID: 16520 RVA: 0x0012B9A2 File Offset: 0x00129BA2
		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06004089 RID: 16521 RVA: 0x0012B9AD File Offset: 0x00129BAD
		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x0600408A RID: 16522 RVA: 0x0012B9B8 File Offset: 0x00129BB8
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x0600408B RID: 16523 RVA: 0x0012B9C0 File Offset: 0x00129BC0
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x0600408C RID: 16524 RVA: 0x0012B9CD File Offset: 0x00129BCD
		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x0600408D RID: 16525 RVA: 0x0012B9DA File Offset: 0x00129BDA
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x0600408E RID: 16526 RVA: 0x0012B9E7 File Offset: 0x00129BE7
		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x0600408F RID: 16527 RVA: 0x0012B9F4 File Offset: 0x00129BF4
		ICollection IDictionary.Keys
		{
			get
			{
				this.EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Key);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06004090 RID: 16528 RVA: 0x0012BA5C File Offset: 0x00129C5C
		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Value);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06004091 RID: 16529 RVA: 0x0012BAC4 File Offset: 0x00129CC4
		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06004092 RID: 16530 RVA: 0x0012BACC File Offset: 0x00129CCC
		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06004093 RID: 16531 RVA: 0x0012BAD4 File Offset: 0x00129CD4
		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06004094 RID: 16532 RVA: 0x0012BADC File Offset: 0x00129CDC
		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06004095 RID: 16533 RVA: 0x0012BAE4 File Offset: 0x00129CE4
		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06004096 RID: 16534 RVA: 0x0012BAEC File Offset: 0x00129CEC
		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06004097 RID: 16535 RVA: 0x0012BAF4 File Offset: 0x00129CF4
		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06004098 RID: 16536 RVA: 0x0012BAFC File Offset: 0x00129CFC
		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06004099 RID: 16537 RVA: 0x0012BB09 File Offset: 0x00129D09
		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		// Token: 0x17000676 RID: 1654
		object IDictionary.this[object key]
		{
			get
			{
				return this.EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData jsonData = this.ToJsonData(value);
				this[(string)key] = jsonData;
			}
		}

		// Token: 0x17000677 RID: 1655
		object IOrderedDictionary.this[int idx]
		{
			get
			{
				this.EnsureDictionary();
				return this.object_list[idx].Value;
			}
			set
			{
				this.EnsureDictionary();
				JsonData jsonData = this.ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
				this.inst_object[keyValuePair.Key] = jsonData;
				KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, jsonData);
				this.object_list[idx] = keyValuePair2;
			}
		}

		// Token: 0x17000678 RID: 1656
		object IList.this[int index]
		{
			get
			{
				return this.EnsureList()[index];
			}
			set
			{
				this.EnsureList();
				JsonData jsonData = this.ToJsonData(value);
				this[index] = jsonData;
			}
		}

		// Token: 0x17000679 RID: 1657
		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name];
			}
			set
			{
				this.EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (this.inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if (this.object_list[i].Key == prop_name)
						{
							this.object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(keyValuePair);
				}
				this.inst_object[prop_name] = value;
				this.json = null;
			}
		}

		// Token: 0x1700067A RID: 1658
		public JsonData this[int index]
		{
			get
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					return this.inst_array[index];
				}
				return this.object_list[index].Value;
			}
			set
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					this.inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
					KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					this.object_list[index] = keyValuePair2;
					this.inst_object[keyValuePair.Key] = value;
				}
				this.json = null;
			}
		}

		// Token: 0x060040A4 RID: 16548 RVA: 0x00002050 File Offset: 0x00000250
		public JsonData()
		{
		}

		// Token: 0x060040A5 RID: 16549 RVA: 0x0012BD67 File Offset: 0x00129F67
		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		// Token: 0x060040A6 RID: 16550 RVA: 0x0012BD7D File Offset: 0x00129F7D
		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		// Token: 0x060040A7 RID: 16551 RVA: 0x0012BD93 File Offset: 0x00129F93
		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		// Token: 0x060040A8 RID: 16552 RVA: 0x0012BDA9 File Offset: 0x00129FA9
		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		// Token: 0x060040A9 RID: 16553 RVA: 0x0012BDC0 File Offset: 0x00129FC0
		public JsonData(object obj)
		{
			if (obj is bool)
			{
				this.type = JsonType.Boolean;
				this.inst_boolean = (bool)obj;
				return;
			}
			if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				this.type = JsonType.String;
				this.inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		// Token: 0x060040AA RID: 16554 RVA: 0x0012BE69 File Offset: 0x0012A069
		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		// Token: 0x060040AB RID: 16555 RVA: 0x0012BE7F File Offset: 0x0012A07F
		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		// Token: 0x060040AC RID: 16556 RVA: 0x0012BE87 File Offset: 0x0012A087
		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		// Token: 0x060040AD RID: 16557 RVA: 0x0012BE8F File Offset: 0x0012A08F
		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		// Token: 0x060040AE RID: 16558 RVA: 0x0012BE97 File Offset: 0x0012A097
		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		// Token: 0x060040AF RID: 16559 RVA: 0x0012BE9F File Offset: 0x0012A09F
		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		// Token: 0x060040B0 RID: 16560 RVA: 0x0012BEA7 File Offset: 0x0012A0A7
		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		// Token: 0x060040B1 RID: 16561 RVA: 0x0012BEC3 File Offset: 0x0012A0C3
		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		// Token: 0x060040B2 RID: 16562 RVA: 0x0012BEDF File Offset: 0x0012A0DF
		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		// Token: 0x060040B3 RID: 16563 RVA: 0x0012BEFB File Offset: 0x0012A0FB
		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		// Token: 0x060040B4 RID: 16564 RVA: 0x0012BF17 File Offset: 0x0012A117
		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		// Token: 0x060040B5 RID: 16565 RVA: 0x0012BF33 File Offset: 0x0012A133
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		// Token: 0x060040B6 RID: 16566 RVA: 0x0012BF44 File Offset: 0x0012A144
		void IDictionary.Add(object key, object value)
		{
			JsonData jsonData = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, jsonData);
			KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>((string)key, jsonData);
			this.object_list.Add(keyValuePair);
			this.json = null;
		}

		// Token: 0x060040B7 RID: 16567 RVA: 0x0012BF87 File Offset: 0x0012A187
		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		// Token: 0x060040B8 RID: 16568 RVA: 0x0012BFA6 File Offset: 0x0012A1A6
		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		// Token: 0x060040B9 RID: 16569 RVA: 0x0012BFB4 File Offset: 0x0012A1B4
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		// Token: 0x060040BA RID: 16570 RVA: 0x0012BFBC File Offset: 0x0012A1BC
		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if (this.object_list[i].Key == (string)key)
				{
					this.object_list.RemoveAt(i);
					break;
				}
			}
			this.json = null;
		}

		// Token: 0x060040BB RID: 16571 RVA: 0x0012C021 File Offset: 0x0012A221
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		// Token: 0x060040BC RID: 16572 RVA: 0x0012C02E File Offset: 0x0012A22E
		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		// Token: 0x060040BD RID: 16573 RVA: 0x0012C04A File Offset: 0x0012A24A
		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		// Token: 0x060040BE RID: 16574 RVA: 0x0012C066 File Offset: 0x0012A266
		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		// Token: 0x060040BF RID: 16575 RVA: 0x0012C082 File Offset: 0x0012A282
		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		// Token: 0x060040C0 RID: 16576 RVA: 0x0012C09E File Offset: 0x0012A29E
		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		// Token: 0x060040C1 RID: 16577 RVA: 0x0012C0BA File Offset: 0x0012A2BA
		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		// Token: 0x060040C2 RID: 16578 RVA: 0x0012C0D1 File Offset: 0x0012A2D1
		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		// Token: 0x060040C3 RID: 16579 RVA: 0x0012C0E8 File Offset: 0x0012A2E8
		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		// Token: 0x060040C4 RID: 16580 RVA: 0x0012C0FF File Offset: 0x0012A2FF
		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		// Token: 0x060040C5 RID: 16581 RVA: 0x0012C116 File Offset: 0x0012A316
		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		// Token: 0x060040C6 RID: 16582 RVA: 0x0012C12D File Offset: 0x0012A32D
		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		// Token: 0x060040C7 RID: 16583 RVA: 0x0012C135 File Offset: 0x0012A335
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		// Token: 0x060040C8 RID: 16584 RVA: 0x0012C13E File Offset: 0x0012A33E
		int IList.Add(object value)
		{
			return this.Add(value);
		}

		// Token: 0x060040C9 RID: 16585 RVA: 0x0012C147 File Offset: 0x0012A347
		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x0012C15B File Offset: 0x0012A35B
		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x0012C169 File Offset: 0x0012A369
		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		// Token: 0x060040CC RID: 16588 RVA: 0x0012C177 File Offset: 0x0012A377
		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		// Token: 0x060040CD RID: 16589 RVA: 0x0012C18D File Offset: 0x0012A38D
		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		// Token: 0x060040CE RID: 16590 RVA: 0x0012C1A2 File Offset: 0x0012A3A2
		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		// Token: 0x060040CF RID: 16591 RVA: 0x0012C1B7 File Offset: 0x0012A3B7
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		// Token: 0x060040D0 RID: 16592 RVA: 0x0012C1D0 File Offset: 0x0012A3D0
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData jsonData = this.ToJsonData(value);
			this[text] = jsonData;
			KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(text, jsonData);
			this.object_list.Insert(idx, keyValuePair);
		}

		// Token: 0x060040D1 RID: 16593 RVA: 0x0012C20C File Offset: 0x0012A40C
		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		// Token: 0x060040D2 RID: 16594 RVA: 0x0012C24C File Offset: 0x0012A44C
		private ICollection EnsureCollection()
		{
			if (this.type == JsonType.Array)
			{
				return (ICollection)this.inst_array;
			}
			if (this.type == JsonType.Object)
			{
				return (ICollection)this.inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		// Token: 0x060040D3 RID: 16595 RVA: 0x0012C284 File Offset: 0x0012A484
		private IDictionary EnsureDictionary()
		{
			if (this.type == JsonType.Object)
			{
				return (IDictionary)this.inst_object;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			this.type = JsonType.Object;
			this.inst_object = new Dictionary<string, JsonData>();
			this.object_list = new List<KeyValuePair<string, JsonData>>();
			return (IDictionary)this.inst_object;
		}

		// Token: 0x060040D4 RID: 16596 RVA: 0x0012C2E4 File Offset: 0x0012A4E4
		private IList EnsureList()
		{
			if (this.type == JsonType.Array)
			{
				return (IList)this.inst_array;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			this.type = JsonType.Array;
			this.inst_array = new List<JsonData>();
			return (IList)this.inst_array;
		}

		// Token: 0x060040D5 RID: 16597 RVA: 0x0012C336 File Offset: 0x0012A536
		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		// Token: 0x060040D6 RID: 16598 RVA: 0x0012C354 File Offset: 0x0012A554
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}
			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}
			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}
			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}
			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object obj2 in obj)
				{
					JsonData.WriteJson((JsonData)obj2, writer);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (object obj3 in obj)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
				return;
			}
		}

		// Token: 0x060040D7 RID: 16599 RVA: 0x0012C49C File Offset: 0x0012A69C
		public int Add(object value)
		{
			JsonData jsonData = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(jsonData);
		}

		// Token: 0x060040D8 RID: 16600 RVA: 0x0012C4C4 File Offset: 0x0012A6C4
		public void Clear()
		{
			if (this.IsObject)
			{
				((IDictionary)this).Clear();
				return;
			}
			if (this.IsArray)
			{
				((IList)this).Clear();
				return;
			}
		}

		// Token: 0x060040D9 RID: 16601 RVA: 0x0012C4E4 File Offset: 0x0012A6E4
		public bool Equals(JsonData x)
		{
			if (x == null)
			{
				return false;
			}
			if (x.type != this.type)
			{
				return false;
			}
			switch (this.type)
			{
			case JsonType.None:
				return true;
			case JsonType.Object:
				return this.inst_object.Equals(x.inst_object);
			case JsonType.Array:
				return this.inst_array.Equals(x.inst_array);
			case JsonType.String:
				return this.inst_string.Equals(x.inst_string);
			case JsonType.Int:
				return this.inst_int.Equals(x.inst_int);
			case JsonType.Long:
				return this.inst_long.Equals(x.inst_long);
			case JsonType.Double:
				return this.inst_double.Equals(x.inst_double);
			case JsonType.Boolean:
				return this.inst_boolean.Equals(x.inst_boolean);
			default:
				return false;
			}
		}

		// Token: 0x060040DA RID: 16602 RVA: 0x0012C5B9 File Offset: 0x0012A7B9
		public JsonType GetJsonType()
		{
			return this.type;
		}

		// Token: 0x060040DB RID: 16603 RVA: 0x0012C5C4 File Offset: 0x0012A7C4
		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
			{
				return;
			}
			switch (type)
			{
			case JsonType.Object:
				this.inst_object = new Dictionary<string, JsonData>();
				this.object_list = new List<KeyValuePair<string, JsonData>>();
				break;
			case JsonType.Array:
				this.inst_array = new List<JsonData>();
				break;
			case JsonType.String:
				this.inst_string = null;
				break;
			case JsonType.Int:
				this.inst_int = 0;
				break;
			case JsonType.Long:
				this.inst_long = 0L;
				break;
			case JsonType.Double:
				this.inst_double = 0.0;
				break;
			case JsonType.Boolean:
				this.inst_boolean = false;
				break;
			}
			this.type = type;
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x0012C664 File Offset: 0x0012A864
		public string ToJson()
		{
			if (this.json != null)
			{
				return this.json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonData.WriteJson(this, new JsonWriter(stringWriter)
			{
				Validate = false
			});
			this.json = stringWriter.ToString();
			return this.json;
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x0012C6B0 File Offset: 0x0012A8B0
		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		// Token: 0x060040DE RID: 16606 RVA: 0x0012C6DC File Offset: 0x0012A8DC
		public override string ToString()
		{
			switch (this.type)
			{
			case JsonType.Object:
				return "JsonData object";
			case JsonType.Array:
				return "JsonData array";
			case JsonType.String:
				return this.inst_string;
			case JsonType.Int:
				return this.inst_int.ToString();
			case JsonType.Long:
				return this.inst_long.ToString();
			case JsonType.Double:
				return this.inst_double.ToString();
			case JsonType.Boolean:
				return this.inst_boolean.ToString();
			default:
				return "Uninitialized JsonData";
			}
		}

		// Token: 0x040043D0 RID: 17360
		private IList<JsonData> inst_array;

		// Token: 0x040043D1 RID: 17361
		private bool inst_boolean;

		// Token: 0x040043D2 RID: 17362
		private double inst_double;

		// Token: 0x040043D3 RID: 17363
		private int inst_int;

		// Token: 0x040043D4 RID: 17364
		private long inst_long;

		// Token: 0x040043D5 RID: 17365
		private IDictionary<string, JsonData> inst_object;

		// Token: 0x040043D6 RID: 17366
		private string inst_string;

		// Token: 0x040043D7 RID: 17367
		private string json;

		// Token: 0x040043D8 RID: 17368
		private JsonType type;

		// Token: 0x040043D9 RID: 17369
		private IList<KeyValuePair<string, JsonData>> object_list;
	}
}
