using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerAbility : MonoBehaviour
{
    [Header("status")]
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;
    public float MaxHp
    {
        get { return maxHp; }
    }
    public float Hp
    {
        get { return hp; }
    }
    [SerializeField] protected float maxSp;
    [SerializeField] protected float sp;

    [Header("UI")]
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected Slider spSlider;

    public void HpSum(float value)
    {
        hp += value;
    }

    
}


