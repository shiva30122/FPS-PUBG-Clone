using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Health_ : MonoBehaviour
{


    public float Max_Health;
    public float Health;

    public UnityEvent Health_Zero;

    private void Start()
    {
        
        Health = Max_Health;

    }

    public void Take_Damage(float Damage_Amount)
    {

        Health-=Damage_Amount;

        if (Health <=0)
        {
            Health_Zero?.Invoke();
        }

    }

    public void Message()
    {
        Debug.Log(" You have killed someone !.... ");

        Destroy(this.gameObject);
    }





}
