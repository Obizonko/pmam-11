import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionListener;
import java.awt.Image;
import java.awt.image.BufferedImage;

public class UIUtils {
    public static Image createIcon(int size) {
        return new BufferedImage(size, size, BufferedImage.TYPE_INT_ARGB);
    }
    public static JButton createButton(String text, ActionListener action) {
        JButton button = new JButton(text);
        button.setFont(new Font("Segoe UI", Font.BOLD, 15));
        button.setFocusPainted(false);
        button.setBackground(new Color(70, 130, 180));
        button.setForeground(Color.WHITE);
        button.setBorder(BorderFactory.createEmptyBorder(10, 20, 10, 20));
        button.setCursor(new Cursor(Cursor.HAND_CURSOR));

        button.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseEntered(java.awt.event.MouseEvent evt) {
                button.setBackground(new Color(100, 149, 237));
            }
            public void mouseExited(java.awt.event.MouseEvent evt) {
                button.setBackground(new Color(70, 130, 180));
            }
        });



        if (action != null) {
            button.addActionListener(action);
        }

        return button;
    }

    public static JButton createButton(String text) {
        return createButton(text, null);
    }

    public static void addPadding(JPanel panel, int top, int left, int bottom, int right) {
        panel.setBorder(BorderFactory.createEmptyBorder(top, left, bottom, right));
    }

    public static void showMessage(String message, String title, int type) {
        JOptionPane.showMessageDialog(null, message, title, type);
    }
}