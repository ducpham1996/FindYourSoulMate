import { Component, Injectable } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { PostService } from '../manager/post.service';
import { Owner } from '../entities/Owner';
import { CheckCookieService } from '../manager/checkcookie.service';

@Component({
    selector: 'postsubmit',
    templateUrl: './postsubmit.component.html'
})
@Injectable()
export class PostSubmitComponent {
    private cookieValue = "Users";
    constructor(private post_service: PostService, private checkcookies_service: CheckCookieService) {
    }
    private filelist: string[] = [];
    private url: any;
    private tmp_files: File[] = [];
    private loading: boolean = false;
    private message: string = "";
    private error: string = "";

    onChange(event: any) {
        for (let i = 0; i < event.target.files.length; i++) {
            this.tmp_files.push(event.target.files[i]);
            ((i) => {
                var file = event.target.files[i];
                if (event.target.files && event.target.files[i]) {
                    var reader = new FileReader();
                    reader.onload = (event: any) => {
                        if (file.type.match('video')) {
                            this.getVideoThumbnail(file);
                        } else {
                            this.url = event.target.result;
                            this.filelist.push(this.url);
                        }
                    }
                    reader.readAsDataURL(event.target.files[i]);
                }
            })(i);
        }
    }
    removeFile(index: number) {
        this.filelist.splice(index, 1);
        this.tmp_files.splice(index, 1);
    }
    getVideoThumbnail(file: File) {
        var video = document.createElement('video');
        video.preload = 'metadata';
        video.src = URL.createObjectURL(file);
        var timeupdate = () => {
            if (snapImage()) {
                video.removeEventListener('timeupdate', timeupdate);
                video.pause();
            }
        };
        video.addEventListener('loadeddata', () => {
            if (snapImage()) {
                video.removeEventListener('timeupdate', timeupdate);
            }
        });
        var snapImage = () => {
            var canvas = document.createElement('canvas');
            var ctx = canvas.getContext('2d');
            ctx!.font = "35pt Arial";
            ctx!.fillStyle = "red";
            ctx!.textAlign = "center";
            ctx!.drawImage(video, 0, 0, video.videoWidth, video.videoHeight);
            ctx!.fillText(this.fromSeconds(video.duration, false), 150, 120);
            var image = canvas.toDataURL();
            this.filelist.push(image);
            //this.thumb_nail = image;
            //var success = image.length > 100000;
            //if (success) {
            //    var img = <HTMLImageElement>document.getElementById('preview');
            //    img!.src = image;
            //}
        }
    }

    onSubmit(content: string) {
        if (content != "" && this.tmp_files.length != 0) {
            this.error = "";
            this.post_submission(content);       
        } 
        if (content == "") {
            if (this.tmp_files.length > 0) {
                this.error = "";
                this.post_submission(content);         
            } else {
                this.error = "Please choose some videos/images and fill-in something ";
            }
        }
        if (this.tmp_files.length == 0) {
            if (content != "") {
                this.error = "";
                this.post_submission(content);
            } else {
                this.error = "Please choose some videos/images and fill-in something ";
            }
        } 
    }
    post_submission(content:string) {
        this.loading = true;
        this.post_service.postPost(content, this.tmp_files).subscribe(result => {
            this.loading = false;
            this.message = result.text();
        }, error => {
            this.loading = false;
            this.error = error.text();
        })
    }

    fromSeconds(seconds: any, showHours: any) {
        var hours = 0;
        if (showHours) {
            hours = Math.floor(seconds / 3600),
                seconds = seconds - hours * 3600;
        }
        var minutes = "";
        minutes = ("0" + Math.floor(seconds / 60)).slice(-2);
        var seconds;
        seconds = ("0" + parseInt(seconds % 60 + "", 10)).slice(-2);

        if (showHours) {
            var timestring = hours + ":" + minutes + ":" + seconds;
        } else {
            var timestring = minutes + ":" + seconds;
        }
        return timestring;
    }
}