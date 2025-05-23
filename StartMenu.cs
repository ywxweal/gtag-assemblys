using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000380 RID: 896
public class StartMenu : MonoBehaviour
{
	// Token: 0x060014C5 RID: 5317 RVA: 0x00065428 File Offset: 0x00063628
	private void Start()
	{
		DebugUIBuilder.instance.AddLabel("Select Sample Scene", 0);
		int sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
		for (int i = 0; i < sceneCountInBuildSettings; i++)
		{
			string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
			int sceneIndex = i;
			DebugUIBuilder.instance.AddButton(Path.GetFileNameWithoutExtension(scenePathByBuildIndex), delegate
			{
				this.LoadScene(sceneIndex);
			}, -1, 0, false);
		}
		DebugUIBuilder.instance.Show();
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x0006549D File Offset: 0x0006369D
	private void LoadScene(int idx)
	{
		DebugUIBuilder.instance.Hide();
		Debug.Log("Load scene: " + idx.ToString());
		SceneManager.LoadScene(idx);
	}

	// Token: 0x04001715 RID: 5909
	public OVROverlay overlay;

	// Token: 0x04001716 RID: 5910
	public OVROverlay text;

	// Token: 0x04001717 RID: 5911
	public OVRCameraRig vrRig;
}
