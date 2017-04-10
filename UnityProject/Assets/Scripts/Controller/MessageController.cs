﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    public static MessageController single;

    // the canvas that directed to the wait screen
    private Canvas _previousCanvas;

    private int _canvasesWaiting;

    [SerializeField]
    private Canvas _pleaseWaitCanvas;
    [SerializeField]
    private Canvas _errorCanvas;
    [SerializeField]
    private Text _errorMessage;

    void Start()
    {
        single = this;
        _canvasesWaiting = 0;
        _pleaseWaitCanvas.enabled = false;
        _errorCanvas.enabled = false;
    }

    private void showPreviousScreen()
    {
        if (_previousCanvas == null)
            MainMenuScript.showWebView();
        else
            _previousCanvas.enabled = true;
    }

    private void hidePreviousScreen()
    {
        if (_previousCanvas == null)
            MainMenuScript.hideWebView();
        else
            _previousCanvas.enabled = false;
    }

    public void displayWaitScreen(Canvas sender)
    {
        if (!_pleaseWaitCanvas.enabled)
        {
            _previousCanvas = sender;
            hidePreviousScreen();
            _pleaseWaitCanvas.enabled = true;
        }
        _canvasesWaiting++;
    }

    public void displayError(Canvas sender, string errorText)
    {
        _canvasesWaiting = 0;
        _errorMessage.text = errorText;
        if (_pleaseWaitCanvas.enabled)
        {
            _pleaseWaitCanvas.enabled = false;
        }
        else
        {
            _previousCanvas = sender;
            hidePreviousScreen();
        }
        _errorCanvas.enabled = true;
    }

    public void closeWaitScreen(bool showPrevious)
    {
        if (_canvasesWaiting > 0) _canvasesWaiting--;
        if (_pleaseWaitCanvas.enabled && _canvasesWaiting == 0)
        {
            _pleaseWaitCanvas.enabled = false;
            if (showPrevious) showPreviousScreen();
        }
    }

    public void onBackFromErrorScreen()
    {
        _errorCanvas.enabled = false;
        showPreviousScreen();
    }
}