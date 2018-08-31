import { Component, Inject,Injectable } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Login } from '../entities/Login';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { CookieService, CookieOptions } from 'angular2-cookie/core';
import { Router } from "@angular/router";

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
@Injectable()
export class LoginComponent {
    private login: Login;
    private loading: boolean = false;
    private message: string = "";
    private cookieValue = "User";
    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string, private _cookieService: CookieService, private router: Router) {
        this.login = new Login("", "", false);
    }

    onLogin() {
        this.loading = true;
        var body = JSON.stringify(this.login);
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        this.http.post(this.baseUrl + "api/Authorization/Authorization", body, requestOptions).subscribe(result => {
            this.message = "Login successfully";
            if (this.login.IsRemember) {
                var expireDate = new Date();
                expireDate.setDate(expireDate.getDate() + 30);
                var cookieOptions = new CookieOptions({ expires: expireDate.toUTCString() })
                this._cookieService.put(this.cookieValue, result.text(), cookieOptions);
            }
            this.loading = false;
            this.router.navigate(['/home']);
        }, error => {
            console.error(error);
            this.loading = false;
            this.message = error.text();           
        })

    }
}