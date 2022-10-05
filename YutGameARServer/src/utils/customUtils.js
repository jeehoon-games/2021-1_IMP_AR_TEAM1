/**
 * @file customUtils.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/* Modules */
const yyyymmdd = require('@base/yyyy-mm-dd/index');
const moment = require('@base/moment');
require("moment-timezone");
moment.tz.setDefault("Asia/Seoul");


/**
 * @class CustomUtils
 */
class CustomUtils {
    /**
     * @static
     * @function logWithTime
     * @description Print the log with current time
     * @param {string} msg msg to print
     */
    static logWithTime(msg) {
        console.log(` # ${msg} - [${yyyymmdd.withTime()}]`);
    }

    /**
     * @static
     * @function dateToSec
     * @param {string} date date format (string)
     * @returns 
     */
    static dateToSec(date) {
        if(date === null) return 0;
        return Math.floor(new Date(date).getTime() / 1000);
    }

    /**
     * @static
     * @function getCurrTime
     * @returns 
     */
    static getCurrTime() {
        return moment().format("YYYY-MM-DD HH:mm:ss");
      }
}

module.exports = CustomUtils;