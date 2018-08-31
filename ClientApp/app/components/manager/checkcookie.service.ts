import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { CookieService } from 'angular2-cookie/core';
import { Router } from "@angular/router";

@Injectable()
export class CheckCookieService {

    private cookieValue: string = "User";
    constructor(private _cookieService: CookieService, private router: Router) {

    }
    checkCookies() {
        if (typeof this._cookieService.get(this.cookieValue) !== 'undefined') {
            console.log(this._cookieService.get(this.cookieValue));
        } else {
            this.router.navigate(['/login']);
        }
    }
    getUserCookie() {
        if (typeof this._cookieService.get(this.cookieValue) !== 'undefined') {
            var data = JSON.parse(this._cookieService.get(this.cookieValue));
            return data.value.data;
        } else {
            return this.router.navigate(['/login']);
        }
    }
    removeCookie() {
        this._cookieService.remove(this.cookieValue);
        //this.router.navigate(['/login']);
    }
}