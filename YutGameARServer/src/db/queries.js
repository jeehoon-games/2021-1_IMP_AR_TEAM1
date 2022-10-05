/**
 * @file queries.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

 module.exports = {
    api: {
        signIn: 'SELECT * FROM userinfo_table WHERE user_id=?',
        signUp: 'INSERT INTO userinfo_table (uid, user_id, user_pw, user_salt, user_name, user_nickname, user_birthday) VALUES (?, ?, ?, ?, ?, ?, ?)',
        getGameData: 'SELECT * FROM record_table WHERE uid=?',
        createGameData: 'INSERT INTO record_table (uid, user_wins, user_defeats, user_rankpoints) VALUES (?, ?, ?, ?)',
    },
};
