import { Inject, Injectable } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Post } from '../entities/Post';
@Injectable()
export class PostService {

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }
    getPosts() {
        return this.http.get(this.baseUrl + "api/Post");
    }
    postPost(content: string, files: File[]) {
        const formData: FormData = new FormData();
        formData.append('content', content);
        if (files.length > 0) {
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i], files[i].name);
            }
        }
        return this.http.post(this.baseUrl + "api/Post/PostSubmission", formData);
    }
    getPreviewSubPost(post_id: string, sub_post_id: string) {
        return this.http.get(this.baseUrl + "api/Post/GetSubPost?post_id=" + post_id + "&sub_post_id=" + sub_post_id);
    }
    editPost(post_id: string, content: string, newfiles: File[], oldFlies: Post[]) {
        const formData: FormData = new FormData();
        formData.append('post_id', post_id);
        formData.append('content', content);
        if (newfiles.length > 0) {
            for (var i = 0; i < newfiles.length; i++) {
                formData.append('news', newfiles[i], newfiles[i].name);
            }
        }
        if (oldFlies.length > 0) {
            for (var i = 0; i < oldFlies.length; i++) {
                formData.append('olds', oldFlies[i]._id);
            }
        }
        return this.http.post(this.baseUrl + "api/Post/PostEditting", formData);
    }
    removePost(post_id: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/Post/RemovePost", query, requestOptions);
    }

}