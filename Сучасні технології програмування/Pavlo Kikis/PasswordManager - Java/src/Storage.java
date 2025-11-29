import java.nio.file.*;
import java.util.*;

public class Storage {
    private static final String FILE = "passwords.dat";

    public static void save(List<PasswordEntry> list) {
        try {
            StringBuilder sb = new StringBuilder();
            for (PasswordEntry e : list) {
                sb.append(escape(e.title)).append("|||")
                  .append(escape(e.username)).append("|||")
                  .append(escape(e.password)).append("|||")
                  .append(escape(e.url)).append("|||")
                  .append(escape(e.notes)).append("|||")
                  .append(e.updated).append("\n");
            }
            Files.writeString(Paths.get(FILE), Crypto.encrypt(sb.toString()));
        } catch (Exception ignored) {}
    }

    public static List<PasswordEntry> load() {
        List<PasswordEntry> list = new ArrayList<>();
        try {
            if (!Files.exists(Paths.get(FILE))) return list;
            String data = Crypto.decrypt(Files.readString(Paths.get(FILE)));
            for (String line : data.split("\n")) {
                if (line.trim().isEmpty()) continue;
                String[] parts = line.split("\\|\\|\\|", 6);
                if (parts.length >= 5) {
                    PasswordEntry e = new PasswordEntry();
                    e.title = unescape(parts[0]);
                    e.username = unescape(parts[1]);
                    e.password = unescape(parts[2]);
                    e.url = unescape(parts[3]);
                    e.notes = unescape(parts.length > 4 ? parts[4] : "");
                    e.updated = parts.length > 5 ? parts[5] : "";
                    list.add(e);
                }
            }
        } catch (Exception ignored) {}
        return list;
    }

    private static String escape(String s) { return s == null ? "" : s.replace("\n", "\\n"); }
    private static String unescape(String s) { return s == null ? "" : s.replace("\\n", "\n"); }
}