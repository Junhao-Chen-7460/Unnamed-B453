using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public string ownerTag;
    public string baseTag;
    public int damage;

    [SerializeField] float AutoDestoryTime = 1f;


    void Start()
    {
        Destroy(gameObject, AutoDestoryTime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (
            (other.GetComponent<BillionController>() != null || other.GetComponent<BaseController>() != null)
             && !other.CompareTag(ownerTag))
        {
            if (other.CompareTag(baseTag))
            {
                return;
            }
            minusHealth(other, damage);
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }

    public void minusHealth(Collider2D other, int damage)
    {
        GameObject obj = other.gameObject;
        String Otag = obj.tag;

        if (Otag == "MRed"
            || Otag == "MBlue"
            || Otag == "MGreen"
            || Otag == "MYellow")
        {
            int Chealth = obj.GetComponent<BillionController>().health;
            if (Chealth <= damage) 
            {
                Destroy(obj);
                AddExp(2);
            } else {
                obj.GetComponent<BillionController>().health -= damage;
                obj.GetComponent<BillionController>().HandleHealthVFX();
            }
                
        }
        if (Otag == "BR"
            || Otag == "BB"
            || Otag == "BG"
            || Otag == "BY")
        {
            int Chealth = other.GetComponent<BaseController>().health;
            if (Chealth <= damage) 
            {
                Destroy(obj);
                AddExp(10);
            } else {
                other.GetComponent<BaseController>().health -= damage;
                other.GetComponent<BaseController>().HandleHealthVFX();
            }
        }
    }

    public void AddExp(int number)
    {
        GameObject Base = GameObject.FindWithTag(baseTag);

        int CurExp = Base.GetComponent<BaseController>().curExp;
        int MaxExp = Base.GetComponent<BaseController>().maxExp;
        
        if (CurExp + number >= MaxExp)
        {
            Base.GetComponent<BaseController>().Upgrade();
            Base.GetComponent<BaseController>().curExp = CurExp + number - MaxExp;
        } else 
        {
            Base.GetComponent<BaseController>().curExp += number;
        }

        Base.GetComponent<BaseController>().HandleExpVFX();

        Debug.Log("exp to: " + Base);
    }
}
