import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { TestComponent } from './components/test/test.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CookieService } from 'angular2-cookie/services/cookies.service';
import { RegisterService } from './components/manager/register.service';
import { PostService } from './components/manager/post.service';
import { CommentService } from './components/manager/comment.service';
import { SubCommentService } from './components/manager/subcomment.service';
import { CheckCookieService } from './components/manager/checkcookie.service';
import { EmojiService } from './components/manager/emoji.service';
import { UserService } from './components/manager/user.service';
import { PostSubmitComponent } from './components/postsubmit/postsubmit.component';
import { PostDisplayComponent } from './components/postdisplay/postdisplay.component';
import { CommentComponent } from './components/commentdisplay/comment.component';
import { SubCommentComponent } from './components/subcommentdisplay/subcomment.component';
import { PostActionComponent } from './components/postaction/postaction.component';
import { SubPostDisplayComponent } from './components/subpost/subpost.component';
import { EditPostComponent } from './components/editpost/editpost.component';
import { ProfileComponent } from './components/profile/profile.component';
import { TimeAgoPipe } from 'time-ago-pipe';
import { SafeHtmlPipe } from './components/pipe/SafeHtmlPile'

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        TestComponent,
        RegisterComponent,
        LoginComponent,
        PostSubmitComponent,
        PostDisplayComponent,
        CommentComponent,
        SubCommentComponent,
        PostActionComponent,
        SubPostDisplayComponent,
        EditPostComponent,
        ProfileComponent,
        TimeAgoPipe,
        SafeHtmlPipe
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'test', component: TestComponent },
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'profile', component: ProfileComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ], providers: [CookieService, RegisterService, PostService, CheckCookieService, CommentService, SubCommentService, EmojiService, UserService]
})
export class AppModuleShared {
}
