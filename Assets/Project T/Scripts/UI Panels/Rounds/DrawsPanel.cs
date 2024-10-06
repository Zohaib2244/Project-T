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
    public List<Match> matches_TMP;
    [SerializeField] private List<DrawPanels> drawPanels;
    void OnEnable()
    {
        if(MainRoundsPanel.Instance.selectedRound.drawGenerated == false)
        {
            SwitchDrawPanel(DrawPanelTypes.DrawOptionsPanel);
        }
        else
        {
            SwitchDrawPanel(DrawPanelTypes.DrawDisplayPanel);
        }
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
