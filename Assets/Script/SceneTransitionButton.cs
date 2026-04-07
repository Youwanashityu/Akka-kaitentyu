using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ボタンを押すと指定したシーンに遷移する汎用コンポーネント。
/// シーン移動前にCancellationTokenをキャンセルして会話を中断します。
/// </summary>
public class SceneTransitionButton : MonoBehaviour
{
    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("遷移先シーン名")]
    [SerializeField] private string _sceneName;

    [Header("ボタン")]
    [SerializeField] private Button _button;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Start()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    // -------------------------------------------------------
    // ボタンイベント
    // -------------------------------------------------------

    private void OnButtonClicked()
    {
        if (string.IsNullOrEmpty(_sceneName))
        {
            Debug.LogWarning("[SceneTransitionButton] シーン名が設定されていません。");
            return;
        }

        // シーン上の全MonoBehaviourを無効化して非同期処理を止める
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            mb.StopAllCoroutines();
        }

        SceneManager.LoadScene(_sceneName);
    }
}