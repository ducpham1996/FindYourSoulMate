import { Component } from '@angular/core';
import { CheckCookieService } from '../manager/checkcookie.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {

    constructor(private cookie_service: CheckCookieService) {

    }

    logout() {
        this.cookie_service.removeCookie();
    }

    getUser() {
        return this.cookie_service.getUserCookie();
    }
}
