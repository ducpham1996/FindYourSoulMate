import { Component, Inject } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Login } from './Login';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'test',
    templateUrl: './test.component.html'
})
export class TestComponent {
    filelist: string[] = [];
    fupload: any;
    url: any;
    public message: string;
    private login: Login;
    private logins: Array<Login>;
    private tmp_files: File[] = [];
    private urlval: any;
    private files: FileList | undefined;
    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.message = "";
        this.login = new Login("", "");
        this.logins = [
            new Login("12", "123"),
            new Login("12", "123"),
            new Login("12", "123"),
            new Login("12", "123")
        ]
    }
    getData(name: string) {
        this.http.get(this.baseUrl + "api/Test/GetTest?id=" + name).subscribe(result => {
            var obj = result.json().data;
            this.message = "" + result.json();
        }, error => console.error(error))
    }
    postData(authorization: Login) {
        var body = JSON.stringify(authorization);
        const formData: FormData = new FormData();
        for (var i = 0; i < this.tmp_files.length; i++) {
            formData.append('files', this.tmp_files[i], this.tmp_files[i].name);
        }
        formData.append('UserName', authorization.UserName);
        formData.append('Password', authorization.Password);
        var headerOptions = new Headers({ 'Content-Type': 'multipart/form-data' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        this.http.post(this.baseUrl + "api/Test", formData).subscribe(result => {
            console.log(result.text());
        }, error => console.error(error));
    }
    onSubmit(form: NgForm) {

        //this.message = form.value;

        this.postData(this.login);
    }
    onSearch(value: string) {
        alert(value);
    }

    onChange(event: any) {
        //this.fupload = event.srcElement.files;  // not working in firefox
        this.fupload = event.target.files;
        // to get the files in array
        //this.tmp_files.push(event.target.files);
        this.files = event.target.files;
//this.tmp_files = event.target.files;
        //console.log(event.target.files.length+' <> '+this.dind.length);
        this.urlval = event.target.files[0].name;
        for (let i = 0; i < event.target.files.length; i++) {
            //console.log(event.target.files[i]);
            this.tmp_files.push(event.target.files[i]);
            if (event.target.files && event.target.files[i]) {
                var reader = new FileReader();
                reader.onload = (event: any) => {
                    this.url = event.target.result;
                    this.filelist.push(this.url); 
                }
                reader.readAsDataURL(event.target.files[i]);
            }
        }
        console.log(this.tmp_files);
    }
}