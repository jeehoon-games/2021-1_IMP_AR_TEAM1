using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Component = UnityEngine.Component;

namespace LogIn
{
    public class LoginManager : MonoBehaviour
    {
        #region Instance variables, struct & Init method

        public Canvas signInCvs, signUpCvs, findIdCvs, resetPwCvs, authCvs;
        public float fadeTime = 0.25f;

        private Canvas _currCvs, _authTmpCvs;
        private bool _isEmailAuthed, _isNickNameAuthed;
        private bool _isCvsChanging, _isAuthTimeExpired;
        private string _authCodeTmp;
        private float _authTimeTmp;
        
        private struct UserInfo
        {
            public string userID;
            public string userPW;
            public string userName;
            public string userNickName;
            public string userBirthday;
        }
        
        #endregion

        
        
        #region  Initialization methods
        private void Init()
        {
            if (signInCvs == null || signUpCvs == null || findIdCvs == null || resetPwCvs == null || authCvs == null)
            {
                print("Cannot find component: Canvas");
            }
            
            StartCoroutine(FadeIn(signInCvs));
            _isCvsChanging = true;
        }

        private void InitBtnListener()
        {
            FindUiInCvs<Button>(signInCvs, "SignInBtn").onClick.AddListener(delegate { OnSignInBtnClick(); });
            FindUiInCvs<Button>(signInCvs, "SignUpBtn").onClick.AddListener(delegate { OnCanvasChangeBtnClick(signUpCvs); });

            FindUiInCvs<Button>(signUpCvs, "IdAuthBtn").onClick.AddListener(delegate { OnCanvasChangeBtnClick(authCvs); });
            FindUiInCvs<Button>(signUpCvs, "NickNameChkBtn").onClick.AddListener(delegate { OnNickNameChkBtnClick(); });
            FindUiInCvs<Button>(signUpCvs, "SignUpBtn").onClick.AddListener(delegate { OnSignUpBtnClick(); });
            FindUiInCvs<Button>(signUpCvs, "FindIdBtn").onClick.AddListener(delegate { OnCanvasChangeBtnClick(findIdCvs); });
            FindUiInCvs<Button>(signUpCvs, "ResetPwBtn").onClick.AddListener(delegate { OnCanvasChangeBtnClick(resetPwCvs); });
            
            FindUiInCvs<Button>(findIdCvs, "FindIdBtn").onClick.AddListener(delegate { OnFindIdBtnClick(); });
            FindUiInCvs<Button>(resetPwCvs, "IdAuthBtn").onClick.AddListener(delegate { OnCanvasChangeBtnClick(authCvs); });
            FindUiInCvs<Button>(resetPwCvs, "ResetPwBtn").onClick.AddListener(delegate { OnResetPwBtnClick(); });
            FindUiInCvs<Button>(authCvs, "AuthBtn").onClick.AddListener(delegate { OnAuthBtnClick(); });
        }

        private void InitSocketIoEvent()
        {
            #region Socket.IO event callback methods
            
            if (FMSocketIOManager.instance.Ready)
            {
                // Sign In Canvas Socket.Io Event
                FMSocketIOManager.instance.On("Event_SignIn_Result", (e) =>
                {
                    Text notice = FindUiInCvs<Text>(signInCvs, "SignInNotice");
                    string data = e.data.Substring(1, e.data.Length - 2);

                    switch (data)
                    {
                        case "Fail":
                            notice.text = "Fail to sign in."; 
                            break;
                        case "AccountNotExist":
                            notice.text = "Your account does not exist.";
                            break;
                        case "WrongPw":
                            notice.text = "Wrong password.";
                            break;
                        case "Success":
                            SceneManager.LoadScene("MainMenuScene");
                            break;
                    }
                });
                FMSocketIOManager.instance.On("Event_SendUserInfo", (e) =>
                {
                    NetworkCore.Instance.UserData = JsonUtility.FromJson<Data.UserData>(e.data);
                    NetworkCore.Instance.DataDump('u');
                });
                FMSocketIOManager.instance.On("Event_SendGameData", (e) =>
                {
                    NetworkCore.Instance.GameData = JsonUtility.FromJson<Data.GameData>(e.data);
                    NetworkCore.Instance.DataDump('g');
                });
                
                
                // Sign Up Canvas Socket.Io Event
                FMSocketIOManager.instance.On("Event_SignUp_Result", (e) =>
                {
                    Text notice = FindUiInCvs<Text>(signUpCvs, "SignUpNotice");
                    string data = e.data.Substring(1, e.data.Length - 2);
                    switch (data)
                    {
                        case "Fail":
                            notice.text = "Fail to sign Up.";
                            break;
                        case "AlreadyExist":
                            notice.text = "Your account already exist.";
                            break;
                        case "Success":
                            ChangeCanvas(_currCvs, signInCvs);
                            break;
                    }
                });
                FMSocketIOManager.instance.On("Event_RequestAuthCode_Result", (e) =>
                {
                    string data = e.data.Substring(1, e.data.Length - 2);
                    switch (data)
                    {
                        case "Fail":
                            break;
                        default:
                            _authCodeTmp = data;
                            break;
                    }
                });
                FMSocketIOManager.instance.On("Event_CheckNickName_Result", (e) =>
                {
                    Text notice = FindUiInCvs<Text>(signUpCvs, "SignUpNotice");
                    string data = e.data.Substring(1, e.data.Length - 2);
                    switch (data)
                    {
                        case "Duplicate":
                            notice.text = "Duplicate nickname.";
                            break;
                        case "Success":
                            _isNickNameAuthed = true;
                            AdjustUiState(FindUiInCvs<InputField>(signUpCvs, "NickNameField"), 0);
                            AdjustUiState(FindUiInCvs<Button>(signUpCvs, "NickNameChkBtn"), 0);
                            break;
                    }
                });
                
                // Find Id Canvas Socket.IO Event
                FMSocketIOManager.instance.On("Event_FindId_Result", (e) =>
                {
                    Text notice = FindUiInCvs<Text>(findIdCvs, "FindIdNotice");
                    string data = e.data.Substring(1, e.data.Length - 2);

                    switch (data)
                    {
                        case "Fail":                // error occured
                            notice.text = "Fail to find user ID.";
                            break;
                        case "AccountNotExist":     // Account does not exist.
                            notice.text = "Your account does not exist.";
                            break;
                        default:                    // Success 
                            notice.text = "Your user ID is: " + data;
                            break;
                    }
                });
                
                // Reset Pw Canvas Socket.IO Event
                FMSocketIOManager.instance.On("Event_ResetPw_Result", (e) =>
                {
                    Text notice = FindUiInCvs<Text>(resetPwCvs, "ResetPwNotice");
                    string data = e.data.Substring(1, e.data.Length - 2);
                    
                    switch (data)
                    {
                        case "Fail":
                            notice.text = "Fail to reset user password.";
                            break;
                        case "AccountNotExist":
                            notice.text = "Your account does not exist.";
                            break;
                        case "Success":
                            notice.text = "Your password has been successfully reset.";
                            break;
                    }
                });
            }
            
            #endregion
        }

        #endregion



        #region Unity event methods

        void Start()
        {
            Init();
            InitBtnListener();
            InitSocketIoEvent();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !_isCvsChanging)
            {
                Canvas dest = null;
                
                if (_currCvs.Equals(signInCvs))
                {
                    Application.Quit();
                    return;
                }
                if (_currCvs.Equals(signUpCvs) || _currCvs.Equals(findIdCvs) || _currCvs.Equals(resetPwCvs)) 
                {
                    dest = signInCvs;
                }
                if(_currCvs.Equals(authCvs))
                {
                    dest = _authTmpCvs;
                }
                
                ChangeCanvas(_currCvs, dest);
            }
            AuthTimer();
        }

        #endregion



        #region Utility fundtions

        // Find specific ui component in canvas object using ui name (ex: IdTextField)
        private T FindUiInCvs<T>(Canvas cvs, string uiName) where T : Component
        {
            T[] uiArr = cvs.GetComponentsInChildren<T>();
            foreach (T ui in uiArr)
            {
                if (ui.name.Equals(uiName))
                {
                    return ui;
                }
            }

            StringBuilder log = new StringBuilder().Append("Cannot find component name: ");
            log.Append(uiName);
            log.Append("  // Type: ");
            log.Append(typeof(T));
            print(log);
            return null;
        }

        private void InitCanvas(Canvas cvs)
        {
            Button authBtn = FindUiInCvs<Button>(cvs, "IdAuthBtn");
            Button nickNameBtn = FindUiInCvs<Button>(cvs, "NickNameChkBtn");
            if (authBtn != null) { authBtn.interactable = true; }
            if (nickNameBtn != null) { nickNameBtn.interactable = true; }
            
            InputField[] textArr = cvs.GetComponentsInChildren<InputField>();
            foreach (InputField field in textArr)
            {
                if (!field.interactable) field.interactable = true;
                field.text = "";
            }
        }

        private void ChangeCanvas(Canvas depart, Canvas dest)
        {
            if (depart is null || dest is null)
            {
                print("Canvas value is null");
                return;
            }
        
            CanvasGroup departCvsGrp = depart.GetComponent<CanvasGroup>();
            CanvasGroup destCvsGrp = dest.GetComponent<CanvasGroup>();

            if (departCvsGrp is null || destCvsGrp is null)
            {
                print("Cannot find component: CanvasGroup");
                return;
            }
            _isCvsChanging = true;
            StartCoroutine(FadeOut(depart, dest));
        }
        
        private void AdjustUiState<T>(T ui, int type) where T : Component
        {
            Selectable selectable = ui.GetComponent<Selectable>();
            if (selectable == null) { return; }
            
            switch (type)
            {
                case 0:     // disable
                    selectable.interactable = false;
                    break;
                case 1:     // enable
                    selectable.interactable = true;
                    break;
            }
        }

        private void AuthTimer()
        {
            if (_currCvs == null) { return; }
            if (_currCvs.Equals(authCvs))
            {
                if (_authTimeTmp > 0)
                {
                    _authTimeTmp -= Time.deltaTime;
                }
                else
                {
                    _authTimeTmp = 0;
                    DisplayTime(_authTimeTmp);
                    _isAuthTimeExpired = true;
                }
                DisplayTime(_authTimeTmp);
            }
        }

        private void DisplayTime(float timeToDisplay)
        {
            InputField field = FindUiInCvs<InputField>(authCvs, "TimeRemainField");
            if(timeToDisplay > 0) { timeToDisplay += 1; }
            float min = Mathf.FloorToInt(timeToDisplay / 60);
            float sec = Mathf.FloorToInt(timeToDisplay % 60);
            field.text = string.Format("{0:00}:{1:00}", min, sec);
        }
        
        #endregion
        
        
        
        #region Coroutines
    
        IEnumerator FadeIn(Canvas dest)
        {
            CanvasGroup cGroup = dest.GetComponent<CanvasGroup>();
            if (cGroup.alpha >= 0) { cGroup.alpha = 0; }
            
            while (cGroup.alpha < 1.0f)
            {
                cGroup.alpha += 0.01f;
                yield return new WaitForSeconds(fadeTime * 0.01f);
            }
        
            _isCvsChanging = false;
            _currCvs = dest;
        }

        IEnumerator FadeOut(Canvas depart, Canvas dest)
        {
            CanvasGroup cGroup = depart.GetComponent<CanvasGroup>();
            if (cGroup.alpha <= 0) { cGroup.alpha = 1.0f; }

            while (cGroup.alpha > 0)
            {
                cGroup.alpha -= 0.01f;
                yield return new WaitForSeconds(fadeTime * 0.01f);
            }
            
            depart.gameObject.SetActive(false);
            dest.gameObject.SetActive(true);
            StartCoroutine(FadeIn(dest));
        }
        
        #endregion

        

        #region Button click callback methods

        // Change canvas (Departure -> Destination)
        void OnCanvasChangeBtnClick(Canvas dest)
        {
            if (!_isCvsChanging)
            {
                
                if (!_currCvs.Equals(authCvs) && !dest.Equals(authCvs)) { InitCanvas(dest); }
                if (!_currCvs.Equals(authCvs)) { _isEmailAuthed = false; }
                ChangeCanvas(_currCvs, dest);

                if (dest.Equals(authCvs) && FMSocketIOManager.instance.Ready)
                {
                    _authTmpCvs = _currCvs;
                    _authTimeTmp = 10f;
                    DisplayTime(_authTimeTmp);
                    
                    if (FMSocketIOManager.instance.Ready)
                    {
                        _isAuthTimeExpired = false;
                        UserInfo userInfo = new UserInfo();
                        userInfo.userID = FindUiInCvs<InputField>(_currCvs, "IdField").text;
                        FMSocketIOManager.instance.Emit("Event_RequestAuthCode", JsonUtility.ToJson(userInfo));
                    }
                }
            }
        }

        void OnSignInBtnClick()
        {
            if (FMSocketIOManager.instance.Ready && !_isCvsChanging)
            {
                InputField idField = FindUiInCvs<InputField>(signInCvs, "IdField");
                InputField pwField = FindUiInCvs<InputField>(signInCvs, "PwField");
                Text notice = FindUiInCvs<Text>(signInCvs, "SignInNotice");

                notice.text = "";
                if (idField.text.Equals(""))
                {
                    notice.text = "Please enter your user id.";
                    return;
                }

                if (pwField.text.Equals(""))
                {
                    notice.text = "Please enter your user password.";
                    return;
                }

                UserInfo userInfo = new UserInfo();
                userInfo.userID = idField.text;
                userInfo.userPW = Crypto.SHAhash(pwField.text, "sha256");
                FMSocketIOManager.instance.Emit("Event_SignIn", JsonUtility.ToJson(userInfo));
            }
        }

        void OnNickNameChkBtnClick()
        {
            if (FMSocketIOManager.instance.Ready && !_isCvsChanging)
            {
                InputField nicknameField = FindUiInCvs<InputField>(signUpCvs, "NickNameField");
                Text notice = FindUiInCvs<Text>(signUpCvs, "SignUpNotice");

                if (nicknameField.text.Equals(""))
                {
                    notice.text = "Please enter your nickname.";
                }
                else
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.userNickName = FindUiInCvs<InputField>(signUpCvs, "NickNameField").text;
                    FMSocketIOManager.instance.Emit("Event_CheckNickName", JsonUtility.ToJson(userInfo));
                }
            }
        }

        void OnSignUpBtnClick()
        {
            if (FMSocketIOManager.instance.Ready && !_isCvsChanging)
            {
                InputField idField = FindUiInCvs<InputField>(signUpCvs, "IdField");
                InputField nicknameField = FindUiInCvs<InputField>(signUpCvs, "NickNameField");
                InputField nameField = FindUiInCvs<InputField>(signUpCvs, "UserNameField");
                InputField birthdayField = FindUiInCvs<InputField>(signUpCvs, "BirthdayField");
                InputField pwField = FindUiInCvs<InputField>(signUpCvs, "PwField");
                InputField pwConfirmField = FindUiInCvs<InputField>(signUpCvs, "PwConfirmField");
                Text notice = FindUiInCvs<Text>(signUpCvs, "SignUpNotice");

                if (!_isEmailAuthed)
                {
                    notice.text = "Please authenticate your email.";
                }
                else if (!_isNickNameAuthed)
                {
                    notice.text = "Please check whether the nickname is duplicated or not.";
                }
                else if (idField.text.Equals(""))
                {
                    notice.text = "Please enter your Id.";
                }
                else if (nicknameField.text.Equals(""))
                {
                    notice.text = "Please enter your nickname.";
                }
                else if (nameField.text.Equals(""))
                {
                    notice.text = "Please enter your name.";
                }
                else if (birthdayField.text.Equals(""))
                {
                    notice.text = "Please enter your birthday.";
                }
                else if (pwField.text.Equals(""))
                {
                    notice.text = "Please enter your password";
                }
                else if (pwConfirmField.text.Equals(""))
                {
                    notice.text = "Please enter the password confirm.";
                }
                else if (!pwField.text.Equals(pwConfirmField.text))
                {
                    notice.text = "Password and password confirm do not match.";
                }
                else
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.userID = idField.text;
                    userInfo.userNickName = nicknameField.text;
                    userInfo.userName = nameField.text;
                    userInfo.userBirthday = birthdayField.text;
                    userInfo.userPW = Crypto.SHAhash(pwField.text, "sha256");
                    FMSocketIOManager.instance.Emit("Event_SignUp", JsonUtility.ToJson(userInfo));
                }
            }
        }

        void OnFindIdBtnClick()
        {
            if (FMSocketIOManager.instance.Ready && !_isCvsChanging)
            {
                InputField nameField = FindUiInCvs<InputField>(findIdCvs, "UserNameField");
                InputField birthdayField = FindUiInCvs<InputField>(findIdCvs, "BirthdayField");
                Text notice = FindUiInCvs<Text>(findIdCvs, "FindIdNotice");

                if (nameField.text.Equals(""))
                {
                    notice.text = "Please enter your name.";
                }
                else if (birthdayField.text.Equals(""))
                {
                    notice.text = "Please enter your birthday.";
                }
                else
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.userName = nameField.text;
                    userInfo.userBirthday = birthdayField.text;
                    FMSocketIOManager.instance.Emit("Event_FindId", JsonUtility.ToJson(userInfo));
                }
            }
        }

        void OnResetPwBtnClick()
        {
            if (FMSocketIOManager.instance.Ready && !_isCvsChanging)
            {
                InputField pwField = FindUiInCvs<InputField>(resetPwCvs, "PwField");
                InputField pwConfirmField = FindUiInCvs<InputField>(resetPwCvs, "PwConfirmField");
                Text notice = FindUiInCvs<Text>(resetPwCvs, "ResetPwNotice");

                if (pwField.text.Equals(""))
                {
                    notice.text = "Please enter your new password.";
                }
                else if (pwConfirmField.text.Equals(""))
                {
                    notice.text = "Please enter a password confirmation.";
                }
                else if(!pwField.text.Equals(pwConfirmField.text))
                {
                    notice.text = "Password and password confirm do not match.";
                }
                else
                {
                    UserInfo userInfo = new UserInfo();
                    userInfo.userID = FindUiInCvs<InputField>(_authTmpCvs, "IdField").text;
                    userInfo.userPW = Crypto.SHAhash(pwField.text, "sha256");
                    FMSocketIOManager.instance.Emit("Event_ResetPw", JsonUtility.ToJson(userInfo));
                }
            }
        }

        void OnAuthBtnClick()
        {
            if (FMSocketIOManager.instance.Ready && !_isCvsChanging)
            {
                InputField authField = FindUiInCvs<InputField>(authCvs, "AuthCodeField");
                Text notice = FindUiInCvs<Text>(authCvs, "AuthNotice");

                if (authField.text.Equals(""))
                {
                    notice.text = "Please enter authentication code.";
                }
                else if (_isAuthTimeExpired)
                {
                    notice.text = "The authentication time has expired.";
                }
                else if (!Crypto.SHAhash(authField.text, "sha256").Equals(_authCodeTmp))
                {
                    notice.text = "The authentication code does not match.";
                }
                else
                {
                    _isEmailAuthed = true;
                    AdjustUiState(FindUiInCvs<InputField>(_authTmpCvs, "IdField"), 0);
                    AdjustUiState(FindUiInCvs<Button>(_authTmpCvs, "IdAuthBtn"), 0);
                    ChangeCanvas(_currCvs, _authTmpCvs);
                }
            }
        }
        
        #endregion
    }
}