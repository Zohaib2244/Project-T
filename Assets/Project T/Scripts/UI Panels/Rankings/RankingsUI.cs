using System.Collections;
using System.Collections.Generic;
using Scripts.Resources;
using UnityEngine;

public class RankingsUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void printToConsole() {
        await AppConstants.instance.GetAllRounds();
        AppConstants.instance.PrintRankings();
    }
}
