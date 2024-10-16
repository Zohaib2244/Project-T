using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AdjudicatorsScript : MonoBehaviour
{
    public GameObject adjudicatorPrefab; // Reference to the prefab
    public Transform scrollViewContent; // Reference to the ScrollView content

    // Start is called before the first frame update
    void Start()
    {
        RefreshAdjudicators();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method to refresh adjudicators
    public void RefreshAdjudicators()
    {
        // Activate loading panel
        // Loading.Instance.ShowLoadingScreen();

        // Clear existing entries
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // await AppConstants.instance.GetAllRounds();
        List<AppConstants.AdjudicatorInfo> adjudicatorsList = AppConstants.instance.GetAllAdjudicatorsInfo();
        adjudicatorsList.ForEach(adjudicator =>
        {
            // Instantiate the prefab
            GameObject adjudicatorEntry = Instantiate(adjudicatorPrefab, scrollViewContent);

            // Set the data
            AdjudicatorPanelEntry panelEntry = adjudicatorEntry.GetComponent<AdjudicatorPanelEntry>();
            panelEntry.SetAdjudicatorData(adjudicator.Name, adjudicator.Institution, adjudicator.Type.ToString());
        });

        // Deactivate loading panel
        // Loading.Instance.HideLoadingScreen();
    }

    // Method to be called when the panel is set to active
    private void OnEnable()
    {
        RefreshAdjudicators();
    }
}