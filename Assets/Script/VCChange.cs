using UnityEngine;
/// <summary>
/// ボタンやイベントに合わせてVCを再生するコンポーネント。
/// インスペクターでVC名を指定し、PlayVC()を呼び出すだけで再生できます。
/// </summary>
public class VCChange : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------
    [Header("再生するVC名（SoundManagerのVC一覧に登録した名前）")]
    [SerializeField] private string vcName;
    // -------------------------------------------------------
    // VC再生
    // -------------------------------------------------------
    /// <summary>
    /// インスペクターで指定したVCをSoundManager経由で再生します。
    /// ボタンのOnClickやAnimationEventから呼び出せます。
    /// </summary>
    public void PlayVC()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[VCChange] SoundManager が見つかりません。");
            return;
        }
        Debug.Log($"[VCChange] PlayVC呼び出し: {vcName}");
        SoundManager.Instance.PlayVC(vcName);
    }
}