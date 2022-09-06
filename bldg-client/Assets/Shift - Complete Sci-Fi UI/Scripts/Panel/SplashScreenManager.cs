using UnityEngine;
using UnityEngine.Events;

namespace Michsky.UI.Shift
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("Resources")]
        public GameObject splashScreen;
        public GameObject mainPanels;

        private Animator splashScreenAnimator;
        private Animator mainPanelsAnimator;
        private TimedEvent ssTimedEvent;

        [Header("Settings")]
        public bool disableSplashScreen;
        public bool enablePressAnyKeyScreen;
        public bool enableLoginScreen;

        MainPanelManager mpm;
        LoginController loginController;


        public InitApp getAppController() {
            GameObject[] found = GameObject.FindGameObjectsWithTag("AppController");
            if (found.Length == 0) return null;
            return found[0].GetComponent<InitApp>();
        }

        void OnEnable()
        {
            if (splashScreenAnimator == null) { splashScreenAnimator = splashScreen.GetComponent<Animator>(); }
            if (ssTimedEvent == null) { ssTimedEvent = splashScreen.GetComponent<TimedEvent>(); }
            if (mainPanelsAnimator == null) { mainPanelsAnimator = mainPanels.GetComponent<Animator>(); }
            if (mpm == null) { mpm = gameObject.GetComponent<MainPanelManager>(); }
            if (loginController == null) { loginController = gameObject.GetComponent<LoginController>(); }
            loginController.setAnimators(splashScreenAnimator);
            // InitApp appCtrl = getAppController();
            // if (appCtrl != null) {
            //     appCtrl.setAnimators(splashScreenAnimator, ssTimedEvent);
            // }

            if (disableSplashScreen == true)
            {
                splashScreen.SetActive(false);
                mainPanels.SetActive(true);

                mainPanelsAnimator.Play("Start");
                mpm.OpenFirstTab();
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                // check whether logged in already
                CurrentResidentController crc = CurrentResidentController.Instance;
                if (!crc.isInitialized()) {
                    // ROLE 3   ////////////////////////////////////////
                    Debug.Log("[SPLASH SCREEN MANAGER] CRC not initialized - show login screen");
                    splashScreenAnimator.Play("Login");
                    ////////////////////////////////////////////////////
                }
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }
        }

        public void LoginScreenCheck()
        {
            if (enableLoginScreen == true && enablePressAnyKeyScreen == true)
                splashScreenAnimator.Play("Press Any Key to Login");

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == true)
            {
                splashScreenAnimator.Play("Press Any Key to Loading");
                ssTimedEvent.StartIEnumerator();
            }

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == false)
            {
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }
        }
    }
}