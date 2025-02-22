using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scripts.UIPanels.RoundPanels;
using System.Linq;


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
        motionInfoSlide_IF.interactable = false;
    }
    private void OnDestroy()
    {
        infoSLideToggle.onSelectOption1 -= InfoSlideOn;
        infoSLideToggle.onSelectOption2 -= InfoSlideOff;
    }

    public Dictionary<string, string> GetMotion()
    {
        Dictionary<string, string> motion = new Dictionary<string, string>
        {
            {motionText_IF.text, motionInfoSlide_IF.text},
        };
        return motion;
    }

    public void SetMotion(Dictionary<string, string> motion)
    {
        if (motion == null || !motion.Any())
        {
            Debug.LogWarning("SetMotion: The provided motion dictionary is null or empty.");
            motionText = " ";
            motionInfoSlide = " ";
        }
        else
        {
            var _motion = motion.First();
            motionText = _motion.Key;
            motionInfoSlide = _motion.Value;
        }

        Debug.Log("motionText: " + motionText);
        Debug.Log("motionInfoSlide: " + motionInfoSlide);

        motionText_IF.text = motionText;
        motionInfoSlide_IF.text = motionInfoSlide;

        if (string.IsNullOrEmpty(motionInfoSlide))
        {
            infoSLideToggle.SelectOption2();
        }
        else
        {
            infoSLideToggle.SelectOption1();
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
