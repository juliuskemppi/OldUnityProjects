using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.UI;


/// <summary>
/// QR code reader
/// </summary>
public class DevCam : MonoBehaviour //https://medium.com/@adrian.n/reading-and-generating-qr-codes-with-c-in-unity-3d-the-easy-way-a25e1d85ba51
{

    //The camera image
    [SerializeField]
    Image m;

    //The webcam image
    public WebCamTexture mCamera;

    //The Qr reader
    IBarcodeReader reader = new BarcodeReader();

    [SerializeField]
    List<Bonus> bonusList = new List<Bonus>();
    private Rect screenRect;
    void Start()
    {
        if(AutoGenerateBonus)
        {
            autoGenerateBonus();
        }

        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        //Initialize the webcam texture
        mCamera = new WebCamTexture();
        mCamera.filterMode = FilterMode.Point;
        
        mCamera.requestedHeight = Screen.height / 3;
        mCamera.requestedWidth = Screen.width / 3;

        //Fill the screen with the camera image
        //m.material.mainTexture = mCamera;
        mCamera.Play();
        StartCoroutine(ReadQr());
    }

    [SerializeField]
    bool AutoGenerateBonus;
    void autoGenerateBonus()
    {
        Bonus b = new Bonus();
        b.Date = DateTime.Now.ToString("dd_MM_yyyy");
        b.bonus = 0;

        bonusList.Add(b);
    }

    void OnGUI()
    {
        // drawing the camera on screen
        GUI.DrawTexture(screenRect, mCamera, ScaleMode.ScaleToFit);
    }


    //The last Qr code that was read
    string lastQr;
    string lastDate;

    IEnumerator ReadQr()
    {
        while (true)
        {
            // get texture Color32 array
            var barcodeBitmap = mCamera.GetPixels32();

            // detect and decode the barcode inside the Color32 array
            var result = reader.Decode(barcodeBitmap, mCamera.width, mCamera.height);

            // do something with the result
            if (result != null)
            {
                //Is the barcode a qr code
                if (result.BarcodeFormat == BarcodeFormat.QR_CODE)
                {
                    //Is the read qr code the same that was already read
                    if (result.Text != lastQr)
                    {
                        //Make the read qr the last qr read
                        lastQr = result.Text;
                        Debug.Log.Add("Qr found");
                        activeBonus(lastQr);
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.SetString("DailyBonus", "");
            Debug.Log.Add("BonusReseted");
        }
    }

    void activeBonus(string bonus)
    {
        string date = bonus.Remove(0, 12);
        Debug.Log.Add(date);

        if (date == DateTime.Now.ToString("dd_MM_yyyy"))
        {
            Debug.Log.Add("Correct date");
            if (PlayerPrefs.GetString("DailyBonus") != date || PlayerPrefs.GetString("DailyBonus") == "")
            {
                Debug.Log.Add(PlayerPrefs.GetString("DailyBonus"));
                foreach (Bonus b in bonusList)
                {
                    if (b.Date == date)
                    {
                        Debug.Log.Add("Active bonus " + b.bonus.ToString());
                        PlayerPrefs.DeleteKey("DailyBonus");
                        PlayerPrefs.SetString("DailyBonus", date);
                        PlayerPrefs.SetInt("Bonus", b.bonus);
                        SoundManager.instance.PlaySoundByName("Success");
                        Invoke("backToMain", 1f);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log.Add("Bonus alredy used!");
                SoundManager.instance.PlaySoundByName("Fail");
            }
        }
        else
        {
            Debug.Log.Add("Date is not correct!");
            SoundManager.instance.PlaySoundByName("Fail");
        }
    }

    void backToMain()
    {
        SceneManager.instance.ChangeScene("Menu");
    }
}

[Serializable]
class Bonus
{
    public string Date;
    public int bonus;
}
