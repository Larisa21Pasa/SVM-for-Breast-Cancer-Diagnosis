import pandas as pd
import csv
import os
import random

import seaborn as sns
from sklearn.model_selection import cross_val_score, GridSearchCV
from sklearn.metrics import confusion_matrix, precision_score, recall_score
from sklearn.preprocessing import MinMaxScaler
from sklearn.metrics import classification_report, confusion_matrix

import numpy as np
import pandas as pd
import requests
from io import StringIO
from sklearn.model_selection import train_test_split
import matplotlib.pyplot as plt
from sklearn.preprocessing import StandardScaler
from sklearn.svm import SVC
from sklearn.model_selection import GridSearchCV, train_test_split
from sklearn.svm import SVC
from sklearn.metrics import accuracy_score

URL_DATASET = "https://archive.ics.uci.edu/ml/machine-learning-databases/breast-cancer-wisconsin/breast-cancer-wisconsin.data"
PATH_DATASET = "breast-cancer-wisconsin.data"
PATH_TRAIN = "breast-cancer-train.csv"
PATH_TEST = "breast-cancer-test.csv"


def download_dataset():
    if os.path.exists(PATH_DATASET):
        print("Fișierul deja există.")
        return

    response = requests.get(URL_DATASET)
    data = response.text

    with open(PATH_DATASET, 'w') as file:
        file.write(data)

    print(f"Fisierul a fost descarcat si salvat la: {PATH_DATASET}")


def normalize_dataset():
    if not os.path.exists(PATH_DATASET):
        print("Fisierul nu exista. Descarca mai intai setul de date.")
        return

    file_path = "breast-cancer-wisconsin.data"
    column_names = ["id", "clump_thickness", "uniformity_of_cell_size", "uniformity_of_cell_shape", "marginal_adhesion",
                    "single_epithelial_cell_size", "bare_nuclei", "bland_chromatin", "normal_nucleoli", "mitoses",
                    "class"]
    df = pd.read_csv(file_path, names=column_names)

    # Delete incomplete data
    df_cleaned = df.replace('?', pd.NA).dropna()

    # Delete id + clsss columns
    X = df_cleaned.drop(['id', 'class'], axis=1)

    # Conversion of classes 2(benign) and 4(malignant) to 1 and -1
    y = df_cleaned['class'].map({2: 1, 4: -1})

    df_cleaned.to_csv(file_path, index=False, header=False)

    # Split the data into training set (70%) and test set (30%)
    # X = features, y = labels (class)
    # random_state = 42 => the dataset will always be split in the same way
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.8, random_state=42)

    # Save the training dataset to a CSV file
    train_file_path = "breast-cancer-train.csv"
    train_df = pd.concat([X_train, y_train], axis=1)
    train_df.to_csv(train_file_path, index=False, header=False)

    # Save the test dataset to a CSV file
    test_file_path = "breast-cancer-test.csv"
    test_df = pd.concat([X_test, y_test], axis=1)
    test_df.to_csv(test_file_path, index=False, header=False)


def choose_parameters():
    # Read the dataset
    df = pd.read_csv(PATH_DATASET, header=None,
                     names=["id", "clump_thickness", "uniformity_of_cell_size", "uniformity_of_cell_shape",
                            "marginal_adhesion", "single_epithelial_cell_size", "bare_nuclei", "bland_chromatin",
                            "normal_nucleoli", "mitoses", "class"])

    # Extract features (X) and labels (y)
    X = df.drop(['class', 'id'], axis=1).values
    y = df["class"].map({2: 0, 4: 1}).values

    # Split the data into training and test sets
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.6, random_state=42)


    # Scale the data to have zero mean and unit variance
    # This step is important for SVM as it is sensitive to the scale of the input features
    scaler = StandardScaler()
    X_train_scaled = scaler.fit_transform(X_train)  # Fit on training data and transform it
    X_test_scaled = scaler.transform(X_test)        # Transform test data using the same scaler

    # Define the grid of parameters for SVM
    param_grid = {'C': [0.00001, 0.0001, 0.001, 0.01, 0.1, 1, 10, 100, 1000, 10000, 100000],
                  'gamma': [0.00001, 0.0001, 0.001, 0.01, 0.1, 1, 10, 100, 1000, 10000, 100000]}

    # Perform grid search using cross-validation
    svm = SVC(kernel='rbf')
    grid_search = GridSearchCV(svm,
                               param_grid,
                               cv=3,
                               n_jobs=-1)
    grid_search.fit(X_train_scaled, y_train)

    # Reshape for heatmap
    scores = grid_search.cv_results_["mean_test_score"].reshape(
        len(param_grid['gamma']),
        len(param_grid['C']))

    # Confusion matrix and analysis
    best_params = grid_search.best_params_
    print("BEST PARAMS AICI",best_params)
    # for alpha in precalculated_alphas:
    best_svm = SVC(kernel='rbf', C=best_params['C'], gamma=best_params['gamma'])
    best_svm.fit(X_train_scaled, y_train)
    y_pred = best_svm.predict(X_test_scaled)

    # Compute confusion matrix
    cm = confusion_matrix(y_test, y_pred)

    # Calculate precision, recall, and specificity
    precision = precision_score(y_test, y_pred)
    recall = recall_score(y_test, y_pred)
    specificity = cm[0, 0] / (cm[0, 0] + cm[0, 1])




    # Interpretation of confusion matrix based on predictions on the test set
    print("\nInterpretation on Test Set:")
    print("* Precision:", precision, "%. Precision is the ratio of true positive predictions to the total predicted positives.")
    print("* Recall:", recall, "%. Recall is the ratio of true positive predictions to the total actual positives.")
    print("* Specificity:", specificity, "%. Specificity is the ratio of true negative predictions to the total actual negatives.")

    # Analysis and Conclusions
    print("\nAnalysis and Conclusions:")
    print("-" * 30)

    # Precision Analysis
    if precision >= 0.95:
        precision_analysis = "The high precision indicates a low rate of false positives, which is crucial in applications where misclassifying negatives is costly."
    elif 0.90 <= precision < 0.95:
        precision_analysis = "The precision is good but may benefit from improvement to reduce false positives further."
    else:
        precision_analysis = "The precision is relatively low, and efforts should be made to improve it for better positive predictive value."

    print(f"Precision: {precision:.2%}")
    print(f"   Precision Analysis: {precision_analysis}")

    # Recall Analysis
    if recall >= 0.95:
        recall_analysis = "The high recall indicates a good ability to capture actual positive cases, which is important in applications where missing positives is critical."
    elif 0.90 <= recall < 0.95:
        recall_analysis = "The recall is good, but there is room for improvement to capture more positive cases."
    else:
        recall_analysis = "The recall is relatively low, and efforts should be made to improve it for better sensitivity."

    print(f"Recall: {recall:.2%}")
    print(f"   Recall Analysis: {recall_analysis}")

    # Specificity Analysis
    if specificity >= 0.95:
        specificity_analysis = "The high specificity indicates a good ability to correctly identify negative cases, minimizing false alarms."
    elif 0.90 <= specificity < 0.95:
        specificity_analysis = "The specificity is good, but there is room for improvement to reduce false negatives further."
    else:
        specificity_analysis = "The specificity is relatively low, and efforts should be made to improve it for better negative predictive value."

    print(f"Specificity: {specificity:.2%}")
    print(f"   Specificity Analysis: {specificity_analysis}")



    # Display the confusion matrix in a tabular format with a legend
    print("\nConfusion Matrix:")
    confusion_df = pd.DataFrame(cm, index=['Actual Non-Cancer', 'Actual Cancer'], columns=['Predicted Non-Cancer', 'Predicted Cancer'])
    print(confusion_df)

    # Legend for Confusion Matrix
    print("\nLegend:")
    print(" - True Positive (TP):", cm[1, 1], "cases of cancer correctly classified as cancer.")
    print(" - False Negative (FN):", cm[1, 0], "cases of cancer incorrectly classified as non-cancer.")
    print(" - True Negative (TN):", cm[0, 0], "cases of non-cancer correctly classified as non-cancer.")
    print(" - False Positive (FP):", cm[0, 1], "cases of non-cancer incorrectly classified as cancer.")

    # Add information about the dataset used for calculations
    print("\nCalculation Details:")
    print("The confusion matrix and metrics were calculated based on predictions made on the test set which actually represent 30% of complete dataset.")

    # Add best parameters and accuracy to the output
    print("\n** Best parameters found during Grid Search are {}".format(best_params))
    print("** Cross-validated Accuracy:", grid_search.best_score_)

    # Heatmap
    plt.figure(figsize=(12, 8))
    sns.heatmap(scores,
                cmap=plt.cm.hot,
                annot=True,
                fmt=".4f",
                cbar=True,
                square=True)

    plt.xlabel("gamma")
    plt.ylabel("C")
    plt.xticks(np.arange(len(param_grid['gamma'])), param_grid['gamma'], rotation=45)
    plt.yticks(np.arange(len(param_grid['C'])), param_grid['C'], rotation=0)

    # Adding labels for better visibility regarding the intersections between C and Gamma
    for i in range(len(scores)):
        for j in range(len(scores[i])):
            plt.text(j, i, round(scores[i][j], 3), color="black", size="small")

    plt.title("Accuracy for different parameters")
    plt.show()

    # Plot accuracy vs C parameter
    plt.figure(figsize=(10, 6))
    plt.title("Accuracy vs C parameter")
    plt.xlabel("C")
    plt.ylabel("Accuracy")
    n = len(param_grid['C'])

    for i in range(n):
        plt.plot(param_grid['C'][:len(scores[i])],  # Slicing for equal dimensions
                 scores[i],
                 'o-', label='gamma=' + str(param_grid['gamma'][i]))

    plt.legend()
    plt.xscale('log')
    plt.show()
print(f"REZULTATE ALEGERE PARAMETRI PENTRU {URL_DATASET}")
download_dataset()
normalize_dataset()
choose_parameters()
# alegere_parametri()

