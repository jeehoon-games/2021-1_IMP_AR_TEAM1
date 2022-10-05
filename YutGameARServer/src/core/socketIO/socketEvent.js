/**
 * @file socketEvent.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Socket.io events for android
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/* Modules */
const database = require('@src/db/database');
const queries = require('@src/db/queries');
const mailer = require('@src/utils/mailer');
const Constants = require('@src/core/socketIO/socketEventConstants');

const Encrypt = require('@src/utils/encrypt');
const CustomUtils = require('@src/utils/customUtils');
const AbstractSocketEvent = require('@src/core/socketIO/abstractSocketEvent');


/**
 * @class SocketEvent
 */
class SocketEvent extends AbstractSocketEvent{
    constructor() {
        super();
    }

    /**
     * @Override
     * @function onConnect
     * @description Callback called when a socket is connected
     * @param {*} socket 
     */
    onConnect(socket) {
        super.onConnect(socket);

        /**
         * socket event [sign in]
         */
        socket.on(Constants.signIn, async (dto) => {
            CustomUtils.logWithTime(`Client requests 'sign in' [${socket.id}]`);
            const userID = dto['userID'];
            const userPW = dto['userPW'];
            const userInfoRow = await database.query(queries.api.signIn, [userID]);

            if(userInfoRow == null) {
                // ID does not exist
                CustomUtils.logWithTime(`Fail to sign in to server [${socket.id}]`);
                socket.emit(Constants.signInResult, Constants.Fail);
            } else {
                const originPwHash = userInfoRow['user_pw'];
                const originPwSalt = userInfoRow['user_salt'];
                const pwHash = await Encrypt.createHash(userPW, originPwSalt);

                if(originPwHash !== pwHash) {
                    // PW does not match
                    CustomUtils.logWithTime(`Password does not match [${socket.id}]`);
                    socket.emit(Constants.signInResult, Constants.WrongPw);
                } else {
                    // delete unnecessary columns
                    delete userInfoRow['user_pw'];
                    delete userInfoRow['user_salt'];

                    const uid = userInfoRow['uid'];
                    const gameDataRow = await database.query(queries.api.getGameData, [uid]);
    
                    if(gameDataRow == null) {
                        // game data does not exist
                        CustomUtils.logWithTime(`Fail to sign in to server {game data does not exist} [${socket.id}]`);
                        socket.emit(Constants.signInResult, Constants.Fail);
                    } else {
                        // success, send data to client
                        CustomUtils.logWithTime(`Success to sign in server [${socket.id}]`);
                        socket.emit(Constants.signInResult, Constants.Success);
                        socket.emit(Constants.sendUserInfo, userInfoRow);
                        socket.emit(Constants.sendGameData, gameDataRow);
                    }
                }
            }
        });


        
        /**
         * socket event [sign up]
         */
        socket.on(Constants.signUp, async (dto) => {
            CustomUtils.logWithTime(`Client requests 'sign up' [${socket.id}]`);
            const userID = dto['userID'];
            const userPW = dto['userPW'];
            const userName = dto['userName'];
            const userNickName = dto['userNickName'];
            const userBirthday = dto['userBirthday'];
            const userSalt = await Encrypt.createSalt(8);
            const uid = await Encrypt.createHash(userID, userSalt);
            const pwHash = await Encrypt.createHash(userPW, userSalt);
            let userInfoRow = await database.query(queries.api.signIn, [userID]);

            if(userInfoRow != null) {
                // account already exist
                CustomUtils.logWithTime(`Fail to sign up {account already exist} [${socket.id}]`);
                socket.emit(Constants.signUpResult, Constants.AlreadyExist);
            } else {
                const queryList = [];
                queryList.push(queries.api.signUp);
                queryList.push(queries.api.createGameData);

                const paramList = [];
                paramList.push([uid, userID, pwHash, userSalt, userName, userNickName, userBirthday]);
                paramList.push([uid, 0, 0, 0]);

                const result = await database.multiQuery(queryList, paramList);
                if(result) {
                    // success, send data to client
                    CustomUtils.logWithTime(`Success to sign up [${socket.id}]`);
                    socket.emit(Constants.sendUserInfo, Constants.Success);
                } else {
                    CustomUtils.logWithTime(`Fail to sign up [${socket.id}]`);
                    socket.emit(Constants.sendUserInfo, Constants.Fail);
                }
            }
        });


        /**
         * socket event [auth code]
         */
         socket.on(Constants.authCode, async (dto) => {
            CustomUtils.logWithTime(`Client request 'auth code' [${socket.id}]`);
            const userID = dto['userID'];
            const authCode = await Encrypt.createAuthCode();
            const result = await mailer.sendAuthCode(userID, authCode);

            if(result) {
                CustomUtils.logWithTime(`Success to send auth code [${socket.id}]`);
                socket.emit(Constants.authCodeResult, code);
            } else {
                CustomUtils.logWithTime(`Fail to send auth code [${socket.id}]`);
                socket.emit(Constants.authCodeResult, Constants.Fail);
            }
        });
    }
}

module.exports = new SocketEvent();