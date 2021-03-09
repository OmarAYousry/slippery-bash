using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen: MonoBehaviour
{
    public static TitleScreen Instance { get; private set; }

    public AnimationCurve sCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    [Header("Presents")]
    [SerializeField] private GameObject presentsParent = null;
    [SerializeField] private TextMeshProUGUI iceGuys = null;
    public float presentsDuration = 3;
    public float presentsFadeDuration = 1;

    [Header("Title")]
    [SerializeField] private GameObject titleParent = null;
    [SerializeField] private ComputeShader shader = null;
    [SerializeField] private RawImage mask = null;
    [Range(0f, 1f)] public float dissolve = 0;
    public float size = 1;
    public float dissolveDuration = 2;
    public Texture dissolvePattern;

    private RenderTexture render;
    private int kernel;

    [Header("Load game")]
    public string gameScene = "MainScene";
    public float minLoadDuration = 1;
    public float loadDissolveDuration = 1;

    private Color toColor;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        render = new RenderTexture(1024, 1024, 1);
        render.enableRandomWrite = true;
        render.Create();

        kernel = shader.FindKernel("CSMain");
        shader.SetTexture(kernel, "Result", render);
        shader.SetTexture(kernel, "Pattern", dissolvePattern);

        mask.texture = render;
    }

    private void Start()
    {
        StartCoroutine(Presenting());
    }


    //---------------------------------------------------------------------------------------------//
    private void SetDissolve(float dissolve)
    {
        shader.SetFloat("Size", size);
        shader.SetFloat("Dissolve", dissolve);
        shader.Dispatch(kernel, 1024 / 8, 1024 / 8, 1);
    }


    //---------------------------------------------------------------------------------------------//

    private IEnumerator Presenting()
    {
        presentsParent.SetActive(true);
        titleParent.SetActive(true);

        // text fade in
        float timer = Time.time;
        float progress = 0;
        while(progress < 1)
        {
            iceGuys.color = Color.Lerp(Color.clear, Color.white, sCurve.Evaluate(progress));
            progress = (Time.time - timer) / presentsFadeDuration;
            yield return null;
        }
        iceGuys.color = Color.white;

        // wait while showing text
        yield return new WaitForSeconds(presentsDuration);

        // text fade out
        timer = Time.time;
        progress = 0;
        while(progress < 1)
        {
            iceGuys.color = Color.Lerp(Color.white, Color.clear, sCurve.Evaluate(progress));
            progress = (Time.time - timer) / presentsFadeDuration;
            yield return null;
        }
        iceGuys.color = Color.clear;

        // title dissolve
        yield return TitleDissolve(true, dissolveDuration);

        // wait before instruction
        yield return new WaitForSeconds(2f);
        presentsParent.SetActive(false);
        yield return LoadGameScene();

        yield return TitleDissolve(false, dissolveDuration);

        gameObject.SetActive(false);
    }

    private IEnumerator TitleDissolve(bool appear, float dissolveDuration)
    {
        if(appear)
            titleParent.SetActive(true);
        SetDissolve(appear ? 1 : 0);
        float timer = Time.time;
        float progress = 0;
        while(progress < 1)
        {
            SetDissolve(appear ? 1 - sCurve.Evaluate(progress) : sCurve.Evaluate(progress));
            progress = (Time.time - timer) / dissolveDuration;
            Debug.Log(appear ? 1 - sCurve.Evaluate(progress) : sCurve.Evaluate(progress));
            yield return null;
        }
        SetDissolve(appear ? 0 : 1);
        if(!appear)
            titleParent.SetActive(false);
    }

    public void LoadGame()
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadingGame());
    }

    private IEnumerator LoadingGame()
    {
        yield return TitleDissolve(true, loadDissolveDuration);
        yield return LoadGameScene();
        yield return TitleDissolve(false, loadDissolveDuration);
        gameObject.SetActive(false);
    }

    private IEnumerator LoadGameScene()
    {
        float endTime = Time.realtimeSinceStartup + minLoadDuration;

        AsyncOperation op = SceneManager.LoadSceneAsync(gameScene);
        while(!op.isDone)
            yield return null;

        while(Time.realtimeSinceStartup < endTime)
            yield return null;
    }
}
