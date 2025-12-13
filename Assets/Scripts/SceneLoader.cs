using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : SingletonBase<SceneLoader>
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSecond;
    [SerializeField] private float showingLogoSecond;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private async UniTask FadeIn()
    {
        // Alphaを1から0に徐々に変更
        SetImageAlpha(1);
        float currentAlpha = fadeImage.color.a;
        float timeElapsed = 0;

        while (timeElapsed < fadeSecond)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(currentAlpha, 0f, timeElapsed / fadeSecond);
            SetImageAlpha(alpha);
            await UniTask.Yield();
        }
        SetImageAlpha(0f);
    }

    private async UniTask FadeOut()
    {
        // Alphaを0から1に徐々に変更
        SetImageAlpha(0);
        float currentAlpha = fadeImage.color.a;
        float timeElapsed = 0;

        while (timeElapsed < fadeSecond)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(currentAlpha, 1f, timeElapsed / fadeSecond);
            SetImageAlpha(alpha);
            await UniTask.Yield();
        }
        SetImageAlpha(1f);
    }

    private void SetImageAlpha(float alpha)
    {
        var color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    public async void LoadSceneWithFade(string sceneName)
    {
        await LoadSceneAsync(sceneName);
    }

    [ContextMenu("DebugScenLoad")]
    public void DebugLoadScene()
    {
        LoadSceneWithFade("GameScene");
    }

    private async UniTask LoadSceneAsync(string sceneName)
    {
        // フェードアウト開始
        await FadeOut();

        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            await UniTask.Yield();
        }

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            await UniTask.Yield();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(showingLogoSecond));

        await FadeIn();
    }
}
