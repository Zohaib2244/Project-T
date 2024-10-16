using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // To handle UI elements

public class RankingsPanel : MonoBehaviour
{
    // Reference to the panels
    public GameObject teamsPanel;
    public GameObject speakersPanel;
    public GameObject participantsPanel;
    public GameObject adjudicatorsSelectPanel;
    public GameObject speakersSelectPanel;

    // Reference to the buttons
    public Button teamsButton;
    public Button speakersButton;
    public Button participantsButton;
    public Button adjudicatorsSelectButton;
    public Button speakersSelectButton;

    // Start is called before the first frame update
    void Start()
    {
        // Set Teams Panel as the default
        teamsPanel.SetActive(true);
        speakersPanel.SetActive(false);
        participantsPanel.SetActive(false);

        // Add listeners to buttons to call the respective methods
        teamsButton.onClick.AddListener(ShowTeamsPanel);
        speakersButton.onClick.AddListener(ShowSpeakersPanel);
        participantsButton.onClick.AddListener(ShowBreaksPanel);
    }

    // Methods to show the respective panels and hide the others
    public void ShowTeamsPanel()
    {
        teamsPanel.SetActive(true);
        speakersPanel.SetActive(false);
        participantsPanel.SetActive(false);
    }

    public void ShowSpeakersPanel()
    {
        teamsPanel.SetActive(false);
        speakersPanel.SetActive(true);
        participantsPanel.SetActive(false);
    }

    public void ShowBreaksPanel()
    {
        teamsPanel.SetActive(false);
        speakersPanel.SetActive(false);
        participantsPanel.SetActive(true);
    }

    public void SelectAdjudicators()
    {
        teamsPanel.SetActive(false);
        speakersPanel.SetActive(false);
        participantsPanel.SetActive(true);
        adjudicatorsSelectPanel.SetActive(true);
    }

    public void SelectSpeakers()
    {
        teamsPanel.SetActive(false);
        speakersPanel.SetActive(false);
        participantsPanel.SetActive(true);
        speakersPanel.SetActive(true);
    }
}
