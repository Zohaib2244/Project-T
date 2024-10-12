using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Scripts.UIPanels;
using Scripts.FirebaseConfig;
using System.Threading.Tasks;
public class Rounds_BallotsPanel : MonoBehaviour
{
    #region Singleton
    public static Rounds_BallotsPanel Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    [SerializeField] private Transform ballotsPanel;
    [SerializeField] private Transform ballotsContent;
    [SerializeField] private GameObject ballotEntryPrefab;
    [SerializeField] private Transform ballotInfoPanel;
    [SerializeField] private Transform overlayPanel;

    void OnEnable()
    {
        ballotsPanel.localScale = Vector3.zero; // Start from zero scale
        ballotsPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            UpdateBallotsList();
        }); // Animate to full scale with a pop effect
    }
    void OnDisable()
    {
        // Clear existing entries
        foreach (Transform child in ballotsContent)
        {
            Destroy(child.gameObject);
        }
    }


    private void UpdateBallotsList()
    {
        // Clear existing entries
        foreach (Transform child in ballotsContent)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("MainRoundsPanel.Instance.selectedRound.matches.Count: " + MainRoundsPanel.Instance.selectedRound.matches.Count);
        // Instantiate new entries and set match data
        for (int i = 0; i < MainRoundsPanel.Instance.selectedRound.matches.Count; i++)
        {
            Debug.Log("MainRoundsPanel.Instance.selectedRound.matches[i]: " + MainRoundsPanel.Instance.selectedRound.matches[i]);
            var match = MainRoundsPanel.Instance.selectedRound.matches[i];
            var drawEntry = Instantiate(ballotEntryPrefab, ballotsContent);
            drawEntry.GetComponent<BallotListEntry>().SetMatch(match, i + 1); // Pass match object and match number (index + 1)
        }
    }
    public void SaveBallots()
    {
       List<Match> allMatches = GetBallotPrefabs();
        
            // Get the selected round
            var selectedRound = MainRoundsPanel.Instance.selectedRound;
        
            if (selectedRound == null)
            {
                Debug.LogWarning("No selected round found.");
                return;
            }

            selectedRound.matches.Clear();
            // Save the draw prefabs to the selected round
            selectedRound.matches = allMatches;
    }

    public void ShowBallotInfo(Match match)
    {
        ballotInfoPanel.gameObject.SetActive(true);
            // Display the match ballot
        ballotInfoPanel.GetComponent<Ballot_InfoPanel>().DisplayMatchBallot(match);
    }
    
    public async void CloseBallotInfo()
    {
    
        // Deactivate the ballotInfoPanel
        ballotInfoPanel.gameObject.SetActive(false);
    
        // Update the ballots list
        // UpdateBallotsList();
    
        // Optionally, load matches from Firestore
        Loading.Instance.ShowLoadingScreen();
        await FirestoreManager.FireInstance.GetAllMatchesFromFirestore(MainRoundsPanel.Instance.selectedRound.roundCategory.ToString(), MainRoundsPanel.Instance.selectedRound.roundId, GetAllMatchesSuccess, GetAllMatchesFail);
    }
       private void GetAllMatchesSuccess(List<Match> matches)
    {
        MainRoundsPanel.Instance.selectedRound.matches = matches;
        UpdateBallotsList();
        Loading.Instance.HideLoadingScreen();
    }
    private void GetAllMatchesFail()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Failed To Update Ballot Panel", Color.red);
        CloseBallotInfo();
    }
    // Helper method to retrieve all ballot prefabs
    private List<Match> GetBallotPrefabs()
    {
        List<Match> matches = new List<Match>();

        // Assuming the parent GameObject containing all ballot prefabs is named "BallotsContainer"
        GameObject ballotsContainer = GameObject.Find("BallotsContainer");

        if (ballotsContainer != null)
        {
            // Iterate through each child of the ballotsContainer
            foreach (Transform child in ballotsContainer.transform)
            {
                // Get the BallotListEntry component and retrieve the match
            if (child.TryGetComponent<BallotListEntry>(out BallotListEntry ballotEntry))
                {if (ballotEntry != null)
                {
                    Match match = ballotEntry.GetSavedBallot();
                    matches.Add(match);
                }}
            }
        }
        else
        {
            Debug.LogWarning("BallotsContainer not found in the scene.");
        }

        return matches;
    }

    
}
