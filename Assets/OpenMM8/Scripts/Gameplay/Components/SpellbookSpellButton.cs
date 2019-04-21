using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Linq;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.EventSystems;
using Assets.OpenMM8.Scripts.Gameplay.Data;

public class SpellbookSpellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SpellType SpellType;
    public Button SpellButton;
    public bool IsClicked = false;

    public SpellbookPageUI Parent;
    
    //public SpellData

    // Use this for initialization
    void Start()
    {
        SpellButton = GetComponent<Button>();
        SpellButton.onClick.AddListener(delegate
        {
            OnSpellClicked();
        });
    }

    private void OnSpellClicked()
    {
        foreach (SpellbookSpellButton btn in Parent.SpellButtons.Values)
        {
            if (!btn.Equals(this))
            {
                btn.IsClicked = false;
            }
        }

        if (IsClicked)
        {
            // Should cast the SpellType spell now

            // This actually closes the spellbook... should figure out a better way
            GameMgr.Instance.PressEscape();
        }

        IsClicked = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SpellData data = DbMgr.Instance.SpellDataDb.Get(SpellType);
        Parent.SpellbookUI.SpellNameText.text = data.Name;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SpellData data = DbMgr.Instance.SpellDataDb.Get(SpellType);

        if (Parent.SpellbookUI.SpellNameText.text == data.Name)
        {
            Parent.SpellbookUI.SpellNameText.text = "";
        }
    }
}
