using System;
using System.Collections;
using UnityEngine;

public class PortalFX_UVAnimation : MonoBehaviour
{
    public int TilesX = 4;
    public int TilesY = 4;
    public float FPS = 30;
    public int StartFrameOffset;
    public bool IsLoop = true;
    public float StartDelay = 0;
    public bool IsReverse;
    public bool IsBump;

    public AnimationCurve FrameOverTime = AnimationCurve.Linear(0, 1, 1, 1);

    private bool isInizialised;
    private int index;
    private int count, allCount;
    private float animationLifeTime;
    private bool isVisible;
    private bool isCorutineStarted;
    private Renderer currentRenderer;
    private Material instanceMaterial;
    private float animationStartTime;
    private bool animationStoped;

    #region Non-public methods

    private void Start()
    {
        currentRenderer = GetComponent<Renderer>();
        InitDefaultVariables();
        isInizialised = true;
        isVisible = true;
        Play();
    }

    private void InitDefaultVariables()
    {
        currentRenderer = GetComponent<Renderer>();
        if (currentRenderer==null)
            throw new Exception("UvTextureAnimator can't get renderer");
        if (!currentRenderer.enabled)
            currentRenderer.enabled = true;
        allCount = 0;
        animationStoped = false;
        animationLifeTime = TilesX * TilesY / FPS;
        count = TilesY * TilesX;
        index = TilesX - 1;
        var offset = Vector3.zero;
        StartFrameOffset = StartFrameOffset - (StartFrameOffset / count) * count;
        var size = new Vector2(1f / TilesX, 1f / TilesY);

        if (currentRenderer!=null) {
            instanceMaterial = currentRenderer.material;
            instanceMaterial.SetTextureScale("_MainTex", size);
            instanceMaterial.SetTextureOffset("_MainTex", offset);
            if (IsBump) {
                instanceMaterial.SetTextureScale("_BumpMap", size);
                instanceMaterial.SetTextureOffset("_BumpMap", offset);
            }
        }
    }

    private void Play()
    {
        if (isCorutineStarted)
            return;
        if (StartDelay > 0.0001f)
            Invoke("PlayDelay", StartDelay);
        else
            StartCoroutine(UpdateCorutine());
        isCorutineStarted = true;
    }

    private void PlayDelay()
    {
        StartCoroutine(UpdateCorutine());
    }

    #region CorutineCode

    private void OnEnable()
    {
        if (!isInizialised)
            return;
        InitDefaultVariables();
        isVisible = true;
        Play();
    }

    private void OnDisable()
    {
        isCorutineStarted = false;
        isVisible = false;
        StopAllCoroutines();
        CancelInvoke("PlayDelay");
    }


    private IEnumerator UpdateCorutine()
    {
        animationStartTime = Time.time;
        while (isVisible && (IsLoop || !animationStoped)) {
            if (!IsReverse)
                UpdateFrame();
            else
                UpdateFrameReversed();

            if (!IsLoop && animationStoped)
                break;
            var frameTime = (Time.time - animationStartTime) / animationLifeTime;
            var currentSpeedFps = FrameOverTime.Evaluate(Mathf.Clamp01(frameTime));
            yield return new WaitForSeconds(1f / (FPS * currentSpeedFps));
        }
        isCorutineStarted = false;
        //currentRenderer.enabled = false;
    }

    #endregion CorutineCode

    private void UpdateFrame()
    {
        ++allCount;
        ++index;
        if (index >= count)
            index = 0;
        if (count==allCount) {
            animationStartTime = Time.time;
            allCount = 0;
            animationStoped = true;
        }
        var offset = new Vector2((float)index / TilesX - (int)(index / TilesX), 1 - (int)(index / TilesX) / (float)TilesY);
        if (currentRenderer!=null) {
            instanceMaterial.SetTextureOffset("_MainTex", offset);
            if (IsBump)
                instanceMaterial.SetTextureOffset("_BumpMap", offset);
            
        }
    }

    private void UpdateFrameReversed()
    {
        --allCount;
        --index;
        if (index <= 0)
            index = count;
        if (count == allCount)
        {
            animationStartTime = Time.time;
            allCount = 0;
            animationStoped = true;
        }
        var offset = new Vector2((float)index / TilesX - (int)(index / TilesX), 1 - (int)(index / TilesX) / (float)TilesY);
        if (currentRenderer!=null) {
            instanceMaterial.SetTextureOffset("_MainTex", offset);
            if (IsBump)
                instanceMaterial.SetTextureOffset("_BumpMap", offset);
        }
    }


    private void OnDestroy()
    {
        if (instanceMaterial!=null) {
            Destroy(instanceMaterial);
            instanceMaterial = null;
        }
    }

    #endregion
}