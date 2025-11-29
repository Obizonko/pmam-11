package org.example;

public interface IClustergram {
    String runClustergram(String datasetName,
                          String csvPath,
                          int kMin,
                          int kMax,
                          String outputPath);
}
