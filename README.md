# SVM-for-Breast-Cancer-Diagnosis

## Project Overview

This project addresses a significant challenge in healthcare: optimizing breast cancer diagnosis using advanced machine learning techniques. The primary focus is on enhancing the performance of the Support Vector Machine (SVM) by integrating an evolutionary algorithm with real-valued coding. The goal is to optimize both the Lagrange multipliers and the model's bias, leading to more accurate classification of breast tumors, which, in turn, contributes to more effective diagnosis and treatment of breast cancer.

## Dataset Description

Dataset Used: "Breast Cancer Wisconsin (Original)" 

Number of Instances: 699

Malignant Cases: 458 (65.6%)

Benign Cases: 241 (34.5%)

Attributes: 11 (e.g., clump thickness, uniformity of cell size and shape, marginal adhesion, nuclear size)

Purpose: These features are crucial for accurately identifying the types of tumors.

## Methodology

## Classification Approach:

Method: One-versus-One Classification

Implementation: Training of a unique SVM model that evaluates each pair of classes separately, simplifying the classification process while maintaining high accuracy.

## SVM Implementation:

The SVM model is designed to distinguish between two classes of cancer: malignant and benign.

Data Handling: Instances from the dataset are initially read, followed by the random generation of values for the chromosomes' genes needed for the evolutionary algorithm.

Value Range: [0; C], where the value of C is demonstrated in Chapter 6.

## Evolutionary Algorithm:

Process: The evolutionary algorithm runs for 100 generations.

Objective: To maximize and adjust the values of the Lagrange multipliers.

Outcomes:
Calculation of support vectors

Determination of bias

Computation of the hyperplane vector

Calculation of the margin (m)

Verification that the sum of alphas and the class is 0

!!! My contribution to the project development includes code implementation—Genetic Algorithm—and documentation.
