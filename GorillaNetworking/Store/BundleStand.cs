using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C82 RID: 3202
	public class BundleStand : MonoBehaviour
	{
		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x06004F60 RID: 20320 RVA: 0x0017A147 File Offset: 0x00178347
		public string playfabBundleID
		{
			get
			{
				return this._bundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x06004F61 RID: 20321 RVA: 0x0017A154 File Offset: 0x00178354
		public void Awake()
		{
			this._bundlePurchaseButton.playfabID = this.playfabBundleID;
			if (this._bundleIcon != null && this._bundleDataReference != null && this._bundleDataReference.bundleImage != null)
			{
				this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
			}
		}

		// Token: 0x06004F62 RID: 20322 RVA: 0x0017A1B7 File Offset: 0x001783B7
		public void InitializeEventListeners()
		{
			this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
			this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
		}

		// Token: 0x06004F63 RID: 20323 RVA: 0x0017A1F1 File Offset: 0x001783F1
		public void NotifyAlreadyOwn()
		{
			this.AlreadyOwnEvent.Invoke();
		}

		// Token: 0x06004F64 RID: 20324 RVA: 0x0017A1FE File Offset: 0x001783FE
		public void ErrorHappened()
		{
			this.ErrorHappenedEvent.Invoke();
		}

		// Token: 0x06004F65 RID: 20325 RVA: 0x0017A20B File Offset: 0x0017840B
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (this._bundlePurchaseButton != null)
			{
				this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
			}
		}

		// Token: 0x06004F66 RID: 20326 RVA: 0x0017A227 File Offset: 0x00178427
		public void UpdateDescriptionText(string descriptionText)
		{
			if (this._bundleDescriptionText != null)
			{
				this._bundleDescriptionText.text = descriptionText;
			}
		}

		// Token: 0x04005276 RID: 21110
		public BundlePurchaseButton _bundlePurchaseButton;

		// Token: 0x04005277 RID: 21111
		[SerializeField]
		public StoreBundleData _bundleDataReference;

		// Token: 0x04005278 RID: 21112
		public GameObject[] EditorOnlyObjects;

		// Token: 0x04005279 RID: 21113
		public Text _bundleDescriptionText;

		// Token: 0x0400527A RID: 21114
		public Image _bundleIcon;

		// Token: 0x0400527B RID: 21115
		public UnityEvent AlreadyOwnEvent;

		// Token: 0x0400527C RID: 21116
		public UnityEvent ErrorHappenedEvent;
	}
}
