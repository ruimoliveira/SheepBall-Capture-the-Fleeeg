using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashFEUP : MonoBehaviour
{
    public Image feupLogo;

    IEnumerator Start()
    {
        feupLogo.canvasRenderer.SetAlpha(0.0f);
        FadeIn();
        yield return new WaitForSeconds(2.5f);
        FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }

    void FadeIn()
    {
        feupLogo.CrossFadeAlpha(1.0f, 0.5f, false);
    }

    void FadeOut()
    {
        feupLogo.CrossFadeAlpha(0.0f, 0.5f, false);

    }
}
