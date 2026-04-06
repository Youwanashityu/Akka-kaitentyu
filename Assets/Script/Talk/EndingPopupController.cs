using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 好感度がMAXになったときに表示するエンディングポップアップ。
/// 閉じるボタンでポップアップを消してゲームを続けられます。
/// </summary>
public class EndingPopupController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _closeButton;

    [Header("エンディングメッセージ")]
    [TextArea]
    [SerializeField] private string _endingMessage = "あなたは彼と仲良くなりました！\nゲームはこれでおしまいです。";

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _closeButton.onClick.AddListener(Hide);
        _messageText.text = _endingMessage;
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // 表示制御
    // -------------------------------------------------------

    /// <summary>
    /// エンディングポップアップを表示します。
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}