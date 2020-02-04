using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;
#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

public class MyIAPManager : MonoBehaviour, IStoreListener {
	private IStoreController m_Controller;
	//private IAppleExtensions m_AppleExtensions;
	#pragma warning disable 0414
	private bool m_IsGooglePlayStoreSelected;
	#pragma warning restore 0414
	private string m_LastTransationID;
	private string m_LastReceipt;
	private bool m_IsLoggedIn = false;
	private bool m_PurchaseInProgress;
	private Selectable m_InteractableSelectable;
	#if UNITY_IPHONE
	string []mIDs = new string[]{"get100000","removeadsfire"};
	#else
	string []mIDs = new string[]{"get100000","removeads"};
	#endif

	#if RECEIPT_VALIDATION
	private CrossPlatformValidator validator;
	#endif
	public void Awake()
	{
		//var module = StandardPurchasingModule.Instance();
		//module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
		//var builder = ConfigurationBuilder.Instance(module);
		//builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAp3BwOFmsmkiGSEj5dF8jxo4zyMsKXWh6Q9Fd6BHfLcCpzI9va9mpC8dYUTmizbSB2651xyAluArc46HHYg9f0fh1hiR9oSmHWGaf8phIzen7pJQaPYgd6BJH+U2gqabz+zwpWAPlx5gSX1saLFnhTTE9P+xgjfaJbFK4nD7sQ9xn7vJ8I1xz8UB1/cMitcLPiLhZYhbeObY4er3JWLD28yJ81HwhaMKHkn/FNDZzBAEwgElz66+L5fzVN8xbsWH2S2xVi9WCRBdBo1pLr9XpfE1Rws+Vr3SYwfjiu1qYBs2cIU8Wqi5LOEs6XdHanMXUY7pVqllF0Fmxul2t3RYkCwIDAQAB");
		//m_IsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android && module.androidStore == AndroidStore.GooglePlay;




		//builder.AddProduct (mIDs [0], ProductType.Consumable, new IDs {
		//	{ mIDs [0], GooglePlay.Name },
		//	{ mIDs [0], MacAppStore.Name },

		//});

		//builder.AddProduct (mIDs [1], ProductType.NonConsumable, new IDs {
		//	{ mIDs [1], GooglePlay.Name },
		//	{ mIDs [1], MacAppStore.Name },
		//});




		//#if RECEIPT_VALIDATION
		//string appIdentifier;
		//#if UNITY_5_6_OR_NEWER
		//appIdentifier = Application.identifier;
		//#else
		//appIdentifier = Application.bundleIdentifier;
		//#endif
		//validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), appIdentifier);
		//#endif
		//UnityPurchasing.Initialize(this, builder);


		//#if UNITY_IPHONE
		//transform.Find ("IAP/RESTORE").gameObject.SetActive (true);
		//#else
		//transform.Find ("IAP/RESTORE").gameObject.SetActive (false);
		//#endif

	}
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		//Debug.Log ("~~OnInitialized");
		//m_Controller = controller;

		//m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();
		//m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
		//Debug.Log("Available items:");
		//int i = 0;
		//foreach (var item in controller.products.all)
		//{
		//	if (item.availableToPurchase)
		//	{
		//		Debug.Log(string.Join(" - ",
		//			new[]
		//			{
		//				item.metadata.localizedTitle,
		//				item.metadata.localizedDescription,
		//				item.metadata.isoCurrencyCode,
		//				item.metadata.localizedPrice.ToString(),
		//				item.metadata.localizedPriceString,
		//				item.transactionID,
		//				item.receipt
		//			}));
		//		if(i==0)
		//			transform.Find ("IAP/GET10000/Text").GetComponent<Text> ().text = item.metadata.localizedDescription + "\n"+item.metadata.localizedPriceString;
		//		if(i==1)
		//			transform.Find ("IAP/ADSFREE/Text").GetComponent<Text> ().text = item.metadata.localizedDescription + "\n"+item.metadata.localizedPriceString;
		//	}
		//	i++;
		//}
		//LogProductDefinitions();
	}

	/// <summary>
	/// This will be called when a purchase completes.
	/// </summary>
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		
		Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id +" ! "+M.COINS);
		Debug.Log("Receipt: " + e.purchasedProduct.receipt);

		if (e.purchasedProduct.definition.id.Contains (mIDs[0])||
			e.purchasedProduct.definition.id.Contains (mIDs[0])) {
			M.COINS += 100000;
			transform.Find ("IAP/Massge/Panel/Text").GetComponent<Text> ().text = "<color=#feda49>Congratulation</color>\n\n You get 100000 coins.";
		}
		if (e.purchasedProduct.definition.id.Contains (mIDs[1])||
			e.purchasedProduct.definition.id.Contains (mIDs[1])) {
			M.isAds = false;
			transform.Find ("IAP/Massge/Panel/Text").GetComponent<Text> ().text = "<color=#feda49>Congratulation</color>\n\n Now game is ads free";
			transform.Find ("IAP/ADSFREE").gameObject.SetActive (M.isAds);
		}
		transform.Find ("IAP/Coin/Text").GetComponent<Text> ().text = M.COINS+"";
		transform.Find ("IAP/Massge").GetComponent<Animator> ().SetBool ("isOpen", true);
		M.Save ();
		Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id +" ~update~ "+M.COINS);

		m_LastTransationID = e.purchasedProduct.transactionID;
		m_LastReceipt = e.purchasedProduct.receipt;
		m_PurchaseInProgress = false;

		#if RECEIPT_VALIDATION
		// Local validation is available for GooglePlay and Apple stores
		if (m_IsGooglePlayStoreSelected ||
		Application.platform == RuntimePlatform.IPhonePlayer ||
		Application.platform == RuntimePlatform.OSXPlayer ||
		Application.platform == RuntimePlatform.tvOS) {
		try {
		var result = validator.Validate(e.purchasedProduct.receipt);
		Debug.Log("Receipt is valid. Contents:");
		foreach (IPurchaseReceipt productReceipt in result) {
		Debug.Log(productReceipt.productID);
		Debug.Log(productReceipt.purchaseDate);
		Debug.Log(productReceipt.transactionID);

		GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
		if (null != google) {
		Debug.Log(google.purchaseState);
		Debug.Log(google.purchaseToken);
		}

		AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
		if (null != apple) {
		Debug.Log(apple.originalTransactionIdentifier);
		Debug.Log(apple.subscriptionExpirationDate);
		Debug.Log(apple.cancellationDate);
		Debug.Log(apple.quantity);
		}
		}
		} catch (IAPSecurityException) {
		Debug.Log("Invalid receipt, not unlocking content");
		return PurchaseProcessingResult.Complete;
		}
		}
		#endif


		#if DELAY_CONFIRMATION
		StartCoroutine(ConfirmPendingPurchaseAfterDelay(e.purchasedProduct));
		return PurchaseProcessingResult.Pending;
		#else
//		UpdateHistoryUI();
		return PurchaseProcessingResult.Complete;
		#endif
	}

	#if DELAY_CONFIRMATION
	private HashSet<string> m_PendingProducts = new HashSet<string>();

	private IEnumerator ConfirmPendingPurchaseAfterDelay(Product p)
	{
	m_PendingProducts.Add(p.definition.id);
	Debug.Log("Delaying confirmation of " + p.definition.id + " for 5 seconds.");
	UpdateHistoryUI();

	yield return new WaitForSeconds(5f);

	Debug.Log("Confirming purchase of " + p.definition.id);
	m_Controller.ConfirmPendingPurchase(p);
	m_PendingProducts.Remove(p.definition.id);
	UpdateHistoryUI();
	}
	#endif

	/// <summary>
	/// This will be called is an attempted purchase fails.
	/// </summary>
	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		Debug.Log("Purchase failed: " + item.definition.id);
		Debug.Log(r);

		m_PurchaseInProgress = false;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("Billing failed to initialize!");
		switch (error)
		{
		case InitializationFailureReason.AppNotKnown:
			Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
			break;
		case InitializationFailureReason.PurchasingUnavailable:
			// Ask the user if billing is disabled in device settings.
			Debug.Log("Billing disabled!");
			break;
		case InitializationFailureReason.NoProductsAvailable:
			// Developer configuration error; check product metadata.
			Debug.Log("No products available for purchase!");
			break;
		}
	}


	/// <summary>
	/// This will be called after a call to IAppleExtensions.RestoreTransactions().
	/// </summary>
	private void OnTransactionsRestored(bool success)
	{
		if (success) {
			Debug.Log ("Transactions restored.01");
			transform.Find ("IAP/Massge/Panel/Text").GetComponent<Text> ().text = "Your purchase has been restored.";
		} else {
			transform.Find ("IAP/Massge/Panel/Text").GetComponent<Text> ().text = "There are no item available for restore at this time";
		}
		transform.Find ("IAP/Massge").GetComponent<Animator> ().SetBool ("isOpen", true);
	}

	private void OnDeferred(Product item)
	{
		Debug.Log("Purchase deferred: " + item.definition.id);

//		if (item.hasReceipt) {
//			if (item.definition.id.Contains (android_id[1])||
//				item.definition.id.Contains (iphone_id[1])) {
//				M.isAds = false;
//			}
//			M.Save ();
//		}
	}


	//public void LoginResult (LoginResultState state, string errorMsg)
	//{
	//	if(state == LoginResultState.LoginSucceed)
	//	{
	//		m_IsLoggedIn = true;
	//	}
	//	else
	//	{
	//		m_IsLoggedIn = false;
	//	}	
	//	Debug.Log ("LoginResult: state: " + state.ToString () + " errorMsg: " + errorMsg);
	//}

	public void RegisterSucceeded(string cmUserName)
	{
		Debug.Log ("RegisterSucceeded: cmUserName = " + cmUserName);

	}

	//public void RegisterFailed (FastRegisterError error, string errorMessage)
	//{
	//	Debug.Log ("RegisterFailed: error = " + error.ToString() + ", errorMessage = " + errorMessage);
	//}







	private void LogProductDefinitions()
	{
		var products = m_Controller.products.all;
		foreach (var product in products) {
			#if UNITY_5_6_OR_NEWER
			Debug.Log(string.Format("id: {0}\nstore-specific id: {1}\ntype: {2}\nenabled: {3}\n", product.definition.id, product.definition.storeSpecificId, product.definition.type.ToString(), product.definition.enabled ? "enabled" : "disabled"));
			#else
			Debug.Log(string.Format("id: {0}\nstore-specific id: {1}\ntype: {2}\n", product.definition.id, product.definition.storeSpecificId, product.definition.type.ToString()));
			#endif
		}
	}

	public void onBuy(int m_SelectedItemIndex){
		if (m_PurchaseInProgress == true) {
			Debug.Log("Please wait, purchasing ...");
			return;
		}
		m_PurchaseInProgress = true;
		Debug.Log (m_SelectedItemIndex + " A:> "+m_Controller);
		Debug.Log (m_SelectedItemIndex + " B:> "+m_Controller.products);
		Debug.Log (m_SelectedItemIndex + " C:> "+m_Controller.products.all);
		Debug.Log (m_SelectedItemIndex + " D:> "+m_Controller.products.all[m_SelectedItemIndex]);
		m_Controller.InitiatePurchase(m_Controller.products.all[m_SelectedItemIndex], "INDIAN FIREBRIGADE"); 
	}
	public void Onrestore(){
		//m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
	}
}
