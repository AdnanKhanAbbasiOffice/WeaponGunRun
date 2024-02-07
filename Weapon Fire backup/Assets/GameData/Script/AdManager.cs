using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using Firebase;
using Falcon;
using Falcon.FalconGoogleUMP;
using Firebase.Analytics;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

public class AdManager : MonoBehaviour
{
	public string AppsFlyerDevID= "UAVHtuYSgwSPXxXQVDGA65";
	[SerializeField]string IronSourceAppID = "1d34e6435";
	[SerializeField] string AppOpenUnitId = "ca-app-pub-3940256099942544/9257395921";

	// App open ads can be preloaded for up to 4 hours.
	private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
	private DateTime _expireTime;
	private AppOpenAd _appOpenAd;


	private void Awake()
    {
		// Use the AppStateEventNotifier to listen to application open/close events.
		// This is used to launch the loaded ad when we open the app.
		AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

		FalconUMP.ShowConsentForm(onSetIronSourceConsent, onInitializeAdmob,onShowPopupATT);
		
		DontDestroyOnLoad(gameObject);
    }

	private void onSetIronSourceConsent(bool consentValue)
	{
		IronSource.Agent.setConsent(consentValue);
		IronSource.Agent.init(IronSourceAppID);

		if (FalconUMP.RequirePrivacyOptionsForm)
		{
			// Show Privacy Button in Setting Panel

		}
		else
		{
			// Hide Privacy Button in Setting Panel
		}
	}
	private void onInitializeAdmob()
    {

		
		MobileAds.Initialize(status =>
		{
            if (status != null)
            {
				// Code block to execute when the initialization status is not null.
				// This is where you can handle the initialization status.
				LoadAppOpenAd(); 
				ShowAppOpenAd();
			}
		});

	}

 
	private void onShowPopupATT()
	{
	
	}
	// Start is called before the first frame update
	void Start()
    {
		FireBaseInitialize();
		IronSourceInitialization();
		IronSourceAdQualityInitialization();
		ShowBanner();
		AppsFlyerInitialization();

	}

    #region Sdks Initializations

    #region IronSource Sdk Initialization
    void IronSourceInitialization()
	{
#if UNITY_ANDROID
	//	IronSourceAppID = "85460dcd";
#elif UNITY_IPHONE
     //IronSourceAppID = "8545d445";
#else
		 IronSourceAppID = "unexpected_platform";
#endif
		Debug.Log("unity-script: MyAppStart Start called");

		//Dynamic config example
		IronSourceConfig.Instance.setClientSideCallbacks(true);

		string id = IronSource.Agent.getAdvertiserId();
		Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);

		Debug.Log("unity-script: IronSource.Agent.validateIntegration");
		IronSource.Agent.validateIntegration();

		Debug.Log("unity-script: unity version" + IronSource.unityVersion());


		//Add AdInfo Impression Event
		IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;


		//Add AdInfo Banner Events
		IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
		IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
		IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
		IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
		IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
		IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

		//Add AdInfo Interstitial Events
		IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
		IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
		IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
		IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
		IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
		IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
		IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;


		//Add AdInfo Rewarded Video Events
		IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
		IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
		IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
		IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
		IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
		IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
		IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;




		// SDK init
		Debug.Log("unity-script: IronSource.Agent.init");
		IronSource.Agent.init(IronSourceAppID);

		IronSource.Agent.init (IronSourceAppID, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);

		

		//IronSource.Agent.initISDemandOnly (appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);

		//Set User ID For Server To Server Integration
		//// IronSource.Agent.setUserId ("UserId");

		// Load Banner example
	
	}

	#endregion

	#region IronSource Ad Quality Sdk Initialization
	public void IronSourceAdQualityInitialization()
    {
		ISAdQualityConfig adQualityConfig = new ISAdQualityConfig();

		// The default user id is Ad Quality internal id. 
		// The only allowed characters for user id are: letters, numbers, @, -, :, =,_ and /. 
		// The user id cannot be null and must be between 2 and 100 characters, otherwise it will be blocked.
		IronSourceAdQuality.ChangeUserId("Ad Quality internal id");

		// The default is false - set to true only to test your Ad Quality integration
		//adQualityConfig.TestMode = true;

		adQualityConfig.LogLevel = ISAdQualityLogLevel.INFO;
		// There are 5 different log levels:
		// ERROR, WARNING, INFO, DEBUG, VERBOSE
		// The default is INFO
		IronSourceAdQuality.Initialize(IronSourceAppID, adQualityConfig);

		// Initialize
		IronSourceAdQuality.Initialize(IronSourceAppID);
	}

	#endregion

	#region Firbase Sdk Initialization
	public void FireBaseInitialize()
	{



		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

				// Set a flag here to indicate whether Firebase is ready to use by your app.
				FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

				//	Debug.LogError(" resolve all Firebase dependencies");

			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});








		//FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
		//	var dependencyStatus = task.Result;
		//	if (dependencyStatus == DependencyStatus.Available)
		//	{
		//		//InitializeFirebaseComponents();
		//	}
		//	else
		//	{
		//		Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
		//		Application.Quit();
		//	}
		//});


		//Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventAdImpression);


	}

	#endregion

	#region AppsFlyer Sdk Initialization

	public void AppsFlyerInitialization()
    {

		AppsFlyer.OnRequestResponse += AppsFlyerOnRequestResponse;

		// In-App response example
		AppsFlyer.OnInAppResponse += (sender, args) =>
		{
			var af_args = args as AppsFlyerRequestEventArgs;
			AppsFlyer.AFLog("AppsFlyerOnRequestResponse", " status code " + af_args.statusCode);
		};

		AppsFlyer.initSDK(AppsFlyerDevID, "appID", this);

		AppsFlyer.startSDK();
		AppsFlyerAdRevenue.start();
	}

	#endregion

	#endregion

	#region  Ads Calling User Define Methods

	public void ShowBanner()
	{
		IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM, (string)"YOUR_PLACEMENT_NAME");
		//IronSource.Agent.loadBanner(new IronSourceBannerSize(320, 50), IronSourceBannerPosition.BOTTOM);

		Dictionary<string, string> additionalParams = new Dictionary<string, string>();
		additionalParams.Add(AFAdRevenueEvent.COUNTRY, "US");
		additionalParams.Add(AFAdRevenueEvent.AD_UNIT, "89b8c0159a50ebd1");
		additionalParams.Add(AFAdRevenueEvent.AD_TYPE, "Banner");
		additionalParams.Add(AFAdRevenueEvent.PLACEMENT, "place");
		additionalParams.Add(AFAdRevenueEvent.ECPM_PAYLOAD, "encrypt");

		additionalParams.Add("custom", "foo");
		additionalParams.Add("custom_2", "bar");
		additionalParams.Add("af_quantity", "1");
		AppsFlyerAdRevenue.logAdRevenue("facebook",
										AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob,
										0.026,
										"USD",
										additionalParams);
	}
	public void HideBanner()
	{
		IronSource.Agent.hideBanner();
	}
	public void DisplayBanner()
	{
		IronSource.Agent.displayBanner();
	}
	public void DestroyBanner()
	{
		IronSource.Agent.destroyBanner();
	}

	public void LoadInterstitial()
	{

		IronSource.Agent.loadInterstitial();
	}
	public void ShowInterstitial()
	{
		AppsFlyer.sendEvent("af_inters_show", null);

		if (IronSource.Agent.isInterstitialReady())
		{
			IronSource.Agent.showInterstitial();

		}
		else
		{
			LoadInterstitial();
		}
	}

	public void ShowRewarded(string reward = "")
	{
		AppsFlyer.sendEvent("af_rewarded_show", null);

		if (IronSource.Agent.isRewardedVideoAvailable())
		{

			IronSource.Agent.showRewardedVideo(reward);

		}
	}

	public void FirebaseEvents(string EventName = "", string ValueTitle = "", string Value = "")
	{

		if (EventName == "level_start")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("level" + GameManager.Instance.currentLevel);
			Firebase.Analytics.FirebaseAnalytics.LogEvent("current_gold" + GameManager.Instance.totalCash);

		}
		else if (EventName == "level_complete")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("level" + GameManager.Instance.currentLevel);
			Firebase.Analytics.FirebaseAnalytics.LogEvent("timeplayed" + "write here total play time of current level");


		}
		else if (EventName == "level_fail")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("level" + GameManager.Instance.currentLevel);
			Firebase.Analytics.FirebaseAnalytics.LogEvent("failcount" + "write here fail counter of current level");



		}
		else if (EventName == "earn_virtual_currency")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("virtual_currency_name" + "Cash");
			Firebase.Analytics.FirebaseAnalytics.LogEvent("value" + (long)GameManager.Instance.totalCash); // total game cash/coins
			Firebase.Analytics.FirebaseAnalytics.LogEvent("source" + ValueTitle);  // Value title is , where user get cash (for example: levelpass, x5bywatchad, gamecomplete etc...)



		}
		else if (EventName == "spend_virtual_currency")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("virtual_currency_name" + "Cash");
			Firebase.Analytics.FirebaseAnalytics.LogEvent("value" + Value); // Value is a cash/currency of item
			Firebase.Analytics.FirebaseAnalytics.LogEvent("item_name" + ValueTitle);  // ValueTitle is a name of the item on which cash/currency spent





		}
		else if (EventName == "ads_reward_load")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_load");

		}
		else if (EventName == "ads_reward_click")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_click");

		}
		else if (EventName == "ads_reward_show_success")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_show_success");

		}
		else if (EventName == "ads_reward_show_fail")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_show_fail");

		}
		else if (EventName == "ads_reward_complete")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_complete");

		}
		else if (EventName == "ad_inter_load_fail")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_load_fail");

		}
		else if (EventName == "ad_inter_load_success")
		{

			Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_load_success");

		}
		else if (EventName == "ad_inter_show")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_show");

		}
		else if (EventName == "ad_inter_click")
		{
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_click");

		}


	}

	public void ShowConsent()
	{

		if (FalconUMP.RequirePrivacyOptionsForm)
		{
			// Show Privacy Button in Setting Panel
			FalconUMP.ShowPrivacyOptionsForm();
		}
		else
		{
			// Hide Privacy Button in Setting Panel
		}

		ActivateConsentButton();
	}

	public void ActivateConsentButton()
	{

		if (FalconUMP.RequirePrivacyOptionsForm)
		{
			// Show Privacy Button in Setting Panel
			GameManager.Instance.uiManager.gamePlay.ConsentButton.SetActive(true);
		}
		else
		{
			// Hide Privacy Button in Setting Panel
			GameManager.Instance.uiManager.gamePlay.ConsentButton.SetActive(false);

		}
	}

	#region  AppOpen Methods
	private void OnDestroy()
	{
		// Always unlisten to events when complete.
		AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
	}
	/// <summary>
	/// Loads the ad.
	/// </summary>
	public void LoadAppOpenAd()
	{
		// Clean up the old ad before loading a new one.
		if (_appOpenAd != null)
		{
			DestroyAppOpenAd();
		}

		Debug.Log("Loading app open ad.");

		// Create our request used to load the ad.
		var adRequest = new AdRequest();

		// Send the request to load the ad.
		AppOpenAd.Load(AppOpenUnitId, adRequest, (AppOpenAd ad, LoadAdError error) =>
		{
			// If the operation failed with a reason.
			if (error != null)
			{
				Debug.LogError("App open ad failed to load an ad with error : "
								+ error);
				return;
			}

			// If the operation failed for unknown reasons.
			// This is an unexpected error, please report this bug if it happens.
			if (ad == null)
			{
				Debug.LogError("Unexpected error: App open ad load event fired with " +
							   " null ad and null error.");
				return;
			}

			// The operation completed successfully.
			Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
			_appOpenAd = ad;

			// App open ads can be preloaded for up to 4 hours.
			_expireTime = DateTime.Now + TIMEOUT;

			// Register to ad events to extend functionality.
			RegisterEventHandlers(ad);

			// Inform the UI that the ad is ready.
			//AdLoadedStatus?.SetActive(true);
		});
	}

	/// <summary>
	/// Shows the ad.
	/// </summary>
	public void ShowAppOpenAd()
	{
		// App open ads can be preloaded for up to 4 hours.
		if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime)
		{
			Debug.Log("Showing app open ad.");
			_appOpenAd.Show();
		}
		else
		{
			Debug.LogError("App open ad is not ready yet.");
		}

		// Inform the UI that the ad is not ready.
		//AdLoadedStatus?.SetActive(false);
	}

	/// <summary>
	/// Destroys the ad.
	/// </summary>
	public void DestroyAppOpenAd()
	{
		if (_appOpenAd != null)
		{
			Debug.Log("Destroying app open ad.");
			_appOpenAd.Destroy();
			_appOpenAd = null;
		}

		// Inform the UI that the ad is not ready.
		//AdLoadedStatus?.SetActive(false);
	}

	/// <summary>
	/// Logs the ResponseInfo.
	/// </summary>
	public void LogResponseInfo()
	{
		if (_appOpenAd != null)
		{
			var responseInfo = _appOpenAd.GetResponseInfo();
			UnityEngine.Debug.Log(responseInfo);
		}
	}

 
    private void OnAppStateChanged(AppState state)
	{
		Debug.Log("App State changed to : " + state);

		// If the app is Foregrounded and the ad is available, show it.
		if (state == AppState.Foreground)
		{
			ShowAppOpenAd();
		}
	}

	#endregion





	#endregion





	#region  All Ironsource Events

	#region Banner Events

	/************* Banner AdInfo Delegates *************/
	//Invoked once the banner has loaded
	void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
	{
	}
	//Invoked when the banner loading process has failed.
	void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
	{
	}
	// Invoked when end user clicks on the banner ad
	void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
	{
	}
	//Notifies the presentation of a full screen content following user click
	void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
	{
	}
	//Notifies the presented screen has been dismissed
	void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
	{
	}
	//Invoked when the user leaves the app
	void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
	{
	}

	#endregion
	
	#region Interstitial Events
	/************* Interstitial AdInfo Delegates *************/
	// Invoked when the interstitial ad was loaded succesfully.
	void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
	{
		Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_load_success");

	}
	// Invoked when the initialization process has failed.
	void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
	{
		Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_load_fail");

		LoadInterstitial();
	}
	// Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
	void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
	{
		Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_show");
		AppsFlyer.sendEvent("af_inters_displayed", null);

		LoadInterstitial();
	}
	// Invoked when end user clicked on the interstitial ad
	void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
	{
		Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_inter_click");

	}
	// Invoked when the ad failed to show.
	void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
	{
	}
	// Invoked when the interstitial ad closed and the user went back to the application screen.
	void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
	{
		LoadInterstitial();
	}
	// Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
	// This callback is not supported by all networks, and we recommend using it only if  
	// it's supported by all networks you included in your build. 
	void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
	{
	}

	#endregion

	#region Rewarded Events


	/************* RewardedVideo AdInfo Delegates *************/
	// Indicates that there’s an available ad.
	// The adInfo object includes information about the ad that was loaded successfully
	// This replaces the RewardedVideoAvailabilityChangedEvent(true) event
void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
{
	Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_load");

}
// Indicates that no ads are available to be displayed
// This replaces the RewardedVideoAvailabilityChangedEvent(false) event
void RewardedVideoOnAdUnavailable()
{
}
// The Rewarded Video ad view has opened. Your activity will loose focus.
void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
{
	Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_show_success");

}
// The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
{
}
// The user completed to watch the video, and should be rewarded.
// The placement parameter will include the reward data.
// When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
{
	string reward = placement.getPlacementName();
	// now call user define method with reward parameter to give reward to user
	 GameManager.Instance.RewardPlayer(reward);


	AppsFlyer.sendEvent("af_rewarded_displayed", null);



	Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_complete");

}
// The rewarded video ad was failed to show.
void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
{
	Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_show_fail");

}
// Invoked when the video ad was clicked.
// This callback is not supported by all networks, and we recommend using it only if
// it’s supported by all networks you included in your build.
void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
{
	Firebase.Analytics.FirebaseAnalytics.LogEvent("ads_reward_click");

}


	#endregion

	#region  Measure ad revenue Event

	private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
	{

		Debug.Log("unity-script:  ImpressionDataReadyEvent impressionData = " + impressionData);
		if (impressionData != null)
		{
			Firebase.Analytics.Parameter[] AdParameters = {
 			new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
  			new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
  			new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
			new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
  			new Firebase.Analytics.Parameter("currency","USD"),
			new Firebase.Analytics.Parameter("value", impressionData.revenue.Value)
		};
			Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);

			var dic = new Dictionary<string, string>
			{
				{"ad_unit_name", impressionData.instanceName },
				{ "ad_format",impressionData.adUnit}

			};
			AppsFlyerAdRevenue.logAdRevenue(impressionData.adNetwork, AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource, impressionData.revenue.Value, "USD", dic);

		}
	}

	#endregion

	#region AppsFlyer Events

	// Sessions response
	void AppsFlyerOnRequestResponse(object sender, EventArgs e)
	{
		var args = e as AppsFlyerRequestEventArgs;
		AppsFlyer.AFLog("AppsFlyerOnRequestResponse", " status code " + args.statusCode);
	}

	#endregion

	#region App Open Ad Events

	private void RegisterEventHandlers(AppOpenAd ad)
	{
		// Raised when the ad is estimated to have earned money.
		ad.OnAdPaid += (AdValue adValue) =>
		{
			Debug.Log(String.Format("App open ad paid {0} {1}.",
				adValue.Value,
				adValue.CurrencyCode));
		};
		// Raised when an impression is recorded for an ad.
		ad.OnAdImpressionRecorded += () =>
		{
			Debug.Log("App open ad recorded an impression.");
		};
		// Raised when a click is recorded for an ad.
		ad.OnAdClicked += () =>
		{
			Debug.Log("App open ad was clicked.");
		};
		// Raised when an ad opened full screen content.
		ad.OnAdFullScreenContentOpened += () =>
		{
			Debug.Log("App open ad full screen content opened.");

			// Inform the UI that the ad is consumed and not ready.
			//AdLoadedStatus?.SetActive(false);
		};
		// Raised when the ad closed full screen content.
		ad.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("App open ad full screen content closed.");

			// It may be useful to load a new ad when the current one is complete.
			LoadAppOpenAd();
		};
		// Raised when the ad failed to open full screen content.
		ad.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("App open ad failed to open full screen content with error : "
							+ error);
		};
	}



    #endregion

 


    #endregion




}
