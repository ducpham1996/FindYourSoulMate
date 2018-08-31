import { Component, Injectable,Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { PostService } from '../manager/post.service';
import { CheckCookieService } from '../manager/checkcookie.service';
import { Post } from '../entities/Post';
import { TimeAgoPipe } from 'time-ago-pipe';
import { SubPostDisplayComponent } from '../subpost/subpost.component';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { CommentService } from '../manager/comment.service';
import { SubCommentService } from '../manager/subcomment.service';
import { EmojiService } from '../manager/emoji.service';
import {EditPostComponent } from '../editpost/editpost.component';

@Component({
    selector: 'postdisplay',
    templateUrl: './postdisplay.component.html',
    styleUrls: ['../css/comments.css', '../css/faceMocion.css', '../css/images-grid.css', '../css/modal.css'],
})
@Injectable()
export class PostDisplayComponent {
    private cookieValue = "Users";
    private posts: Post[] = [];
    private tmp_post: Post = new Post;

    @Input()
    private subpost_display_component: SubPostDisplayComponent = new SubPostDisplayComponent(this.checkcookies_service, this.post_service, this.comment_service, this.subcomment_service, this.emoji_service);

    @Input()
    private edit_post_component: EditPostComponent = new EditPostComponent(this.checkcookies_service,this.post_service);

    //private messageSource = new BehaviorSubject<string>("Hello");
    //currentMessage = this.messageSource.asObservable();

    //private postSource = new BehaviorSubject<Post>(new Post);
    //currentParentPost = this.postSource.asObservable();
    //private subPostSource = new BehaviorSubject<Post>(new Post);
    //currentSubPost = this.subPostSource.asObservable();
    //private showModalSource = new BehaviorSubject<boolean>(false);
    //showModal = this.showModalSource.asObservable();

    constructor(private checkcookies_service: CheckCookieService, private post_service: PostService, private comment_service: CommentService, private subcomment_service: SubCommentService, private emoji_service: EmojiService) {
        this.getData();
    }
    getData() {
        this.post_service.getPosts().subscribe(result => {
            this.posts = result.json();
        }, error => console.error(error))
    }
    getUser() {
        return this.checkcookies_service.getUserCookie();
    }
    viewSubPost(i: number, j: number) {
        this.subpost_display_component.openSubPostModal(this.posts[i], this.posts[i].video_image[j]);
    }
    editPost(i: number) {
        this.edit_post_component.openEditPostModal(this.posts[i]);
    }
    removePost(i: number) {
        this.post_service.removePost(this.posts[i]._id).subscribe(result => {
            this.posts.splice(i, 1);
        }, error => console.error(error));   
    }
}