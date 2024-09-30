using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scripts.UIPanels.RoundPanels;


public class MotionsEntry : MonoBehaviour
{
    [SerializeField] private TMP_InputField motionText_IF;
    [SerializeField] private TMP_InputField motionInfoSlide_IF;
    [SerializeField] private Toggle infoSLideToggle;

    private string motionText;
    private string motionInfoSlide;


    void Start()
    {
        infoSLideToggle.onSelectOption1 += InfoSlideOn;
        infoSLideToggle.onSelectOption2 += InfoSlideOff;
    }

    public Dictionary<string, string> GetMotion()
    {
        Dictionary<string, string> motion = new Dictionary<string, string>
        {
            {"motionText", motionText_IF.text},
            {"motionInfoSlide", motionInfoSlide_IF.text}
        };
        return motion;
    }
    public void SetMotion(Dictionary<string, object> motion)
    {
        motionText = motion.ContainsKey("motionText") ? motion["motionText"].ToString() : "";
        motionInfoSlide = motion.ContainsKey("motionInfoSlide") ? motion["motionInfoSlide"].ToString() : "";

        motionText_IF.text = motionText;
        motionInfoSlide_IF.text = motionInfoSlide;

        // Check if the text in the input field is null or empty
        if (string.IsNullOrEmpty(motionInfoSlide))
        {
            infoSLideToggle.SelectOption2();
        }
    }
    private void InfoSlideOn()
    {
        motionInfoSlide_IF.interactable = true;
    }
    private void InfoSlideOff()
    {
        motionInfoSlide_IF.interactable = false;
    }
    public void SaveMotion()
    {
      Round_MotionsPanel.Instance.SaveMotion(GetMotion());
    }
}
