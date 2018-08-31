export class User {
    public _id: string = "";
    public userName: string= "";
    public userPic: string = "";
    constructor(_id: string, userName: string, userPic: string) {
        this._id = _id;
        this.userName = userName;
        this.userPic = userPic;
    }
}