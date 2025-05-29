using System;
using UnityEngine;

public static class Homography {
    public delegate double[] SolverDelegate(double[,] A, double[] B);

    public static Matrix4x4 Calculate(Vector2[] srcPoints, Vector2[] dstPoints, SolverDelegate solverDelegate) {
        // Check if the number of points is correct
        if (srcPoints.Length != 4 || dstPoints.Length != 4) {
            throw new ArgumentException("Four source and destination points are required for homography calculation.");
        }

        double[,] A = new double[8, 8];
        double[] B = new double[8];

        for (int i = 0; i < 4; i++) {
            A[i * 2, 0] = -srcPoints[i][0];
            A[i * 2, 1] = -srcPoints[i][1];
            A[i * 2, 2] = -1;
            A[i * 2, 6] = srcPoints[i][0] * dstPoints[i][0];
            A[i * 2, 7] = srcPoints[i][1] * dstPoints[i][0];

            A[i * 2 + 1, 3] = -srcPoints[i][0];
            A[i * 2 + 1, 4] = -srcPoints[i][1];
            A[i * 2 + 1, 5] = -1;
            A[i * 2 + 1, 6] = srcPoints[i][0] * dstPoints[i][1];
            A[i * 2 + 1, 7] = srcPoints[i][1] * dstPoints[i][1];

            B[i * 2] = -dstPoints[i][0];
            B[i * 2 + 1] = -dstPoints[i][1];
        }

        var solution = solverDelegate(A, B);// Accord.Math.Matrix.Solve(A, B);

        var homographyMatrix = Matrix4x4.identity;
        homographyMatrix[0, 0] = (float)solution[0];
        homographyMatrix[0, 1] = (float)solution[1];
        homographyMatrix[0, 2] = (float)solution[2];
        homographyMatrix[1, 0] = (float)solution[3];
        homographyMatrix[1, 1] = (float)solution[4];
        homographyMatrix[1, 2] = (float)solution[5];
        homographyMatrix[2, 0] = (float)solution[6];
        homographyMatrix[2, 1] = (float)solution[7];
        homographyMatrix[2, 2] = 1;

        return homographyMatrix;
    }

}
