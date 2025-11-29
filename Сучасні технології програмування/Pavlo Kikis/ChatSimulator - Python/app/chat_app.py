from app.chat_window import ChatWindow, USERS
from app.message_manager import MessageManager
import tkinter as tk

class ChatApp:
    def __init__(self):
        self.manager = MessageManager()
        self.window = ChatWindow(self)

    def send_message(self):
        text = self.window.get_input()
        if not text:
            return
        user = self.window.get_user()
        color = USERS[user][0]
        self.manager.add_message(user, text, color)
        self.refresh_messages()

    def load_history(self):
        self.refresh_messages()

    def refresh_messages(self):
        self.window.listbox.delete(0, tk.END)
        for msg in self.manager.get_messages():
            self.window.add_message(msg)

    def clear_chat(self):
        self.manager.clear()
        self.refresh_messages()

    def run(self):
        self.window.run()