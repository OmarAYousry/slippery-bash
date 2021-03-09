using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
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
    [Range(0f,1f)] public float dissolve = 0;
    public float size = 1;
    public float dissolveDuration = 2;
    public Texture dissolvePattern;

    private RenderTexture render;
    private int kernel;

    [Header("Instruction")]
    [SerializeField] private TextMeshProUGUI instruction = null;
    public float instructionOffset = 2;
    public float breatheSpeed = 1;

    private Color toColor;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
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

    private void Update()
    {
        if(instruction.gameObject.activeInHierarchy)
        {
            InstructionsBreathe();
            CheckForInput();
        }
    }


    //---------------------------------------------------------------------------------------------//
    private void SetDissolve(float dissolve)
    {
        shader.SetFloat("Size", size);
        shader.SetFloat("Dissolve", dissolve);
        shader.Dispatch(kernel, 1024 / 8, 1024 / 8, 1);
    }

    private void InstructionsBreathe()
    {
        toColor.r = toColor.g = toColor.b = 1;
        toColor.a = Mathf.Sin(Time.time * breatheSpeed) / 4 + .75f;
        instruction.color = toColor;
    }

    private void CheckForInput()
    {
        if(Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(ClosingTitle());
        }
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
        titleParent.SetActive(true);
        instruction.gameObject.SetActive(false);
        SetDissolve(1);
        timer = Time.time;
        progress = 0;
        while(progress < 1)
        {
            SetDissolve(1 - sCurve.Evaluate(progress));
            progress = (Time.time - timer) / dissolveDuration;
            yield return null;
        }
        SetDissolve(0);

        // wait before instruction
        yield return new WaitForSeconds(instructionOffset);

        // show instructions
        presentsParent.SetActive(false);
        instruction.gameObject.SetActive(true);
    }

    private IEnumerator ClosingTitle()
    {
        instruction.gameObject.SetActive(false);
        SetDissolve(0);
        float timer = Time.time;
        float progress = 0;
        while(progress < 1)
        {
            SetDissolve(sCurve.Evaluate(progress));
            progress = (Time.time - timer) / dissolveDuration;
            yield return null;
        }
        SetDissolve(1);

        titleParent.SetActive(false);
        gameObject.SetActive(false);
    }
}
