import { Owner } from './Owner';
export class Post_submit {
    public content: string = "";
    public files: File[] = [];
    public author: Owner;

    constructor(content: string, files: File[], author: Owner) {
        this.files = files;
        this.content = content;
        this.author = author;
    }
}