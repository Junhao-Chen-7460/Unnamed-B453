using System.Collections;
using UnityEngine;

public class Flags : MonoBehaviour
{
    [SerializeField] float blinkTime = 0.25f;

    [SerializeField] GameObject flag1;
    [SerializeField] GameObject flag2;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= blinkTime)
        {
            timer = 0f;
            Blink();
        }
    }
    void Blink()
    {
        flag1.SetActive(!flag1.activeSelf);
        flag2.SetActive(!flag2.activeSelf);
    }



}
