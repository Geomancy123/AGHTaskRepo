using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Flag : MonoBehaviour
{

    [SerializeField]
    private E_Color m_FlagColor;

    [SerializeField]
    private GameObject m_RedBanner, m_BlueBanner;

    [SerializeField]
    private GameObject m_FlagModel;

    [SerializeField]
    public bool m_FlagActive = true;


    // Start is called before the first frame update
    void Start()
    {
        //sets flag color
        SetFlagColor(m_FlagColor);

        //toggles visibility depending
        ActivateFlag(m_FlagActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFlagColor(E_Color _col)
    {
        m_FlagColor = _col;

        if (m_FlagColor == E_Color.RED)
        {
            m_BlueBanner.SetActive(false);
        }
        else if (m_FlagColor == E_Color.BLUE)
        {
            m_RedBanner.SetActive(false);
        }
    }    


    public void ActivateFlag(bool _state)
    {
        m_FlagActive = _state;
       
       m_FlagModel.SetActive(_state);
        
    }
}
