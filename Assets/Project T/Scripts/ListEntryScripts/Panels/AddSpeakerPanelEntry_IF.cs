using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scripts.ListEntry
{public class AddSpeakerPanelEntry_IF : MonoBehaviour
{
    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private TMP_InputField ContactInputField;
    [SerializeField] private TMP_InputField EmailInputField;
    [SerializeField] private TMP_Text speakernumberText;

    public Speaker GetSpeaker
    {
        get
        {
            Speaker speaker = new Speaker();
            speaker.speakerName = NameInputField.text;
            speaker.speakerPhone = ContactInputField.text;
            speaker.speakerEmail = EmailInputField.text;
            return speaker;
        }
    }
    public void SetSpeakerInfo(Speaker speaker)
    {
        NameInputField.text = speaker.speakerName;
        ContactInputField.text = speaker.speakerPhone;
        EmailInputField.text = speaker.speakerEmail;
    }
    public void SetSpeakerNumber(int number)
    {
        speakernumberText.text = "Speaker " + number;
    }   
}}