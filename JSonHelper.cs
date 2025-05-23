using System;
using UnityEngine;

// Token: 0x020003E5 RID: 997
public static class JSonHelper
{
	// Token: 0x0600180B RID: 6155 RVA: 0x000751F2 File Offset: 0x000733F2
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JSonHelper.Wrapper<T>>(json).Items;
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x000751FF File Offset: 0x000733FF
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x00075212 File Offset: 0x00073412
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x020003E6 RID: 998
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x04001AE9 RID: 6889
		public T[] Items;
	}
}
