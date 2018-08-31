import { Owner } from './Owner';
export class Comment {
    public _id: string = "";
    public owner: Owner = new Owner;
    public message: string = "";
    public date_created: Date = new Date;
    public like: number = 0;
    public love: number = 0;
    public angry: number = 0;
    public scare: number = 0;
    public haha: number = 0;
    public sad: number = 0;
    public amaze: number = 0;
    public suprise: number = 0;
    public comments: number = 0;
    public sub_comments: Comment[] = [];
    public has_like: string = "like";
    public is_own: boolean = false;
    public no_of_comment: number = 0;
}