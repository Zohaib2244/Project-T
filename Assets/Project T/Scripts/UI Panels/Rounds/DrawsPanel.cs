using System.Collections;
using System.Collections.Generic;
using Scripts.UIPanels;
using UnityEngine;
using Scripts.Resources;
using System;

[Serializable]
public class DrawPanels
{
    public DrawPanelTypes drawPanelType;
    public Transform drawPanel;
    
}
public class DrawsPanel : MonoBehaviour
{
    #region Singleton
    public static DrawsPanel Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    #region Essentials
    public List<Match> matches_TMP = new List<Match>();
    [SerializeField] private List<DrawPanels> drawPanels;
    void OnEnable()
    {
        if(MainRoundsPanel.Instance.selectedRound.drawGenerated == false)
        {
            SwitchDrawPanel(DrawPanelTypes.DrawOptionsPanel);
        }
        else if(MainRoundsPanel.Instance.selectedRound.drawGenerated == true)
        {
            matches_TMP = MainRoundsPanel.Instance.selectedRound.matches;
            SwitchDrawPanel(DrawPanelTypes.DrawDisplayPanel);
        }

    }
    #endregion

    public void RegenerateDraw()
    {
        MainRoundsPanel.Instance.selectedRound.drawGenerated = false;
        MainRoundsPanel.Instance.selectedRound.ballotsAdded = false;
        SwitchDrawPanel(DrawPanelTypes.DrawOptionsPanel);
        matches_TMP.Clear();
    }
    public void SwitchDrawPanel(DrawPanelTypes panel)
    {
        foreach (var drawPanel in drawPanels)
        {
            if (drawPanel.drawPanelType == panel)
            {
                drawPanel.drawPanel.gameObject.SetActive(true);
            }
            else
            {
                drawPanel.drawPanel.gameObject.SetActive(false);
            }
        }
    }
}
