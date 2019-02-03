/***************************************************************
* file: FartGaugeUI.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the FartGaugeUI
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FartGaugeUI : MonoBehaviour {

    //Fields
    public Image FartGauge;
    public Text FartVal;

    public float curFart;
    public float maxFart;

    //method: Start
    //purpose: constructor
    private void Start()
    {
        Panda panda = GetComponent<Panda>();
        maxFart = panda.GetMaxFartEnergy();
        curFart = panda.GetFartEnergy();
        UpdateFartGauge();
    }

    //Method: UpdateFartGauge
    //Purpose: updates the fart gauge UI to reflect curFart value
    private void UpdateFartGauge()
    {
        float ratio = curFart / maxFart;
        FartGauge.rectTransform.localScale = new Vector2(ratio, 1);
        FartVal.text = curFart.ToString() + '/' + maxFart.ToString();
    }

    //Method: IncreaseFart
    //Purpose: increases the value in curFart
    public void IncreaseFart(float amt)
    {
        curFart += amt;
        if (curFart > maxFart)
        {
            curFart = maxFart;
        }
        UpdateFartGauge();
    }

    //Method: SetFart
    //Purpose: set curFart to amt passed in
    public void SetFart(float amt)
    {
        curFart = amt;
        if (curFart > maxFart)
        {
            curFart = maxFart;
        }
        if (curFart < 0)
        {
            curFart = 0;
        }
        UpdateFartGauge();
    }

    //Method: DecreaseFart
    //Purpose: decreases the value in curFart
    public void DecreaseFart(float amt)
    {
        curFart -= amt;
        if (curFart < 0) {
            curFart = 0;
        }
        UpdateFartGauge();
    }
}
