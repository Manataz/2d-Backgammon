using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetItem : MonoBehaviour
{
    [Header("General")]
    public bool choose;
    public bool forClass;
    public bool forPremium;
    public int count;
    public string classTarget;
    private Vector2 scale_default;

    [Header("Color")]
    public Color normalColor;
    public Color chooseColor;
    
    [Header("Access")]
    public Text countTextUI;
    public Image mainImage;
    public Image vertexImage;
    public GameObject bord;
    public GameObject forClassButtonC;
    public GameObject forClassButtonB;
    public GameObject forClassButtonA;
    public GameObject goPremiumButton;
    public GameObject _lock;

    private SearchSystem _searchSystem;
    private Controller _controller;

    private void Start()
    {
        _searchSystem = FindObjectOfType<SearchSystem>();
        _controller = FindObjectOfType<Controller>();

        scale_default = transform.localScale;
    }

    void Update()
    {
        mainImage.enabled = choose || forClass || forPremium;
        bord.SetActive(!choose);
        forClassButtonC.SetActive(forClass && classTarget == "C");
        forClassButtonB.SetActive(forClass && classTarget == "B");
        forClassButtonA.SetActive(forClass && classTarget == "A");
        goPremiumButton.SetActive(forPremium);
        countTextUI.text = count.ToString();
        _lock.SetActive(forClass || forPremium);

        if (forClass || forPremium)
        {
            if (_searchSystem.betCount == count)
                _searchSystem.betCount = 0;
        }
        
        if (choose)
            vertexImage.color = chooseColor;
        else
            vertexImage.color = normalColor;

        transform.localScale = Vector2.Lerp(transform.localScale, scale_default, 2 * Time.deltaTime);
    }

    public void ButtonClick()
    {
        if (!forClass && !forPremium && !_searchSystem.searching)
        {
            if (_controller != null)
                _controller.clickAudio.Play();
                
            transform.localScale = scale_default * 1.1f;
            _searchSystem.betCount = count;
            _searchSystem.UpdateBetItems();
        }
    }
}
