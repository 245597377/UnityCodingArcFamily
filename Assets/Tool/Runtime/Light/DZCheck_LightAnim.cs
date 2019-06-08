using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DZCheck_LightAnim : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public float ChangeIntinMax = 10.0f;
    [SerializeField]
    public float ChangeIntinMin = 1.0f;
    [SerializeField]
    public float ChangeSpeed = 0.2f;

    private float CurrTarget;
    public Light mCurrHandleLight;

    private bool enableAnim = false;
    void Start()
    {
        mCurrHandleLight  =GetComponent<Light>();
        DoClampCheck();
        enableAnim = false;
        StartCoroutine(DoWaitStart());
    }

    IEnumerator DoWaitStart()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 2.0f));
        enableAnim = true;
    }

    void DoClampCheck()
    {
        if (mCurrHandleLight.intensity >= ChangeIntinMax)
        {
            CurrTarget = ChangeIntinMin - 0.2f;
        }
        else if (mCurrHandleLight.intensity <= ChangeIntinMin)
        {
            CurrTarget = ChangeIntinMax + 0.2f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!enableAnim)
            return;
        DoClampCheck();
        mCurrHandleLight.intensity = Mathf.Lerp(mCurrHandleLight.intensity, CurrTarget, Time.deltaTime* ChangeSpeed);
    }
}
