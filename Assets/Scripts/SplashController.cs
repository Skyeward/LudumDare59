using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _backgroundCG;
    [SerializeField] private Image _logoImage;
    
    
    private void Start()
    {
        StartCoroutine(DoIntro());
    }
    
    
    private IEnumerator DoIntro()
    {
        yield return new WaitForSeconds(3f);
        
        yield return StartCoroutine(FadeInBackground());
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(FadeInLogo());
        
        GetComponent<AudioSource>().Play();
        
        yield return new WaitForSeconds(3f);
        
        yield return StartCoroutine(FadeOutLogo());
        
        yield return new WaitForSeconds(0.25f);
        
        SceneManager.LoadScene("PlanetPuzzleScene");
    }
    
    
    private IEnumerator FadeInBackground()
    {
        float t = 0;
        float totalTime = 1f;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            
            _backgroundCG.alpha = t;
            
            yield return null;
        }
    }
    
    
    private IEnumerator FadeInLogo()
    {
        float t = 0;
        float totalTime = 1f;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            
            _logoImage.color = new Color(1, 1, 1, t);
            
            yield return null;
        }
    }
    
    
    private IEnumerator FadeOutLogo()
    {
        float t = 0;
        float totalTime = 1f;
        
        while (t < 1)
        {
            t += Time.deltaTime / totalTime;
            
            _logoImage.color = new Color(1, 1, 1, 1 - t);
            
            yield return null;
        }
    }
}
