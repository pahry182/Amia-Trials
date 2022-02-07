using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButtonDescWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private MainSceneController _msc;
    [SerializeField] private float time = 0.5f;
    [SerializeField] private string spellID;
    [SerializeField] private string skillID;
    private bool isPointerDown = false;
    private bool isLongPressed = false;
    private float elapsedTime = 0f;

    //private Button button;

    private void Awake()
    {
        //button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    private void Update()
    {
        if (isPointerDown && !isLongPressed)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= time)
            {
                isLongPressed = true;
                elapsedTime = 0f;
                Action();
                //if (button.interactable)
                    
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        isLongPressed = false;
        elapsedTime = 0f;
    }

    public void Action()
    {
        if (skillID == "")
        {
            foreach (var item in GameManager.Instance._player.GetComponent<PlayerSpell>().spellList)
            {
                if (item.spellName == spellID)
                {
                    _msc._dscw.gameObject.SetActive(true);
                    _msc._dscw.GetData(item);
                    break;
                }
            }
        }
        else
        {
            foreach (var item in GameManager.Instance._player.GetComponent<PlayerSkill>().playerSkills)
            {
                if (item.skillName == skillID)
                {
                    _msc._dscw.gameObject.SetActive(true);
                    _msc._dscw.GetData(item);
                    break;
                }
            }
        }
    }
}
