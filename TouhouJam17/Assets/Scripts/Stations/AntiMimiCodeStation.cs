using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AntiMimiCodeStation : BaseStation
{
    [SerializeField] private float randomCheckInterval = 20f;
    [SerializeField] private float activationChance = 0.5f;

    [SerializeField] private float timeBeforeExploding = 15;
    [SerializeField] private int numberLength = 10;
    [SerializeField] private StationKeypadButton returnKey;
    [SerializeField] private StationKeypadButton[] numberKeys = new StationKeypadButton[10];
    [SerializeField] private List<int> playerNumbers;
    [SerializeField] private List<int> goalNumbers;
    [SerializeField] private TextMeshProUGUI goalNumberText;
    [SerializeField] private TextMeshProUGUI playerNumberText;
    [SerializeField] private Image fillImage;
    private float passedTime;
    private Coroutine myCoroutine;

    private void Start()
    {
        for (int i = 0; i < numberKeys.Length; i++)
        {
            int number = i;
            numberKeys[i].SetSymbol((number).ToString());
            numberKeys[i].OnActivated += () => AddNumber(number);
            numberKeys[i].OnActivated += RefreshTexts;
        }

        returnKey.OnActivated += RemoveNumber;
        returnKey.OnActivated += RefreshTexts;

        RefreshTexts();

        DisableKeypad();
    }

    private void Update()
    {
        if (myCoroutine != null) return;

        passedTime += Time.deltaTime;
        if (passedTime >= randomCheckInterval)
        {
            float random = Random.Range(0f, 1f);
            if (random <= activationChance)
            {
                Activate();
            }
        }
    }

    private void EnableKeypad()
    {
        for (int i = 0; i < numberKeys.Length; i++)
        {
            numberKeys[i].SetEnabled();
        }

        returnKey.SetEnabled();
    }

    private void DisableKeypad()
    {
        for (int i = 0; i < numberKeys.Length; i++)
        {
            numberKeys[i].SetDisabled();
        }

        returnKey.SetDisabled();
    }

    private void RefreshTexts()
    {
        goalNumberText.text = string.Empty;
        for (int i = 0; i < goalNumbers.Count; i++)
        {
            goalNumberText.text += goalNumbers[i].ToString();
            if (i != goalNumbers.Count - 1)
            {
                goalNumberText.text += "-";
            }
        }

        playerNumberText.text = string.Empty;
        for (int i = 0; i < playerNumbers.Count; i++)
        {
            playerNumberText.text += playerNumbers[i].ToString();
            if (i != playerNumbers.Count - 1)
            {
                playerNumberText.text += "-";
            }
        }
    }

    private void Activate()
    {
        if (myCoroutine == null)
        {
            myCoroutine = StartCoroutine(NumberInputMinigame());
        }
    }

    private IEnumerator NumberInputMinigame()
    {
        playerNumbers.Clear();
        goalNumbers.Clear();
        for (int i = 0; i < numberLength; i++)
        {
            goalNumbers.Add(Random.Range(0, 10));
        }

        AlarmLight.PlayAlarm(AlarmType.Mimi);
        
        goalNumberText.gameObject.SetActive(true);
        playerNumberText.gameObject.SetActive(true);
        EnableKeypad();

        RefreshTexts();
        float time = 0;
        while (time < timeBeforeExploding && CompareNumberLists() == false)
        {
            time += Time.deltaTime;
            fillImage.fillAmount = 1 - (time / timeBeforeExploding);
            yield return null;
        }

        AlarmLight.ForceStopAlarm();
        DisableKeypad();
        if (time >= timeBeforeExploding)
        {
            PlayerController.Instance.JumpScaare(EMonsterType.Mima);
            //Play end sequence (Alarms blaring, Mimi ICBM approaching before exploding the player and cutting to black)
            //send to main menu
        }
        else
        {
            fillImage.fillAmount = 1;
            goalNumberText.gameObject.SetActive(false);
            playerNumberText.gameObject.SetActive(false);
            myCoroutine = null;
            passedTime = -20f;
        }
    }

    private void AddNumber(int aNumber)
    {
        playerNumbers.Add(aNumber);
    }

    private void RemoveNumber()
    {
        if (playerNumbers.Count > 0)
        {
            playerNumbers.RemoveAt(playerNumbers.Count - 1);
        }
    }

    private bool CompareNumberLists()
    {
        if (playerNumbers.Count != goalNumbers.Count) return false;

        for (int i = 0; i < playerNumbers.Count; i++)
        {
            if (playerNumbers[i] != goalNumbers[i]) return false;
        }

        return true;
    }
}