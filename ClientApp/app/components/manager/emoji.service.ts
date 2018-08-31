import { Inject, Injectable } from '@angular/core';
import { Http, Response, RequestOptions, RequestMethod, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
@Injectable()
export class EmojiService {

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }
    postLike(post_id: string, sub_post_id: string, emo: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"emo\" : \"" + emo + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/Emoji/PostLike", query, requestOptions);
    }
    commentLike(post_id: string, sub_post_id: string, comment_id: string, emo: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"comment_id\" : \"" + comment_id + "\",\"emo\" : \"" + emo + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/Emoji/CommentLike", query, requestOptions);
    }

    subCommentLike(post_id: string, sub_post_id: string, comment_id: string, subcomment_id: string, emo: string) {
        var query = "{ \"post_id\" : \"" + post_id + "\"";
        if (sub_post_id != "") {
            query += ",\"sub_post_id\" : \"" + sub_post_id + "\"";
        }
        query += ",\"comment_id\" : \"" + comment_id + "\",\"sub_comment_id\" : \"" + subcomment_id + "\",\"emo\" : \"" + emo + "\"}";
        var headerOptions = new Headers({ 'Content-Type': 'application/json' });
        var requestOptions = new RequestOptions({ method: RequestMethod.Post, headers: headerOptions });
        return this.http.post(this.baseUrl + "api/Emoji/SubCommentLike", query, requestOptions);
    }
}