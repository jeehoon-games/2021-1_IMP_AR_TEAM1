/**
 * @file config.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */


/* Modules */
require('dotenv').config();

module.exports = {
    // node mailer configuration
    mailer: {
        service: process.env.MAILER_SERVICE,
        host: process.env.MAILER_HOST,
        port: process.env.MAILER_PORT,
        secure: true,
        auth: { 
            user: process.env.MAILER_ID,
            pass: process.env.MAILER_PW,
        },
    },

    // database configuration
    database: {
        host     : process.env.DB_HOST,
        port     : process.env.DB_PORT,
        user     : process.env.DB_USER,
        password : process.env.DB_PASSWORD,
        database : process.env.DB_NAME ,
        waitForConnections: true,
        connectionLimit: 10,
    },
};
