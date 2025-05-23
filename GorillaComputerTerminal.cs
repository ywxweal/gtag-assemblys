using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006D6 RID: 1750
public class GorillaComputerTerminal : MonoBehaviour, IBuildValidation
{
	// Token: 0x06002B98 RID: 11160 RVA: 0x000D7218 File Offset: 0x000D5418
	public bool BuildValidationCheck()
	{
		if (this.myScreenText == null || this.myFunctionText == null || this.monitorMesh == null)
		{
			Debug.LogErrorFormat(base.gameObject, "gorilla computer terminal {0} is missing screen text, function text, or monitor mesh. this will break lots of computer stuff", new object[] { base.gameObject.name });
			return false;
		}
		return true;
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x000D7276 File Offset: 0x000D5476
	private void OnEnable()
	{
		if (GorillaComputer.instance == null)
		{
			base.StartCoroutine(this.<OnEnable>g__OnEnable_Local|4_0());
			return;
		}
		this.Init();
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x000D729C File Offset: 0x000D549C
	private void Init()
	{
		GameEvents.ScreenTextChangedEvent.AddListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.AddListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.AddListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
		this.myScreenText.text = GorillaComputer.instance.screenText.Text;
		this.myFunctionText.text = GorillaComputer.instance.functionSelectText.Text;
		if (GorillaComputer.instance.screenText.currentMaterials != null)
		{
			this.monitorMesh.materials = GorillaComputer.instance.screenText.currentMaterials;
		}
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x000D7354 File Offset: 0x000D5554
	private void OnDisable()
	{
		GameEvents.ScreenTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnScreenTextChanged));
		GameEvents.FunctionSelectTextChangedEvent.RemoveListener(new UnityAction<string>(this.OnFunctionTextChanged));
		GameEvents.ScreenTextMaterialsEvent.RemoveListener(new UnityAction<Material[]>(this.OnMaterialsChanged));
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x000D73A3 File Offset: 0x000D55A3
	public void OnScreenTextChanged(string text)
	{
		this.myScreenText.text = text;
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x000D73B1 File Offset: 0x000D55B1
	public void OnFunctionTextChanged(string text)
	{
		this.myFunctionText.text = text;
	}

	// Token: 0x06002B9E RID: 11166 RVA: 0x000D73BF File Offset: 0x000D55BF
	private void OnMaterialsChanged(Material[] materials)
	{
		this.monitorMesh.materials = materials;
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x000D73CD File Offset: 0x000D55CD
	[CompilerGenerated]
	private IEnumerator <OnEnable>g__OnEnable_Local|4_0()
	{
		yield return new WaitUntil(() => GorillaComputer.instance != null);
		yield return null;
		this.Init();
		yield break;
	}

	// Token: 0x0400319A RID: 12698
	public TextMeshPro myScreenText;

	// Token: 0x0400319B RID: 12699
	public TextMeshPro myFunctionText;

	// Token: 0x0400319C RID: 12700
	public MeshRenderer monitorMesh;
}
