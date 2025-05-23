using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000376 RID: 886
[Serializable]
public class Spawnable : ISerializationCallbackReceiver
{
	// Token: 0x06001482 RID: 5250 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnBeforeSerialize()
	{
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x000643B0 File Offset: 0x000625B0
	public void OnAfterDeserialize()
	{
		if (this.ClassificationLabel != "")
		{
			this._editorClassificationIndex = Spawnable.<OnAfterDeserialize>g__IndexOf|4_0(this.ClassificationLabel, OVRSceneManager.Classification.List);
			if (this._editorClassificationIndex < 0)
			{
				Debug.LogError("[Spawnable] OnAfterDeserialize() " + this.ClassificationLabel + " not found. The Classification list in OVRSceneManager has likely changed");
				return;
			}
		}
		else
		{
			this._editorClassificationIndex = 0;
		}
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x00064424 File Offset: 0x00062624
	[CompilerGenerated]
	internal static int <OnAfterDeserialize>g__IndexOf|4_0(string label, IEnumerable<string> collection)
	{
		int num = 0;
		using (IEnumerator<string> enumerator = collection.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == label)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x040016DD RID: 5853
	public SimpleResizable ResizablePrefab;

	// Token: 0x040016DE RID: 5854
	public string ClassificationLabel = "";

	// Token: 0x040016DF RID: 5855
	[SerializeField]
	private int _editorClassificationIndex;
}
