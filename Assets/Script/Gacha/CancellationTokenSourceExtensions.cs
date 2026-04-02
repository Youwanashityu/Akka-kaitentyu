using System.Threading;

/// <summary>
/// CancellationTokenSourceの拡張メソッド集。
/// UniTaskと組み合わせて使います。
/// </summary>
public static class CancellationTokenSourceExtensions
{
    /// <summary>
    /// CancellationTokenSourceをキャンセル・破棄してから新しいものを返します。
    /// </summary>
    public static CancellationTokenSource Reset(this CancellationTokenSource cts)
    {
        cts?.Cancel();
        cts?.Dispose();
        return new CancellationTokenSource();
    }

    /// <summary>
    /// CancellationTokenSourceをキャンセル・破棄してnullを返します。
    /// OnDestroy時に呼びます。
    /// </summary>
    public static CancellationTokenSource Clear(this CancellationTokenSource cts)
    {
        cts?.Cancel();
        cts?.Dispose();
        return null;
    }

    /// <summary>
    /// 2つのCancellationTokenをリンクした新しいTokenを返します。
    /// どちらかがキャンセルされると両方キャンセルされます。
    /// </summary>
    public static CancellationToken LinkedToken(this CancellationTokenSource cts, CancellationToken other)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(cts.Token, other).Token;
    }
}