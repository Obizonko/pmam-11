import tkinter as tk
from tkinter import ttk, Menu

USERS = {
    "User A": ("#0084ff", "A"),
    "User B": ("#44cc77", "B"),
    "User C": ("#ff6655", "C"),
    "User D": ("#9966ff", "D"),
    "User E": ("#ffbb33", "E"),
    "User F": ("#f50057", "F")
}

class ChatWindow(tk.Tk):
    def __init__(self, controller):
        super().__init__()
        self.controller = controller
        self.title("Chat Simulator")
        self.geometry("500x600")
        self.minsize(500, 600)
        self.dark_mode = False

        self._create_widgets()
        self._apply_theme()
        self.after(100, self.controller.load_history)

    def _create_widgets(self):
        # Header
        self.header = tk.Frame(self, height=50)
        self.header.pack(fill=tk.X)
        self.header.pack_propagate(False)

        self.title_label = tk.Label(self.header, text="Chat Simulator",
                                    font=("Segoe UI", 12, "bold"), fg="white")
        self.title_label.pack(side=tk.LEFT, padx=15, pady=12)

        self.theme_btn = tk.Button(self.header, text="Dark Mode", font=("Segoe UI", 11),
                                   bd=0, relief="flat", command=self.toggle_theme)
        self.theme_btn.pack(side=tk.RIGHT, padx=15)

        # Messages
        container = tk.Frame(self)
        container.pack(fill=tk.BOTH, expand=True, padx=12, pady=12)

        self.listbox = tk.Listbox(container, font=("Segoe UI", 10), bd=0,
                                  highlightthickness=0, activestyle="none")
        self.listbox.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

        scrollbar = ttk.Scrollbar(container, command=self.listbox.yview)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
        self.listbox.config(yscrollcommand=scrollbar.set)

        # Context menu
        self.ctx_menu = Menu(self, tearoff=0)
        self.ctx_menu.add_command(label="Копіювати", command=self.copy_message)
        self.ctx_menu.add_command(label="Видалити все", command=self.controller.clear_chat)
        self.listbox.bind("<Button-3>", lambda e: self.ctx_menu.tk_popup(e.x_root, e.y_root))

        # Input area
        self.bottom = tk.Frame(self)
        self.bottom.pack(fill=tk.X, padx=12, pady=(0, 12))

        self.user_var = tk.StringVar(value="User A")
        self.user_menu = ttk.OptionMenu(self.bottom, self.user_var, "User A", *USERS.keys())
        self.user_menu.pack(side=tk.LEFT, padx=(0, 8))

        self.entry = tk.Text(self.bottom, height=3, font=("Segoe UI", 10),
                             wrap=tk.WORD, undo=True, bd=1, relief="solid")
        self.entry.pack(side=tk.LEFT, fill=tk.X, expand=True, pady=8)
        self.entry.bind("<Shift-Return>", lambda e: None)
        self.entry.bind("<Return>", lambda e: (self.controller.send_message(), "break"))

        self.send_btn = ttk.Button(self.bottom, text="Send", command=self.controller.send_message)
        self.send_btn.pack(side=tk.RIGHT, padx=(8, 0), pady=8)

    def _apply_theme(self):
        if self.dark_mode:
            bg = "#1e1e1e"
            fg = "#e0e0e0"
            header_bg = "#0078d4"
            input_bg = "#2d2d2d"
            list_bg = "#252525"
            menu_bg = "#2d2d2d"
            menu_fg = "#e0e0e0"
            menu_sel = "#404040"
            self.theme_btn.config(text="Light Mode")
        else:
            bg = "#f0f2f5"
            fg = "#000000"
            header_bg = "#0084ff"
            input_bg = "#ffffff"
            list_bg = "#ffffff"
            menu_bg = "#f0f0f0"
            menu_fg = "#000000"
            menu_sel = "#0078d4"
            self.theme_btn.config(text="Dark Mode")

        # Frames
        self.config(bg=bg)
        self.header.config(bg=header_bg)
        self.title_label.config(bg=header_bg)
        self.bottom.config(bg=bg)

        # Listbox
        self.listbox.config(bg=list_bg, fg=fg, selectbackground="#0078d4")

        # Text input
        self.entry.config(bg=input_bg, fg=fg, insertbackground=fg)

        # ttk styles
        style = ttk.Style()
        style.theme_use("clam")

        style.configure("TButton", background=header_bg, foreground="white", padding=6)
        style.map("TButton",
                  background=[("active", "#0068c0" if self.dark_mode else "#0070dd")])

        style.configure("TMenubutton",
                        background=input_bg,
                        foreground=fg,
                        arrowcolor=fg)
        style.map("TMenubutton",
                  background=[("active", "#3a3a3a" if self.dark_mode else "#e5e5e5")],
                  foreground=[("active", fg)])

        # Context menu
        self.ctx_menu.config(
            bg=menu_bg,
            fg=menu_fg,
            activebackground=menu_sel,
            activeforeground="white",
            selectcolor=fg
        )

        self.theme_btn.config(bg=header_bg, fg="white", activebackground="#0060b0" if self.dark_mode else "#0070cc")

    def toggle_theme(self):
        self.dark_mode = not self.dark_mode
        self._apply_theme()
        self.controller.refresh_messages()

    def add_message(self, msg_data):
        user, text, time, color = msg_data["user"], msg_data["text"], msg_data["time"], msg_data["color"]
        avatar = USERS[user][1]

        header_text = f"User {avatar} • {time}  "
        self.listbox.insert(tk.END, header_text)
        self.listbox.itemconfig(tk.END, fg=color)

        self.listbox.insert(tk.END, f"   {text}")
        self.listbox.itemconfig(tk.END, fg="white" if self.dark_mode else "black")

        self.listbox.insert(tk.END, "")
        self.listbox.see(tk.END)

    def get_input(self):
        text = self.entry.get("1.0", tk.END).strip()
        self.entry.delete("1.0", tk.END)
        return text

    def get_user(self):
        return self.user_var.get()

    def copy_message(self):
        try:
            sel = self.listbox.curselection()
            if sel:
                text = self.listbox.get(sel[0])
                self.clipboard_clear()
                self.clipboard_append(text)
        except:
            pass

    def run(self):
        self.mainloop()