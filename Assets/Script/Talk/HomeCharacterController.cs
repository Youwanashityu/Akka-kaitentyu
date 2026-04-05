using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ホーム画面のキャラクター表示と切り替えボタンを管理するコントローラー。
/// </summary>
public class HomeCharacterController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("キャラ切り替えボタン")]
    [SerializeField] private Button _characterSwitchButton;

    [Header("ポップアップ")]
    [SerializeField] private CharacterSelectPopupController _selectPopup;

    [Header("会話・サウンド")]
    [SerializeField] private HomeTalkController _homeTalkController;
    [SerializeField] private CharacterSoundPlayer _characterSoundPlayer;
    [SerializeField] private TalkController _talkController;

    [Header("初期キャラクター（デフォルト表示キャラ）")]
    [SerializeField] private CharacterScriptable _initialCharacter;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private CharacterScriptable _currentCharacter;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _characterSwitchButton.onClick.AddListener(OnCharacterSwitchButtonClicked);

        // 初期キャラクターを設定
        SetCharacter(_initialCharacter);
    }

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnCharacterSwitchButtonClicked()
    {
        _selectPopup.Show(_currentCharacter, OnCharacterSelected);
    }

    private void OnCharacterSelected(CharacterScriptable character)
    {
        SetCharacter(character);
    }

    // -------------------------------------------------------
    // キャラクター切り替え
    // -------------------------------------------------------

    /// <summary>
    /// ホーム画面に表示するキャラクターを切り替えます。
    /// </summary>
    private void SetCharacter(CharacterScriptable character)
    {
        if (character == null) return;

        _currentCharacter = character;

        // 画像・ボイス辞書を更新
        _characterSoundPlayer.Setup(
            character.GetImageDict(),
            character.GetVoiceDict()
        );

        // デフォルト画像をセット
        _talkController.Initialize(character.DefaultSprite, character.MiniDefaultSprite);

        // CSVファイル名をキャラクター名で更新
        _homeTalkController.SetCharacter(character.CharacterName);

        Debug.Log($"[HomeCharacterController] キャラクター切り替え: {character.CharacterName}");
    }
}