using DG.Tweening;
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
                drawPanel.drawPanel.localScale = Vector3.zero; // Start from scale zero
                drawPanel.drawPanel.DOScale(Vector3.one, 0.5f); // Animate to scale one over 0.5 seconds
            }
            else
            {
                drawPanel.drawPanel.DOScale(Vector3.zero, 0.5f).OnComplete(() => drawPanel.drawPanel.gameObject.SetActive(false)); // Animate to scale zero over 0.5 seconds and then deactivate
            }
        }
    }
    public void OpenEditDrawPanel()
    {
        SwitchDrawPanel(DrawPanelTypes.DrawEditPanel);
    }

    public void CloseEditDrawPanel()
    {
        SwitchDrawPanel(DrawPanelTypes.DrawDisplayPanel);
    }
}
