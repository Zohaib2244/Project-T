using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundsList : MonoBehaviour
{
    public GameObject buttonPrefab; // Reference to the button prefab
    public Transform scrollViewContent; // Reference to the Scroll View content

    // Start is called before the first frame update
    void Start()
    {
        int noOfPrelims = AppConstants.instance.selectedTouranment.noOfPrelims;
        PopulateScrollView(noOfPrelims);
    }

    void PopulateScrollView(int noOfPrelims)
    {
        for (int i = 1; i <= noOfPrelims; i++)
        {
            GameObject button = Instantiate(buttonPrefab, scrollViewContent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Round " + i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}