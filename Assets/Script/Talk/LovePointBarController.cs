using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 好感度バーの表示を管理するコントローラー。
/// Imageコンポーネントを使ってバーの長さを制御します。
/// FillImageのImageTypeをFilled・FillMethodをHorizontalに設定してください。
/// </summary>
public class LovePointBarController : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("UI")]
    [Tooltip("ImageType:Filled、FillMethod:Horizontalに設定したImage")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _levelText;

    [Header("ゲーム設定")]
    [SerializeField] private GameSettingScriptable _gameSetting;

    // -------------------------------------------------------
    // 表示更新
    // -------------------------------------------------------

    /// <summary>
    /// 好感度ポイントに応じてバーとレベルテキストを更新します。
    /// </summary>
    public void UpdateBar(int lovePoint)
    {
        var level = _gameSetting.GetLikeLevel(lovePoint);
        var (min, max) = GetLevelRange(level);

        _fillImage.fillAmount = Mathf.InverseLerp(min, max, lovePoint);
        _levelText.text = $"Lv.{level}";
    }

    // -------------------------------------------------------
    // ヘルパー
    // -------------------------------------------------------

    /// <summary>
    /// レベルの最小・最大ポイントを返します。
    /// </summary>
    private (int min, int max) GetLevelRange(int level)
    {
        return level switch
        {
            1 => (0, _gameSetting.Like2Threshold),
            2 => (_gameSetting.Like2Threshold, _gameSetting.Like3Threshold),
            3 => (_gameSetting.Like3Threshold, _gameSetting.Like4Threshold),
            _ => (_gameSetting.Like3Threshold, _gameSetting.Like4Threshold),
        };
    }
}