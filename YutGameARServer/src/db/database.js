/**
 * @file database.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */


/* Modules */
const mysql2 = require('@base/mysql2/promise');
const MySQL = require('@src/db/mysql');
const Cache = require('@src/utils/cache');
const CustomUtils = require('@src/utils/customUtils');


/**
 * @class Database
 */
class Database {
    #keyCount;
    #cache;

    /**
     * @constructor
     * @property #MAX_CACHE_SIZE
     * @property #cache
     */
    constructor() {
        this.#keyCount = 0;
        this.#cache = new Cache();
    }

    dump() {
        CustomUtils.logWithTime(`Bucket size: ${this.#cache.size()}`);
        for(let key of this.#cache.keys()) {
            const instance = this.#cache.get(key);
            CustomUtils.logWithTime(` --- bucket ${key} --- `);
            instance.dump();
            console.log('\n');
        }
    }

    /**
     * 
     * @returns 
     */
    async #createInstance() {
        const instance = new MySQL();
        await instance.init();
        this.#cache.add(this.#keyCount++, instance);
        return this.#keyCount - 1;
    }


    /**
     * 
     * @param {*} instKey 
     * @returns 
     */
    async #removeInstance(instKey) {
        if(!this.#cache.contains(instKey)) return;

        const instance = this.#cache.get(instKey);
        if(instance.connExist()) {
            CustomUtils.logWithTime(`Cannot remove MySQL instance (The connection still exists.)`);
        } else {
            CustomUtils.logWithTime(`MySQL instance ${instKey} removed from database cache`);
            instance.clear();
            this.#cache.remove(instKey);
        }
    }

    /**
     * 
     * @returns {[number, number, mysql2.Connection]}
     */
    async #findConnection() {
        let conn = null;
        let instKey = null;
        let connKey = null;

        const keys = this.#cache.keys();
        for(instKey of keys) {
            const instance = this.#cache.get(instKey);
            [connKey, conn] = await instance.getConnection();
            if(conn != null) break;
        }

        if(conn == null) {
            instKey = await this.#createInstance();
            const instance = this.#cache.get(instKey);
            [connKey, conn] = await instance.getConnection();
        }

        return [instKey, connKey, conn];
    }

    /**
     * 
     * @param {*} queryStr 
     * @param {*} params 
     * @returns 
     */
    async query(queryStr, params = null) {
        try {
            const [instKey, connKey, conn] = await this.#findConnection();
            if(conn == null) return null;

            let rows = null;
            try {
                //begin transaction & commit
                await conn.beginTransaction();
                rows = (await conn.execute(queryStr, params))[0][0];
                await conn.commit();
            } catch(err) {
                // error occurred - rollback
                CustomUtils.logWithTime(err);
                await conn.rollback();
            } finally {
                this.#cache.get(instKey).removeConnection(connKey);
                return rows;
            }
        } catch(err) {
            CustomUtils.logWithTime(err);
            return null;
        }
    }

    /**
     * 
     * @param {*} queryStr 
     * @param {*} params 
     * @returns 
     */
    async multiQuery(queryList, paramList) {
        try {
            const [instKey, connKey, conn] = await this.#findConnection();
            if(conn == null) return null;

            let result = false;
            try {
                //begin transaction & commit
                await conn.beginTransaction();
                for(let i = 0; i < queryList.length; i++) {
                    const query = queryList[i];
                    const param = paramList[i];
                    await conn.execute(query, param);
                }
                await conn.commit();
                result = true;
            } catch(err) {
                // error occurred - rollback
                CustomUtils.logWithTime(err);
                await conn.rollback();
                result = false;
            } finally {
                this.#cache.get(instKey).removeConnection(connKey);
                return result;
            }
        } catch(err) {
            CustomUtils.logWithTime(err);
            return false;
        }
    }
}


module.exports = new Database();