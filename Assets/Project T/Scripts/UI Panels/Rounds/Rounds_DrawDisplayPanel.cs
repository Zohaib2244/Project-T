using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Scripts.UIPanels;
using Scripts.FirebaseConfig;

public class Rounds_DrawDisplayPanel : MonoBehaviour
{
    [SerializeField] private Transform drawDisplayPanel;
    [SerializeField] private Transform drawContent;
    [SerializeField] private GameObject drawEntryPrefab;


    void OnEnable()
    {
        drawDisplayPanel.localScale = Vector3.zero; // Start from zero scale
        drawDisplayPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack); // Animate to full scale with a pop effect
        UpdateMatchesList();
    }
    void OnDisable()
    {
        // Clear existing entries
        foreach (Transform child in drawContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateMatchesList()
    {
        // Clear existing entries
        foreach (Transform child in drawContent)
        {
            Destroy(child.gameObject);
        }
    
        // Instantiate new entries and set match data
        for (int i = 0; i < DrawsPanel.Instance.matches_TMP.Count; i++)
        {
            var match = DrawsPanel.Instance.matches_TMP[i];
            var drawEntry = Instantiate(drawEntryPrefab, drawContent);
            drawEntry.GetComponent<MatchListEntry>().SetMatch(match, i + 1); // Pass match object and match number (index + 1)
        }
    }
    public async void SaveDraw()
    {
        Loading.Instance.ShowLoadingScreen();
    
        if ( MainRoundsPanel.Instance.selectedRound == null)
        {
            Debug.LogWarning("No selected round found.");
            return;
        }
    MainRoundsPanel.Instance.selectedRound.matches.Clear();
         MainRoundsPanel.Instance.selectedRound.matches = DrawsPanel.Instance.matches_TMP;
        // MainRoundsPanel.Instance.selectedRound.matches.Clear();
        // MainRoundsPanel.Instance.selectedRound.matches = allMatches;
        MainRoundsPanel.Instance.selectedRound.drawGenerated = true;
        Debug.Log("Draw saved: " + MainRoundsPanel.Instance.selectedRound.matches.Count);
        // Set the draw generated flag to true
        MainRoundsPanel.Instance.UpdatePanelSwitcherButtonsStates();
    //   await FirestoreManager.FireInstance.SaveAllMatchesToFirestore( MainRoundsPanel.Instance.selectedRound.roundCategory.ToString(), MainRoundsPanel.Instance.selectedRound.roundId, DrawsPanel.Instance.matches_TMP, OnMatchesSaveSuccess, OnMatchesSaveFail);
    }
    private List<Match> GetDrawPrefabs()
    {
        List<Match> drawPrefabs = new List<Match>();
    
        foreach (Transform child in drawContent)
        {
            var drawEntry = child.GetComponent<MatchListEntry>();
            drawPrefabs.Add(drawEntry.GetMatch());
        }
    
        return drawPrefabs;
    }
    
    private void OnMatchesSaveSuccess(List<Match> matches)
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Draw saved successfully.", Color.green);
        // Save the draw prefabs to the selected round
         MainRoundsPanel.Instance.selectedRound.matches.Clear();
         MainRoundsPanel.Instance.selectedRound.matches = matches;
        // MainRoundsPanel.Instance.selectedRound.matches.Clear();
        // MainRoundsPanel.Instance.selectedRound.matches = allMatches;
        MainRoundsPanel.Instance.selectedRound.drawGenerated = true;
        Debug.Log("Draw saved: " + MainRoundsPanel.Instance.selectedRound.matches.Count);
        // Set the draw generated flag to true
        MainRoundsPanel.Instance.UpdatePanelSwitcherButtonsStates();
    }
    private void OnMatchesSaveFail()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Failed to save draw.", Color.red);
    }
}
