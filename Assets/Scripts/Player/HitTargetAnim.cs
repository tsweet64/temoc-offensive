using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

class HitTargetAnim : MonoBehaviour
{
    Renderer r;
    Material mat;
    [SerializeField]
    float overlayTime;
    bool isActive = false;
    float duration = 0.05f;
    float animTime = 0;
    Coroutine startup = null;

    void Start()
    {
        r = GetComponent<Renderer>();
        mat = r.material;
    }

    public void setOverlayTime(float val) {
        overlayTime = val;

        //initiate startup animation
        if(overlayTime > 0 && !isActive) {
            isActive = true;
            StopAllCoroutines();
            startup = StartCoroutine(Startup(duration));
        }

        //initiate shutdown animation
        if(overlayTime <= 0 && isActive) {
            isActive = false;
            StartCoroutine(Shutdown(duration));
        }
    }

    IEnumerator Startup(float duration) {
        r.enabled = true;
        for(float time = animTime * duration; time <= duration; time += Time.deltaTime) {
            animTime = time / duration;
            mat.SetFloat("_AnimTime", animTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Shutdown(float duration) {
        yield return new WaitForEndOfFrame();
        if(startup != null)
            StopCoroutine(startup);
        for(float time = animTime * duration; time >= 0; time -= Time.deltaTime) {
            animTime = time / duration;
            mat.SetFloat("_AnimTime", animTime);
            yield return new WaitForEndOfFrame();
        }
        r.enabled = false;
    }
}