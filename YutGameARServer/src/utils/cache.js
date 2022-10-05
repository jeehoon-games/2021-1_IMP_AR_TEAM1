/**
 * @file cache.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/**
 * @class Cache
 */
 class Cache {
    #cache;

    constructor() {
        this.#cache = {};
    }

    /**
     * @function add
     * @description add new key - value pair to cache (dictionary)
     * 
     * @param {*} key 
     * @param {*} value 
     */
    add(key, value) {
        this.#cache[key] = value;
    }

    /**
     * @function remove
     * @description remove element by key
     * 
     * @param {*} key 
     */
    remove(key) {
        delete this.#cache[key];
    }

    
    /**
     * 
     * @param {*} key 
     * @returns 
     */
    get(key) {
        const value = this.#cache[key];
        return value === undefined ? null : value;
    }

    /**
     * 
     * @returns 
     */
    keys() {
        return Object.keys(this.#cache);
    }

    /**
     * 
     * @returns 
     */
    size() {
        return this.keys().length;
    }

    /**
     * 
     * @param {*} key 
     * @returns 
     */
    contains(key) {
        return key in this.#cache;
    }

    /**
     * 
     */
    clear() {
        this.#cache = null;
    }

    /**
     * 
     * @returns 
     */
    getKeyFirst() {
        return this.keys()[0];
    }

    /**
     * 
     * @returns 
     */
    getKeyLast() {
        return this.keys()[this.size() - 1];
    }
}

module.exports = Cache;