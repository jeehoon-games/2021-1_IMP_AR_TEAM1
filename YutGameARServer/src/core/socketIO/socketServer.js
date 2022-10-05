/**
 * @file socketServer.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */


/* Modules */
require('dotenv').config();
const Http = require('http');
const express = require('@base/express/index');
const SocketIO = require('@base/socket.io/lib/index');
const CustomUtils = require('@src/utils/customUtils');
const socketEvent = require('@src/core/socketIO/socketEvent');


/**
* @class SocketIOClient
* @description socket.io server based on http, express
*/
class SocketServer {
    #httpServer;
    #expressApp;
    #socketIO;

    /**
     * @function isInit
     * @description Returns whether the server is initialized or not.
     * @returns {boolean}
     */
    isInit() {
        return this.#socketIO !== null;
    }

    /**
     * @function init
     * @description Initialize the server
     */
    init(opts = {}) {
        // create express app
        this.#expressApp = express();

        // attach app to http server
        this.#httpServer = Http.createServer(this.#expressApp);

        // attach http server to socket io
        this.#socketIO = new SocketIO().attach(this.#httpServer, opts);

        this.#expressApp.get('/', (req, res) => {
            res.send('Yut Game AR server is running');
        });
    }

    /**
     * @async
     * @function run
     * @description Run the socket.io server
     */
    run() {
    // listen http server
        this.#httpServer.listen(process.env.SERVER_PORT, () => {
            CustomUtils.logWithTime(`Yut Game AR server running on port ${process.env.SERVER_PORT}`);
        });

        // io event: client connection
        this.#socketIO.on('connection', socketEvent.onConnect);
    }
}

module.exports = SocketServer;