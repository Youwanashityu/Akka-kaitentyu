using UnityEngine;

/// <summary>
/// ゲーム全体の共通設定をまとめたScriptableObject。
/// 好感度の閾値・上昇量、プレゼントの好感度上昇量を管理します。
/// Assets を右クリック → Data/GameSetting で作成できます。
/// </summary>
[CreateAssetMenu(fileName = "GameSetting", menuName = "Data/GameSetting")]
public class GameSettingScriptable : ScriptableObject
{
    // -------------------------------------------------------
    // 好感度設定
    // -------------------------------------------------------

    [Header("好感度閾値")]
    [Tooltip("この値以上でLike2になる")]
    [SerializeField] private int _like2Threshold = 100;

    [Tooltip("この値以上でLike3になる")]
    [SerializeField] private int _like3Threshold = 300;

    [Tooltip("この値以上でMAX（Like4）になる＝エンディング")]
    [SerializeField] private int _like4Threshold = 600;

    [Header("会話の好感度上昇量")]
    [Tooltip("会話1回あたりの好感度上昇量")]
    [SerializeField] private int _talkLoveAmount = 10;

    // -------------------------------------------------------
    // プレゼント設定
    // -------------------------------------------------------

    [Header("プレゼントの好感度上昇量")]
    [Tooltip("GachaItemとそれを渡したときの好感度上昇量の対応表")]
    [SerializeField] private PresentLoveSetting[] _presentSettings;

    // -------------------------------------------------------
    // プロパティ
    // -------------------------------------------------------

    public int Like2Threshold => _like2Threshold;
    public int Like3Threshold => _like3Threshold;
    public int Like4Threshold => _like4Threshold;
    public int TalkLoveAmount => _talkLoveAmount;

    // -------------------------------------------------------
    // メソッド
    // -------------------------------------------------------

    /// <summary>
    /// 好感度の値からLikeレベル（1〜3）を返します。
    /// MAXに達している場合は3を返します。
    /// </summary>
    public int GetLikeLevel(int lovePoint)
    {
        if (lovePoint >= _like3Threshold) return 3;
        if (lovePoint >= _like2Threshold) return 2;
        return 1;
    }

    /// <summary>
    /// MAXに達しているか返します。
    /// </summary>
    public bool IsMax(int lovePoint) => lovePoint >= _like4Threshold;

    /// <summary>
    /// 指定したGachaItemをプレゼントしたときの好感度上昇量を返します。
    /// 設定がない場合は0を返します。
    /// </summary>
    public int GetPresentLoveAmount(GachaItem item)
    {
        foreach (var setting in _presentSettings)
        {
            if (setting.Item == item) return setting.LoveAmount;
        }
        return 0;
    }
}

/// <summary>
/// プレゼントアイテムと好感度上昇量のペア。
/// </summary>
[System.Serializable]
public class PresentLoveSetting
{
    [SerializeField] private GachaItem _item;
    [SerializeField] private int _loveAmount;

    public GachaItem Item => _item;
    public int LoveAmount => _loveAmount;
}