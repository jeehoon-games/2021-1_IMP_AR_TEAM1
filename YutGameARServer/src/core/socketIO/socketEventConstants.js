/**
 * @file socketEventConstants.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

module.exports = {
    signIn: 'Event_SignIn',
    signInResult: 'Event_SignIn_Result',
    sendUserInfo: 'Event_SendUserInfo',
    sendGameData: 'Event_SendGameData',

    signUp: 'Event_SignUp',
    signUpResult: 'Event_SignUp_Result',

    authCode: 'Event_RequestAuthCode',
    authCodeResult: 'Event_RequestAuthCode_Result',
    
    findId: 'findId',
    findIdResult: 'findIdResult',

    resetPassword: 'resetPassword',
    resetPasswordResult: 'resetPasswordResult',

    Fail: 'Fail',
    WrongPw: 'WrongPw',
    AlreadyExist: 'AlreadyExist',
    Duplicate: 'Duplicate',
    Success: 'Success',
}