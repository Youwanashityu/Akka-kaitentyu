using UnityEngine;

public class LinkOpener : MonoBehaviour
{
    public string url;

    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
