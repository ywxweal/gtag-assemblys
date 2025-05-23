using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000389 RID: 905
public class UiSceneMenu : MonoBehaviour
{
	// Token: 0x060014E5 RID: 5349 RVA: 0x00065DFC File Offset: 0x00063FFC
	private void Awake()
	{
		this.m_activeScene = SceneManager.GetActiveScene();
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
			this.CreateLabel(i, scenePathByBuildIndex);
		}
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x00065E34 File Offset: 0x00064034
	private void Update()
	{
		int sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
		if (this.InputPrevScene())
		{
			this.ChangeScene((this.m_activeScene.buildIndex - 1 + sceneCountInBuildSettings) % sceneCountInBuildSettings);
		}
		else if (this.InputNextScene())
		{
			this.ChangeScene((this.m_activeScene.buildIndex + 1) % sceneCountInBuildSettings);
		}
		UiSceneMenu.s_lastThumbstickL = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
		UiSceneMenu.s_lastThumbstickR = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x00065E9D File Offset: 0x0006409D
	private bool InputPrevScene()
	{
		return this.KeyboardPrevScene() || this.ThumbstickPrevScene(OVRInput.Controller.LTouch) || this.ThumbstickPrevScene(OVRInput.Controller.RTouch);
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x00065EB9 File Offset: 0x000640B9
	private bool InputNextScene()
	{
		return this.KeyboardNextScene() || this.ThumbstickNextScene(OVRInput.Controller.LTouch) || this.ThumbstickNextScene(OVRInput.Controller.RTouch);
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x00065ED5 File Offset: 0x000640D5
	private bool KeyboardPrevScene()
	{
		return Input.GetKeyDown(KeyCode.UpArrow);
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x00065EE1 File Offset: 0x000640E1
	private bool KeyboardNextScene()
	{
		return Input.GetKeyDown(KeyCode.DownArrow);
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x00065EED File Offset: 0x000640ED
	private bool ThumbstickPrevScene(OVRInput.Controller controller)
	{
		return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).y >= 0.9f && this.GetLastThumbstickValue(controller).y < 0.9f;
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x00065F17 File Offset: 0x00064117
	private bool ThumbstickNextScene(OVRInput.Controller controller)
	{
		return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller).y <= -0.9f && this.GetLastThumbstickValue(controller).y > -0.9f;
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x00065F41 File Offset: 0x00064141
	private Vector2 GetLastThumbstickValue(OVRInput.Controller controller)
	{
		if (controller != OVRInput.Controller.LTouch)
		{
			return UiSceneMenu.s_lastThumbstickR;
		}
		return UiSceneMenu.s_lastThumbstickL;
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x00065F52 File Offset: 0x00064152
	private void ChangeScene(int nextScene)
	{
		SceneManager.LoadScene(nextScene);
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x00065F5C File Offset: 0x0006415C
	private void CreateLabel(int sceneIndex, string scenePath)
	{
		string text = Path.GetFileNameWithoutExtension(scenePath);
		text = Regex.Replace(text, "[A-Z]", " $0").Trim();
		if (this.m_activeScene.buildIndex == sceneIndex)
		{
			text = "Open: " + text;
		}
		TextMeshProUGUI textMeshProUGUI = Object.Instantiate<TextMeshProUGUI>(this.m_labelPf);
		textMeshProUGUI.SetText(string.Format("{0}. {1}", sceneIndex + 1, text), true);
		textMeshProUGUI.transform.SetParent(this.m_layoutGroup.transform, false);
	}

	// Token: 0x04001747 RID: 5959
	[Header("Settings")]
	[SerializeField]
	private VerticalLayoutGroup m_layoutGroup;

	// Token: 0x04001748 RID: 5960
	[SerializeField]
	private TextMeshProUGUI m_labelPf;

	// Token: 0x04001749 RID: 5961
	private static Vector2 s_lastThumbstickL;

	// Token: 0x0400174A RID: 5962
	private static Vector2 s_lastThumbstickR;

	// Token: 0x0400174B RID: 5963
	private Scene m_activeScene;
}
