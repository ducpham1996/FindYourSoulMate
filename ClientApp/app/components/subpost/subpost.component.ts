import { Component, Injectable, Input, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { CommentService } from '../manager/comment.service';
import { CheckCookieService } from '../manager/checkcookie.service';
import { Comment } from '../entities/Comment';
import { Post } from '../entities/Post';
import { Owner } from '../entities/Owner';
import { TimeAgoPipe } from 'time-ago-pipe';
import { SubCommentComponent } from '../subcommentdisplay/subcomment.component';
import { SubCommentService } from '../manager/subcomment.service';
import { EmojiService } from '../manager/emoji.service';
import { PostDisplayComponent } from "../postdisplay/postdisplay.component"
import { PostService } from '../manager/post.service';
import { CommentComponent } from '../commentdisplay/comment.component';
import { PostActionComponent } from '../postaction/postaction.component';
 
@Component({
    selector: 'subpost',
    templateUrl: './subpost.component.html',
    styleUrls: ['../css/comments.css', '../css/faceMocion.css', '../css/modal.css', '../css/images-grid.css'],
})
@Injectable()
export class SubPostDisplayComponent {

    private cookieValue = "Users";
    private isOpen: boolean = false;
    //private postdisplay_component: PostDisplayComponent;
    @ViewChild(PostActionComponent) post_action_comp: PostActionComponent = new PostActionComponent(this.checkcookies_service, this.comment_service, this.subcomment_service, this.emoji_service);
    @ViewChild(CommentComponent) comment_comp: CommentComponent = new CommentComponent(this.checkcookies_service, this.comment_service, this.subcomment_service, this.emoji_service);

    message: string = "";
    private parent_post: Post = new Post;
    private sub_post: Post = new Post;
    private preview: Post[] = [];
    prev: boolean = false;
    next: boolean = false;

    constructor(private checkcookies_service: CheckCookieService, private post_service: PostService, private comment_service: CommentService, private subcomment_service: SubCommentService, private emoji_service: EmojiService) {
        //this.postdisplay_component = new PostDisplayComponent(post_service, checkcookies_service);
        //this.postdisplay_component.currentMessage.subscribe(message => this.message = message);
        //this.postdisplay_component.currentParentPost.subscribe(post => this.parent_post = post);
        //this.postdisplay_component.currentSubPost.subscribe(post => this.sub_post = post);
        //this.postdisplay_component.showModal.subscribe(showModal => this.isOpen = showModal);
        //this.subCommentComponent = new SubCommentComponent(checkcookies_service, comment_service, this.subcomment_service, this.emoji_service);
    }
    getUser() {
        return this.checkcookies_service.getUserCookie();
    }
    openSubPostModal(parent_post: Post, sub_post: Post) {
        this.parent_post = parent_post;
        this.sub_post = sub_post;
        this.getSubPost(parent_post._id, sub_post._id);
        var modal = (<HTMLDivElement>document.getElementById('sub_post_modal'));
        modal.setAttribute('style', 'display:block');
    }
    closeModal() {
        var modal = (<HTMLDivElement>document.getElementById('sub_post_modal'));
        var video = (<HTMLVideoElement>document.getElementById('sub_post_video'));
        if (video != null) {
            video.muted = true;
        }
        modal.setAttribute('style', 'display:none');
    }
    getSubPost(parent_post_id: string, sub_post_id: string) {
        this.post_service.getPreviewSubPost(parent_post_id, sub_post_id).subscribe(result => {
            this.preview = result.json();
            this.sub_post = this.preview[1];
            var hasPrev = this.preview[0]._id;
            if (hasPrev != null) {
                this.prev = true
                this.next = true;
            } else {
                this.next = true;
                this.prev = false;
            }
            if (this.preview.length == 2) {
                this.next = false;
                this.prev = true;
            }
            this.comment_comp.clearComments();
            this.post_action_comp.activeGetComments();
        }, error => console.error(error));
    }
    backward() {
        if (this.preview[0] != null) {
            this.sub_post = this.preview[0];
            this.getSubPost(this.parent_post._id, this.sub_post._id);
        }
    }
    forward() {
        if (this.preview[2] != null) {
            this.sub_post = this.preview[2];
            this.getSubPost(this.parent_post._id, this.sub_post._id);
        }
    }
}