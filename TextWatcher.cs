using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C4 RID: 1732
public class TextWatcher : MonoBehaviour
{
	// Token: 0x06002B31 RID: 11057 RVA: 0x000D47B9 File Offset: 0x000D29B9
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x000D47DF File Offset: 0x000D29DF
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x000D47F8 File Offset: 0x000D29F8
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04003041 RID: 12353
	public WatchableStringSO textToCopy;

	// Token: 0x04003042 RID: 12354
	private Text myText;
}
