using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラ切り替えポップアップの1キャラ分のボタン。
/// キャラクターアイコンと名前を表示します。
/// </summary>
public class CharacterSelectButton : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;

    [Header("選択状態の見た目")]
    [SerializeField] private GameObject _selectedFrame;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    public CharacterScriptable Character { get; private set; }

    // -------------------------------------------------------
    // セットアップ
    // -------------------------------------------------------

    /// <summary>
    /// キャラクターデータとコールバックをセットします。
    /// </summary>
    public void Setup(CharacterScriptable character, Action<CharacterScriptable> onSelected)
    {
        Character = character;
        _icon.sprite = character.DefaultSprite;
        _nameText.text = character.CharacterName;
        _button.onClick.AddListener(() => onSelected(character));
    }

    /// <summary>
    /// 選択状態の見た目を更新します。
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (_selectedFrame != null)
            _selectedFrame.SetActive(isSelected);
    }
}