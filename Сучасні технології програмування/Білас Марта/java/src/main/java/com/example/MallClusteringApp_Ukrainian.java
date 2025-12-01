package com.example;

import javax.swing.*;
import javax.swing.border.EmptyBorder;
import javax.swing.border.LineBorder;
import javax.swing.table.DefaultTableModel;
import javax.swing.plaf.basic.BasicScrollBarUI;
import javax.swing.plaf.basic.BasicComboBoxUI;
import javax.swing.plaf.basic.BasicArrowButton;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.util.*;
import java.util.List;
import java.util.stream.Collectors;

/**
 * MallClusteringApp_Ukrainian.java
 *
 * ФІНАЛЬНА РОБОЧА ВЕРСІЯ: Повна українізація інтерфейсу, стилізовані ComboBox та скролбари.
 */
public class MallClusteringApp_Ukrainian extends JFrame {

    // --- 1. Конфігурація та Ініціалізація ---
    private static final int MAX_K = 7;
    private static final int MIN_K = 2;
    private int currentK = 5;

    private final List<DataPoint> initialData;
    private final List<double[]> rawData;

    private ClusterResult currentResult;
    private final ClusteringPanel clusteringPanel;
    private final MetricsPanel metricsPanel;
    private final LegendPanel legendPanel;
    private final ResultFormPanel resultFormPanel;

    private final JComboBox<Integer> kSelector;
    private final JComboBox<String> featureXSelector;
    private final JComboBox<String> featureYSelector;
    private final JTabbedPane tabbedPane;
    private final List<String> ALL_FEATURE_NAMES = Arrays.asList(
            "Річний Дохід (тис. $)", "Оцінка Витрат (1-100)", "Вік (Роки)", "Кредитний Рейтинг (1-10)"
    );

    private static final Color BORDER_COLOR = new Color(25, 140, 200);
    private static final Color ACCENT_COLOR = new Color(18, 60, 120);

    private static final Color[] CLUSTER_COLORS = {
            new Color(255, 99, 132),
            new Color(54, 162, 235),
            new Color(75, 192, 192),
            new Color(255, 205, 86),
            new Color(153, 102, 255),
            new Color(255, 159, 64),
            new Color(199, 199, 199)
    };

    public MallClusteringApp_Ukrainian() {
        // Ініціалізація даних та компонентів
        rawData = loadMallData();
        initialData = normalizeData(rawData);

        // Ініціалізація ComboBox
        kSelector = new JComboBox<>(getKOptions());
        kSelector.setSelectedItem(currentK);

        String[] featureOptions = ALL_FEATURE_NAMES.toArray(new String[0]);
        featureXSelector = new JComboBox<>(featureOptions);
        featureYSelector = new JComboBox<>(featureOptions);
        featureXSelector.setSelectedIndex(0);
        featureYSelector.setSelectedIndex(1);


        tabbedPane = new JTabbedPane();
        clusteringPanel = new ClusteringPanel();
        metricsPanel = new MetricsPanel();
        legendPanel = new LegendPanel();
        resultFormPanel = new ResultFormPanel();

        setTitle("Кластерний Аналіз Клієнтів ТЦ");
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        setSize(1400, 820);
        setLocationRelativeTo(null);

        setLayout(new BorderLayout());

        add(createHeader(), BorderLayout.NORTH);
        add(createMainPanel(), BorderLayout.CENTER);

        runClustering(null);
        setVisible(true);
    }

    // =========================================================================
    //                            ДИЗАЙНЕЛСЬКІ МЕТОДИ
    // =========================================================================

    private JPanel createHeader() {
        JPanel header = new JPanel() {
            @Override
            protected void paintComponent(Graphics g) {
                super.paintComponent(g);
                Graphics2D g2 = (Graphics2D) g;
                GradientPaint gp = new GradientPaint(
                        0, 0, new Color(18, 60, 120),
                        getWidth(), getHeight(), new Color(25, 140, 200)
                );
                g2.setPaint(gp);
                g2.fillRect(0, 0, getWidth(), getHeight());
            }
        };
        header.setOpaque(false);
        header.setLayout(new BorderLayout());
        header.setBorder(new EmptyBorder(12, 18, 12, 18));

        JLabel title = new JLabel("Обробка Кластерного Аналізу Клієнтів ТЦ", JLabel.LEFT);
        title.setFont(new Font("Segoe UI", Font.BOLD, 22));
        title.setForeground(Color.WHITE);
        header.add(title, BorderLayout.WEST);

        // --- ЗМІНА: Використання GridBagLayout для запобігання накладанню ---
        JPanel controlPanel = new JPanel(new GridBagLayout());
        controlPanel.setOpaque(false);
        GridBagConstraints gbc = new GridBagConstraints();
        // Відступи між елементами
        gbc.insets = new Insets(0, 10, 0, 10);
        gbc.anchor = GridBagConstraints.WEST;

        int col = 0;

        // 1. K-селектор
        gbc.gridx = col++;
        gbc.gridy = 0;
        controlPanel.add(makeLabel("K:"), gbc);

        gbc.gridx = col++;
        controlPanel.add(createStyledComboBoxContainer(kSelector), gbc);

        // 2. X-Вісь
        gbc.gridx = col++;
        controlPanel.add(makeLabel("X-Вісь:"), gbc);

        gbc.gridx = col++;
        controlPanel.add(createStyledComboBoxContainer(featureXSelector), gbc);

        // 3. Y-Вісь
        gbc.gridx = col++;
        controlPanel.add(makeLabel("Y-Вісь:"), gbc);

        gbc.gridx = col++;
        controlPanel.add(createStyledComboBoxContainer(featureYSelector), gbc);

        // 4. Кнопка "Запустити Аналіз"
        JButton runButton = makeModernButton("Запустити Аналіз", new Color(0, 150, 150));
        runButton.addActionListener(this::runClustering);
        gbc.gridx = col++;
        // Більший відступ перед кнопкою для візуального розділення
        gbc.insets = new Insets(0, 20, 0, 0);
        controlPanel.add(runButton, gbc);

        // Примітка: Довга мітка про активні ознаки була видалена, щоб не перевантажувати хедер.
        // controlPanel.add(new JLabel(" | Активні ознаки: " + ALL_FEATURE_NAMES.get(featureXSelector.getSelectedIndex()) + ", " + ALL_FEATURE_NAMES.get(featureYSelector.getSelectedIndex())));

        header.add(controlPanel, BorderLayout.EAST);
        return header;
    }

    private JLabel makeLabel(String text) {
        JLabel label = new JLabel(text);
        label.setForeground(Color.WHITE);
        label.setFont(new Font("Segoe UI", Font.BOLD, 14));
        return label;
    }

    private JComponent createStyledComboBoxContainer(JComboBox<?> box) {
        // Стилізація самого JComboBox
        box.setFont(new Font("Consolas", Font.BOLD, 14));
        box.setForeground(ACCENT_COLOR);
        box.setBackground(Color.WHITE);
        box.setUI(new CustomComboBoxUI()); // Застосовуємо стилізований UI

        // Створення контейнера з рамкою для "крутого" вигляду
        JPanel boxContainer = new JPanel(new BorderLayout());
        boxContainer.setBorder(new LineBorder(BORDER_COLOR, 2, true));
        boxContainer.setBackground(Color.WHITE);
        boxContainer.add(box, BorderLayout.CENTER);

        // Встановлення розміру для динамічної ширини
        boxContainer.setMaximumSize(new Dimension(300, 30));
        boxContainer.setPreferredSize(new Dimension(box.getPreferredSize().width + 10, 30));

        return boxContainer;
    }


    private Integer[] getKOptions() {
        Integer[] kOptions = new Integer[MAX_K - MIN_K + 1];
        for (int i = 0; i < kOptions.length; i++) {
            kOptions[i] = MIN_K + i;
        }
        return kOptions;
    }

    private JPanel createMainPanel() {
        JPanel main = new JPanel(new BorderLayout()) {
            @Override
            protected void paintComponent(Graphics g) {
                super.paintComponent(g);
                Graphics2D g2 = (Graphics2D) g;
                GradientPaint gp = new GradientPaint(0, 0, new Color(240, 240, 240), getWidth(), getHeight(), new Color(200, 200, 200));
                g2.setPaint(gp);
                g2.fillRect(0, 0, getWidth(), getHeight());
            }
        };
        main.setBorder(new EmptyBorder(10, 10, 10, 10));

        tabbedPane.setFont(new Font("Segoe UI", Font.BOLD, 14));
        tabbedPane.setOpaque(false);

        // --- C. Вкладка "1. Датасет та Результати" (Перша)
        JPanel resultsTabPanel = new JPanel(new BorderLayout());
        JScrollPane initialTablePanel = createInitialEmptyTablePanel();

        JSplitPane dataSplitPane = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT, initialTablePanel, resultFormPanel);
        dataSplitPane.setResizeWeight(0.7);

        resultsTabPanel.add(dataSplitPane, BorderLayout.CENTER);
        tabbedPane.addTab("1. Дані та Результати", resultsTabPanel);

        // --- A. Вкладка "2. Візуалізація Кластерів" (Друга)
        JPanel vizPanel = new JPanel(new BorderLayout());
        vizPanel.add(clusteringPanel, BorderLayout.CENTER);
        vizPanel.add(legendPanel, BorderLayout.EAST);
        tabbedPane.addTab("2. Візуалізація Кластерів", vizPanel);

        // --- B. Вкладка "3. Аналіз Метрик" (Третя)
        tabbedPane.addTab("3. Аналіз Метрик", metricsPanel);

        main.add(tabbedPane, BorderLayout.CENTER);
        return main;
    }

    private JScrollPane createInitialEmptyTablePanel() {
        // Динамічний набір колонок
        String[] colNames = new String[ALL_FEATURE_NAMES.size() * 2 + 1];
        for(int i = 0; i < ALL_FEATURE_NAMES.size(); i++) {
            colNames[i] = ALL_FEATURE_NAMES.get(i) + " (Сирі)";
            colNames[i + ALL_FEATURE_NAMES.size()] = ALL_FEATURE_NAMES.get(i) + " (Норм)";
        }
        colNames[ALL_FEATURE_NAMES.size() * 2] = "ID Кластера";

        JTable table = new JTable(new DefaultTableModel(null, colNames));
        table.setFont(new Font("Consolas", Font.PLAIN, 14));
        table.setAutoResizeMode(JTable.AUTO_RESIZE_OFF);

        JScrollPane scrollPane = new JScrollPane(table);
        // СТИЛІЗАЦІЯ СКРОЛБАРІВ
        customizeScrollPane(scrollPane);

        return scrollPane;
    }

    private void customizeScrollPane(JScrollPane scrollPane) {
        scrollPane.getVerticalScrollBar().setUI(new CustomScrollBarUI(new Color(54, 162, 235)));
        scrollPane.getHorizontalScrollBar().setUI(new CustomScrollBarUI(new Color(54, 162, 235)));
        scrollPane.setBorder(BorderFactory.createEmptyBorder());
    }

    private JButton makeModernButton(String text, Color color) {
        JButton b = new JButton(text) {
            @Override
            protected void paintComponent(Graphics g) {
                Graphics2D g2 = (Graphics2D) g;
                g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
                g2.setColor(getModel().isPressed() ? color.darker() : getModel().isRollover() ? color.brighter() : color);
                g2.fillRoundRect(0, 0, getWidth(), getHeight(), 12, 12);
                super.paintComponent(g);
            }
        };
        b.setContentAreaFilled(false);
        b.setBorderPainted(false);
        b.setFocusPainted(false);
        b.setForeground(Color.WHITE);
        b.setFont(new Font("Segoe UI", Font.BOLD, 14));
        b.setPreferredSize(new Dimension(180, 38));
        return b;
    }

    // =========================================================================
    //                            ЛОГІКА КЛАСТЕРИЗАЦІЇ
    // =========================================================================

    private JScrollPane createResultsTablePanel(List<double[]> rawData, List<DataPoint> clusteredData) {
        java.lang.String[] rawNames = ALL_FEATURE_NAMES.stream().map(s -> s + " (Сирі)").toArray(java.lang.String[]::new);
        java.lang.String[] normNames = ALL_FEATURE_NAMES.stream().map(s -> s + " (Норм)").toArray(java.lang.String[]::new);

        List<java.lang.String> columnList = new ArrayList<>();
        columnList.addAll(Arrays.asList(rawNames));
        columnList.addAll(Arrays.asList(normNames));
        columnList.add("ID Кластера");

        java.lang.String[] columnNames = columnList.toArray(new java.lang.String[0]);
        Object[][] tableData = new Object[rawData.size()][columnNames.length];

        int numFeatures = ALL_FEATURE_NAMES.size();

        for (int i = 0; i < rawData.size(); i++) {
            double[] rawRow = rawData.get(i);
            double[] normRow = clusteredData.get(i).getFeatures();
            int clusterId = clusteredData.get(i).getClusterId();

            int col = 0;
            // Сирі дані
            for (int j = 0; j < numFeatures; j++) {
                tableData[i][col++] = String.format("%.2f", rawRow[j]);
            }
            // Нормалізовані дані
            for (int j = 0; j < numFeatures; j++) {
                tableData[i][col++] = String.format("%.4f", normRow[j]);
            }
            tableData[i][col] = clusterId;
        }

        JTable table = new JTable(new DefaultTableModel(tableData, columnNames));
        table.setFont(new Font("Consolas", Font.PLAIN, 14));
        JScrollPane scrollPane = new JScrollPane(table);
        table.setAutoResizeMode(JTable.AUTO_RESIZE_OFF);

        customizeScrollPane(scrollPane);

        return scrollPane;
    }


    private List<double[]> loadMallData() {
        // Дані тепер мають 4 ознаки (Income, Spending, Age, Credit Score)
        List<double[]> data = new ArrayList<>();
        Random rand = new Random();

        for (int i = 0; i < 50; i++) data.add(new double[]{rand.nextDouble() * 20 + 20, rand.nextDouble() * 20 + 10, rand.nextDouble() * 20 + 20, rand.nextDouble() * 2 + 1}); // Low/Low
        for (int i = 0; i < 50; i++) data.add(new double[]{rand.nextDouble() * 20 + 70, rand.nextDouble() * 20 + 70, rand.nextDouble() * 20 + 30, rand.nextDouble() * 2 + 8}); // High/High
        for (int i = 0; i < 50; i++) data.add(new double[]{rand.nextDouble() * 20 + 20, rand.nextDouble() * 20 + 70, rand.nextDouble() * 20 + 50, rand.nextDouble() * 2 + 5}); // Low Income/High Spend
        for (int i = 0; i < 50; i++) data.add(new double[]{rand.nextDouble() * 20 + 70, rand.nextDouble() * 20 + 10, rand.nextDouble() * 20 + 60, rand.nextDouble() * 2 + 3}); // High Income/Low Spend

        return data;
    }

    private List<DataPoint> normalizeData(List<double[]> rawData) {
        if (rawData.isEmpty()) return Collections.emptyList();
        int numFeatures = ALL_FEATURE_NAMES.size();
        double[] mins = new double[numFeatures];
        double[] maxs = new double[numFeatures];

        for (int j = 0; j < numFeatures; j++) {
            mins[j] = rawData.get(0)[j];
            maxs[j] = rawData.get(0)[j];
        }
        for (double[] row : rawData) {
            for (int j = 0; j < numFeatures; j++) {
                mins[j] = Math.min(mins[j], row[j]);
                maxs[j] = Math.max(maxs[j], row[j]);
            }
        }

        List<DataPoint> normalizedData = new ArrayList<>();
        for (double[] row : rawData) {
            double[] normFeatures = new double[numFeatures];
            for (int j = 0; j < numFeatures; j++) {
                double range = maxs[j] - mins[j];
                normFeatures[j] = (range == 0) ? 0.5 : (row[j] - mins[j]) / range;
            }
            normalizedData.add(new DataPoint(normFeatures));
        }
        return normalizedData;
    }

    private void runClustering(ActionEvent e) {
        currentK = (Integer) kSelector.getSelectedItem();
        if (currentK < MIN_K || currentK > MAX_K) return;

        // Оновлюємо індекси візуалізації
        int selectedIndexX = featureXSelector.getSelectedIndex();
        int selectedIndexY = featureYSelector.getSelectedIndex();

        currentResult = null;

        List<DataPoint> dataCopy = initialData.stream()
                .map(p -> new DataPoint(p.getFeatures()))
                .collect(Collectors.toList());

        KMeansClusterer clusterer = new KMeansClusterer(dataCopy, currentK, 100);
        List<DataPoint> clusteredPoints = clusterer.cluster();

        currentResult = new ClusterResult(clusteredPoints, clusterer.getCentroids());

        // Оновлення таблиці результатів:
        JScrollPane newScrollPane = createResultsTablePanel(rawData, clusteredPoints);

        // Оновлення компонента на вкладці 1
        JPanel resultsTabPanel = (JPanel) tabbedPane.getComponentAt(0);
        JSplitPane dataSplitPane = (JSplitPane) resultsTabPanel.getComponent(0);
        dataSplitPane.setLeftComponent(newScrollPane);

        // Оновлення форми результатів
        resultFormPanel.updateResults(currentResult, calculateMetricScores(new SilhouetteCalculator()), calculateMetricScores(new CalinskiHarabaszCalculator()));

        // Оновлення всіх візуалізацій
        clusteringPanel.setVisualizationIndices(selectedIndexX, selectedIndexY); // Встановлюємо нові ознаки
        clusteringPanel.repaint();
        metricsPanel.repaint();
        legendPanel.repaint();
        metricsPanel.revalidate();
    }

    private Map<Integer, Double> calculateMetricScores(Object calculator) {
        Map<Integer, Double> scores = new HashMap<>();
        for (int k_val = MIN_K; k_val <= MAX_K; k_val++) {
            final int currentKValue = k_val;
            List<DataPoint> testDataCopy = initialData.stream()
                    .map(p -> new DataPoint(p.getFeatures()))
                    .collect(Collectors.toList());

            KMeansClusterer testClusterer = new KMeansClusterer(testDataCopy, currentKValue, 100);
            List<DataPoint> clusteredPoints = testClusterer.cluster();

            double score = 0.0;
            if (calculator instanceof SilhouetteCalculator) {
                score = ((SilhouetteCalculator) calculator).calculateOverallSilhouette(clusteredPoints);
            } else if (calculator instanceof CalinskiHarabaszCalculator) {
                score = ((CalinskiHarabaszCalculator) calculator).calculateCH(clusteredPoints, testClusterer.getCentroids());
            }
            scores.put(currentKValue, score);
        }
        return scores;
    }

    // --- 4. Класи Моделі Даних та Алгоритмів ---

    private class DataPoint {
        private double[] features;
        private int clusterId = -1;
        public DataPoint(double... features) { this.features = features; }
        public double[] getFeatures() { return features; }
        public int getClusterId() { return clusterId; }
        public void setClusterId(int clusterId) { this.clusterId = clusterId; }
        public double distanceTo(DataPoint other) {
            double sum = 0;
            for (int i = 0; i < features.length; i++) sum += Math.pow(features[i] - other.features[i], 2);
            return Math.sqrt(sum);
        }
    }

    private class ClusterResult {
        private final List<DataPoint> clusteredPoints;
        private final List<DataPoint> centroids;
        public ClusterResult(List<DataPoint> clusteredPoints, List<DataPoint> centroids) {
            this.clusteredPoints = clusteredPoints;
            this.centroids = centroids;
        }
        public List<DataPoint> getClusteredPoints() { return clusteredPoints; }
        public List<DataPoint> getCentroids() { return centroids; }
    }

    private class KMeansClusterer {
        private final List<DataPoint> dataPoints;
        private final int k;
        private final int maxIterations;
        private final List<DataPoint> centroids = new ArrayList<>();

        public KMeansClusterer(List<DataPoint> dataPoints, int k, int maxIterations) {
            this.dataPoints = dataPoints; this.k = k; this.maxIterations = maxIterations;
        }
        public List<DataPoint> getCentroids() { return centroids; }
        private void initializeCentroids() {
            Random random = new Random();
            Set<Integer> initialIndices = new HashSet<>();
            while (initialIndices.size() < k) {
                initialIndices.add(random.nextInt(dataPoints.size()));
            }
            for (int index : initialIndices) {
                centroids.add(new DataPoint(dataPoints.get(index).getFeatures()));
            }
        }
        private void assignPointsToClusters() {
            for (DataPoint point : dataPoints) {
                double minDistance = Double.MAX_VALUE;
                int closestCluster = -1;

                for (int i = 0; i < centroids.size(); i++) {
                    double distance = point.distanceTo(centroids.get(i));
                    if (distance < minDistance) {
                        minDistance = distance;
                        closestCluster = i;
                    }
                }
                point.setClusterId(closestCluster);
            }
        }
        private boolean updateCentroids() {
            boolean changed = false;

            for (int i = 0; i < k; i++) {
                final int clusterIndex = i;
                List<DataPoint> pointsInCluster = dataPoints.stream()
                        .filter(p -> p.getClusterId() == clusterIndex)
                        .collect(Collectors.toList());

                if (pointsInCluster.isEmpty()) { continue; }

                double[] sumFeatures = new double[dataPoints.get(0).getFeatures().length];
                for (DataPoint point : pointsInCluster) {
                    for (int j = 0; j < sumFeatures.length; j++) {
                        sumFeatures[j] += point.getFeatures()[j];
                    }
                }

                double[] newFeatures = new double[sumFeatures.length];
                for (int j = 0; j < sumFeatures.length; j++) {
                    newFeatures[j] = sumFeatures[j] / pointsInCluster.size();
                }

                DataPoint newCentroid = new DataPoint(newFeatures);
                if (newCentroid.distanceTo(centroids.get(i)) > 1e-6) { changed = true; }
                centroids.get(i).features = newFeatures;
            }
            return changed;
        }
        public List<DataPoint> cluster() {
            initializeCentroids();
            for (int i = 0; i < maxIterations; i++) {
                assignPointsToClusters();
                if (!updateCentroids()) break;
            }
            assignPointsToClusters();
            return dataPoints;
        }
    }

    private class SilhouetteCalculator {
        public double calculateOverallSilhouette(List<DataPoint> points) {
            if (points == null || points.size() <= 1) return 0.0;
            Map<Integer, List<DataPoint>> clusters = points.stream()
                    .collect(Collectors.groupingBy(DataPoint::getClusterId));
            if (clusters.size() <= 1) return 0.0;
            double totalSilhouette = 0.0;
            for (DataPoint point : points) {
                int currentClusterId = point.getClusterId();
                List<DataPoint> currentCluster = clusters.get(currentClusterId);
                double a_i = 0.0;
                if (currentCluster.size() > 1) {
                    double sumDistances = currentCluster.stream()
                            .filter(other -> other != point)
                            .mapToDouble(point::distanceTo)
                            .sum();
                    a_i = sumDistances / (currentCluster.size() - 1);
                }
                double b_i = Double.MAX_VALUE;
                for (Map.Entry<Integer, List<DataPoint>> entry : clusters.entrySet()) {
                    int otherClusterId = entry.getKey();
                    if (otherClusterId != currentClusterId) {
                        List<DataPoint> otherCluster = entry.getValue();
                        if (otherCluster.isEmpty()) continue;
                        double avgDistance = otherCluster.stream()
                                .mapToDouble(point::distanceTo)
                                .average()
                                .orElse(Double.MAX_VALUE);
                        b_i = Math.min(b_i, avgDistance);
                    }
                }
                double s_i = (b_i != Double.MAX_VALUE) ? (b_i - a_i) / Math.max(a_i, b_i) : 0.0;
                totalSilhouette += s_i;
            }
            return totalSilhouette / points.size();
        }
    }

    private class CalinskiHarabaszCalculator {
        public double calculateCH(List<DataPoint> points, List<DataPoint> centroids) {
            if (points.get(0) == null) return 0.0;
            int N = points.size();
            int K = centroids.size();
            if (K <= 1 || N <= K) return 0.0;
            int numFeatures = ALL_FEATURE_NAMES.size();
            double[] globalMean = new double[numFeatures];
            for (DataPoint p : points) {
                for (int i = 0; i < numFeatures; i++) {
                    globalMean[i] += p.getFeatures()[i];
                }
            }
            for (int i = 0; i < numFeatures; i++) {
                globalMean[i] /= N;
            }
            DataPoint globalCenter = new DataPoint(globalMean);

            Map<Integer, List<DataPoint>> clusters = points.stream()
                    .collect(Collectors.groupingBy(DataPoint::getClusterId));

            double Tr_B = 0.0;
            for (int i = 0; i < K; i++) {
                DataPoint centroid = centroids.get(i);
                int n_i = clusters.getOrDefault(i, Collections.emptyList()).size();
                double distSq = centroid.distanceTo(globalCenter);
                Tr_B += (n_i * distSq * distSq);
            }

            double Tr_W = 0.0;
            for (int i = 0; i < K; i++) {
                DataPoint centroid = centroids.get(i);
                for (DataPoint p : clusters.getOrDefault(i, Collections.emptyList())) {
                    double distSq = p.distanceTo(centroid);
                    Tr_W += (distSq * distSq);
                }
            }
            double CH_score = (Tr_B / (K - 1)) / (Tr_W / (N - K));
            return CH_score;
        }
    }

    // --- 5. Візуалізація Кластерів (Вкладка 2) ---

    private class ClusteringPanel extends JPanel {
        private final int PADDING = 40;
        private final int DOT_SIZE = 8;

        // Динамічні індекси
        private int visFeatureXIndex = 0;
        private int visFeatureYIndex = 1;

        public ClusteringPanel() { setLayout(new BorderLayout()); }

        public void setVisualizationIndices(int x, int y) {
            this.visFeatureXIndex = x;
            this.visFeatureYIndex = y;
        }

        @Override
        protected void paintComponent(Graphics g) {
            super.paintComponent(g);
            Graphics2D g2d = (Graphics2D) g;
            g2d.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

            if (currentResult == null) return;

            int plotWidth = getWidth() - 2 * PADDING;
            int plotHeight = getHeight() - 2 * PADDING;
            int plotXStart = PADDING;
            int plotYStart = PADDING;

            g2d.drawRect(plotXStart, plotYStart, plotWidth, plotHeight);

            g2d.setColor(Color.BLACK);
            g2d.setFont(new Font("Arial", Font.BOLD, 14));
            g2d.drawString("2D Візуалізація Кластерів (K=" + currentK + ")", plotXStart, plotYStart - 10);

            // Динамічні підписи осей
            g2d.drawString(ALL_FEATURE_NAMES.get(visFeatureXIndex) + " (Норм.)", plotXStart + plotWidth / 2 - 80, plotYStart + plotHeight + 30);
            g2d.rotate(-Math.PI / 2);
            g2d.drawString(ALL_FEATURE_NAMES.get(visFeatureYIndex) + " (Норм.)", -(plotYStart + plotHeight / 2 + 80), plotXStart - 25);
            g2d.rotate(Math.PI / 2);

            for (DataPoint point : currentResult.getClusteredPoints()) {
                // Використовуємо динамічні індекси
                double x = point.getFeatures()[visFeatureXIndex];
                double y = point.getFeatures()[visFeatureYIndex];

                int screenX = plotXStart + (int) (x * plotWidth);
                int screenY = plotYStart + plotHeight - (int) (y * plotHeight);

                int clusterId = point.getClusterId();
                if (clusterId >= 0 && clusterId < CLUSTER_COLORS.length) g2d.setColor(CLUSTER_COLORS[clusterId]);
                else g2d.setColor(Color.LIGHT_GRAY);
                g2d.fillOval(screenX - DOT_SIZE / 2, screenY - DOT_SIZE / 2, DOT_SIZE, DOT_SIZE);
            }

            g2d.setColor(Color.BLACK);
            g2d.setStroke(new BasicStroke(2));
            for (DataPoint centroid : currentResult.getCentroids()) {
                // Використовуємо динамічні індекси
                double x = centroid.getFeatures()[visFeatureXIndex];
                double y = centroid.getFeatures()[visFeatureYIndex];

                int screenX = plotXStart + (int) (x * plotWidth);
                int screenY = plotYStart + plotHeight - (int) (y * plotHeight);

                g2d.drawRect(screenX - DOT_SIZE, screenY - DOT_SIZE, DOT_SIZE * 2, DOT_SIZE * 2);
            }
        }
    }

    // --- 6. Панель Метрик (Вкладка 3) ---

    private class MetricsPanel extends JPanel {
        public MetricsPanel() {
            setLayout(new GridLayout(2, 1, 10, 10)); // Розділення на 2 графіки

            add(new MetricGraphPanel("Оцінка Силуету (Максимум краще, -1 до 1)", false));
            add(new MetricGraphPanel("Оцінка Калінскі-Харабаш (Максимум краще)", true));
        }

        private class MetricGraphPanel extends JPanel {
            private final String title;
            private final boolean isCH;
            private final MallClusteringApp_Ukrainian outerClass;

            public MetricGraphPanel(String title, boolean isCH) {
                this.title = title;
                this.isCH = isCH;
                this.outerClass = MallClusteringApp_Ukrainian.this;
                setBackground(Color.WHITE);
            }

            @Override
            protected void paintComponent(Graphics g) {
                super.paintComponent(g);
                if (outerClass.currentResult == null) return;

                Graphics2D g2d = (Graphics2D) g;
                g2d.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

                int PADDING = 40;
                int width = getWidth();
                int height = getHeight();
                int plotWidth = width - 2 * PADDING;
                int plotHeight = height - 2 * PADDING;
                int xStart = PADDING;
                int yStart = PADDING;

                Map<Integer, Double> scores = outerClass.calculateMetricScores(isCH ? new CalinskiHarabaszCalculator() : new SilhouetteCalculator());

                g2d.setColor(Color.BLACK);
                g2d.setFont(new Font("Arial", Font.BOLD, 14));
                g2d.drawString(title, xStart, yStart - 5);
                g2d.drawRect(xStart, yStart, plotWidth, plotHeight);

                drawMetricGraph(g2d, scores, xStart, yStart, plotWidth, plotHeight, isCH);
            }

            private void drawMetricGraph(Graphics2D g2d, Map<Integer, Double> scores, int xStart, int yStart, int width, int height, boolean isCH) {
                if (scores.isEmpty()) return;

                int numK = MAX_K - MIN_K + 1;
                int stepWidth = width / numK;

                double maxScore = scores.values().stream().mapToDouble(d -> d).max().orElse(1.0);
                double minScore = isCH ? 0 : scores.values().stream().mapToDouble(d -> d).min().orElse(-1.0);
                double scoreRange = maxScore - minScore;
                if (scoreRange == 0) scoreRange = 1.0;

                int zeroLineY = yStart + height;
                if (!isCH) {
                    double zeroNorm = (0.0 - minScore) / scoreRange;
                    zeroLineY = yStart + height - (int)(zeroNorm * height);
                    g2d.setColor(Color.GRAY.darker());
                    g2d.drawLine(xStart, zeroLineY, xStart + width, zeroLineY);
                    g2d.setColor(Color.BLACK);
                }

                int prevX = -1;
                int prevY = -1;
                int markerSize = 6;

                for (int k_val = MIN_K; k_val <= MAX_K; k_val++) {
                    double score = scores.getOrDefault(k_val, 0.0);
                    int index = k_val - MIN_K;

                    int pointX = xStart + index * stepWidth + stepWidth / 2;

                    double scoreNorm = (score - minScore) / scoreRange;
                    int pointY = yStart + height - (int)(scoreNorm * height);

                    if (prevX != -1) {
                        g2d.setStroke(new BasicStroke(2));
                        g2d.setColor(Color.BLUE.darker());
                        g2d.drawLine(prevX, prevY, pointX, pointY);
                    }

                    g2d.setColor(k_val == outerClass.currentK ? Color.RED.darker() : Color.BLACK);
                    g2d.fillOval(pointX - markerSize / 2, pointY - markerSize / 2, markerSize, markerSize);

                    String scoreStr = String.format(isCH ? "%.0f" : "%.2f", score);
                    g2d.setFont(new Font("Arial", Font.PLAIN, 10));
                    g2d.drawString(scoreStr, pointX - 10, pointY - 10);

                    g2d.drawString(String.valueOf(k_val), pointX - 3, yStart + height + 15);

                    if (k_val == outerClass.currentK) {
                        g2d.setStroke(new BasicStroke(3));
                        g2d.setColor(Color.RED.darker());
                        g2d.drawOval(pointX - markerSize / 2 - 2, pointY - markerSize / 2 - 2, markerSize + 4, markerSize + 4);
                    }

                    prevX = pointX;
                    prevY = pointY;
                }
                g2d.setFont(new Font("Arial", Font.BOLD, 12));
                g2d.drawString("K", xStart + width / 2 - 10, yStart + height + 35);
            }
        }
    }

    // --- 7. Панель Легенди ---

    private class LegendPanel extends JPanel {
        public LegendPanel() {
            setPreferredSize(new Dimension(200, 0));
            setBackground(new Color(250, 250, 250));
        }

        @Override
        protected void paintComponent(Graphics g) {
            super.paintComponent(g);
            Graphics2D g2d = (Graphics2D) g;
            g2d.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

            g2d.setColor(Color.BLACK);
            g2d.setFont(new Font("Arial", Font.BOLD, 14));
            g2d.drawString("Легенда Кластерів", 15, 30);

            if (currentResult == null) return;

            int startY = 60;
            int colorBoxSize = 15;
            int currentClusters = currentResult.getCentroids().size();

            for (int i = 0; i < currentClusters; i++) {
                g2d.setColor(CLUSTER_COLORS[i % CLUSTER_COLORS.length]);
                g2d.fillRect(15, startY + i * 30, colorBoxSize, colorBoxSize);

                g2d.setColor(Color.BLACK);
                g2d.setFont(new Font("Arial", Font.PLAIN, 12));

                String description;
                if (currentClusters == 4) { // Оскільки тепер 4 кластери в симуляції
                    description = switch (i) {
                        case 0 -> "Низькі витрати/Молоді";
                        case 1 -> "Високі витрати/Високий дохід";
                        case 2 -> "Низький дохід/Високі витрати";
                        case 3 -> "Високий дохід/Низькі витрати";
                        default -> "Кластер " + (i + 1);
                    };
                } else {
                    description = "Кластер " + (i + 1);
                }

                g2d.drawString(description, 15 + colorBoxSize + 10, startY + i * 30 + 13);

                g2d.setColor(Color.BLACK);
                g2d.drawRect(15 + colorBoxSize + 10 + g2d.getFontMetrics().stringWidth(description) + 5,
                        startY + i * 30, colorBoxSize, colorBoxSize);
            }
        }
    }

    // --- 8. Форма Числових Результатів (Нова Панель) ---

    private class ResultFormPanel extends JPanel {
        private final JLabel silhouetteLabel;
        private final JLabel calinskiLabel;
        private final JTextArea centroidArea;

        public ResultFormPanel() {
            setLayout(new BorderLayout(10, 10));
            setBorder(BorderFactory.createTitledBorder(
                    BorderFactory.createLineBorder(Color.GRAY),
                    "Числові Результати (K=" + currentK + ")",
                    0, 0,
                    new Font("Segoe UI", Font.BOLD, 14),
                    Color.BLACK));

            // 1. Панель метрик (Північ)
            JPanel metricsPanel = new JPanel(new GridLayout(2, 1, 5, 5));
            silhouetteLabel = new JLabel("Силует: N/A");
            calinskiLabel = new JLabel("Кал.-Харабаш: N/A");

            metricsPanel.add(silhouetteLabel);
            metricsPanel.add(calinskiLabel);
            metricsPanel.setBorder(BorderFactory.createEmptyBorder(10, 10, 0, 10));

            add(metricsPanel, BorderLayout.NORTH);

            // 2. Панель центроїдів (Центр)
            centroidArea = new JTextArea();
            centroidArea.setEditable(false);
            centroidArea.setFont(new Font("Monospaced", Font.PLAIN, 12));
            JScrollPane centroidScrollPane = new JScrollPane(centroidArea);

            // Стилізація скролбарів JTextArea
            customizeScrollPane(centroidScrollPane);

            add(centroidScrollPane, BorderLayout.CENTER);
        }

        public void updateResults(ClusterResult result, Map<Integer, Double> silScores, Map<Integer, Double> chScores) {
            // Оновлення заголовка
            ((javax.swing.border.TitledBorder) getBorder()).setTitle("Числові Результати (K=" + currentK + ")");

            // Оновлення метрик
            double sil = silScores.getOrDefault(currentK, 0.0);
            double ch = chScores.getOrDefault(currentK, 0.0);

            silhouetteLabel.setText(String.format("Силует: %.4f", sil));
            calinskiLabel.setText(String.format("Кал.-Харабаш: %.4f", ch));

            // Оновлення центроїдів
            StringBuilder sb = new StringBuilder();
            sb.append(String.format("Центроїди (K=%d):\n", currentK));
            sb.append("-------------------------------------------\n");

            int c = 0;
            int numFeatures = ALL_FEATURE_NAMES.size();
            for (DataPoint centroid : result.getCentroids()) {
                sb.append(String.format("C%d: ", c + 1));
                for (int i = 0; i < numFeatures; i++) {
                    sb.append(String.format("%s: %.4f, ", ALL_FEATURE_NAMES.get(i), centroid.getFeatures()[i]));
                }
                sb.setLength(sb.length() - 2); // Видалення останньої коми та пробілу
                sb.append("\n");
                c++;
            }
            centroidArea.setText(sb.toString());
            repaint();
        }
    }

    // --- 9. Клас для Стилізації Скролбарів (CustomScrollBarUI) ---

    private class CustomScrollBarUI extends BasicScrollBarUI {
        private final Color thumbColor;

        CustomScrollBarUI(Color thumbColor) {
            this.thumbColor = thumbColor;
        }

        @Override
        protected void configureScrollBarColors() {
            this.thumbDarkShadowColor = thumbColor.darker();
            this.thumbLightShadowColor = thumbColor.brighter();
            this.thumbHighlightColor = thumbColor.brighter();
            this.trackColor = new Color(240, 240, 240);
        }

        @Override
        protected void installDefaults() {
            super.installDefaults();
            UIManager.put("ScrollBar.width", 12);
        }


        // Прибираємо стандартні кнопки зі стрілками
        @Override
        protected JButton createDecreaseButton(int orientation) {
            return createZeroButton();
        }

        @Override
        protected JButton createIncreaseButton(int orientation) {
            return createZeroButton();
        }

        private JButton createZeroButton() {
            JButton button = new JButton();
            button.setPreferredSize(new Dimension(0, 0));
            button.setMinimumSize(new Dimension(0, 0));
            button.setMaximumSize(new Dimension(0, 0));
            return button;
        }

        @Override
        protected void paintThumb(Graphics g, JComponent c, Rectangle thumbBounds) {
            Graphics2D g2 = (Graphics2D) g.create();
            g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

            g2.setColor(thumbColor);
            g2.fillRoundRect(thumbBounds.x + 2, thumbBounds.y + 2,
                    thumbBounds.width - 4, thumbBounds.height - 4, 10, 10);
            g2.dispose();
        }
    }

    // --- 10. Клас для Стилізації JComboBox (Новий) ---

    private class CustomComboBoxUI extends BasicComboBoxUI {

        @Override
        public void installUI(JComponent c) {
            super.installUI(c);
            // Приховуємо стандартну рамку, оскільки контейнер малює свою
            c.setBorder(new EmptyBorder(2, 4, 2, 4));
        }

        @Override
        protected JButton createArrowButton() {
            // Створюємо плоску кнопку зі стрілкою
            JButton button = new BasicArrowButton(
                    BasicArrowButton.SOUTH,
                    Color.WHITE, // Фон кнопки
                    Color.WHITE, // Shadow
                    ACCENT_COLOR, // Dark Shadow (стрілка буде цим кольором)
                    Color.WHITE  // Highlight
            );
            button.setBorder(BorderFactory.createEmptyBorder()); // Прибираємо рамку
            return button;
        }

        @Override
        public void paintCurrentValueBackground(Graphics g, Rectangle bounds, boolean hasFocus) {
            // Забезпечуємо, що фон тексту завжди білий
            g.setColor(comboBox.getBackground());
            g.fillRect(bounds.x, bounds.y, bounds.width, bounds.height);
        }

        // Перевизначаємо, щоб запобігти малюванню рамки ComboBox, оскільки її малює контейнер
        @Override
        public void paint(Graphics g, JComponent c) {
            Rectangle r = rectangleForCurrentValue();
            paintCurrentValue(g, r, hasFocus);
        }
    }


    public static void main(String[] args) {
        try {
            UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
        } catch (Exception e) {
            e.printStackTrace();
        }
        SwingUtilities.invokeLater(MallClusteringApp_Ukrainian::new);
    }
}