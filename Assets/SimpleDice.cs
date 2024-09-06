using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleDice : MonoBehaviour
{
    [SerializeField] private RectTransform rect;

    [Space()]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TMP_InputField diceInputField;
    [SerializeField] private TMP_InputField descInputField;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI descRealText;

    [Space()]
    [SerializeField] private GameObject textTemplate;
    [SerializeField] private RectTransform textContent;
    [SerializeField] private RectTransform upperInputField;

    [Space()]
    [SerializeField] private GameObject slotTemplate;
    [SerializeField] private RectTransform slotContent;

    [Space()]
    [SerializeField] private Color32[] colors;
    [SerializeField] private AudioSource audioSource;

    private List<GameObject> texts = new List<GameObject>();
    private int textIndex, slotIndex, textLine;

    private LogData logData;

    private bool isUpperInputOpened;

    private void Awake()
    {
        logData = DataHelper.LoadData();

        descInputField.onValueChanged.AddListener(ResizeDescInputFieldSize);
    }

    private void Start()
    {
        var safeArea = Screen.safeArea;
        var minAnchor = safeArea.position;
        var maxAnchor = minAnchor + safeArea.size;
        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;
        rect.anchorMin = minAnchor;
        rect.anchorMax = maxAnchor;

        int[] diceList = new int[] { 2, 4, 6, 8, 10, 12, 20, 100 };
        foreach (int i in diceList)
        {
            int index = i;
            GameObject instant = Instantiate(slotTemplate, slotContent);
            instant.SetActive(true);
            instant.transform.GetChild(1).GetComponent<TMP_Text>().text = $"{index}";
            instant.transform.GetChild(2).GetComponent<ExtendedButton>().onClick.AddListener(() => DiceRoll(index));
            
            if (slotIndex >= colors.Length)
                slotIndex = 0;
            instant.transform.GetChild(0).GetComponent<Image>().color = colors[slotIndex];
            slotIndex++;
        }

        foreach (var datum in logData.diceData)
        {
            GameObject instant = SetDataToScroll(datum);
            texts.Add(instant);
            int position = texts.Count - 6 < 0 ? 0 : texts.Count - 6;
            textContent.anchoredPosition = new Vector3(0f, position * 250f, 0f);
        }

        countText.text = $"{texts.Count}";
    }

    private void Update()
    {
        timeText.text = DateTime.Now.ToString("HH:mm:ss");
    }

    private void OnApplicationQuit()
    {
        DataHelper.SaveData(logData);
    }

    public void DiceRoll(TMP_InputField field)
    {
        if (field.text.Length == 0)
            return;

        int.TryParse(field.text, out int dice);
        DiceRoll(dice);
    }

    public void OnClickedDescButton()
    {
        if (isUpperInputOpened)
        {
            descInputField.text = string.Empty;
            upperInputField.sizeDelta = Vector2.zero;
            upperInputField.anchoredPosition = new Vector2(upperInputField.anchoredPosition.x, 0f);
            isUpperInputOpened = false;
            return;
        }

        upperInputField.sizeDelta = Vector2.up * 180f;
        upperInputField.anchoredPosition = new Vector2(upperInputField.anchoredPosition.x, TouchScreenKeyboard.area.height);
        isUpperInputOpened = true;
    }

    private void DiceRoll(int dice)
    {
        audioSource.Play();
        int result = UnityEngine.Random.Range(1, dice + 1);

        DiceDatum datum = new DiceDatum()
        {
            index = logData.diceData.Count,
            title = descInputField.text,
            range = dice,
            result = result,
        };

        GameObject instant = SetDataToScroll(datum);
        texts.Add(instant);
        int position = texts.Count - 6 < 0 ? 0 : texts.Count - 6;
        textContent.anchoredPosition = new Vector3(0f, position * 250f, 0f);

        logData.diceData.Add(datum);

        countText.text = $"{texts.Count}";

        upperInputField.sizeDelta = Vector2.zero;
        diceInputField.text = string.Empty;
        descInputField.text = string.Empty;
        isUpperInputOpened = false;
    }

    private GameObject SetDataToScroll(DiceDatum datum)
    {
        GameObject instant = Instantiate(textTemplate, textContent);
        instant.SetActive(true);

        var textObject = instant.transform.GetChild(1);
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = $"{datum.result} (1 ~ {datum.range}) : {descInputField.text}";

        var buttonObject = instant.transform.GetChild(2);
        var button = buttonObject.GetComponent<ExtendedButton>();
        button.LongTouchEventListener = (b) => OnTouchResultText(b, instant, datum);

        var rect = instant.GetComponent<RectTransform>();
        StartCoroutine(SetTextHeightRoutine(rect, text));

        if (textIndex >= colors.Length)
            textIndex = 0;
        instant.transform.GetChild(0).GetComponent<Image>().color = colors[textIndex];
        textIndex++;

        return instant;
    }

    private IEnumerator SetTextHeightRoutine(RectTransform rect, TextMeshProUGUI text)
    {
        yield return null;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, Mathf.Max(150f, text.textInfo.lineCount * 150f));
    }

    private void OnTouchResultText(bool isLongTouch, GameObject self, DiceDatum datum)
    {
        if (isLongTouch)
        {
            DeleteText(self);
            logData.diceData.Remove(datum);
            return;
        }
    }

    private void DeleteText(GameObject self)
    {
        texts.Remove(self);
        Destroy(self);
        countText.text = $"{texts.Count}";
    }

    private void ResizeDescInputFieldSize(string text)
    {
        descRealText.text = text;
        var textLine = descRealText.textInfo.lineCount;
        if (this.textLine != textLine)
        {
            this.textLine = textLine;
            upperInputField.sizeDelta = Vector2.up * MathF.Max(180f, textLine * 150f);
        }
    }
}
