using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラ切り替えポップアップのUIを管理するコントローラー。
/// チュートリアルキャラ＋所持SSRキャラの一覧を表示して選択を受け付けます。
/// </summary>
public class CharacterSelectPopupController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Transform _buttonListParent;
    [SerializeField] private CharacterSelectButton _buttonPrefab;

    [Header("チュートリアルキャラクター（最初から選べる2人）")]
    [SerializeField] private CharacterScriptable[] _tutorialCharacters;

    [Header("ガチャキャラクター（全SSRキャラ）")]
    [SerializeField] private CharacterScriptable[] _gachaCharacters;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private Action<CharacterScriptable> _onSelected;
    private CharacterScriptable _currentSelected;
    private readonly List<CharacterSelectButton> _buttons = new();

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _closeButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // 表示制御
    // -------------------------------------------------------

    /// <summary>
    /// キャラ選択ポップアップを表示します。
    /// </summary>
    public void Show(CharacterScriptable currentSelected, Action<CharacterScriptable> onSelected)
    {
        _onSelected = onSelected;
        _currentSelected = currentSelected;
        RefreshButtonList();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // ボタンリスト更新
    // -------------------------------------------------------

    private void RefreshButtonList()
    {
        foreach (var btn in _buttons)
            Destroy(btn.gameObject);
        _buttons.Clear();

        // チュートリアルキャラは常に表示
        foreach (var chara in _tutorialCharacters)
        {
            AddButton(chara);
        }

        // ガチャキャラは所持しているものだけ表示
        foreach (var chara in _gachaCharacters)
        {
            if (chara.GachaItem == null) continue;
            if (!IsOwned(chara.GachaItem)) continue;
            AddButton(chara);
        }
    }

    private void AddButton(CharacterScriptable chara)
    {
        var btn = Instantiate(_buttonPrefab, _buttonListParent);
        btn.Setup(chara, OnCharacterSelected);
        btn.SetSelected(chara == _currentSelected);
        _buttons.Add(btn);
    }

    /// <summary>
    /// GachaManagerのインベントリにSSRキャラが入っているか確認します。
    /// </summary>
    private bool IsOwned(GachaItem item)
    {
        return GachaManager.Instance.Inventory.ContainsKey(item);
    }

    // -------------------------------------------------------
    // 選択処理
    // -------------------------------------------------------

    private void OnCharacterSelected(CharacterScriptable character)
    {
        _currentSelected = character;
        UpdateSelectionVisual();
        Hide();
        _onSelected?.Invoke(character);
    }

    private void UpdateSelectionVisual()
    {
        foreach (var btn in _buttons)
        {
            btn.SetSelected(btn.Character == _currentSelected);
        }
    }
}