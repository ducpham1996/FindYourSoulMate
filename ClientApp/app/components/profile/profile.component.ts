import { Component } from '@angular/core';
import { CheckCookieService } from '../manager/checkcookie.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../manager/user.service';

@Component({
    selector: 'profile',
    templateUrl: './profile.component.html',
    styleUrls: ['../css/profile.css']
})
export class ProfileComponent {

    constructor(private cookie_service: CheckCookieService, private route: ActivatedRoute, private user_service: UserService) {
        this.route.queryParams.subscribe(params => {
            let _id = params['uid'];
            this.getProfile(_id);
        });
    }

    getUser() {
        return this.cookie_service.getUserCookie();
    }
    getProfile(_id: string) {
        this.user_service.getUserProfile(_id).subscribe(result => {
            console.log(result.json());
        }, error => console.error(error))
    }
}
