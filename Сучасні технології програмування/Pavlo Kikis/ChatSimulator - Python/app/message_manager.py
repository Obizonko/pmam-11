import json
from datetime import datetime
from pathlib import Path

class MessageManager:
    SAVE_FILE = Path("chat_history.json")

    def __init__(self):
        self.messages = self.load_history()

    def add_message(self, user, text, color):
        timestamp = datetime.now().strftime("%H:%M")
        msg = {
            "user": user,
            "text": text,
            "time": timestamp,
            "color": color
        }
        self.messages.append(msg)
        self.save_history()

    def get_messages(self):
        return self.messages

    def clear(self):
        self.messages.clear()
        self.save_history()

    def save_history(self):
        data = [{"user": m["user"], "text": m["text"], "time": m["time"], "color": m["color"]} 
                for m in self.messages]
        self.SAVE_FILE.write_text(json.dumps(data, ensure_ascii=False, indent=2), encoding="utf-8")

    def load_history(self):
        if not self.SAVE_FILE.exists():
            return []
        try:
            data = json.loads(self.SAVE_FILE.read_text(encoding="utf-8"))
            return data
        except:
            return []