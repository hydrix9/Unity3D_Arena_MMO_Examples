using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public Text[] texts;
    public int lineHeightScaling; //uses this to make the tooltip bigger when populated
    public int baseHeight; //height without data that scales the height using above field like description

    private void Awake()
    {
        texts = GetComponentsInChildren<Text>();
        Debug.Log("tt initialized");
    }

    public virtual void Populate()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj">the object that uses the baseHeight- basically the backgrounding panel</param>
    public void ResetScale(RectTransform obj)
    {
        obj.sizeDelta = new Vector2(obj.sizeDelta.x, baseHeight);
    }

    public virtual void AddScale(RectTransform obj, int lines)
    {
        obj.sizeDelta = new Vector2(obj.sizeDelta.x, obj.sizeDelta.y + lines * lineHeightScaling);
    }


    public void PopulateAsSpell(Player player, Spell spell)
    {
        if(spell == null)
        {
            Debug.LogWarning("empty spell trying to be populated");
            return;
        }

        //calculate hypothetical stats for tooltip... I know... I know... but at least it should show the TRUE values, which is awesome
        spellHit spellHit = new spellHit(null, player, spell);
        spell.CalcCastFlatOffensive(spellHit);
        spell.CalcCastPerOffensive(spellHit);
        player.CalcCastFlatOffensive(spellHit);
        player.CalcCastPerOffensive(spellHit);

        texts[0].text = spell.name;
        texts[1].text = "range: " + spellHit.range;
        if (spellHit.castTime > 0)
            texts[2].text = spellHit.castTime + " sec cast";
        else
            texts[2].text = "instant cast";

        texts[3].text = spell.description;
        Canvas.ForceUpdateCanvases(); //cause texts to be updated, giving us a readable cache
        //scale the background to description height
        ResetScale(transform.GetChild(0).GetComponent<RectTransform>());
        AddScale(transform.GetChild(0).GetComponent<RectTransform>(), texts[3].cachedTextGenerator.lines.Count);

    }
}