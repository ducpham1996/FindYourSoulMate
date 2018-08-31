import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { PostService } from '../manager/post.service';
import { Post } from '../entities/Post';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
    
})
export class HomeComponent {
    constructor(private post_service: PostService) {
    
    }
}
