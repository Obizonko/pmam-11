import * as readline from 'readline';

type NodeType = 'FILE' | 'DIR';

interface IFileSystemNode {
    name: string;
    type: NodeType;
    parent: Directory | null;
    getPath(): string;
}

class TextFile implements IFileSystemNode {
    type: NodeType = 'FILE';
    parent: Directory | null = null;
    
    // Явне оголошення властивостей
    name: string;
    content: string;

    constructor(name: string, content: string = "") {
        this.name = name;
        this.content = content;
    }

    getPath(): string {
        return this.parent ? `${this.parent.getPath()}/${this.name}` : this.name;
    }
}

class Directory implements IFileSystemNode {
    type: NodeType = 'DIR';
    parent: Directory | null = null;
    children: Map<string, IFileSystemNode> = new Map();
    
    // Явне оголошення властивості
    name: string;

    constructor(name: string) {
        this.name = name;
    }

    getPath(): string {
        if (!this.parent) return ""; 
        return this.parent.parent ? `${this.parent.getPath()}/${this.name}` : `/${this.name}`;
    }

    addNode(node: IFileSystemNode) {
        if (this.children.has(node.name)) {
            throw new Error(`Error: '${node.name}' already exists.`);
        }
        node.parent = this;
        this.children.set(node.name, node);
    }

    getNode(name: string): IFileSystemNode | undefined {
        return this.children.get(name);
    }

    removeNode(name: string): boolean {
        return this.children.delete(name);
    }
}

// --- SYSTEM KERNEL (Logic) ---

class TerminalSystem {
    private root: Directory;
    private currentDir: Directory;
    private rl: readline.Interface;

    constructor() {
        this.root = new Directory("root");
        this.currentDir = this.root;

        const home = new Directory("home");
        const user = new Directory("user");
        const welcome = new TextFile("readme.txt", "Welcome to TS-Shell v1.0!\nDon't panic.");

        user.addNode(welcome);
        home.addNode(user);
        this.root.addNode(home);

        this.rl = readline.createInterface({
            input: process.stdin,
            output: process.stdout
        });
    }

    public start() {
        this.prompt();
        
        this.rl.on('line', (line) => {
            const args = line.trim().split(' ');
            const command = args[0];
            const params = args.slice(1);

            try {
                this.execute(command, params);
            } catch (e: any) {
                console.log(`\x1b[31m${e.message}\x1b[0m`);
            }

            this.prompt();
        });
    }

    private prompt() {
        const path = this.currentDir === this.root ? "/" : this.currentDir.getPath();
        this.rl.setPrompt(`\x1b[36m${path}\x1b[0m $ `);
        this.rl.prompt();
    }

    private execute(cmd: string, args: string[]) {
        switch (cmd) {
            case 'ls':
                this.cmdLs();
                break;
            case 'mkdir':
                if (!args[0]) throw new Error("Usage: mkdir <dirname>");
                this.cmdMkdir(args[0]);
                break;
            case 'touch':
                if (!args[0]) throw new Error("Usage: touch <filename> [content...]");
                const content = args.slice(1).join(" "); 
                this.cmdTouch(args[0], content);
                break;
            case 'cd':
                if (!args[0]) throw new Error("Usage: cd <path>");
                this.cmdCd(args[0]);
                break;
            case 'cat':
                if (!args[0]) throw new Error("Usage: cat <filename>");
                this.cmdCat(args[0]);
                break;
            case 'rm':
                if (!args[0]) throw new Error("Usage: rm <name>");
                this.cmdRm(args[0]);
                break;
            case 'help':
                console.log("Commands: ls, cd, mkdir, touch, cat, rm, exit");
                break;
            case 'exit':
                console.log("Bye!");
                process.exit(0);
                break;
            case '':
                break;
            default:
                throw new Error(`Command not found: ${cmd}`);
        }
    }

    private cmdLs() {
        if (this.currentDir.children.size === 0) {
            console.log("(empty)");
            return;
        }
        this.currentDir.children.forEach((node) => {
            if (node.type === 'DIR') {
                console.log(`\x1b[34m[DIR] ${node.name}\x1b[0m`);
            } else {
                console.log(`      ${node.name}`);
            }
        });
    }

    private cmdMkdir(name: string) {
        this.currentDir.addNode(new Directory(name));
    }

    private cmdTouch(name: string, content: string) {
        this.currentDir.addNode(new TextFile(name, content));
    }

    private cmdRm(name: string) {
        if (!this.currentDir.removeNode(name)) {
            throw new Error(`Cannot delete '${name}': No such file or directory`);
        }
        console.log(`Deleted ${name}`);
    }

    private cmdCat(name: string) {
        const node = this.currentDir.getNode(name);
        if (!node) throw new Error(`File '${name}' not found.`);
        if (node.type === 'DIR') throw new Error(`'${name}' is a directory.`);
        
        console.log((node as TextFile).content);
    }

    private cmdCd(path: string) {
        if (path === "..") {
            if (this.currentDir.parent) {
                this.currentDir = this.currentDir.parent;
            }
            return;
        }
        
        if (path === "/") {
            this.currentDir = this.root;
            return;
        }

        const node = this.currentDir.getNode(path);
        if (!node) throw new Error(`Directory '${path}' not found.`);
        if (node.type !== 'DIR') throw new Error(`'${path}' is not a directory.`);
        
        this.currentDir = node as Directory;
    }
}

const term = new TerminalSystem();
term.start();