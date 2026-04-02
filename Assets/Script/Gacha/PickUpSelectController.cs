using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PickUpキャラ選択ポップアップのUIを管理するコントローラー。
/// キャラクター画像と名前のボタンを表示し、選択されたキャラをGachaManagerに渡します。
/// </summary>
public class PickUpSelectController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("キャラクター選択ボタン（最大2体）")]
    [SerializeField] private PickUpCharacterButton[] _characterButtons;

    [Header("閉じるボタン")]
    [SerializeField] private Button _closeButton;

    [Header("PickUp対象キャラクター（GachaManagerと同じ順番で設定）")]
    [SerializeField] private GachaItem[] _pickUpCharacters;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _closeButton.onClick.AddListener(Hide);

        for (int i = 0; i < _characterButtons.Length && i < _pickUpCharacters.Length; i++)
        {
            var character = _pickUpCharacters[i];
            var button = _characterButtons[i];
            button.Setup(character, OnCharacterSelected);
        }

        UpdateSelectionVisual();
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // 表示制御
    // -------------------------------------------------------

    /// <summary>
    /// PickUp選択ポップアップを表示します。
    /// </summary>
    public void Show()
    {
        UpdateSelectionVisual();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------
    // 選択処理
    // -------------------------------------------------------

    private void OnCharacterSelected(GachaItem character)
    {
        // 同じキャラをもう一度押したら選択解除
        if (GachaManager.Instance.SelectedPickUp == character)
        {
            GachaManager.Instance.SelectPickUp(null);
        }
        else
        {
            GachaManager.Instance.SelectPickUp(character);
        }

        UpdateSelectionVisual();
        Hide();
    }

    /// <summary>
    /// 現在の選択状態に合わせてボタンの見た目を更新します。
    /// </summary>
    private void UpdateSelectionVisual()
    {
        foreach (var button in _characterButtons)
        {
            button.SetSelected(button.Character == GachaManager.Instance.SelectedPickUp);
        }
    }
}