export class Login {
    public UserName: string;
    public PassWord: string;
    public IsRemember: boolean;

    constructor(UserName: string, PassWord: string, IsRemember: boolean) {
        this.UserName = UserName;
        this.PassWord = PassWord;
        this.IsRemember = IsRemember;
    }
}