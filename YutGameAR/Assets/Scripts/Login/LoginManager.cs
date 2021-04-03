using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using FMSocketIO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    #region Instance variables & Init method

    public Canvas signInCvs, signUpCvs, findIdCvs, resetPwCvs, authCvs;

    private class UserInfo
    {
        public string userID;
        public string userPW;
        public string userName;
        public string userNickName;
        public string userBirthday;
    }

    private FMSocketIOManager _instance;
    private NetworkCore _networkCore;
    private Canvas _currCvs;
    private float _fadeTime = 0.25f;
    private bool _isCvsChanging;
    private bool _isEmailAuthed;
    private bool _isNickNameAuthed;
    private bool _isConnected;
    private string _authCode;
    
    private void Init()
    {
        if (signInCvs == null || signUpCvs == null || findIdCvs == null || resetPwCvs == null)
        {
            print("Cannot find component: Canvas");
        }

        signInCvs.GetComponent<CanvasGroup>().alpha = 0;
        signUpCvs.GetComponent<CanvasGroup>().alpha = 0;
        findIdCvs.GetComponent<CanvasGroup>().alpha = 0;
        resetPwCvs.GetComponent<CanvasGroup>().alpha = 0;
        authCvs.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(FadeIn(signInCvs.GetComponent<CanvasGroup>()));
        StartCoroutine(WaitForConnection());
        _isCvsChanging = true;

        signInCvs.GetComponentsInChildren<Button>()[0].onClick.AddListener(delegate { OnSignInClick(); }); 
        signInCvs.GetComponentsInChildren<Button>()[1].onClick.AddListener(delegate { OnCvsChangeBtnClick(signInCvs, signUpCvs); });
        
        signUpCvs.GetComponentsInChildren<Button>()[0].onClick.AddListener(delegate { OnCvsChangeBtnClick(signUpCvs, authCvs); });
        signUpCvs.GetComponentsInChildren<Button>()[1].onClick.AddListener(delegate { OnNicknameVerifyClick(); });
        signUpCvs.GetComponentsInChildren<Button>()[2].onClick.AddListener(delegate { OnSignUpRequestClick(); });
        signUpCvs.GetComponentsInChildren<Button>()[3].onClick.AddListener(delegate { OnCvsChangeBtnClick(signUpCvs, findIdCvs); });
        signUpCvs.GetComponentsInChildren<Button>()[4].onClick.AddListener(delegate { OnCvsChangeBtnClick(signUpCvs, resetPwCvs); });
        
        authCvs.GetComponentsInChildren<Button>()[0].onClick.AddListener(delegate { OnSendCodeBtnClick(); });
        authCvs.GetComponentsInChildren<Button>()[1].onClick.AddListener(delegate { OnAuthBtnClick(); });
    }

    #endregion

    
    
    #region Button click methods
    
    void OnCvsChangeBtnClick(Canvas from, Canvas to)
    {
        if (!_isCvsChanging)
        {
            ChangeCanvas(from, to);
        }
    }
    
    // Sign In Canvas Buttons event
    void OnSignInClick()
    {
        if (!_isCvsChanging && _instance.IsWebSocketConnected())
        {
            UserInfo info = new UserInfo();
            info.userID = signInCvs.GetComponentsInChildren<InputField>()[0].text;
            info.userPW = Crypto.SHAhash(signInCvs.GetComponentsInChildren<InputField>()[1].text, "sha256");
            _instance.Emit("Event_SignIn", JsonUtility.ToJson(info));
        }
    }

    // Sign Up Canvas Buttons event
    void OnNicknameVerifyClick()
    {
        if (!_isCvsChanging && _instance.IsWebSocketConnected())
        {
            UserInfo info = new UserInfo();
            info.userNickName = signUpCvs.GetComponentsInChildren<InputField>()[1].text;
            _instance.Emit("Event_CheckNickName", JsonUtility.ToJson(info));
        }
    }

    void OnSignUpRequestClick()
    {
        if (!_isCvsChanging && _instance.IsWebSocketConnected())
        {
            InputField[] fields = signUpCvs.GetComponentsInChildren<InputField>();

            if (!_isEmailAuthed)
            {
                print("Please complete the email authentication.");
            }
            else if (!_isNickNameAuthed)
            {
                print("Please complete the nickname authentication.");
            }
            else if (!fields[4].text.Equals(fields[5].text))
            {
                print("Please check your password / password confirm.");
            }
            else
            {
                UserInfo info = new UserInfo();
                info.userID = fields[0].text;
                info.userNickName = fields[1].text;
                info.userName = fields[2].text;
                info.userBirthday = fields[3].text;
                info.userPW = Crypto.SHAhash(fields[4].text, "sha256");
                _instance.Emit("Event_SignUp", JsonUtility.ToJson(info));
                ChangeCanvas(signUpCvs, signInCvs);
            }
        }
    }
    
    void OnSendCodeBtnClick()
    {
        if (!_isCvsChanging && _instance.IsWebSocketConnected())
        {
            UserInfo info = new UserInfo();
            info.userID = authCvs.GetComponentsInChildren<InputField>()[0].text;
            _instance.Emit("Event_Authentication", JsonUtility.ToJson(info));
        }
    }

    void OnAuthBtnClick()
    {
        if (!_isCvsChanging)
        {
            string hashCode = Crypto.SHAhash(authCvs.GetComponentsInChildren<InputField>()[1].text, "sha256");

            if (hashCode.Equals(_authCode))
            {
                InputField field = signUpCvs.GetComponentsInChildren<InputField>()[0];
                Color color = field.gameObject.GetComponent<Image>().color;
                field.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
                field.enabled = false;
                
                Button btn = signUpCvs.GetComponentsInChildren<Button>()[0];
                color = btn.gameObject.GetComponent<Image>().color;
                btn.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
                btn.enabled = false;
                
                _isEmailAuthed = true;
                ChangeCanvas(authCvs, signUpCvs);
            }
            else
            {
                print("wrong code");
            }
        }
    }

    #endregion


    #region Socket.io event callback methods
    
    
    #endregion
    
    
    #region Utility fundtions

    private void ChangeCanvas(Canvas from, Canvas to)
    {
        if (from is null || to is null)
        {
            print("Canvas value is null");
            return;
        }
        
        CanvasGroup fCvsGrp = from.GetComponent<CanvasGroup>();
        CanvasGroup tCvsGrp = to.GetComponent<CanvasGroup>();

        if (fCvsGrp is null || tCvsGrp is null)
        {
            print("Cannot find component: CanvasGroup");
            return;
        }

        if (!to.Equals(signUpCvs))
        {
            _isEmailAuthed = false;
            _isNickNameAuthed = false;
            
            InputField authField = signUpCvs.GetComponentsInChildren<InputField>()[0];
            Color color = authField.gameObject.GetComponent<Image>().color;
            authField.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);

            Button authBtn = signUpCvs.GetComponentsInChildren<Button>()[0];
            color = authBtn.gameObject.GetComponent<Image>().color;
            authBtn.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);

            InputField field = signUpCvs.GetComponentsInChildren<InputField>()[1];
            color = field.gameObject.GetComponent<Image>().color;
            field.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);

            Button btn = signUpCvs.GetComponentsInChildren<Button>()[1];
            color = btn.gameObject.GetComponent<Image>().color;
            btn.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
            
            authField.enabled = authBtn.enabled = field.enabled = btn.enabled = true;
        }

        _isCvsChanging = true;
        StartCoroutine(FadeOut(fCvsGrp, tCvsGrp));
    }

    #endregion

    
    
    #region Unity event functions

    void Awake()
    {
        Init();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!_isCvsChanging && Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeCanvas(_currCvs, signInCvs);
        }
    }

    #endregion
    
    
    
    #region Coroutines for fade effect
    
    IEnumerator FadeIn(CanvasGroup tGrp)
    {
        while (tGrp.alpha < 1.0f)
        {
            tGrp.alpha += 0.01f;
            yield return new WaitForSeconds(_fadeTime * 0.01f);
        }
        
        _isCvsChanging = false;
        _currCvs = tGrp.gameObject.GetComponent<Canvas>();
    }

    IEnumerator FadeOut(CanvasGroup fGrp, CanvasGroup tGrp)
    {
        while (fGrp.alpha > 0)
        {
            fGrp.alpha -= 0.01f;
            yield return new WaitForSeconds(_fadeTime * 0.01f);
        }
        
        fGrp.gameObject.SetActive(false);
        tGrp.gameObject.SetActive(true);
        StartCoroutine(FadeIn(tGrp));
    }

    IEnumerator WaitForConnection()
    {
        while (FMSocketIOManager.instance is null)
        {
            yield return null;
        }
        _instance = FMSocketIOManager.instance;
        _networkCore = GameObject.FindWithTag("Network").GetComponent<NetworkCore>();
        
        while (!_isConnected) 
        {
            _isConnected = _instance.Ready;
            yield return null;
        }
        
        _instance.On("Event_SignIn_Result", (e) =>
        {
            string data = e.data.Substring(1, e.data.Length - 2);

            switch (data)
            {
                case "Fail":
                    print("Fail");
                    break;
                case "NotExist":
                    print("NotExist");
                    break;
                case "WrongPW":
                    print("WrongPW");
                    break;
                case "Success":
                    print("Success");
                    SceneManager.LoadScene("YutExample");
                    break;
            }
        });
        
        _instance.On("Event_SendUserInfo", (e) =>
        {
            UserData uData = JsonUtility.FromJson<UserData>(e.data);
            _networkCore.UserData = uData;
        });
        
        _instance.On("Event_SendGameData", (e) =>
        {
            GameData gData = JsonUtility.FromJson<GameData>(e.data);
            _networkCore.GameData = gData;
        });
        
        _instance.On("Event_CheckNickName_Result", (e) =>
        {
            if (e.data.Equals("true"))
            {
                InputField field = signUpCvs.GetComponentsInChildren<InputField>()[1];
                Color color = field.gameObject.GetComponent<Image>().color;
                field.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.6f);
                field.enabled = false;
                
                Button btn = signUpCvs.GetComponentsInChildren<Button>()[1];
                color = btn.gameObject.GetComponent<Image>().color;
                btn.gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.6f);
                btn.enabled = false;
                
                _isNickNameAuthed = true;
            }
        });
        
        _instance.On("Event_SendAuthCode", (e) =>
        {
            _authCode = e.data.Substring(1, e.data.Length - 2);
        });
    }
    
    #endregion
}
