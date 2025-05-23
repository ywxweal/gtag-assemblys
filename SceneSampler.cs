using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000346 RID: 838
public class SceneSampler : MonoBehaviour
{
	// Token: 0x060013CD RID: 5069 RVA: 0x0005FFE0 File Offset: 0x0005E1E0
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0005FFF0 File Offset: 0x0005E1F0
	private void Update()
	{
		bool flag = OVRInput.GetActiveController() == OVRInput.Controller.Touch || OVRInput.GetActiveController() == OVRInput.Controller.LTouch || OVRInput.GetActiveController() == OVRInput.Controller.RTouch;
		this.displayText.SetActive(flag);
		if (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			this.currentSceneIndex++;
			if (this.currentSceneIndex >= SceneManager.sceneCountInBuildSettings)
			{
				this.currentSceneIndex = 0;
			}
			SceneManager.LoadScene(this.currentSceneIndex);
		}
		Vector3 vector = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + Vector3.up * 0.09f;
		this.displayText.transform.position = vector;
		this.displayText.transform.rotation = Quaternion.LookRotation(vector - Camera.main.transform.position);
	}

	// Token: 0x04001600 RID: 5632
	private int currentSceneIndex;

	// Token: 0x04001601 RID: 5633
	public GameObject displayText;
}
