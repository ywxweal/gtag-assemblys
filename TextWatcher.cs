using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006C4 RID: 1732
public class TextWatcher : MonoBehaviour
{
	// Token: 0x06002B32 RID: 11058 RVA: 0x000D485D File Offset: 0x000D2A5D
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x000D4883 File Offset: 0x000D2A83
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x000D489C File Offset: 0x000D2A9C
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04003043 RID: 12355
	public WatchableStringSO textToCopy;

	// Token: 0x04003044 RID: 12356
	private Text myText;
}
