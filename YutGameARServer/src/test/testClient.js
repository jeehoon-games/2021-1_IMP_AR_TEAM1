/**
 * @file testClient.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - socket.io client for testing api server
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/* Modules */
const CustomUtils = require('@src/utils/customUtils');
const Constants = require('@src/core/socketIO/socketEventConstants');

/**
 * @class SocketIOClient
 */
 class TestClient {
    #io;

    run(uri) {
        this.#io = require('@base/socket.io-client')(uri);
        this.#io.on('connect', () => {
            CustomUtils.logWithTime(`Client connects to server [${this.#io.id}]`);
            this.signUpTest();
        });
    }

    signInTest() {
        this.#io.emit(Constants.signIn, {
            userID: 'abc3@naver.com', 
            userPW: 'abab12',
        });

        this.#io.on(Constants.sendUserInfo, (dto) => {
            CustomUtils.logWithTime(`sign in result1: ${this.#io.id}`);
            console.log(dto);
        });

        
        this.#io.on(Constants.sendGameData, (dto) => {
            CustomUtils.logWithTime(`sign in result2: ${this.#io.id}`);
            console.log(dto);
        });
    }

    signUpTest() {
        this.#io.emit(Constants.signUp, {
            userID: 'abc3@naver.com', 
            userPW: 'abab12',
            userName: 'abc',
            userNickName: 'nick',
            userBirthday: '19970608',
        });

        this.#io.on(Constants.signUpResult, (dto) => {
            CustomUtils.logWithTime(`sign up result: ${this.#io.id}`);
            console.log(dto);
        });
    }
}

module.exports = TestClient;