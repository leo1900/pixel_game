using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.UI;

public class BR : MonoBehaviour
{
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnBtnClick);
    }
    
    void Start()
    {
        
    }

    public void OnBtnClick()
    {
        CatNativeBridge.Copy("112233");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
