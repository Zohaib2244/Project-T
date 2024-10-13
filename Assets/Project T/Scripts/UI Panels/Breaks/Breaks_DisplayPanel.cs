using UnityEngine;
using TMPro;
using Scripts.Resources;
using DG.Tweening;

public class Breaks_DisplayPanel : MonoBehaviour
{
    [Header("Side Panel")]
    [SerializeField] private TMP_Text selectedOpenTeamstxt;
    [SerializeField] private TMP_Text selectedNoviceTeamstxt;

     [Header("Breaks List Panel")]
    [SerializeField] private Transform openBreaksListPanel;
    [SerializeField] private Transform openBreaksListPanelContent;
    [SerializeField] private GameObject openBreaksListPanelPrefab;
    [SerializeField] private Transform noviceBreaksListPanel;
    [SerializeField] private Transform noviceBreaksListPanelContent;
    [SerializeField] private GameObject noviceBreaksListPanelPrefab;
    [Header("Animation Vectors")]
    [SerializeField] private float openPanelInVector;
    [SerializeField] private float openPanelOutVector;
    [SerializeField] private float novicePanelInVector;
    [SerializeField] private float novicePanelOutVector;

    [SerializeField] private Toggle teamCategoryToggle;
    [SerializeField] private BreaksPanel breaksPanel;

    void OnEnable()
    {
        teamCategoryToggle.onSelectOption1 = OpenOpenEligiblePanel;
        teamCategoryToggle.onSelectOption2 = OpenNoviceEligiblePanel;
        ConfigureScreen();
        teamCategoryToggle.SelectOption(1);
    }

    void OnDisable()
    {
        teamCategoryToggle.onSelectOption1 = null;
        teamCategoryToggle.onSelectOption2 = null;
        openBreaksListPanel.gameObject.SetActive(false);
        noviceBreaksListPanel.gameObject.SetActive(false);
    }
    void ConfigureScreen()
    {
        selectedOpenTeamstxt.text = breaksPanel.breakingOpenTeams.Count.ToString();
        selectedNoviceTeamstxt.text = breaksPanel.breakingNoviceTeams.Count.ToString();
    }
    public void OpenOpenEligiblePanel()
    {
        if (noviceBreaksListPanel.gameObject.activeSelf)
            noviceBreaksListPanel.DOLocalMoveY(novicePanelOutVector, 0.5f).OnComplete(() => noviceBreaksListPanel.gameObject.SetActive(false));

        openBreaksListPanel.gameObject.SetActive(true);
        openBreaksListPanel.localPosition = new Vector3(0, -Screen.height, 0);
        openBreaksListPanel.DOLocalMoveY(openPanelInVector, 0.5f).OnComplete(() => UpdateOpenTeamList());
    }
    public void OpenNoviceEligiblePanel()
    {
        if (openBreaksListPanel.gameObject.activeSelf)
            openBreaksListPanel.DOLocalMoveY(openPanelOutVector, 0.5f).OnComplete(() => openBreaksListPanel.gameObject.SetActive(false));

        noviceBreaksListPanel.gameObject.SetActive(true);
        noviceBreaksListPanel.localPosition = new Vector3(0, -Screen.height, 0);
        noviceBreaksListPanel.DOLocalMoveY(novicePanelInVector, 0.5f).OnComplete(() => UpdateNoviceTeamList());
    }

    
    void UpdateOpenTeamList()
    {
        // Clear existing entries
        foreach (Transform child in openBreaksListPanelContent)
        {
            Destroy(child.gameObject);
        }
    
        // Sort the teams based on the chosen break parameter
    
        // Instantiate and set up team entries
        for (int i = 0; i < breaksPanel.breakingOpenTeams.Count; i++)
        {
            Team team = breaksPanel.breakingOpenTeams[i];
            GameObject teamEntry = Instantiate(openBreaksListPanelPrefab, openBreaksListPanelContent);
            teamEntry.GetComponent<Team_LE_BreakDisplay>().SetTeam(team, i + 1); // Pass the ranking index
        }
    }
    void UpdateNoviceTeamList()
    {
        // Clear existing entries
        foreach (Transform child in noviceBreaksListPanelContent)
        {
            Destroy(child.gameObject);
        }
        // Instantiate and set up team entries
        for (int i = 0; i < breaksPanel.breakingNoviceTeams.Count; i++)
        {
            Team team = breaksPanel.breakingNoviceTeams[i];
            GameObject teamEntry = Instantiate(noviceBreaksListPanelPrefab, noviceBreaksListPanelContent);
            teamEntry.GetComponent<Team_LE_BreakDisplay>().SetTeam(team, i + 1); // Pass the ranking index
        }
    }

}
