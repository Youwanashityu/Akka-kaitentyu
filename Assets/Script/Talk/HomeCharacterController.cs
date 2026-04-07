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
    [SerializeField] private PresentTalkController _presentTalkController;
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
        SetCharacter(_initialCharacter);
    }

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnCharacterSwitchButtonClicked()
    {
        // 会話中はキャラ切り替えをブロック
        if (_homeTalkController.IsTalking) return;
        _selectPopup.Show(_currentCharacter, OnCharacterSelected);
    }

    private void OnCharacterSelected(CharacterScriptable character)
    {
        SetCharacter(character);
    }

    // -------------------------------------------------------
    // キャラクター切り替え
    // -------------------------------------------------------

    private void SetCharacter(CharacterScriptable character)
    {
        if (character == null) return;
        Debug.Log($"[HomeCharacterController] SetCharacter: {character.CharacterName} DefaultSprite: {character.DefaultSprite?.name}");


        _currentCharacter = character;

        _characterSoundPlayer.Setup(
            character.GetImageDict(),
            character.GetVoiceDict()
        );

        _talkController.Initialize(character.DefaultSprite, character.MiniDefaultSprite);

        _homeTalkController.SetCharacter(character.CharacterName, character.IsTutorialCharacter);
        _presentTalkController.SetCharacter(character.CharacterName);

        Debug.Log($"[HomeCharacterController] キャラクター切り替え: {character.CharacterName}");
    }
}