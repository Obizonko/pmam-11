import javax.swing.*;
import javax.swing.table.*;
import java.awt.*;
import java.awt.datatransfer.StringSelection;
import java.awt.event.*;
import java.text.SimpleDateFormat;
import java.util.*;
import java.util.List;
import java.util.stream.Collectors;

public class Main extends JFrame {
    private JTable table;
    private DefaultTableModel model;
    private JTextField searchField;
    private List<PasswordEntry> allEntries = new ArrayList<>();
    private List<PasswordEntry> filtered = new ArrayList<>();

    public Main() {
        setTitle("Password Manager");
        setSize(900, 580);
        setDefaultCloseOperation(DO_NOTHING_ON_CLOSE);
        setLocationRelativeTo(null);

        initUI();
        loadData();

        addWindowListener(new WindowAdapter() {
            public void windowClosing(WindowEvent e) {
                if (JOptionPane.showConfirmDialog(Main.this,
                        "Вийти з програми?\nВсі зміни буде збережено.", "Вихід",
                        JOptionPane.YES_NO_OPTION) == 0) {
                    Storage.save(allEntries);
                    System.exit(0);
                }
            }
        });
    }

    private void initUI() {
        Color bgColor = new Color(28, 28, 30);
        Color inputColor = new Color(38, 38, 40);
        Color btnColor = new Color(50, 50, 55);
        Color btnHover = new Color(75, 75, 80);
        Color selectColor = new Color(70, 120, 255);

        getContentPane().setBackground(bgColor);
        setLayout(new BorderLayout());

        // Заголовок
        JLabel title = new JLabel("Passwords", SwingConstants.CENTER);
        title.setFont(new Font("Segoe UI", Font.BOLD, 22));
        title.setForeground(Color.WHITE);
        title.setBorder(BorderFactory.createEmptyBorder(10, 0, 10, 0));
        add(title, BorderLayout.NORTH);

        // Пошук
        JPanel searchPanel = new JPanel(new BorderLayout(5, 0));
        searchPanel.setBackground(bgColor);
        searchPanel.setBorder(BorderFactory.createEmptyBorder(5, 10, 5, 10));

        searchField = new JTextField();
        searchField.setFont(new Font("Segoe UI", Font.PLAIN, 13));
        searchField.setForeground(Color.WHITE);
        searchField.setBackground(inputColor);
        searchField.setCaretColor(Color.WHITE);
        searchField.setBorder(BorderFactory.createEmptyBorder(6, 10, 6, 10));
        searchField.setOpaque(true);
        searchField.addKeyListener(new KeyAdapter() { public void keyReleased(KeyEvent e){ filterTable(); } });

        JButton clearBtn = createFlatButton("х", e -> { searchField.setText(""); filterTable(); });
        clearBtn.setFont(new Font("Segoe UI", Font.BOLD, 12));
        clearBtn.setPreferredSize(new Dimension(28, 28));

        searchPanel.add(searchField, BorderLayout.CENTER);
        searchPanel.add(clearBtn, BorderLayout.EAST);

        // Таблиця
        String[] cols = {"Назва", "Логін", "Пароль", "URL", "Оновлено"};
        model = new DefaultTableModel(cols, 0) { public boolean isCellEditable(int r, int c){ return false; } };
        table = new JTable(model);
        table.setRowHeight(28);
        table.setFont(new Font("Segoe UI", Font.PLAIN, 13));
        table.setForeground(Color.WHITE);
        table.setBackground(inputColor);
        table.setSelectionBackground(selectColor);
        table.setSelectionForeground(Color.WHITE);
        table.setGridColor(new Color(45,45,45));
        table.setShowGrid(false);

        JTableHeader header = table.getTableHeader();
        header.setBackground(bgColor);
        header.setForeground(Color.LIGHT_GRAY);
        header.setFont(new Font("Segoe UI", Font.BOLD, 12));
        header.setOpaque(true);

        table.addMouseListener(new MouseAdapter() { public void mouseClicked(MouseEvent e){ if(e.getClickCount()==2) editSelected(); } });

        JScrollPane scroll = new JScrollPane(table);
        scroll.setBorder(BorderFactory.createEmptyBorder());
        scroll.getViewport().setBackground(inputColor);

        // Кнопки
        JPanel btnPanel = new JPanel(new FlowLayout(FlowLayout.CENTER, 8, 8));
        btnPanel.setBackground(bgColor);

        btnPanel.add(createFlatButton("Додати", e -> showEditDialog(null)));
        btnPanel.add(createFlatButton("Редагувати", e -> editSelected()));
        btnPanel.add(createFlatButton("Видалити", e -> deleteSelected()));
        btnPanel.add(createFlatButton("Копіювати", e -> copyPassword()));

        // Центр
        JPanel center = new JPanel(new BorderLayout());
        center.setBackground(bgColor);
        center.add(searchPanel, BorderLayout.NORTH);
        center.add(scroll, BorderLayout.CENTER);
        center.add(btnPanel, BorderLayout.SOUTH);

        add(center, BorderLayout.CENTER);
    }

    private JButton createFlatButton(String text, ActionListener action){
        JButton btn = new JButton(text);
        btn.setFont(new Font("Segoe UI", Font.PLAIN, 13));
        btn.setForeground(Color.WHITE);
        btn.setBackground(new Color(50,50,55));
        btn.setOpaque(true);
        btn.setBorderPainted(false);
        btn.setFocusPainted(false);
        btn.setContentAreaFilled(true);
        btn.setCursor(new Cursor(Cursor.HAND_CURSOR));
        btn.addActionListener(action);
        btn.addMouseListener(new MouseAdapter(){
            public void mouseEntered(MouseEvent e){ btn.setBackground(new Color(75,75,80)); }
            public void mouseExited(MouseEvent e){ btn.setBackground(new Color(50,50,55)); }
        });
        return btn;
    }

    private void loadData(){ allEntries = Storage.load(); filtered = new ArrayList<>(allEntries); refreshTable(); }

    private void refreshTable(){
        model.setRowCount(0);
        SimpleDateFormat sdf = new SimpleDateFormat("dd.MM.yyyy HH:mm");
        for(PasswordEntry e : filtered){
            model.addRow(new Object[]{ e.title, e.username, "••••••", e.url.isEmpty() ? "—": e.url, e.updated!=null? e.updated:"Невідомо" });
        }
    }

    private void filterTable(){
        String query = searchField.getText().toLowerCase().trim();
        if(query.isEmpty()) filtered = new ArrayList<>(allEntries);
        else filtered = allEntries.stream()
                .filter(e -> e.title.toLowerCase().contains(query) ||
                        e.username.toLowerCase().contains(query) ||
                        e.url.toLowerCase().contains(query) ||
                        e.notes.toLowerCase().contains(query))
                .collect(Collectors.toList());
        refreshTable();
    }

    private void showEditDialog(PasswordEntry entry){
        JDialog dialog = new JDialog(this, entry==null?"Новий запис":"Редагування запису", true);
        dialog.setSize(430, 400);
        dialog.setLocationRelativeTo(this);
        dialog.setLayout(new BorderLayout());
        Color bg = new Color(28,28,30);
        Color inputColor = new Color(38,38,40);

        JPanel panel = new JPanel(new GridBagLayout());
        panel.setBackground(bg);
        GridBagConstraints gbc = new GridBagConstraints();
        gbc.insets = new Insets(4,4,4,4); gbc.fill = GridBagConstraints.HORIZONTAL;

        JTextField titleField = createDarkTextField(entry!=null?entry.title:"", inputColor);
        JTextField userField = createDarkTextField(entry!=null?entry.username:"", inputColor);
        JPasswordField passField = createDarkPasswordField(entry!=null?entry.password:"", inputColor);
        JTextField urlField = createDarkTextField(entry!=null?entry.url:"", inputColor);

        JTextArea notesArea = new JTextArea(entry!=null?entry.notes:"",4,20);
        notesArea.setLineWrap(true); notesArea.setWrapStyleWord(true);
        notesArea.setForeground(Color.WHITE); notesArea.setBackground(inputColor); notesArea.setCaretColor(Color.WHITE);
        JScrollPane notesScroll = new JScrollPane(notesArea);
        notesScroll.setVerticalScrollBarPolicy(ScrollPaneConstants.VERTICAL_SCROLLBAR_AS_NEEDED);

        String[] labels = {"Назва:","Логін:","Пароль:","URL:","Нотатки:"};
        JComponent[] fields = {titleField,userField,passField,urlField,notesScroll};

        for(int i=0;i<labels.length;i++){
            gbc.gridx=0; gbc.gridy=i;
            JLabel l = new JLabel(labels[i]); l.setForeground(Color.WHITE); l.setFont(new Font("Segoe UI",Font.PLAIN,13));
            panel.add(l,gbc);
            gbc.gridx=1;
            panel.add(fields[i],gbc);
        }

        JButton genBtn = createFlatButton("Згенерувати пароль", e -> passField.setText(PasswordGenerator.generate(20)));
        gbc.gridx=0; gbc.gridy=labels.length; gbc.gridwidth=2;
        panel.add(genBtn,gbc);

        JButton saveBtn = createFlatButton("Зберегти", e -> {
            if(titleField.getText().trim().isEmpty()){ JOptionPane.showMessageDialog(dialog,"Введіть назву запису!"); return; }
            PasswordEntry current = entry!=null?entry:new PasswordEntry();
            if(entry==null) allEntries.add(current);
            current.title = titleField.getText().trim();
            current.username = userField.getText().trim();
            current.password = new String(passField.getPassword());
            current.url = urlField.getText().trim();
            current.notes = notesArea.getText();
            current.updated = new SimpleDateFormat("dd.MM.yyyy HH:mm").format(new Date());
            Storage.save(allEntries);
            filterTable();
            dialog.dispose();
        });

        JButton cancelBtn = createFlatButton("Скасувати", e -> dialog.dispose());

        JPanel bottom = new JPanel(new FlowLayout(FlowLayout.RIGHT,5,5));
        bottom.setBackground(bg);
        bottom.add(saveBtn); bottom.add(cancelBtn);

        dialog.add(panel,BorderLayout.CENTER); dialog.add(bottom,BorderLayout.SOUTH);
        dialog.setVisible(true);
    }

    private JTextField createDarkTextField(String text, Color bgColor){
        JTextField f = new JTextField(text);
        f.setBackground(bgColor);
        f.setForeground(Color.WHITE);
        f.setCaretColor(Color.WHITE);
        f.setOpaque(true);
        f.setBorder(BorderFactory.createEmptyBorder(4,6,4,6));
        return f;
    }

    private JPasswordField createDarkPasswordField(String text, Color bgColor){
        JPasswordField f = new JPasswordField(text);
        f.setBackground(bgColor);
        f.setForeground(Color.WHITE);
        f.setCaretColor(Color.WHITE);
        f.setOpaque(true);
        f.setBorder(BorderFactory.createEmptyBorder(4,6,4,6));
        return f;
    }

    private void editSelected(){ int r=table.getSelectedRow(); if(r!=-1) showEditDialog(filtered.get(table.convertRowIndexToModel(r))); }
    private void deleteSelected(){ int r=table.getSelectedRow(); if(r!=-1 && JOptionPane.showConfirmDialog(this,"Видалити запис назавжди?","Підтвердження",JOptionPane.YES_NO_OPTION)==0){ allEntries.remove(filtered.get(table.convertRowIndexToModel(r))); filterTable(); } }
    private void copyPassword(){ int r=table.getSelectedRow(); if(r!=-1){ String pass=filtered.get(table.convertRowIndexToModel(r)).password; Toolkit.getDefaultToolkit().getSystemClipboard().setContents(new StringSelection(pass),null); JOptionPane.showMessageDialog(this,"Пароль скопійовано в буфер обміну!"); } }

    public static void main(String[] args){ SwingUtilities.invokeLater(() -> new Main().setVisible(true)); }
}
