import java.util.Base64;

public class Crypto {
    private static final String KEY = "MyUltraSecretPasswordManagerKey2025!@#";

    public static String encrypt(String data) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.length(); i++) {
            char c = data.charAt(i);
            char k = KEY.charAt(i % KEY.length());
            sb.append((char)(c ^ k));
        }
        return Base64.getEncoder().encodeToString(sb.toString().getBytes());
    }

    public static String decrypt(String encrypted) {
        try {
            byte[] decoded = Base64.getDecoder().decode(encrypted);
            String s = new String(decoded);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.length(); i++) {
                char c = s.charAt(i);
                char k = KEY.charAt(i % KEY.length());
                sb.append((char)(c ^ k));
            }
            return sb.toString();
        } catch (Exception e) {
            return "";
        }
    }
}