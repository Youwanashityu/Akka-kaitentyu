using UnityEngine;

/// <summary>
/// コインの所持数を管理するマネージャー。
/// シングルトンとして動作します。
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    // -------------------------------------------------------
    // インスペクター設定
    // -------------------------------------------------------

    [Header("コイン初期所持数")]
    [SerializeField] private int _initialCoinCount = 6666666;

    // -------------------------------------------------------
    // 内部状態
    // -------------------------------------------------------

    private int _coinCount;

    public int CoinCount => _coinCount;

    // -------------------------------------------------------
    // ライフサイクル
    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _coinCount = _initialCoinCount;
    }

    // -------------------------------------------------------
    // コイン操作
    // -------------------------------------------------------

    /// <summary>
    /// コインを追加します。
    /// </summary>
    public void AddCoin(int amount)
    {
        _coinCount += amount;
        Debug.Log($"[CoinManager] コイン追加: {amount} → 残り{_coinCount}");
    }

    /// <summary>
    /// コインを消費します。足りない場合はfalseを返します。
    /// </summary>
    public bool TrySpendCoin(int amount)
    {
        if (_coinCount < amount) return false;
        _coinCount -= amount;
        Debug.Log($"[CoinManager] コイン消費: {amount} → 残り{_coinCount}");
        return true;
    }
}