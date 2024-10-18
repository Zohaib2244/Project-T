using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpeakersScript : MonoBehaviour
{
    public GameObject speakersPrefab; // Reference to the prefab
    public Transform scrollViewContent; // Reference to the ScrollView content

    // Start is called before the first frame update
    void Start()
    {
        RefreshSpeakers();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method to refresh speakers
    public void RefreshSpeakers()
    {
        // Activate loading panel
        // Loading.Instance.ShowLoadingScreen();

        // Clear existing entries
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // await AppConstants.instance.GetAllRounds();
        List<AppConstants.SpeakerInfo> speakersList = AppConstants.instance.GetAllSpeakersInfo();
        speakersList.ForEach(speaker =>
        {
            // Instantiate the prefab
            GameObject speakerEntry = Instantiate(speakersPrefab, scrollViewContent);

            // Set the data
            SpeakersPanelEntry panelEntry = speakerEntry.GetComponent<SpeakersPanelEntry>();
            panelEntry.SetSpeakersData(speaker.Name, speaker.Category.ToString(), speaker.Team, speaker.Institution, speaker.Contact, speaker.Email);
        });

        // Deactivate loading panel
        // Loading.Instance.HideLoadingScreen();
    }

    // Method to be called when the panel is set to active
    private void OnEnable()
    {
        RefreshSpeakers();
    }
}