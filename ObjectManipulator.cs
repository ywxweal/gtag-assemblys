using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200033A RID: 826
public class ObjectManipulator : MonoBehaviour
{
	// Token: 0x06001382 RID: 4994 RVA: 0x0005E40C File Offset: 0x0005C60C
	private void Start()
	{
		if (this.passthrough)
		{
			this.passthrough.colorMapEditorBrightness = -1f;
			this.passthrough.colorMapEditorContrast = -1f;
		}
		base.StartCoroutine(this.StartDemo());
		if (this.objectNameLabel)
		{
			this.objectNameLabel.font.material.renderQueue = 4600;
		}
		if (this.objectInstructionsLabel)
		{
			this.objectInstructionsLabel.font.material.renderQueue = 4600;
		}
		if (this.objectInfoBG)
		{
			this.objectInfoBG.materialForRendering.renderQueue = 4599;
		}
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x0005E4C4 File Offset: 0x0005C6C4
	private void Update()
	{
		Vector3 localControllerPosition = OVRInput.GetLocalControllerPosition(this.controller);
		Quaternion localControllerRotation = OVRInput.GetLocalControllerRotation(this.controller);
		this.FindHoverObject(localControllerPosition, localControllerRotation);
		if (this.hoverObject && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, this.controller))
		{
			this.grabObject = this.hoverObject;
			this.GrabHoverObject(this.grabObject, localControllerPosition, localControllerRotation);
		}
		if (this.grabObject)
		{
			this.grabTime += Time.deltaTime * 5f;
			this.grabTime = Mathf.Clamp01(this.grabTime);
			this.ManipulateObject(this.grabObject, localControllerPosition, localControllerRotation);
			if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, this.controller))
			{
				this.ReleaseObject();
				return;
			}
		}
		else
		{
			this.grabTime -= Time.deltaTime * 5f;
			this.grabTime = Mathf.Clamp01(this.grabTime);
		}
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x0005E5B0 File Offset: 0x0005C7B0
	private void GrabHoverObject(GameObject grbObj, Vector3 controllerPos, Quaternion controllerRot)
	{
		this.localGrabOffset = Quaternion.Inverse(controllerRot) * (this.grabObject.transform.position - controllerPos);
		this.localGrabRotation = Quaternion.Inverse(controllerRot) * this.grabObject.transform.rotation;
		this.rotationOffset = 0f;
		if (this.grabObject.GetComponent<GrabObject>())
		{
			this.grabObject.GetComponent<GrabObject>().Grab(this.controller);
			this.grabObject.GetComponent<GrabObject>().grabbedRotation = this.grabObject.transform.rotation;
			this.AssignInstructions(this.grabObject.GetComponent<GrabObject>());
		}
		this.handGrabPosition = controllerPos;
		this.handGrabRotation = controllerRot;
		this.camGrabPosition = Camera.main.transform.position;
		this.camGrabRotation = Camera.main.transform.rotation;
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x0005E6A4 File Offset: 0x0005C8A4
	private void ReleaseObject()
	{
		if (this.grabObject.GetComponent<GrabObject>())
		{
			if (this.grabObject.GetComponent<GrabObject>().objectManipulationType == GrabObject.ManipulationType.Menu)
			{
				this.grabObject.transform.position = this.handGrabPosition + this.handGrabRotation * this.localGrabOffset;
				this.grabObject.transform.rotation = this.handGrabRotation * this.localGrabRotation;
			}
			this.grabObject.GetComponent<GrabObject>().Release();
		}
		this.grabObject = null;
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x0005E73A File Offset: 0x0005C93A
	private IEnumerator StartDemo()
	{
		this.demoObjects.SetActive(false);
		float timer = 0f;
		float fadeTime = 1f;
		while (timer <= fadeTime)
		{
			timer += Time.deltaTime;
			float num = Mathf.Clamp01(timer / fadeTime);
			if (this.passthrough)
			{
				this.passthrough.colorMapEditorBrightness = Mathf.Lerp(-1f, 0f, num);
				this.passthrough.colorMapEditorContrast = Mathf.Lerp(-1f, 0f, num);
			}
			yield return null;
		}
		this.demoObjects.SetActive(true);
		Vector3 normalized = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
		this.demoObjects.transform.position = Camera.main.transform.position + normalized;
		this.demoObjects.transform.rotation = Quaternion.LookRotation(normalized);
		yield break;
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x0005E74C File Offset: 0x0005C94C
	private void FindHoverObject(Vector3 controllerPos, Quaternion controllerRot)
	{
		RaycastHit[] array = Physics.RaycastAll(controllerPos, controllerRot * Vector3.forward);
		float num = float.PositiveInfinity;
		float num2 = 2f;
		bool flag = true;
		Vector3 vector = Vector3.zero;
		foreach (RaycastHit raycastHit in array)
		{
			float num3 = Vector3.Distance(raycastHit.point, controllerPos);
			if (num3 < num)
			{
				this.hoverObject = raycastHit.collider.gameObject;
				num = num3;
				num2 = (this.grabObject ? num3 : (num3 - 0.1f));
				vector = raycastHit.point;
			}
		}
		if (array.Length == 0)
		{
			this.hoverObject = null;
		}
		foreach (Collider collider in Physics.OverlapSphere(controllerPos, 0.05f))
		{
			this.hoverObject = collider.gameObject;
			flag = false;
			vector = collider.ClosestPoint(controllerPos);
			vector += (Camera.main.transform.position - vector).normalized * 0.05f;
		}
		if (this.objectInfo && this.objectInstructionsLabel)
		{
			bool flag2 = this.hoverObject || this.grabObject;
			this.objectInfo.gameObject.SetActive(flag2);
			Vector3 vector2 = vector - Camera.main.transform.position;
			if (this.hoverObject && !this.grabObject)
			{
				Vector3 vector3 = vector - vector2.normalized * 0.05f;
				this.objectInfo.position = Vector3.Lerp(vector3, this.objectInfo.position, this.grabTime);
				this.objectInfo.rotation = Quaternion.LookRotation(vector2);
				this.objectInfo.localScale = Vector3.one * vector2.magnitude * 2f;
				if (this.hoverObject.GetComponent<GrabObject>())
				{
					this.AssignInstructions(this.hoverObject.GetComponent<GrabObject>());
				}
			}
			else if (this.grabObject)
			{
				Vector3 vector4 = controllerPos + (Camera.main.transform.position - controllerPos).normalized * 0.1f;
				this.objectInfo.position = Vector3.Lerp(this.objectInfo.position, vector4, this.grabTime);
				this.objectInfo.rotation = Quaternion.LookRotation(this.objectInfo.position - Camera.main.transform.position);
				this.objectInfo.localScale = Vector3.one;
				if (this.grabObject.GetComponent<GrabObject>())
				{
					flag = this.grabObject.GetComponent<GrabObject>().showLaserWhileGrabbed;
				}
			}
		}
		if (this.laser)
		{
			this.laser.positionCount = 2;
			Vector3 vector5 = controllerPos + controllerRot * (Vector3.forward * 0.05f);
			this.cursorPosition = controllerPos + controllerRot * (Vector3.forward * num2);
			this.laser.SetPosition(0, vector5);
			this.laser.SetPosition(1, this.cursorPosition);
			this.laser.enabled = flag;
			if (this.grabObject && this.grabObject.GetComponent<GrabObject>())
			{
				this.grabObject.GetComponent<GrabObject>().CursorPos(this.cursorPosition);
			}
		}
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x0005EB10 File Offset: 0x0005CD10
	private void ManipulateObject(GameObject obj, Vector3 controllerPos, Quaternion controllerRot)
	{
		bool flag = true;
		Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.controller);
		if (obj.GetComponent<GrabObject>())
		{
			flag = false;
			switch (obj.GetComponent<GrabObject>().objectManipulationType)
			{
			case GrabObject.ManipulationType.Default:
				flag = true;
				break;
			case GrabObject.ManipulationType.ForcedHand:
				obj.transform.position = Vector3.Lerp(obj.transform.position, controllerPos, this.grabTime);
				obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, controllerRot, this.grabTime);
				break;
			case GrabObject.ManipulationType.DollyHand:
			{
				float num = this.localGrabOffset.z + vector.y * 0.01f;
				num = Mathf.Clamp(num, 0.1f, 2f);
				this.localGrabOffset = Vector3.forward * num;
				obj.transform.position = Vector3.Lerp(obj.transform.position, controllerPos + controllerRot * this.localGrabOffset, this.grabTime);
				obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, controllerRot, this.grabTime);
				break;
			}
			case GrabObject.ManipulationType.DollyAttached:
				obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
				obj.transform.rotation = controllerRot * this.localGrabRotation;
				this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
				break;
			case GrabObject.ManipulationType.HorizontalScaled:
			{
				obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
				if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, this.controller))
				{
					obj.transform.localScale = this.ClampScale(obj.transform.localScale, vector);
				}
				else
				{
					this.rotationOffset -= vector.x;
					this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
				}
				Vector3 vector2 = obj.GetComponent<GrabObject>().grabbedRotation * Vector3.forward;
				vector2 = new Vector3(vector2.x, 0f, vector2.z);
				obj.transform.rotation = Quaternion.Euler(0f, this.rotationOffset, 0f) * Quaternion.LookRotation(vector2);
				break;
			}
			case GrabObject.ManipulationType.VerticalScaled:
			{
				obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
				if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, this.controller))
				{
					obj.transform.localScale = this.ClampScale(obj.transform.localScale, vector);
				}
				else
				{
					this.rotationOffset -= vector.x;
					this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
				}
				Vector3 vector3 = obj.GetComponent<GrabObject>().grabbedRotation * Vector3.up;
				vector3 = new Vector3(vector3.x, 0f, vector3.z);
				obj.transform.rotation = Quaternion.LookRotation(Vector3.up, Quaternion.Euler(0f, this.rotationOffset, 0f) * vector3);
				break;
			}
			case GrabObject.ManipulationType.Menu:
			{
				Vector3 vector4 = this.handGrabPosition + this.handGrabRotation * Vector3.forward * 0.4f;
				Quaternion quaternion = Quaternion.LookRotation(vector4 - this.camGrabPosition);
				obj.transform.position = Vector3.Lerp(obj.transform.position, vector4, this.grabTime);
				obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, quaternion, this.grabTime);
				break;
			}
			default:
				flag = true;
				break;
			}
		}
		if (flag)
		{
			obj.transform.position = controllerPos + controllerRot * this.localGrabOffset;
			obj.transform.Rotate(Vector3.up * -vector.x);
			this.ClampGrabOffset(ref this.localGrabOffset, vector.y);
		}
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x0005EF24 File Offset: 0x0005D124
	private void ClampGrabOffset(ref Vector3 localOffset, float thumbY)
	{
		Vector3 vector = localOffset + Vector3.forward * thumbY * 0.01f;
		if (vector.z > 0.1f)
		{
			localOffset = vector;
		}
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x0005EF68 File Offset: 0x0005D168
	private Vector3 ClampScale(Vector3 localScale, Vector2 thumb)
	{
		float num = localScale.x + thumb.x * 0.01f;
		if (num <= 0.1f)
		{
			num = 0.1f;
		}
		float num2 = localScale.z + thumb.y * 0.01f;
		if (num2 <= 0.1f)
		{
			num2 = 0.1f;
		}
		return new Vector3(num, 0f, num2);
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x0005EFC8 File Offset: 0x0005D1C8
	private void CheckForDominantHand()
	{
		if (this.hoverObject || this.grabObject)
		{
			return;
		}
		if (this.controller == OVRInput.Controller.RTouch)
		{
			if (OVRInput.Get(OVRInput.RawButton.LHandTrigger, OVRInput.Controller.Active))
			{
				this.controller = OVRInput.Controller.LTouch;
				return;
			}
		}
		else if (OVRInput.Get(OVRInput.RawButton.RHandTrigger, OVRInput.Controller.Active))
		{
			this.controller = OVRInput.Controller.RTouch;
		}
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x0005F02C File Offset: 0x0005D22C
	private void AssignInstructions(GrabObject targetObject)
	{
		if (this.objectNameLabel)
		{
			this.objectNameLabel.text = targetObject.ObjectName;
		}
		if (this.objectInstructionsLabel)
		{
			if (this.grabObject)
			{
				this.objectInstructionsLabel.text = targetObject.ObjectInstructions;
				return;
			}
			this.objectInstructionsLabel.text = "Grip Trigger to grab";
		}
	}

	// Token: 0x040015AE RID: 5550
	private OVRInput.Controller controller = OVRInput.Controller.RTouch;

	// Token: 0x040015AF RID: 5551
	private GameObject hoverObject;

	// Token: 0x040015B0 RID: 5552
	private GameObject grabObject;

	// Token: 0x040015B1 RID: 5553
	private float grabTime;

	// Token: 0x040015B2 RID: 5554
	private Vector3 localGrabOffset = Vector3.zero;

	// Token: 0x040015B3 RID: 5555
	private Quaternion localGrabRotation = Quaternion.identity;

	// Token: 0x040015B4 RID: 5556
	private Vector3 camGrabPosition = Vector3.zero;

	// Token: 0x040015B5 RID: 5557
	private Quaternion camGrabRotation = Quaternion.identity;

	// Token: 0x040015B6 RID: 5558
	private Vector3 handGrabPosition = Vector3.zero;

	// Token: 0x040015B7 RID: 5559
	private Quaternion handGrabRotation = Quaternion.identity;

	// Token: 0x040015B8 RID: 5560
	private Vector3 cursorPosition = Vector3.zero;

	// Token: 0x040015B9 RID: 5561
	private float rotationOffset;

	// Token: 0x040015BA RID: 5562
	public LineRenderer laser;

	// Token: 0x040015BB RID: 5563
	public Transform objectInfo;

	// Token: 0x040015BC RID: 5564
	public TextMesh objectNameLabel;

	// Token: 0x040015BD RID: 5565
	public TextMesh objectInstructionsLabel;

	// Token: 0x040015BE RID: 5566
	public Image objectInfoBG;

	// Token: 0x040015BF RID: 5567
	public GameObject demoObjects;

	// Token: 0x040015C0 RID: 5568
	public OVRPassthroughLayer passthrough;
}
