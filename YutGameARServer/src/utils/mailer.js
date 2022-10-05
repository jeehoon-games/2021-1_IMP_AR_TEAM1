/**
 * @file mailer.js
 * @author arcane222, Lee Hong Jun
 * @description
 *  - Version: 1.0.0
 *  - Last Modified: 2022.10.03
 */

 const Encrypt = require('@src/utils/encrypt');
 const mailer = require('@base/nodemailer');
 const config = require('@root/cfg/config');
 
 class Mailer {
     /**
      * @function constructor
      * @description constructor of Mailer class (Initialize nodemailer's transporter - read config file)
      */
     constructor() {
         this.transporter = mailer.createTransport(config.mailer);
     }
 
     /**
      * @async
      * @function sendMail
      * @description send e-mail to destination.
      *
      * @param {string} dest Destination for sending e-mail. (ex. user's e-mail address)
      * @param {string} subject The title of the e-mail.
      * @param {string} text Text to be displayed in e-mail.
      * @throws {error}
      */
      async sendMail(dest, subject, html) {
         const mailOps = {
             from: config.mailer.auth.user,
             to: dest,
             subject,
             html,
         };
 
         try {
             await this.transporter.sendMail(mailOps);
             this.transporter.close();
         } catch (err) {
             throw err;
         }
     }
 
     /**
      * @async @function sendAuthCode
      * @description Send authentication code e-mail to destination.
      *
      * @param {string} dest Destination for sending authentication e-mail. (ex. user's e-mail address)
      * @param {string} code Auth code to send to client
      * @throws {error}
      */
     async sendAuthCode(dest, code) {
         let result = false;
         try {
             const subject = 'YutGameAR Service: Authentication Code';
             const html =         
             `<font size="4" face="Arial" color="900c3f"> YutGameAR: Authentication Code </font>
               <br>
              <font size="2" face="Arial"> Hello, ${dest} </font>
               <br>
              <font size="2" face="Arial"> Thank you for playing our game. </font>
               <br>
              <font size="2" face="Arial"> This is your authentication code. </font>
               <br>
              <font size="2" face="Arial"> Code: </font> <font size="4" face="Arial"> ${code} </font>
               <br>
              <font size="2" face="Arial" color="F00000"> "DO NOT SHOW THE CODE TO OTHERS </font>
               <br>`;

             await this.sendMail(dest, subject, html);
             result = true;
         } catch (err) {
             throw err;
         } finally {
             return result;
         }
     }
 }
 
 module.exports = new Mailer()