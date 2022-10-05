/**
 * @file encrypt.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

/* Module */
const util = require('util');
const crypto = require('crypto');

/**
 * @class Encrypt
 */
class Encrypt {
    /**
     * @static
     * @async
     * @function createAuthCode
     * @returns random authentication code
     */
    static async createAuthCode() {
        const randomBytes = await util.promisify(crypto.randomBytes)(4);
        const number = parseInt(randomBytes.toString('hex'), 16);
        return number.toString();
    }

    /**
     * @async @function createSalt
     * @description Create random salt string.
     *
     * @param {number} size Size of random bytes. (default: 8)
     * @returns {promise<string>} Promise of salt string. (ex. arg size: 16, -> return salt string size: 32 (x2))
     * @example
     * In async function,
     * ...
     * const salt = await encryption.createhSalt();
     * ...
     */
     static async createSalt(size = 4) {
        let salt = null;
        try {
            salt = await util.promisify(crypto.randomBytes)(size);
            salt = salt.toString('hex');
        } catch (err) {
            console.error(err);
        } finally {
            return salt;
        }
    }

    /**
     * @async @function createHash
     * @description Encrypt the string using hash algorithm.
     *
     * @param {string} str String to encrypt.
     * @param {string} salt String to be used as salt.
     * @returns {promise<string>} Promise of hash string.
     * @example
     * In async function,
     * ...
     * const salt = await encryption.createSalt();
     * const hash = await encryption.createHash('string to encrypt', salt);
     * ...
     */
    static async createHash(str, salt='') {
        let hash = null;
        try {
            hash = await util.promisify(crypto.pbkdf2)(str, salt, 100000, salt.length, 'sha512');
            hash = hash.toString('hex');
        } catch (err) {
            console.error(err);
        } finally {
            return hash;
        }
    }
}

module.exports = Encrypt;