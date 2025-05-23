using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x02000C04 RID: 3076
	public class OVROverlaySample : MonoBehaviour
	{
		// Token: 0x06004BFD RID: 19453 RVA: 0x00167E8C File Offset: 0x0016608C
		private void Start()
		{
			DebugUIBuilder.instance.AddLabel("OVROverlay Sample", 0);
			DebugUIBuilder.instance.AddDivider(0);
			DebugUIBuilder.instance.AddLabel("Level Loading Example", 0);
			DebugUIBuilder.instance.AddButton("Simulate Level Load", new DebugUIBuilder.OnClick(this.TriggerLoad), -1, 0, false);
			DebugUIBuilder.instance.AddButton("Destroy Cubes", new DebugUIBuilder.OnClick(this.TriggerUnload), -1, 0, false);
			DebugUIBuilder.instance.AddDivider(0);
			DebugUIBuilder.instance.AddLabel("OVROverlay vs. Application Render Comparison", 0);
			DebugUIBuilder.instance.AddRadio("OVROverlay", "group", delegate(Toggle t)
			{
				this.RadioPressed("OVROverlayID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			this.applicationRadioButton = DebugUIBuilder.instance.AddRadio("Application", "group", delegate(Toggle t)
			{
				this.RadioPressed("ApplicationID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			this.noneRadioButton = DebugUIBuilder.instance.AddRadio("None", "group", delegate(Toggle t)
			{
				this.RadioPressed("NoneID", "group", t);
			}, 0).GetComponentInChildren<Toggle>();
			DebugUIBuilder.instance.Show();
			this.CameraAndRenderTargetSetup();
			this.cameraRenderOverlay.enabled = true;
			this.cameraRenderOverlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
			this.spawnedCubes.Capacity = this.numObjectsPerLevel * this.numLevels;
		}

		// Token: 0x06004BFE RID: 19454 RVA: 0x00167FE4 File Offset: 0x001661E4
		private void Update()
		{
			if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
			{
				if (this.inMenu)
				{
					DebugUIBuilder.instance.Hide();
				}
				else
				{
					DebugUIBuilder.instance.Show();
				}
				this.inMenu = !this.inMenu;
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				this.TriggerLoad();
			}
		}

		// Token: 0x06004BFF RID: 19455 RVA: 0x0016804C File Offset: 0x0016624C
		private void ActivateWorldGeo()
		{
			this.worldspaceGeoParent.SetActive(true);
			this.uiGeoParent.SetActive(false);
			this.uiCamera.SetActive(false);
			this.cameraRenderOverlay.enabled = false;
			this.renderingLabelOverlay.enabled = true;
			this.renderingLabelOverlay.textures[0] = this.applicationLabelTexture;
			Debug.Log("Switched to ActivateWorldGeo");
		}

		// Token: 0x06004C00 RID: 19456 RVA: 0x001680B4 File Offset: 0x001662B4
		private void ActivateOVROverlay()
		{
			this.worldspaceGeoParent.SetActive(false);
			this.uiCamera.SetActive(true);
			this.cameraRenderOverlay.enabled = true;
			this.uiGeoParent.SetActive(true);
			this.renderingLabelOverlay.enabled = true;
			this.renderingLabelOverlay.textures[0] = this.compositorLabelTexture;
			Debug.Log("Switched to ActivateOVROVerlay");
		}

		// Token: 0x06004C01 RID: 19457 RVA: 0x0016811C File Offset: 0x0016631C
		private void ActivateNone()
		{
			this.worldspaceGeoParent.SetActive(false);
			this.uiCamera.SetActive(false);
			this.cameraRenderOverlay.enabled = false;
			this.uiGeoParent.SetActive(false);
			this.renderingLabelOverlay.enabled = false;
			Debug.Log("Switched to ActivateNone");
		}

		// Token: 0x06004C02 RID: 19458 RVA: 0x0016816F File Offset: 0x0016636F
		private void TriggerLoad()
		{
			base.StartCoroutine(this.WaitforOVROverlay());
		}

		// Token: 0x06004C03 RID: 19459 RVA: 0x0016817E File Offset: 0x0016637E
		private IEnumerator WaitforOVROverlay()
		{
			Transform transform = this.mainCamera.transform;
			Transform transform2 = this.loadingTextQuadOverlay.transform;
			Vector3 vector = transform.position + transform.forward * this.distanceFromCamToLoadText;
			vector.y = transform.position.y;
			transform2.position = vector;
			this.cubemapOverlay.enabled = true;
			this.loadingTextQuadOverlay.enabled = true;
			this.noneRadioButton.isOn = true;
			yield return new WaitForSeconds(0.1f);
			this.ClearObjects();
			this.SimulateLevelLoad();
			this.cubemapOverlay.enabled = false;
			this.loadingTextQuadOverlay.enabled = false;
			yield return null;
			yield break;
		}

		// Token: 0x06004C04 RID: 19460 RVA: 0x0016818D File Offset: 0x0016638D
		private void TriggerUnload()
		{
			this.ClearObjects();
			this.applicationRadioButton.isOn = true;
		}

		// Token: 0x06004C05 RID: 19461 RVA: 0x001681A4 File Offset: 0x001663A4
		private void CameraAndRenderTargetSetup()
		{
			float x = this.cameraRenderOverlay.transform.localScale.x;
			float y = this.cameraRenderOverlay.transform.localScale.y;
			float z = this.cameraRenderOverlay.transform.localScale.z;
			float num = 2160f;
			float num2 = 1200f;
			float num3 = num * 0.5f;
			float num4 = num2;
			float num5 = this.mainCamera.GetComponent<Camera>().fieldOfView / 2f;
			float num6 = 2f * z * Mathf.Tan(0.017453292f * num5);
			float num7 = num4 / num6 * x;
			float num8 = num6 * this.mainCamera.GetComponent<Camera>().aspect;
			float num9 = num3 / num8 * x;
			float num10 = y / 2f;
			float num11 = x / y;
			this.uiCamera.GetComponent<Camera>().orthographicSize = num10;
			this.uiCamera.GetComponent<Camera>().aspect = num11;
			if (this.uiCamera.GetComponent<Camera>().targetTexture != null)
			{
				this.uiCamera.GetComponent<Camera>().targetTexture.Release();
			}
			RenderTexture renderTexture = new RenderTexture((int)num9 * 2, (int)num7 * 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			Debug.Log("Created RT of resolution w: " + num9.ToString() + " and h: " + num7.ToString());
			renderTexture.hideFlags = HideFlags.DontSave;
			renderTexture.useMipMap = true;
			renderTexture.filterMode = FilterMode.Trilinear;
			renderTexture.anisoLevel = 4;
			renderTexture.autoGenerateMips = true;
			this.uiCamera.GetComponent<Camera>().targetTexture = renderTexture;
			this.cameraRenderOverlay.textures[0] = renderTexture;
		}

		// Token: 0x06004C06 RID: 19462 RVA: 0x00168340 File Offset: 0x00166540
		private void SimulateLevelLoad()
		{
			int num = 0;
			for (int i = 0; i < this.numLoopsTrigger; i++)
			{
				num++;
			}
			Debug.Log("Finished " + num.ToString() + " Loops");
			Vector3 position = this.mainCamera.transform.position;
			position.y = 0.5f;
			for (int j = 0; j < this.numLevels; j++)
			{
				for (int k = 0; k < this.numObjectsPerLevel; k++)
				{
					float num2 = (float)k * 3.1415927f * 2f / (float)this.numObjectsPerLevel;
					float num3 = ((k % 2 == 0) ? 1.5f : 1f);
					Vector3 vector = new Vector3(Mathf.Cos(num2), 0f, Mathf.Sin(num2)) * this.cubeSpawnRadius * num3;
					vector.y = (float)j * this.heightBetweenItems;
					GameObject gameObject = Object.Instantiate<GameObject>(this.prefabForLevelLoadSim, vector + position, Quaternion.identity);
					Transform transform = gameObject.transform;
					transform.LookAt(position);
					Vector3 eulerAngles = transform.rotation.eulerAngles;
					eulerAngles.x = 0f;
					transform.rotation = Quaternion.Euler(eulerAngles);
					this.spawnedCubes.Add(gameObject);
				}
			}
		}

		// Token: 0x06004C07 RID: 19463 RVA: 0x00168498 File Offset: 0x00166698
		private void ClearObjects()
		{
			for (int i = 0; i < this.spawnedCubes.Count; i++)
			{
				Object.DestroyImmediate(this.spawnedCubes[i]);
			}
			this.spawnedCubes.Clear();
			GC.Collect();
		}

		// Token: 0x06004C08 RID: 19464 RVA: 0x001684DC File Offset: 0x001666DC
		public void RadioPressed(string radioLabel, string group, Toggle t)
		{
			if (string.Compare(radioLabel, "OVROverlayID") == 0)
			{
				this.ActivateOVROverlay();
				return;
			}
			if (string.Compare(radioLabel, "ApplicationID") == 0)
			{
				this.ActivateWorldGeo();
				return;
			}
			if (string.Compare(radioLabel, "NoneID") == 0)
			{
				this.ActivateNone();
			}
		}

		// Token: 0x04004EB7 RID: 20151
		private bool inMenu;

		// Token: 0x04004EB8 RID: 20152
		private const string ovrOverlayID = "OVROverlayID";

		// Token: 0x04004EB9 RID: 20153
		private const string applicationID = "ApplicationID";

		// Token: 0x04004EBA RID: 20154
		private const string noneID = "NoneID";

		// Token: 0x04004EBB RID: 20155
		private Toggle applicationRadioButton;

		// Token: 0x04004EBC RID: 20156
		private Toggle noneRadioButton;

		// Token: 0x04004EBD RID: 20157
		[Header("App vs Compositor Comparison Settings")]
		public GameObject mainCamera;

		// Token: 0x04004EBE RID: 20158
		public GameObject uiCamera;

		// Token: 0x04004EBF RID: 20159
		public GameObject uiGeoParent;

		// Token: 0x04004EC0 RID: 20160
		public GameObject worldspaceGeoParent;

		// Token: 0x04004EC1 RID: 20161
		public OVROverlay cameraRenderOverlay;

		// Token: 0x04004EC2 RID: 20162
		public OVROverlay renderingLabelOverlay;

		// Token: 0x04004EC3 RID: 20163
		public Texture applicationLabelTexture;

		// Token: 0x04004EC4 RID: 20164
		public Texture compositorLabelTexture;

		// Token: 0x04004EC5 RID: 20165
		[Header("Level Loading Sim Settings")]
		public GameObject prefabForLevelLoadSim;

		// Token: 0x04004EC6 RID: 20166
		public OVROverlay cubemapOverlay;

		// Token: 0x04004EC7 RID: 20167
		public OVROverlay loadingTextQuadOverlay;

		// Token: 0x04004EC8 RID: 20168
		public float distanceFromCamToLoadText;

		// Token: 0x04004EC9 RID: 20169
		public float cubeSpawnRadius;

		// Token: 0x04004ECA RID: 20170
		public float heightBetweenItems;

		// Token: 0x04004ECB RID: 20171
		public int numObjectsPerLevel;

		// Token: 0x04004ECC RID: 20172
		public int numLevels;

		// Token: 0x04004ECD RID: 20173
		public int numLoopsTrigger = 500000000;

		// Token: 0x04004ECE RID: 20174
		private List<GameObject> spawnedCubes = new List<GameObject>();
	}
}
