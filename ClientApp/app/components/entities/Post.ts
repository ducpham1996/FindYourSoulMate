import { Owner } from './Owner';
export class Post {
    public _id: string = "";
    public description: string = "";
    public owner: Owner = new Owner;
    public date_created: Date = new Date;
    public video_image: Post[] = [];
    public type: string = "";
    public link: string = "";
    public has_modified: boolean = false;
    public feeling: string = "";
    public modify_date: Date = new Date;
    public location: string = "";
    public like: number = 0;
    public love: number = 0;
    public angry: number = 0;
    public scare: number = 0;
    public haha: number = 0;
    public sad: number = 0;
    public amaze: number = 0;
    public suprise: number = 0;
    public comments: number = 0;
    public sub_comments: number = 0;
    public no_of_comment: number = 0;
    public status: number = 0;
    public has_like: string = "";
    public is_own: boolean = false;
}