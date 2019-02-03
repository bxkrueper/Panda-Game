/***************************************************************
* file: HpBarUI.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the HpBarUI
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUI : MonoBehaviour {

    public Image HpBar;
    public Text HpVal;

    public float curHp;
    public float maxHp;

    //method: Start
    //purpose: constructor
    private void Start () {
        Panda panda = GetComponent<Panda>();
        maxHp = panda.GetMaxHealth();
        curHp = maxHp;
        UpdateHealthBar();
	}
	
	//Method: UpdateHealthBar
    //Purpose: updates the health bar UI to relect curHp
	public void UpdateHealthBar () {
        float ratio = curHp / maxHp;
        HpBar.rectTransform.localScale = new Vector2(ratio, 1);
        HpVal.text = curHp.ToString() + '/' + maxHp.ToString();
	}

    //Method: IncreaseHp
    //Purpose: increases the value in curHp
    public void IncreaseHp(float amt)
    {
        curHp += amt;
        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
        UpdateHealthBar();
        print("curHP: " + curHp);
    }

    //Method: DecreaseHp
    //Purpose: decreases the value in curHp
    public void DecreaseHp (float amt) {
        curHp -= amt;
        if (curHp < 0) {
            curHp = 0;
        }
        UpdateHealthBar();
        print("curHP: " + curHp);
    }
    
}
