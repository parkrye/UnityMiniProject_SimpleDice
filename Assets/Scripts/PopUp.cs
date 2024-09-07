using System.Collections;
using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    [SerializeField] private GameObject noticeUI = default;
    private TextMeshProUGUI noticeText = default;
    private ExtendedButton noticeButton = default;
    
    private IEnumerator noticeCoroutine = default;

    private void Awake()
    {
        noticeText = noticeUI.GetComponentInChildren<TextMeshProUGUI>();
        noticeButton = noticeUI.GetComponent<ExtendedButton>();
        noticeButton.onClick.AddListener(() => noticeUI.SetActive(false));
    }

    public void ShowNotice(string message, float time = 0f)
    {
        noticeText.text = message;
        noticeUI.SetActive(true);

        if (time > 0f)
        {
            if (noticeCoroutine != null)
                StopCoroutine(noticeCoroutine);
            StartCoroutine(noticeCoroutine = NoticeDisappearRoutine(time));
        }
    }

    private IEnumerator NoticeDisappearRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        noticeUI.SetActive(false);
    }
}
