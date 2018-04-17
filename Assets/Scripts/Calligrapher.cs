/*  Calligrapher - Sam Caulker
 * 
 *  Desc:   Facilitates UI and score updates
 * 
 */

using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine.UI;
using UnityEngine;

public sealed class Calligrapher : MonoBehaviour {

    [SerializeField] private Text txt_redScoreText, txt_blueScoreText;
    [SerializeField] private Image img_redCTFIcon, img_blueCTFIcon;
    [SerializeField] private Image img_redHockeyIcon, img_blueHockeyIcon;
    [SerializeField] private Image img_redNecroIcon, img_blueNecroIcon;

    [SerializeField] private Text txt_redRiftBossHealthText, txt_blueRiftBossHealthText;
    [SerializeField] private Image img_redBossIcon, img_blueBossIcon;

    [SerializeField] private Text txt_redObjvTitle, txt_blueObjvTitle;
    [SerializeField] private Text txt_redObjvDescription, txt_blueObjvDescription;
    [SerializeField] private Text txt_redPauseObjvTitle, txt_bluePauseObjvTitle;
    [SerializeField] private Text txt_redPauseObjvDescription, txt_bluePauseObjvDescription;
 
    [SerializeField] private Image img_redPopupBacking, img_bluePopupBacking;
    [SerializeField] private Image img_redFlashBacking, img_blueFlashBacking;
    [SerializeField] private Text  txt_redRoomCounter, txt_blueRoomCounter;

    private float f_redStartTime, f_blueStartTime;  // controls UI pop-up fading
    private float f_redFlashTime, f_blueFlashTime;  // separate timers for flash to avoid overwriting, since both animations play at roughly the same time.


    #region Singletons
    // Singleton
    private static Calligrapher instance;
    public static Calligrapher Instance {
        get { return instance; }
    }
    #endregion

    /*/////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

    #region UI Initialization and Updating Methods
    // update score (CTF and Ice Hockey)
    public void UpdateScoreUI(Constants.Global.Color colorIn, int scoreIn) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redScoreText.text = scoreIn.ToString();
        }
        else if (colorIn == Constants.Global.Color.BLUE) {
            txt_blueScoreText.text = scoreIn.ToString();
        }
    }

    // update health (Rift Boss)
	public void UpdateRiftBossHealthUI(Constants.Global.Color colorIn, float healthIn) {
        if (healthIn <= 0) {
            healthIn = 0;
        }
        
        if (colorIn == Constants.Global.Color.RED) {
            txt_redRiftBossHealthText.text = Mathf.CeilToInt(healthIn).ToString();  //TODO: new ui txt objects
        }
        else if (colorIn == Constants.Global.Color.BLUE) {
            txt_blueRiftBossHealthText.text = Mathf.CeilToInt(healthIn).ToString();
        }
    }

    //----------------------------
    // Initialization of different objectives
    public void CTFInit(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redScoreText.transform.parent.gameObject.SetActive(true);
            img_redCTFIcon.gameObject.SetActive(true);

            txt_redObjvTitle.text = txt_redPauseObjvTitle.text = Constants.ObjectiveText.C_CTFTitle;
            txt_redObjvDescription.text = txt_redPauseObjvDescription.text = Constants.ObjectiveText.C_CTFDescription;
        }
        else {
            txt_blueScoreText.transform.parent.gameObject.SetActive(true);
            img_blueCTFIcon.gameObject.SetActive(true);

            txt_blueObjvTitle.text = txt_bluePauseObjvTitle.text = Constants.ObjectiveText.C_CTFTitle;
            txt_blueObjvDescription.text = txt_bluePauseObjvDescription.text = Constants.ObjectiveText.C_CTFDescription;
        }
        UpdateScoreUI(colorIn, 0);
        PopupFadeIn(colorIn);
    }

    public void IceHockeyInit(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redScoreText.transform.parent.gameObject.SetActive(true);
            img_redHockeyIcon.gameObject.SetActive(true);

            txt_redObjvTitle.text = txt_redPauseObjvTitle.text = Constants.ObjectiveText.C_HockeyTitle;
            txt_redObjvDescription.text = txt_redPauseObjvDescription.text = Constants.ObjectiveText.C_HockeyDescription;
        }
        else {
            txt_blueScoreText.transform.parent.gameObject.SetActive(true);
            img_blueHockeyIcon.gameObject.SetActive(true);

            txt_blueObjvTitle.text = txt_bluePauseObjvTitle.text = Constants.ObjectiveText.C_HockeyTitle;
            txt_blueObjvDescription.text = txt_bluePauseObjvDescription.text = Constants.ObjectiveText.C_HockeyDescription;
        }
        UpdateScoreUI(colorIn, 0);
        PopupFadeIn(colorIn);
    }

    public void DefeatNecromancersInit(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redScoreText.transform.parent.gameObject.SetActive(true);
            img_redNecroIcon.gameObject.SetActive(true);

            txt_redObjvTitle.text = txt_redPauseObjvTitle.text = Constants.ObjectiveText.C_DefeatNecromancersTitle;
            txt_redObjvDescription.text = txt_redPauseObjvDescription.text = Constants.ObjectiveText.C_DefeatNecromancersDescription;
        }
        else {
            txt_blueScoreText.transform.parent.gameObject.SetActive(true);
            img_blueNecroIcon.gameObject.SetActive(true);

            txt_blueObjvTitle.text = txt_bluePauseObjvTitle.text = Constants.ObjectiveText.C_DefeatNecromancersTitle;
            txt_blueObjvDescription.text = txt_bluePauseObjvDescription.text = Constants.ObjectiveText.C_DefeatNecromancersDescription;
        }
        UpdateScoreUI(colorIn, 0);
        PopupFadeIn(colorIn);
    }

    public void RiftBossInit(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redRiftBossHealthText.transform.parent.gameObject.SetActive(true);
            img_redBossIcon.gameObject.SetActive(true);

            txt_redObjvTitle.text = txt_redPauseObjvTitle.text = Constants.ObjectiveText.C_BossTitle;
            txt_redObjvDescription.text = txt_redPauseObjvDescription.text = Constants.ObjectiveText.C_BossDescription;
            PopupFadeIn(Constants.Global.Color.RED);
        }
        else {
            txt_blueRiftBossHealthText.transform.parent.gameObject.SetActive(true);
            img_blueBossIcon.gameObject.SetActive(true);

            txt_blueObjvTitle.text = txt_bluePauseObjvTitle.text = Constants.ObjectiveText.C_BossTitle;
            txt_blueObjvDescription.text = txt_bluePauseObjvDescription.text = Constants.ObjectiveText.C_BossDescription;
            PopupFadeIn(Constants.Global.Color.BLUE);
        }
        UpdateRiftBossHealthUI(colorIn, Constants.ObjectiveStats.C_RiftBossMaxHealth);
    }

    //----------------------------
    // Flash to mask room switching.

    public void Flash(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            f_redFlashTime = Time.realtimeSinceStartup;
            StartCoroutine(RedFlash());
        } else {
            f_blueFlashTime = Time.realtimeSinceStartup;
            StartCoroutine(BlueFlash());
        }
    }

    public void RoomUpdate(Constants.Global.Color colorIn, int i_currentRoom) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redRoomCounter.text = "Room:\n" + i_currentRoom + "/5";
        } else {
            txt_blueRoomCounter.text = "Room:\n" + i_currentRoom + "/5";
        }

    }

    //----------------------------
    // Reset of different UI objects
    public void ScoreReset(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redScoreText.transform.parent.gameObject.SetActive(false);
            img_redHockeyIcon.gameObject.SetActive(false);
            img_redCTFIcon.gameObject.SetActive(false);
            img_redNecroIcon.gameObject.SetActive(false);
        }
        else {
            txt_blueScoreText.transform.parent.gameObject.SetActive(false);
            img_blueHockeyIcon.gameObject.SetActive(false);
            img_blueCTFIcon.gameObject.SetActive(false);
            img_blueNecroIcon.gameObject.SetActive(false);
        }
    }

    public void RiftBossReset(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            txt_redRiftBossHealthText.transform.parent.gameObject.SetActive(false);
            img_redBossIcon.gameObject.SetActive(false);
        }
        else {
            txt_blueRiftBossHealthText.transform.parent.gameObject.SetActive(false);
            img_blueBossIcon.gameObject.SetActive(false);
        }
    }
    #endregion

    #region UI Fade In/Out Methods
    //----------------------------
    // Fade in/out objective description UI at the start of each objective
    private void PopupFadeIn(Constants.Global.Color colorIn) {
        Time.timeScale = 0;
        if (colorIn == Constants.Global.Color.RED) {
            f_redStartTime = Time.realtimeSinceStartup;
            img_redFlashBacking.color = Color.white;
            StartCoroutine(FadeInRed());
        } else {
            f_blueStartTime = Time.realtimeSinceStartup;
            img_blueFlashBacking.color = Color.white;
            StartCoroutine(FadeInBlue());
        }
    }

/*    private void PopupFadeOut(Constants.Global.Color colorIn) {
        if (colorIn == Constants.Global.Color.RED) {
            f_redStartTime = Time.time;
            InvokeRepeating("FadeOutRed", 0.1f, 0.075f);
        } else {
            f_blueStartTime = Time.time;
            InvokeRepeating("FadeOutBlue", 0.1f, 0.075f);
        }
    } */

    private IEnumerator FadeInRed() {
        float timer = (Time.realtimeSinceStartup - f_redStartTime);
        float fracJourney = timer / 1f;
        img_redPopupBacking.color = Color.Lerp(img_redPopupBacking.color, new Color(0,0,0,0.2f), fracJourney);
        txt_redObjvTitle.color = Color.Lerp(txt_redObjvTitle.color, new Color(1,1,1,1), fracJourney);
        txt_redObjvDescription.color = Color.Lerp(txt_redObjvDescription.color, new Color(1,1,1,1), fracJourney);
        if (timer > 5f) {
            StopCoroutine(FadeInRed());
            StartCoroutine(FadeOutRed());
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.075f);
        StartCoroutine(FadeInRed());  
    }

    private IEnumerator FadeInBlue() {
        float timer = (Time.realtimeSinceStartup - f_blueStartTime);
        float fracJourney = timer / 1f;
        img_bluePopupBacking.color = Color.Lerp(img_bluePopupBacking.color, new Color(0,0,0,0.2f), fracJourney);
        txt_blueObjvTitle.color = Color.Lerp(txt_blueObjvTitle.color, new Color(1,1,1,1), fracJourney);
        txt_blueObjvDescription.color = Color.Lerp(txt_blueObjvDescription.color, new Color(1,1,1,1), fracJourney);
        if (timer > 5f) {
            StopCoroutine(FadeInBlue());
            StartCoroutine(FadeOutBlue());
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.075f);
        StartCoroutine(FadeInBlue());    

    }

    private IEnumerator FadeOutRed() {
        float timer = (Time.realtimeSinceStartup - f_redStartTime);
        float fracJourney = timer / 1f;
        img_redPopupBacking.color = Color.Lerp(img_redPopupBacking.color, new Color(0,0,0,0), fracJourney);
        txt_redObjvTitle.color = Color.Lerp(txt_redObjvTitle.color, new Color(1,1,1,0), fracJourney);
        txt_redObjvDescription.color = Color.Lerp(txt_redObjvDescription.color, new Color(1,1,1,0), fracJourney);
        if (timer > 2f) {
            Time.timeScale = 1;
            StopCoroutine(FadeOutRed());
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.075f);
        StartCoroutine(FadeOutRed());
    }

    private IEnumerator FadeOutBlue() {
        float timer = (Time.realtimeSinceStartup - f_blueStartTime);
        float fracJourney = timer / 1f;
        img_bluePopupBacking.color = Color.Lerp(img_bluePopupBacking.color, new Color(0,0,0,0), fracJourney);
        txt_blueObjvTitle.color = Color.Lerp(txt_blueObjvTitle.color, new Color(1,1,1,0), fracJourney);
        txt_blueObjvDescription.color = Color.Lerp(txt_blueObjvDescription.color, new Color(1,1,1,0), fracJourney);
        if (timer > 2f) {
            Time.timeScale = 1;
            StopCoroutine(FadeOutBlue());
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.075f);
        StartCoroutine(FadeOutBlue());
    }

    private IEnumerator BlueFlash() {
        float timer = (Time.realtimeSinceStartup - f_blueFlashTime);
        float fracJourney = timer / 0.4f;
        img_blueFlashBacking.color = Color.Lerp(img_blueFlashBacking.color, new Color(1,1,1,0), fracJourney);
        if (timer > 0.4f) {
            StopCoroutine(BlueFlash());
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.075f);
        StartCoroutine(BlueFlash());
    }

    private IEnumerator RedFlash() {
        float timer = (Time.realtimeSinceStartup - f_redFlashTime);
        float fracJourney = timer / 0.4f;
        img_redFlashBacking.color = Color.Lerp(img_redFlashBacking.color, new Color(1,1,1,0), fracJourney);
        if (timer > 0.4f) {
            StopCoroutine(RedFlash());
            yield break;
        }
        yield return new WaitForSecondsRealtime(0.075f);
        StartCoroutine(RedFlash());
    }
    #endregion

    /*/////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

    #region Unity Overrides
    void Awake() {
        instance = this;
    }
    #endregion
}
