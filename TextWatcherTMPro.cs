using System;
using TMPro;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public class TextWatcherTMPro : MonoBehaviour
{
	// Token: 0x06002B35 RID: 11061 RVA: 0x000D4806 File Offset: 0x000D2A06
	private void Start()
	{
		this.myText = base.GetComponent<TextMeshPro>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x000D482C File Offset: 0x000D2A2C
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x000D4845 File Offset: 0x000D2A45
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04003043 RID: 12355
	public WatchableStringSO textToCopy;

	// Token: 0x04003044 RID: 12356
	private TextMeshPro myText;
}
