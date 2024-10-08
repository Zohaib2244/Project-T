using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Scripts.UI;
using Scripts.Resources;
using Scripts.FirebaseConfig;
using DG.Tweening;

namespace Scripts.UIPanels{
public class UserLogin : MonoBehaviour
    {
        [SerializeField] private TMP_Text loginStatusText;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private Button signInButton;
        
        public void TestSignIn()
        {
            FirebaseConnector.Instance.SignInUser("zohaibangry123@gmail.com", "aspiree1-571", OnSignInSuccess, OnSignInFailed);
        }

        public void SignIn()
        {
            //make a click animation
            signInButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 1).OnComplete(() => {
                signInButton.transform.localScale = new Vector3(1, 1, 1);
            
            Loading.Instance.ShowLoadingScreen();
            signInButton.interactable = false;
            if (string.IsNullOrEmpty(emailInputField.text) || string.IsNullOrEmpty(passwordInputField.text))
            {
                loginStatusText.text = "Please enter email and password!";
                signInButton.interactable = true;
                Loading.Instance.HideLoadingScreen();
                return;
            }
            FirebaseConnector.Instance.SignInUser(emailInputField.text, passwordInputField.text, OnSignInSuccess, OnSignInFailed);
            });
         }
        private void OnSignInSuccess()
        {
                            Loading.Instance.HideLoadingScreen();

            loginStatusText.text = "Login Successful!";
            FirestoreManager.FireInstance.GetAdminFromFirestore(FirebaseConnector.Instance.GetUserId(), onAdminInfoRetrievalSuccess, onAdminInfoRetrievalFailed);
        //MainUIManager.Instance.SwitchPanel(Panels.TournamentSelectionPanel);
        }
        private void OnSignInFailed(string errorMessage)
        {
                            Loading.Instance.HideLoadingScreen();

            signInButton.interactable = true;
            loginStatusText.text = "Login Failed! Error: " + errorMessage;
        }

        private void onAdminInfoRetrievalSuccess(Admin admin)
        {
                            Loading.Instance.HideLoadingScreen();

           // AppConstants.instance.DebugAdminInfo();
            AppConstants.instance.selectedAdmin = admin;
            MainUIManager.Instance.SwitchPanel(Panels.TournamentSelectionPanel);
        }
        private void onAdminInfoRetrievalFailed()
        {
                            Loading.Instance.HideLoadingScreen();

            loginStatusText.text = "Login Failed! Try Again!";
            signInButton.interactable = true;
        }
        private void OnDisable() {
            loginStatusText.text = "";
            emailInputField.text = "";
            passwordInputField.text = "";
            if(signInButton!=null) signInButton.interactable = true;
                            Loading.Instance.HideLoadingScreen();

        }
    }
}