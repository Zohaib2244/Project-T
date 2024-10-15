using UnityEngine;

public class DrawEditListEntry : MonoBehaviour
{
    Match myMatch;
    AdvancedDropdown OGDropdown;
    AdvancedDropdown OODropdown;
    AdvancedDropdown CGDropdown;
    AdvancedDropdown CODropdown;

    AdvancedDropdown Adj1Dropdown;
    AdvancedDropdown Adj2Dropdown;
    AdvancedDropdown Adj3Dropdown;

    public void SetMatch(Match match)
    {
        myMatch = match;
    }
}
