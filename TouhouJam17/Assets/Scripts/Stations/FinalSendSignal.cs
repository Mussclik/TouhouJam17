using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalSendSignal : BaseStation
{
    public event Action OnFinishGame = () => { };

    [SerializeField] private TextMeshProUGUI remainingSignalsText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image progressImageFill;
    [SerializeField] private StationButton sendSignalButton;
    [SerializeField] private StationButton finishGameButton;

    [SerializeField] public int signalsToWin = 20;
    private const string ProgressTextPrefix = "Progress: ";
    private const string RemainingSignalsTextPrefix = "Dream Signals: ";

    private void Start()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        if (processedSignals >= 20)
        {
            finishGameButton.SetEnabled();
        }
        else
        {
            finishGameButton.SetDisabled();
        }

        if (unprocessedSignals >= 1)
        {
            sendSignalButton.SetEnabled();
        }
        else
        {
            sendSignalButton.SetDisabled();
        }
    }

    private void UpdateVisuals()
    {
        float progress = (float)processedSignals / (float)signalsToWin;
        progress = Mathf.Clamp01(progress);
        string newProgressText = ((int)(progress * 100f)).ToString();
        progressText.text = $"{ProgressTextPrefix} {newProgressText}%";
        remainingSignalsText.text = $"{RemainingSignalsTextPrefix} {processedSignals}/{signalsToWin}";
        progressImageFill.fillAmount = progress;
    }

    private void OnEnable()
    {
        sendSignalButton.OnActivated += ProcessSignal;
        sendSignalButton.OnActivated += UpdateVisuals;
        finishGameButton.OnActivated += WinGame;
    }

    private void OnDisable()
    {
        sendSignalButton.OnActivated -= ProcessSignal;
        sendSignalButton.OnActivated -= UpdateVisuals;
        finishGameButton.OnActivated -= WinGame;
    }

    private void WinGame()
    {
        //Disable all enemy scripts,
        //disable player inputs,
        //fade to black
        Debug.Log("YouWon! :3c");
    }
}