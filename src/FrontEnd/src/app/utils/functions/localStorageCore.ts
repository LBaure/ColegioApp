import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
var CryptoJS = require("crypto-js");
const PUBLICKEY = environment.apiDecryptKey;

@Injectable({
  providedIn: 'root'
})

export class localStorageCore {
  constructor () {}

  /**
   * setItem
   */
  public setItem(key: string, value: string) {
    var keyPublic = CryptoJS.enc.Utf8.parse(PUBLICKEY);
    var iv = CryptoJS.enc.Utf8.parse(PUBLICKEY);
    var config = {
      keySize: 128 / 8,
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    };

    const KEY_STORAGE = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(key), keyPublic, config).toString();
    const VALUE_STORAGE = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(value), keyPublic, config).toString();
    localStorage.setItem(KEY_STORAGE, VALUE_STORAGE);
  }

  public getItem(keyName: string) {
    var keyPublic = CryptoJS.enc.Utf8.parse(PUBLICKEY);
    var iv = CryptoJS.enc.Utf8.parse(PUBLICKEY);
    var config = {
      keySize: 128 / 8,
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    };

    const KEY_STORAGE = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(keyName), keyPublic, config).toString();
    const data = localStorage.getItem(KEY_STORAGE) ?? "";
    const plainText = CryptoJS.AES.decrypt(data, keyPublic, config);
    return  plainText.toString(CryptoJS.enc.Utf8);
  }

}
