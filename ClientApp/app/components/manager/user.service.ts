import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { CookieService } from 'angular2-cookie/core';
import { Router } from "@angular/router";
import { Http } from '@angular/http';

@Injectable()
export class UserService {

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }
    getUserProfile(_id: string) {
        return this.http.get(this.baseUrl + "api/Profile/getProfile?_id=" + _id);
    }
}
