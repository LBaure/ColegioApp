import { environment } from "src/environments/environment";
var CryptoJS = require("crypto-js");

var publicKey = environment.apiDecryptKey;
export function encode(params: any) {

    var str:string;
    var key = CryptoJS.enc.Utf8.parse(publicKey);
    var iv = CryptoJS.enc.Utf8.parse(publicKey);
    var config = {
      keySize: 128 / 8,
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    }

    if (typeof params === 'string'){
      return str =   CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(params), key, config).toString();
    }
    if (typeof params === 'object') {
        return str = CryptoJS.AES.encrypt(JSON.stringify(params), key, config).toString();
    }
}
