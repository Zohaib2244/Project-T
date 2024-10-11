using System.Collections;
using System.Collections.Generic;
using Scripts.Resources;
using UnityEngine;

public class TeamPanel : MonoBehaviour
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
        Debug.Log("this is a console test.");
        await AppConstants.instance.GetAllRounds();
    }
}
