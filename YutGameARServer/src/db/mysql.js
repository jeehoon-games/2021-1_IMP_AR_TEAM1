/**
 * @file mysql.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/* Modules */
const mysql2 = require('@base/mysql2/promise');
const config = require('@root/cfg/config');
const CustomUtils = require('@src/utils/customUtils');
const Cache = require('@src/utils/cache');


/**
 * @class mysql
 */
 class MySQL {
    #connPoll;
    #cache;
    #keyStack;
    #RE_CONN_DELAY = 1000;  // 1 sec
    #MAX_RE_CONN_COUNT = 10;

    /**
     * @constructor
     * @property connPoll
     * @property connection
     */
    constructor() {
        this.#connPoll = null;
        this.#cache = null;
        this.#keyStack = [];
    }

    /**
     * @private
     * @function getConnPoll
     * @description get connection poll
     * 
     * @returns {mysql2.Pool}
     */
    #getConnPoll() {
        return this.#connPoll;
    }

    /**
     * @async @function init
     * @description initialize MySQL object
     * 
     * @function init
     */
    async init() {
        if(this.#connPoll == null) {
            this.#connPoll = mysql2.createPool(config.database);
            this.#cache = new Cache();
            for(let i = config.database.connectionLimit - 1; i > -1; i--) {
                this.#keyStack.push(i);
            }
        }
    }

    /**
     * @function available
     * @description Check that whether object can get a connection or not (whether connection poll is not full)
     * 
     * @returns {boolean}
     */
    #available(){
        return this.#cache.size() < config.database.connectionLimit;
    }
    
    /**
     * @function connExist
     * @description check that instance has any connection
     */
    connExist() {
        return this.#cache.size() > 0;
    }
    
    /**
     * @function clear
     * @description clear this instance
     */
    clear() {
        this.#connPoll = null;
        this.#cache.clear();
        this.#cache = null;
    }

    /**
     * @function dump
     * @description print the MySQL object information
     */
    dump() {
        CustomUtils.logWithTime(`Current connections: ${this.#cache.keys()}`);
        CustomUtils.logWithTime(`Current keyStack: ${this.#keyStack}`);
    }

    /**
     * @function getConnection
     * @description get database connection
     * 
     * @returns {[number, mysql2.Connection]} [connection key, connection object]
     */
    async getConnection(reConnCount = 0) {
        if(reConnCount == this.#MAX_RE_CONN_COUNT) {
            CustomUtils.logWithTime(`Maximum db connection error exceeded`);
            return [-1, null];
        }

        if(!this.#available()) {
            CustomUtils.logWithTime(`Maximum db connection - cannot get connection`);
            return [-1, null];
        }
        
        try {
            if(this.#getConnPoll() == null) {
                CustomUtils.logWithTime(`Initialize the database objet first`);
                return null;
            }
            const conn = await this.#getConnPoll().getConnection();
            const key = this.#keyStack.pop();
            this.#cache.add(key, conn);
            return [key, conn];
        } catch(err) {
            CustomUtils.logWithTime(`DB connection error - reconnection count: ${++reConnCount}`);
            setTimeout(() => {this.getConnection(reConnCount)}, this.#RE_CONN_DELAY);
        }
    }

    /**
     * Removes connection from cache (also release in mysql connection poll)
     * Return the key value to the stack for recycling
     * @param {number} connKey 
     * @returns 
     */
    removeConnection(connKey) {
        if(!this.#cache.contains(connKey)) return;
        
        try {
            const conn = this.#cache.get(connKey);
            conn.release();
            this.#cache.remove(connKey);
            this.#keyStack.push(connKey);
        } catch(err) {
            console.error(e);
        }
    }
}

module.exports = MySQL;