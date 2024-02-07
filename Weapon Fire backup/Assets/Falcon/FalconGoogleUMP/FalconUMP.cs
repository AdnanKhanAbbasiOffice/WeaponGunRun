using System;
using System.Collections;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace Falcon.FalconGoogleUMP
{
    public class FalconUMP
    {
        private const string TCF_TL_CONSENT = "IABTCF_AddtlConsent";
        private const string IS_CONSENT_VALUE = "2878";
        private const string RESET_CMP = "f_reset_cmp";

        internal static void ShowConsentForm(Action onSetIronSourceConsent, Action onInitializeAdmob, Action action, object onShowPopupATT)
        {
            throw new NotImplementedException();
        }



        private enum ConfigResult
        {
            None = 0, //Không show consent form
            MediationWithoutCmp = 1,
            MediationWithCmp = 2
        }

        private static int _configResult = 0;

        private static Action<bool> _onSetIronSourceConsent;
        private static Action _onInitializeAdmob;
        private static Action _onShowPopupATT;
        private static bool _onRemoteConfig;

        public static void ShowConsentForm(
            Action<bool> onSetIronSourceConsent, Action onInitializeAdmob, Action onShowPopupATT)
        {
            _onSetIronSourceConsent = onSetIronSourceConsent;
            _onInitializeAdmob = onInitializeAdmob;
            _onShowPopupATT = onShowPopupATT;

            FalconConfig.OnUpdateFromNet += (_, _) => { _onRemoteConfig = true; };

            Debug.Log("Start CoWaitRemoteConfig");
            FalconMain.Instance.StartCoroutine(CoWaitRemoteConfig());
        }

        static IEnumerator CoWaitRemoteConfig()
        {
            float waitTime = 5f;
            while (!_onRemoteConfig && waitTime > 0)
            {
                waitTime -= Time.unscaledDeltaTime;
                yield return null;
            }

            _configResult = FalconConfig.Instance<FalconUMPConfig>().f_ump_consent;

            var resetConfig = FalconConfig.Instance<FalconUMPConfig>().f_reset_cmp;
            if (resetConfig && !PlayerPrefs.HasKey(RESET_CMP))
            {
                Debug.Log("FalconUMP > Reset CMP");

                PlayerPrefs.SetInt(RESET_CMP, 1);
                ConsentInformation.Reset();
            }

            AfterGetRemoteConfig();
        }

        private static void AfterGetRemoteConfig()
        {
            Debug.Log("FalconUMP > AfterGetRemoteConfig: " + _configResult);

            if (_configResult == (int)ConfigResult.None)
            {
                _onSetIronSourceConsent?.Invoke(true);
                _onInitializeAdmob?.Invoke();
                _onShowPopupATT?.Invoke();
                return;
            }

            if (_configResult == (int)ConfigResult.MediationWithoutCmp)
            {
                _onSetIronSourceConsent?.Invoke(true);
            }

            //Config = 2: làm đúng luật.
            //Show popup CMP, sau khi nhận giá trị consent trả về thì khởi tạo GMA
            //Set consent cho IronSource tùy thuộc vào giá trị lấy được từ CMP -> Khởi tạo IronSource

            // Set tag for under age of consent.
            // Here false means users are not under age of consent.
            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
            };

            // Check the current consent information status.
            Debug.Log("FalconUMP > AfterGetRemoteConfig === ConsentStatus: " + ConsentInformation.ConsentStatus);
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }

        private static void OnConsentInfoUpdated(FormError consentError)
        {
            Debug.Log("FalconUMP > OnConsentInfoUpdated === " + _configResult);

            if (consentError != null)
            {
                // Handle the error.
                Debug.LogError("FalconUMP > OnConsentInfoUpdated === Error Code: " + consentError.ErrorCode + ", Message: " + consentError.Message);

                if (_configResult == (int)ConfigResult.MediationWithCmp)
                {
                    CallIronSourceSetConsentEvent();
                }
                _onShowPopupATT?.Invoke();
                return;
            }

            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            Debug.Log("FalconUMP > OnConsentInfoUpdated === ConsentStatus: " + ConsentInformation.ConsentStatus);
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                Debug.Log("FalconUMP > LoadAndShowConsentFormIfRequired");

                if (_configResult == (int)ConfigResult.MediationWithCmp)
                {
                    CallIronSourceSetConsentEvent();
                }
                _onShowPopupATT?.Invoke();

                if (formError != null)
                {
                    // Consent gathering failed.
                    Debug.LogError("FalconUMP > LoadAndShowConsentFormIfRequired ===  Error Code: " + formError.ErrorCode + ", Message: " + formError.Message);
                    return;
                }

                // Consent has been gathered.
                if (ConsentInformation.CanRequestAds())
                {
                    Debug.Log("FalconUMP > LoadAndShowConsentFormIfRequired === CanRequestAds");
                    _onInitializeAdmob?.Invoke();
                }
            });
        }

        /// <summary>
        /// Check if it's necessary to show the Privacy Options Form
        /// </summary>
        public static bool RequirePrivacyOptionsForm => ConsentInformation.PrivacyOptionsRequirementStatus ==
                                                        PrivacyOptionsRequirementStatus.Required;

        /// <summary>
        /// Show Privacy Options form if required
        /// </summary>
        public static void ShowPrivacyOptionsForm()
        {
            Debug.Log("FalconUMP > ShowPrivacyOptionsForm");

            ConsentForm.ShowPrivacyOptionsForm((FormError formError) =>
            {
                if (formError != null)
                {
                    Debug.LogError("FalconUMP > ShowPrivacyOptionsForm === Error: " + formError);
                }
            });
        }

        private static void CallIronSourceSetConsentEvent()
        {
            string tcf = PlayerPrefs.GetString(TCF_TL_CONSENT, string.Empty);
            var consent = tcf.Contains(IS_CONSENT_VALUE);

            Debug.Log("FalconUMP > CallIronSourceSetConsentEvent: " + consent);
            _onSetIronSourceConsent?.Invoke(consent);
        }
    }
}