import { Component, Injectable, Input, ViewEncapsulation } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { CheckCookieService } from '../manager/checkcookie.service';
import { CommentComponent } from '../commentdisplay/comment.component';
import { Post } from '../entities/Post';
import { CommentService } from '../manager/comment.service';
import { SubCommentService } from '../manager/subcomment.service';
import { EmojiService } from '../manager/emoji.service'
import { ConnectionBackend } from '@angular/http';

@Component({
    selector: 'postaction',
    templateUrl: './postaction.component.html',
    styleUrls: ['../css/comments.css', '../css/faceMocion.css'],
    encapsulation: ViewEncapsulation.None
})
@Injectable()
export class PostActionComponent   {

    @Input()
    displaycomment_component: CommentComponent;

    @Input()
    post: Post;

    @Input()
    sub_post: Post;

    @Input()
    tmp_post: Post;
    disableGetComment = false;
    iconHtml = "<i class='glyphicon glyphicon-thumbs-up'> Like</i>";

    constructor(private checkcookies_service: CheckCookieService, private comment_service: CommentService, private subcomment_service: SubCommentService, private emoji_service: EmojiService) {
        this.displaycomment_component = new CommentComponent(this.checkcookies_service, this.comment_service, subcomment_service, this.emoji_service);
        this.post = new Post;
        this.sub_post = new Post;
        this.tmp_post = new Post;
    }
    activeGetComments() {
        this.disableGetComment = false;
    }
    displayComments() {
        this.displaycomment_component.getComments();
        this.disableGetComment = true;
    }

    onChoseEmoji(e: any, p_id: string) {
        var timer;
        var delay = 600;
        var faceIcon = (<HTMLDivElement>document.getElementById('faceIconbar_' + p_id));
        timer = setTimeout(() => {
            var RatonX = e.pageX - 480; var RatonY = e.pageY - 320;
            faceIcon.setAttribute("style", "margin-top:-90px;z-index: 1050; position: absolute ;display:block ");
        }, delay);
        faceIcon.setAttribute("style", "z-index: 1050; position: absolute ;display:none ");
    }
    onMouseOut(e: any, p_id: string) {
        var faceIcon = (<HTMLDivElement>document.getElementById('faceIconbar_' + p_id));
        faceIcon.setAttribute("style", "z-index: 1050; position: absolute ;display:none ");
    }
    postLike(p_id: string, emo: string) {
        var faceIcon = (<HTMLAnchorElement>document.getElementById('post_like_icon_' + p_id));
        var has_like = faceIcon.getAttribute("name");
        if (has_like == emo) {
            if (emo == 'none') {
                emo = 'like';
                faceIcon.innerHTML = "<i></i>";
                faceIcon.setAttribute("class", "Selector selectorFace20 " + emo + "20");
                faceIcon.setAttribute("name", emo);
            } else {
                faceIcon.setAttribute("class", "");
                faceIcon.innerHTML = "<i class='glyphicon glyphicon-thumbs-up'> Like</i>";
                faceIcon.setAttribute("name", "none");
            }
        } else {
                faceIcon.innerHTML = "<i></i>";
                faceIcon.setAttribute("class", "Selector selectorFace20 " + emo + "20");
                faceIcon.setAttribute("name", emo);
        }
        this.emoji_service.postLike(this.post._id, this.sub_post._id, emo).subscribe(result => {
        }, error => console.error(error))
    }
}