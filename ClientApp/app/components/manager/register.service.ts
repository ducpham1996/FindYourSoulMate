import { Inject, Injectable } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Register } from '../entities/Register';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class RegisterService {

    
    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }
    checkEmail(email: string): boolean {
        var flag = false;
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        this.http.post(this.baseUrl + "api/Register/CheckEmail", "{ email:'" + email + "'}", requestOptions).subscribe(result => {
            flag = true;
        }, error => {
            console.log(error.text());
        })
        return flag;
    }
    register(register: Register) {
        const formData: FormData = new FormData();
        formData.append('first_name', register.first_name);
        formData.append('last_name', register.last_name);
        formData.append('birth_date', register.birth_date);
        if (register.profile_img[0] != null) {
            formData.append('profile_img', register.profile_img[0], register.profile_img[0].name);
        }
        formData.append('gender', register.gender + "");
        formData.append('phone_number', register.phone_number);
        formData.append('email', register.email);
        formData.append('password', register.password);
        return this.http.post(this.baseUrl + "api/Register/Registration", formData);           
    }

}