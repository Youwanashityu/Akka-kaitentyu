using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 好感度がMAXになったときに表示するエンディングポップアップ。
/// タイトルに戻るボタンとゲームを続けるボタンを表示します。
/// </summary>
public class EndingPopupController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _backToTitleButton;
    [SerializeField] private Button _continueButton;

    [Header("タイトルシーン名")]
    [SerializeField] private string _titleSceneName = "Title";

    [Header("エンディングメッセージ")]
    [TextArea]
    [SerializeField] private string _endingMessage = "あなたは彼と仲良くなりました！\nゲームはこれでおしまいです。";

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _backToTitleButton.onClick.AddListener(OnBackToTitleClicked);
        _continueButton.onClick.AddListener(OnContinueClicked);
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

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnBackToTitleClicked()
    {
        SceneManager.LoadScene(_titleSceneName);
    }

    private void OnContinueClicked()
    {
        Hide();
    }
}