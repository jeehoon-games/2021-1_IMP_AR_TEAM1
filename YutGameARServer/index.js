/**
 * @file index.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Node.js (socket.io) server for YutGameAR (Unity)
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/* Modules */
require('module-alias/register');
require('dotenv').config();
const SocketServer = require('@src/core/socketIO/socketServer');
const TestClient = require('@src/test/testClient');
const server = new SocketServer();
const client = new TestClient();

class Main {
    /**
     * @static
     * @function run
     */
    static async run(type) {
        try {
            switch(type) {
                case 'server':
                    server.init();
                    server.run();
                    break;

                case 'client':
                    client.run(process.env.SERVER_URI);
                    break;
            }
        } catch(e) {
            console.error(e);
        }
    }
}

const type = process.argv.slice(2)[0];
Main.run(type);
