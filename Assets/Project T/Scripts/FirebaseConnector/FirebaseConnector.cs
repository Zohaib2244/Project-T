using Firebase.Firestore;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Scripts.Resources;
using System;


namespace Scripts.FirebaseConfig
{
    public class FirebaseConnector : MonoBehaviour
    {
        #region Singleton
        private static FirebaseConnector _instance;
        public static FirebaseConnector Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("FirebaseConnector instance is not initialized.");
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Firebase Variables
        protected FirebaseFirestore db;
        private FirebaseAuth auth;
        private string userId; // Class-level variable to store the UserId

        //colelction Variables
        private CollectionReference instituitionsRef;
        private CollectionReference teamRef;



        //Getters
        public FirebaseFirestore Db => db;
        public FirebaseAuth Auth => auth;
        public string UserId => userId;
        public void SetUserId(string userId)
        {
            this.userId = userId;
        }

        // State variables
        public bool isFirebaseReady = false;
        public bool isLoggedIn = false;
        public UnityEvent OnFirebaseReady;
          #endregion
      
        #region Essentials
        void Start()
        {
            InitializeFirebaseAsync();
        }
        public async void InitializeFirebaseAsync(UnityAction onSuccess = null)
        {
            var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                isFirebaseReady = true;
                db = FirebaseFirestore.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                OnFirebaseReady.Invoke();
                Debug.Log("<color=green>Firebase initialized and Firestore connected.</color>");
                onSuccess?.Invoke();
            }
            else
            {
                Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        }


        // public void ConfigureFirestoreReferences()
        // {
        //     if (!string.IsNullOrEmpty(AppConstants.instance.selectedTouranment.tournamentId))
        //     {
        //         instituitionsRef = db.Collection("tournaments").Document(AppConstants.instance.selectedTouranment.tournamentId).Collection("instituitions");
        //         teamRef = db.Collection("tournaments").Document(AppConstants.instance.selectedTouranment.tournamentId).Collection("teams");

        //     }
        //     else
        //     {
        //         Debug.LogError("Selected Tournament ID is null or empty.");
        //     }
        // }
        #endregion
       
        #region Auth Functions
        public async void SignInUser(string email, string password, UnityAction onSuccess, UnityAction<string> onFailure)
        {
            Debug.Log("<color=yellow>Signing in user...</color>");
            try
            {
                var signInTask = await auth.SignInWithEmailAndPasswordAsync(email, password);
                FirebaseUser user = signInTask.User; // Get the user from the task
                userId = user.UserId; // Store the UserId
                Debug.LogFormat("<color=green>User signed in successfully: {0} ({1})</color>", user.DisplayName, user.UserId);
                isLoggedIn = true;
                onSuccess?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + ex);
                onFailure?.Invoke("Login failed. Please try again.");
            }
        }
        public string GetUserId()
        {
            return auth.CurrentUser.UserId;
        }
        #endregion
    }
}