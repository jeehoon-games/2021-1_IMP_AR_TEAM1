/**
 * @file abstractSocketEvent.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Socket.io events for android
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/* Modules */
const User = require('@src/user/user');
const Cache = require('@src/utils/cache');
const CustomUtils = require('@src/utils/customUtils');

let socketUserCache;   // key: socket.id , value: user object
let userSocketCache;   // key: user's id , value : socket object

/**
 * @class AbstractSocketEvent
 */
class AbstractSocketEvent {
    constructor() {
        socketUserCache = new Cache();
        userSocketCache = new Cache();
    }

    /**
     * @Override
     * @function onConnect
     * @description Callback called when a socket is connected
     * @param {Socket} socket 
     */
    onConnect(socket) {
        CustomUtils.logWithTime(`Client connect ${socket.id}`);
        socketUserCache.add(socket.id, User.createEmpty());

        socket.on('disconnect', () => {
            CustomUtils.logWithTime(`Client disconnect ${socket.id}`);
            socketUserCache.get(socket.id).clear();
            socketUserCache.remove(socket.id);
        });
    }
}

module.exports = AbstractSocketEvent;