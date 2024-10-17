using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DrawEditListEntry : MonoBehaviour
{
    public BritishMatch_TMP myMatch;
    public AdvancedDropdown OGDropdown;
    public AdvancedDropdown OODropdown;
    public AdvancedDropdown CGDropdown;
    public AdvancedDropdown CODropdown;

    public AdvancedDropdown Adj1Dropdown;
    public AdvancedDropdown Adj2Dropdown;
    public AdvancedDropdown Adj3Dropdown;
    

    public void SetMatch(BritishMatch_TMP match)
    {
        myMatch = match;
    }

    void PopulateDropDown()
    {
        List<string> teamNames = new List<string>();
        foreach (var team in myMatch.teamsInMatch)
        {
            teamNames.Add(team.TeamName);
        }

        OGDropdown.SetOptions(teamNames);
        OODropdown.SetOptions(teamNames);
        CGDropdown.SetOptions(teamNames);
        CODropdown.SetOptions(teamNames);
    }
}