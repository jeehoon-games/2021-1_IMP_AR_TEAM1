/**
 * @file mysql.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

class User {
    #id

    clear() {
        this.#id = null;
    }

    static createEmpty() {
        return new User();
    }
}

module.exports = User;