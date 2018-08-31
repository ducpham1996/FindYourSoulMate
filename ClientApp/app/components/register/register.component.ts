import { Component, Inject, Injectable } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Register } from '../entities/Register';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { RegisterService } from '../manager/register.service';

@Component({
    selector: 'register',
    templateUrl: './register.component.html'
})
@Injectable()
export class RegisterComponent {
    private loading: boolean = false;
    private message: string = "";
    private error: string = "";
    private register: Register = new Register;
    private file: string = "";
    private fUpload: File[] = [];
    private typingTimer: any;      
    private doneTypingInterval: number = 2000;
    private isValidEmail: boolean = false;
    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string, private register_service: RegisterService) {

    }
    onRegister() {
        this.register.profile_img = this.fUpload;
        this.register_service.register(this.register).subscribe(result => {
            console.log(result.text());
            this.message = result.text();
        }, error => {
            console.error(error);
            this.error = error.text();
        });;
    }
    onChange(event: any) {
        this.fUpload = event.target.files;
        if (event.target.files && event.target.files[0]) {
            var reader = new FileReader();
            reader.onload = (event: any) => {
                this.file = event.target.result;
            }
            reader.readAsDataURL(event.target.files[0]);
        }
    }
    onKeyUp(form: any) {
        if (this.register.email != "") {
            this.loading = true;
            form.form.controls['email'].setErrors({ 'error': true });
        }
        clearTimeout(this.typingTimer);
        this.typingTimer = setTimeout(result => {
            this.doneTyping(form);
        }, this.doneTypingInterval);
    }
    onKeyDown(form: any) {
        clearTimeout(this.typingTimer);
    }
    onPassWordKeyUp(form: any) {
        if (this.register.reenter_password != this.register.password) {
            form.form.controls['reenter_password'].setErrors({ 'isMatch': true });
        }
    }

    doneTyping(form: any) {
        if (this.register.email != "") {
            var body = JSON.stringify(this.register);
            var headerOptions = new Headers({ 'Content-Type': 'application/json' });
            var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
            this.http.post(this.baseUrl + "api/Register/CheckEmail", "{ \"email\":\"" + this.register.email + "\"}", requestOptions).subscribe(result => {
                this.loading = false;
                form.form.controls['email'].setErrors(null);
            }, error => {
                this.loading = false;
                form.form.controls['email'].setErrors({ 'isExist': true });
            })
        }
    }
}