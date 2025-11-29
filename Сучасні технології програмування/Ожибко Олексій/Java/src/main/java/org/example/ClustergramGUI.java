package org.example;

import py4j.GatewayServer;

import javax.imageio.ImageIO;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

public class ClustergramGUI extends JFrame {

    private JComboBox<String> datasetCombo;
    private JComboBox<String> experimentCombo;
    private JTextField csvPathField;
    private JTextField kMinField;
    private JTextField kMaxField;
    private JTextArea logArea;
    private JLabel imageLabel;

    private static GatewayServer gatewayServer;
    private IClustergram clustergramService;

    public ClustergramGUI() {
        super("Кластерний аналіз (clustergram + Py4J)");

        initPy4J();
        initUI();
    }

    // ---------------- PY4J ----------------
    private void initPy4J() {
        try {
            gatewayServer = new GatewayServer();
            gatewayServer.start();

            System.out.println("Java: GatewayServer запущено на порту " + gatewayServer.getPort());

            clustergramService = (IClustergram) gatewayServer.getPythonServerEntryPoint(
                    new Class[]{IClustergram.class}
            );

            System.out.println("Java: Python entry point (IClustergram) отримано.");
        } catch (Exception ex) {
            ex.printStackTrace();
            JOptionPane.showMessageDialog(this,
                    "Помилка підключення Py4J. Переконайтесь, що Python-сервер запущений.",
                    "Помилка Py4J",
                    JOptionPane.ERROR_MESSAGE);
        }
    }

    // ---------------- UI ----------------
    private void initUI() {
        setDefaultCloseOperation(WindowConstants.EXIT_ON_CLOSE);
        setLayout(new BorderLayout(10, 10));
        ((JComponent) getContentPane()).setBorder(new EmptyBorder(10, 10, 10, 10));

        JPanel controlPanel = new JPanel(new GridBagLayout());
        GridBagConstraints gbc = new GridBagConstraints();
        gbc.insets = new Insets(4, 4, 4, 4);
        gbc.fill = GridBagConstraints.HORIZONTAL;

        int row = 0;

        // ДАТАСЕТ
        gbc.gridx = 0; gbc.gridy = row;
        controlPanel.add(new JLabel("Датасет:"), gbc);

        datasetCombo = new JComboBox<>(new String[]{"diamonds", "titanic"});
        gbc.gridx = 1; gbc.gridy = row;
        controlPanel.add(datasetCombo, gbc);
        row++;

        // ЕКСПЕРИМЕНТИ
        gbc.gridx = 0; gbc.gridy = row;
        controlPanel.add(new JLabel("Експеримент:"), gbc);

        experimentCombo = new JComboBox<>();
        gbc.gridx = 1; gbc.gridy = row;
        controlPanel.add(experimentCombo, gbc);
        row++;

        // ШЛЯХ ДО CSV
        gbc.gridx = 0; gbc.gridy = row;
        controlPanel.add(new JLabel("Шлях до CSV:"), gbc);

        csvPathField = new JTextField(30);
        gbc.gridx = 1; gbc.gridy = row;
        controlPanel.add(csvPathField, gbc);

        JButton browseBtn = new JButton("Обрати файл...");
        gbc.gridx = 2; gbc.gridy = row;
        controlPanel.add(browseBtn, gbc);

        browseBtn.addActionListener(e -> chooseCsvFile());
        row++;

        // k_min
        gbc.gridx = 0; gbc.gridy = row;
        controlPanel.add(new JLabel("k мінімальне:"), gbc);

        kMinField = new JTextField("2", 5);
        gbc.gridx = 1; gbc.gridy = row;
        controlPanel.add(kMinField, gbc);
        row++;

        // k_max
        gbc.gridx = 0; gbc.gridy = row;
        controlPanel.add(new JLabel("k максимальне:"), gbc);

        kMaxField = new JTextField("8", 5);
        gbc.gridx = 1; gbc.gridy = row;
        controlPanel.add(kMaxField, gbc);
        row++;

        // КНОПКА
        JButton runBtn = new JButton("Запустити кластеризацію");
        gbc.gridx = 0; gbc.gridy = row;
        gbc.gridwidth = 3;
        controlPanel.add(runBtn, gbc);
        gbc.gridwidth = 1;

        runBtn.addActionListener(e -> runClustergram());

        add(controlPanel, BorderLayout.NORTH);

        // ПАНЕЛЬ ЗОБРАЖЕНЬ
        imageLabel = new JLabel("Результат clustergram буде тут", SwingConstants.CENTER);
        JScrollPane imageScroll = new JScrollPane(imageLabel);
        imageScroll.setPreferredSize(new Dimension(700, 400));
        add(imageScroll, BorderLayout.CENTER);

        // ЛОГИ
        logArea = new JTextArea(7, 40);
        logArea.setEditable(false);
        JScrollPane logScroll = new JScrollPane(logArea);
        logScroll.setBorder(BorderFactory.createTitledBorder("Логи"));
        add(logScroll, BorderLayout.SOUTH);

        // ВАЖЛИВО: тільки після створення UI!
        updateExperimentOptions();
        datasetCombo.addActionListener(e -> updateExperimentOptions());

        setSize(950, 720);
        setLocationRelativeTo(null);
    }

    // ----------------- ОНОВЛЕННЯ ЕКСПЕРИМЕНТІВ -----------------
    private void updateExperimentOptions() {
        experimentCombo.removeAllItems();

        String ds = (String) datasetCombo.getSelectedItem();

        if ("diamonds".equalsIgnoreCase(ds)) {
            experimentCombo.addItem("1 — всі числові ознаки");
            experimentCombo.addItem("2 — carat + log(price)");
            experimentCombo.addItem("3 — carat, depth, table (carat < 2.5)");
            csvPathField.setText("data/diamonds.csv");
        } else {
            experimentCombo.addItem("1 — Числова кластеризація (age, fare, sibsp, parch)");
            experimentCombo.addItem("2 — Кластеризація віку + логарифм вартості (fare)");
            experimentCombo.addItem("3 — Кластеризація категоріальних + числових змінних");
            csvPathField.setText("data/titanic.csv");
        }

        experimentCombo.setSelectedIndex(0);
    }

    // ----------------- ВИБІР CSV -----------------
    private void chooseCsvFile() {
        JFileChooser fc = new JFileChooser();
        if (fc.showOpenDialog(this) == JFileChooser.APPROVE_OPTION) {
            csvPathField.setText(fc.getSelectedFile().getAbsolutePath());
        }
    }

    // ----------------- ЗАПУСК КЛАСТЕРИЗАЦІЇ -----------------
    private void runClustergram() {
        if (clustergramService == null) {
            appendLog("Python entry point не знайдений. Запустіть Python-сервер.");
            return;
        }

        String baseDataset = (String) datasetCombo.getSelectedItem();
        int expId = experimentCombo.getSelectedIndex() + 1;
        String encodedDataset = baseDataset + ":" + expId;

        String csvPath = csvPathField.getText().trim();

        int kMin, kMax;
        try {
            kMin = Integer.parseInt(kMinField.getText().trim());
            kMax = Integer.parseInt(kMaxField.getText().trim());
        } catch (NumberFormatException ex) {
            appendLog("k_min і k_max мають бути цілими.");
            return;
        }

        if (kMin < 2 || kMax <= kMin) {
            appendLog("Невірні значення k.");
            return;
        }

        File outFile = new File("output",
                "clustergram_" + baseDataset + "_exp" + expId +
                        "_k" + kMin + "-" + kMax + ".png");
        String outputPath = outFile.getAbsolutePath();

        appendLog("Запуск кластеризації:");
        appendLog("  Датасет: " + baseDataset);
        appendLog("  Експеримент: " + expId);
        appendLog("  CSV: " + csvPath);
        appendLog("  k: " + kMin + " → " + kMax);
        appendLog("  Вивід: " + outputPath);

        try {
            String msg = clustergramService.runClustergram(
                    encodedDataset, csvPath, kMin, kMax, outputPath);

            appendLog("Python: " + msg);
            displayImage(outputPath);

        } catch (Exception ex) {
            appendLog("Помилка: " + ex.getMessage());
            ex.printStackTrace();
        }
    }

    // ----------------- ВІДОБРАЖЕННЯ PNG -----------------
    private void displayImage(String path) {
        File imgFile = new File(path);
        if (!imgFile.exists()) {
            appendLog("Файл зображення не знайдено: " + path);
            return;
        }

        try {
            BufferedImage img = ImageIO.read(imgFile);
            if (img != null) {
                Image scaled = img;

                int maxW = 800, maxH = 600;
                if (img.getWidth() > maxW || img.getHeight() > maxH) {
                    double scale = Math.min(
                            (double) maxW / img.getWidth(),
                            (double) maxH / img.getHeight()
                    );
                    scaled = img.getScaledInstance(
                            (int) (img.getWidth() * scale),
                            (int) (img.getHeight() * scale),
                            Image.SCALE_SMOOTH
                    );
                }

                imageLabel.setIcon(new ImageIcon(scaled));
                imageLabel.setText(null);
            }
        } catch (IOException e) {
            appendLog("Помилка читання зображення.");
            e.printStackTrace();
        }
    }

    private void appendLog(String msg) {
        logArea.append(msg + "\n");
        logArea.setCaretPosition(logArea.getDocument().getLength());
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> new ClustergramGUI().setVisible(true));
    }
}
