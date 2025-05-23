using System;
using TMPro;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public class TextWatcherTMPro : MonoBehaviour
{
	// Token: 0x06002B36 RID: 11062 RVA: 0x000D48AA File Offset: 0x000D2AAA
	private void Start()
	{
		this.myText = base.GetComponent<TextMeshPro>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x000D48D0 File Offset: 0x000D2AD0
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x000D48E9 File Offset: 0x000D2AE9
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04003045 RID: 12357
	public WatchableStringSO textToCopy;

	// Token: 0x04003046 RID: 12358
	private TextMeshPro myText;
}
