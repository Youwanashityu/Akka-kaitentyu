using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PickUp選択ポップアップ内のキャラクターボタン1つ分。
/// キャラ画像・名前の表示と選択状態の視覚的フィードバックを担います。
/// </summary>
public class PickUpCharacterButton : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _characterImage;
    [SerializeField] private TMP_Text _nameText;

    [Header("選択状態の見た目")]
    [SerializeField] private GameObject _selectedFrame; // 選択中に表示する枠など

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    public GachaItem Character { get; private set; }

    // -------------------------------------------------------
    // セットアップ
    // -------------------------------------------------------

    /// <summary>
    /// キャラクターデータとコールバックを設定します。
    /// </summary>
    public void Setup(GachaItem character, Action<GachaItem> onSelected)
    {
        Character = character;
        _characterImage.sprite = character.Icon;
        _nameText.text = character.DisplayName;
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