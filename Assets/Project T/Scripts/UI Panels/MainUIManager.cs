using System.Collections.Generic;
using UnityEngine;
using Scripts.Resources;
using DG.Tweening;


namespace Scripts.UI
{
    [System.Serializable]
    public class PanelItem
    {
        public Panels panel;
        public GameObject gameObject;
    }
    public class MainUIManager : MonoBehaviour
    {
        public static MainUIManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public List<PanelItem> panels = new List<PanelItem>();
        private Panels activePanel;

        public Panels ActivePanel
        {
            get { return activePanel; }
            set { activePanel = value; }
        }
        public void OpenHomePanel()
        {
            SwitchPanel(Panels.TournamentHomePanel);
            Debug.Log("Home Panel Opened");
        }

        public void SwitchPanel(Panels panel)
        {
            if(panel == Panels.EmailPanel)
            {
                foreach(PanelItem item in panels)
                {
                    if(item.panel == panel)
                    {
                        item.gameObject.SetActive(true);
                        Vector3 pos = item.gameObject.transform.position;
                        item.gameObject.transform.position = new Vector3(item.gameObject.transform.position.x, -Screen.height, item.gameObject.transform.position.z);
                        item.gameObject.transform.DOMoveY(pos.y, 0.5f);
                        activePanel = panel;
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
                return;
            }

            foreach (PanelItem item in panels)
            {
                if (item.panel == panel)
                {
                    item.gameObject.SetActive(true);
                    activePanel = panel;
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
}