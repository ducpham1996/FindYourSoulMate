import { Inject, Injectable } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class CommentService {

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }
    postComment(post_id: string, sub_post_id: string, comment_text: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"comment_text\" : \"" + comment_text + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/Comment/InsertComment", query, requestOptions);
    }
    getComments(post_id: string, sub_post_id: string, no_of_comment: number) {
        var url = "api/Comment/GetComments?post_id=" + post_id;
        if (sub_post_id != "") {
            url += "&sub_post_id=" + sub_post_id;
        }
        return this.http.get(this.baseUrl + url + "&noOfcomment=" + no_of_comment);
    }
    deleteComment(post_id: string, sub_post_id: string, comment_id: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"comment_id\" : \"" + comment_id + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/Comment/DeleteComment", query, requestOptions);
    }

    editComment(post_id: string, sub_post_id: string, comment_id: string, comment_text: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"comment_id\" : \"" + comment_id + "\", \"comment_text\" : \"" + comment_text + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/Comment/EditComment", query, requestOptions);
    }
}