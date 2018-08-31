import { Component, Injectable, Input, Output, ViewEncapsulation, OnChanges, SimpleChanges, SimpleChange } from '@angular/core';
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

@Component({
    selector: 'comment',
    templateUrl: './comment.component.html',
    styleUrls: ['../css/comments.css', '../css/faceMocion.css'],
})
@Injectable()
export class CommentComponent  {
    private cookieValue = "Users";
    public comments: Comment[] = [];
    public tmp_list: Comment[] = [];
    private comment = new Comment();
    private comment_text: string = "";
    private message: string = "";
    private error: string = "";
    loading: boolean = false;
    noMoreCommentLeft = false;
    load_comment_success: boolean = false;
    iconHTML = "<i class='glyphicon glyphicon-thumbs-up'> Like</i>";
    //@Output()
    //comment_has_loaded: EventEmitter<boolean> = new EventEmitter<boolean>();

    @Input()
    private subCommentComponent: SubCommentComponent;

    public parent_post: Post;

    @Input() set post(parent_post: Post) {
        this.parent_post = parent_post;
    }

    get getPost(): Post {
        return this.parent_post;
    }

    @Input()
    sub_post: Post;

    constructor(private checkcookies_service: CheckCookieService, private comment_service: CommentService, private subcomment_service: SubCommentService, private emoji_service: EmojiService) {
        this.parent_post = new Post;
        this.sub_post = new Post;
        this.subCommentComponent = new SubCommentComponent(checkcookies_service, comment_service, this.subcomment_service, this.emoji_service);
    }
    getUser() {
        return this.checkcookies_service.getUserCookie();
    }
    clearComments() {
        this.comments = [];
    }

    getComments() {
        this.loading = true;
        var no_of_comnent = 0
        if (this.sub_post._id != "") {
            no_of_comnent = this.sub_post.no_of_comment;
        } else {
            no_of_comnent = this.parent_post.no_of_comment;
        }
        this.comment_service.getComments(this.parent_post._id, this.sub_post._id, no_of_comnent).subscribe(result => {
            this.tmp_list = result.json();
            for (var i = this.tmp_list.length; i--;) {
                this.comments.unshift(this.tmp_list[i]);
            }
            if (this.sub_post._id == "") {
                this.parent_post.no_of_comment--;
                this.loading = false;
                if (this.parent_post.no_of_comment == 0) {
                    this.noMoreCommentLeft = false;
                }
                if (this.parent_post.no_of_comment > 0) {
                    this.noMoreCommentLeft = true;
                }
            } else {
                this.sub_post.no_of_comment--;
                this.loading = false;
                if (this.sub_post.no_of_comment == 0) {
                    this.noMoreCommentLeft = false;
                }
                if (this.sub_post.no_of_comment > 0) {
                    this.noMoreCommentLeft = true;
                }
            }

        }, error => console.error(error))

    }
    submit(message: string, event: any) {
        if (event.keyCode == 13) {
            if (!event.shiftKey) {
                event.preventDefault();
                if (message == "") {
                    this.message = "";
                    this.error = "Please enter something";
                } else {
                    this.comment_service.postComment(this.parent_post._id, this.sub_post._id, message).subscribe(result => {
                        this.message = "Submit successfully";
                        this.error = "";
                        if (this.sub_post._id == "") {
                            this.parent_post.comments++;
                        } else {
                            this.sub_post.comments++;
                        }                    
                        this.comment = result.json();
                        this.comments.push(this.comment);
                    }, error => console.error(error))
                }
            }
        }

    }
    showReplyBox(c_id: string) {
        this.subCommentComponent.showReplyBox(c_id);
    }
    showEditBox(index: number) {
        this.comment = this.comments[index];
        var edit_box = (<HTMLTextAreaElement>document.getElementById('edit_comment_form_' + this.comment._id));
        if (edit_box.getAttribute("style") == "display: block") {
            edit_box.setAttribute("style", "display: none");
            (<HTMLParagraphElement>document.getElementById('comment_text_' + this.comment._id)).setAttribute("style", "display: block;word-wrap: break-word;white-space: pre-line;");
        } else {
            edit_box.setAttribute("style", "display: block");
            (<HTMLParagraphElement>document.getElementById('comment_text_' + this.comment._id)).setAttribute("style", "display: none");
        }
    }
    deleteComment(index: number) {
        this.comment = this.comments[index];
        this.comment_service.deleteComment(this.parent_post._id, this.sub_post._id, this.comment._id).subscribe(result => {
            this.comments.splice(index, 1);
            this.parent_post.comments--;
        }, error => console.error(error))
    }

    editComment(event: any, index: number, message: string) {
        if (event.keyCode == 13) {
            if (!event.shiftKey) {
                event.preventDefault();
                if (message != "") {
                    this.comment = this.comments[index];
                    this.comment_service.editComment(this.parent_post._id, this.sub_post._id, this.comment._id, message).subscribe(result => {
                        (<HTMLTextAreaElement>document.getElementById('edit_comment_form_' + this.comment._id)).setAttribute("style", "display: none");
                        (<HTMLParagraphElement>document.getElementById('comment_text_' + this.comment._id)).setAttribute("style", "display: block;word-wrap: break-word;white-space: pre-line;");
                        this.comment.message = message;
                    }, error => console.error(error))
                }
            }
        }

    }
    onChoseEmoji(e: any, c_id: string) {
        var timer;
        var delay = 800;
        var faceIcon = (<HTMLDivElement>document.getElementById('faceIconbar_' + c_id));
        timer = setTimeout(() => {
            var RatonX = e.pageX - 20; var RatonY = e.pageY - 200;
            faceIcon.setAttribute("style", "top:" + RatonY + "; left:" + RatonX + "; z-index: 1050; position: absolute ;display:block ");
        }, delay);
        faceIcon.setAttribute("style", "z-index: 1050; position: absolute ;display:none ");
    }
    onMouseOut(e: any, c_id: string) {
        var faceIcon = (<HTMLDivElement>document.getElementById('faceIconbar_' + c_id));
        faceIcon.setAttribute("style", "z-index: 1050; position: absolute ;display:none ");
    }

    commentLike(c_id: string, emo: string) {
        var faceIcon = (<HTMLAnchorElement>document.getElementById('comment_like_icon_' + c_id));
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
        this.emoji_service.commentLike(this.parent_post._id, this.sub_post._id, c_id, emo).subscribe(result => {
        }, error => console.error(error))
    }

}