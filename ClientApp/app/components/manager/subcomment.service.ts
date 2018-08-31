import { Inject, Injectable } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class SubCommentService {

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }
    postComment(post_id: string, sub_post_id: string, parent_comment_id: string, comment_text: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"parent_comment_id\" : \"" + parent_comment_id + "\",\"reply_text\" : \"" + comment_text + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/SubComment/CreateSubComment", query, requestOptions);
    }
    getSubComments(post_id: string, sub_post_id: string, comment_id: string, no_of_comment: number) {
        var url = "api/SubComment/GetSubComments?post_id=" + post_id;
        if (sub_post_id != "") {
            url += "&sub_post_id=" + sub_post_id;
        }

        return this.http.get(this.baseUrl + url + "&comment_id=" + comment_id + "&noOfcomment=" + no_of_comment);
    }
    deleteSubComment(post_id: string, sub_post_id: string, comment_id: string, sub_comment_id: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"comment_id\" : \"" + comment_id + "\", \"sub_comment_id\" : \"" + sub_comment_id + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/SubComment/DeleteSubComment", query, requestOptions);
    }

    editSubComment(post_id: string, sub_post_id: string, comment_id: string, sub_comment_id: string, comment_text: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"comment_id\" : \"" + comment_id + "\",\"sub_comment_id\": \"" + sub_comment_id + "\",\"comment_text\" : \"" + comment_text + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/SubComment/EditSubComment", query, requestOptions);
    }
}