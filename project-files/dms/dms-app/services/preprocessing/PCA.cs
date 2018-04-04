using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.services.preprocessing
{
    class PCA
    {
        private IList<double[]> _eigenVectors = null;

        public PCA()
        {

        }

        public PCA(List<double[]> dataSet, double accuracyQR, int maxIterationQR, int componentsNumber)
        {

            double[][] cov = CovarianceMatrixOfData(dataSet);

            IList<double[][]> eigen = EigenVectorValuesExtractionQRIterative(cov.ToList(), accuracyQR, maxIterationQR);
            IList<double[]> eigenVectors = eigen[0].ToList();// LinearAlgebra.DecomposeMatrixToColumnVectors(eigen[0].ToList());

            if (componentsNumber > eigenVectors.Count)
            {
                throw new ArgumentException("componentsNumber > eigenVectors.Count");
            }

            _eigenVectors = new List<double[]>();
            for (int i = 0; i < componentsNumber; i++)
            {
                _eigenVectors.Add(eigenVectors[i]);
            }

        }

        private double ScalarVectorProduct(double[] x, double[] y)
        {
            int n = x.Count();
            double res = 0.0;
            for (int i = 0; i < n; i++)
            {
                res += x[i] * y[i];
            }
            return res;
        }

        public double Covariance(double[] x, double[] y)
        {
            int n = x.Count();
            double res = ScalarVectorProduct(x, y);
            res /= n;
            res -= (x.Sum() / n) * (y.Sum() / n);

            return res;
        }

        public double[][] CovarianceMatrixOfData(IList<double[]> values)
        {
            double[][] covMatrix = new double[values.Count][];
            for (int i = 0; i < values.Count; i++)
            {
                covMatrix[i] = new double[values.Count];
                for (int j = 0; j < values.Count; j++)
                {
                    covMatrix[i][j] = Covariance(values[i], values[j]);
                }
            }
            return covMatrix;
        }

        private double NormOfVector(double[] x)
        {
            double res = 0.0;
            foreach(var val in x)
            {
                res += val * val;
            }
            res = Math.Sqrt(res);
            return res;
        }

        private double[] ScalarToVectorProduct(double k, double[] x)
        {
            double[] res = new double[x.Count()];
            for(int i = 0; i < x.Count(); i++)
            {
                res[i] = k * x[i];
            }
            return res;
        }

        public double[] VectorProjection(double[] a, double[] b)
        {
            double k = ScalarVectorProduct(a, b) / ScalarVectorProduct(b, b);
            return ScalarToVectorProduct(k, b);
        }
        private double[][] Transpose(double[][] a)
        {//тока квадратное
            double tmp;
            for (int i = 0; i < a.Count(); i++)
            {
                for (int j = 0; j < i; j++)
                {
                    tmp = a[i][j];
                    a[i][j] = a[j][i];
                    a[j][i] = tmp;
                }
            }
            return a;
        }

        IList<double[][]> QRDecompositionGS(List<double[]> av)
        {
            int n = av.Count;
            List<double[]> u = new List<double[]>();
            u.Add(av[0]);
            List<double[]> e = new List<double[]>();
            e.Add(ScalarToVectorProduct(1 / NormOfVector(u[0]), u[0]));

            for (int i = 1; i < n; i++)
            {

                double[] projAcc = new double[n];
                for (int j = 0; j < projAcc.Length; j++)
                {
                    projAcc[j] = 0;
                }
                for (int j = 0; j < i; j++)
                {
                    double[] proj = VectorProjection(av[i], e[j]);
                    for (int k = 0; k < projAcc.Length; k++)
                    {
                        projAcc[k] += proj[k];
                    }
                }

                double[] ui = new double[n];
                for (int j = 0; j < ui.Length; j++)
                {
                    ui[j] = av[i][j] - projAcc[j];//??a[j][i]
                }

                u.Add(ui);

                e.Add(ScalarToVectorProduct(1 / NormOfVector(u[i]), u[i]));
            }
            
            double[][] q = new double[n][];
            for (int i = 0; i < q.Length; i++)
            {
                q[i] = new double[n];
                for (int j = 0; j < q[i].Length; j++)
                {
                    q[i][j] = e[j][i];
                }
            }


            double[][] r = new double[n][];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new double[n];
                for (int j = 0; j < r[i].Length; j++)
                {
                    if (i >= j)
                    {
                        r[i][j] = ScalarVectorProduct(e[j], av[i]);
                    }
                    else
                    {
                        r[i][j] = 0;
                    }
                }
            }
            //Возможно не нужно транспонировать!!!
            r = Transpose(r);
            q = Transpose(q);

            List<double[][]> res = new List<double[][]>();
            res.Add(q);
            res.Add(r);
            return res;
        }

        double[][] MatricesProduct(double[][] a, double[][] b)
        {
            if (a.Length != b.Length)
                throw new System.ArgumentException("Не совпадают размерности матриц");
            double[][] c = new double[a.Length][]; //Столько же строк, сколько в А; столько столбцов, сколько в B 
            for (int i = 0; i < a.Length; ++i)
            {
                c[i] = new double[b.Length];
                for (int j = 0; j < b.Length; ++j)
                {
                    c[i][j] = 0;
                    for (int k = 0; k < a.Length; ++k)
                    { //ТРЕТИЙ цикл, до A.m=B.n
                        c[i][j] += a[i][k] * b[k][j]; //Собираем сумму произведений
                    }
                }
            }
            return c;
        }

        IList<double[][]> EigenVectorValuesExtractionQRIterative(List<double[]> a, double accuracy, int maxIterations)
        {
            double[][] aItr = a.ToArray();
            double[][] q = null;

            for (int i = 0; i < maxIterations; i++)
            {
                IList<double[][]> qr = QRDecompositionGS(aItr.ToList());
                aItr = MatricesProduct(qr[1], qr[0]);
                if (q == null)
                {
                    q = qr[0];
                }
                else
                {
                    double[][] qNew = MatricesProduct(q, qr[0]);
                    bool accuracyAcheived = true;
                    for (int n = 0; n < q.Length; n++)
                    {
                        for (int m = 0; m < q[n].Length; m++)
                        {
                            if (Math.Abs(Math.Abs(qNew[n][m]) - Math.Abs(q[n][m])) > accuracy)
                            {
                                accuracyAcheived = false;
                                break;
                            }
                        }
                        if (!accuracyAcheived)
                        {
                            break;
                        }
                    }
                    q = qNew;
                    if (accuracyAcheived)
                    {
                        break;
                    }
                }
            }

            List<double[][]> res = new List<double[][]>();
            res.Add(q);
            res.Add(aItr.ToArray());//???
            return res;
        }

        public double[] Transform(double[] dataItem)
        {
            if (_eigenVectors[0].Length != dataItem.Length)
            {
                throw new ArgumentException("_eigenVectors[0].Length != dataItem.Length");
            }
            double[] res = new double[_eigenVectors.Count];
            for (int i = 0; i < _eigenVectors.Count; i++)
            {
                res[i] = 0;
                for (int j = 0; j < dataItem.Length; j++)
                {
                    res[i] += _eigenVectors[i][j] * dataItem[j];
                }
            }
            return res;
        }

        // todo:: Неверно!!!!!!!!!!!!!
        public double[] Reconstruct(double[] transformedDataItem)
        {
            if (_eigenVectors.Count != transformedDataItem.Length)
            {
                throw new ArgumentException("_eigenVectors.Count != transformedDataItem.Length");
            }
            double[] res = new double[_eigenVectors[0].Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = 0;
                for (int j = 0; j < _eigenVectors.Count; j++)
                {
                    res[i] += _eigenVectors[j][i] * transformedDataItem[j];
                }
            }
            return res;
        }

        public void TestCovarianceM()
        {
            List<double[]> dataSet = new List<double[]> {
                new double[] { 1, 5, 3 },
                new double[] { 2, 6, 9 },
                new double[] { 1, 1, 2 },
                new double[] { 4, 5, 6 } };
            double[][] cov = CovarianceMatrixOfData(dataSet);
            string str = "";
        }

        private void TestQRDecompositionGS()
        {
            /*double[][] a = new double[][]
            {
                new double[] {12, -51, 4},
                new double[] {6, 167, -68},
                new double[] {-4, 24, -41}
            };
            List<double[]> c = new List<double[]>
            {
              new double[] {12, -51, 4},
              new double[] {6, 167, -68},
              new double[] {-4, 24, -41}
            };*/

            List<double[]> b = new List<double[]>
            {
              new double[] {20, 3, 4},
              new double[] {3, 2, -5},
              new double[] {4, -5, 9}
            };
            IList<double[][]> res = QRDecompositionGS(b);

            /*
            double[][] expQ = new double[][]
                        {
                new double[] {6.0/7, -69.0/175, -58.0/175},
                new double[] {3.0/7, 158.0/175, 6.0/175},
                new double[] {-2.0/7, 6.0/35, -33.0/35}
                        };
            double[][] expR = new double[][]
                        {
                new double[] {14, 21, -14},
                new double[] {0, 175, -70},
                new double[] {0, 0, 35}
                        };*/
        }

        private void TestEigenVectors()
        {
            List<double[]> a = new List<double[]>
                      {
              new double[] {1, 2, 4},
              new double[] {2, 9, 8},
              new double[] {4, 8, 2}
                      };

            IList<double[][]> ev = EigenVectorValuesExtractionQRIterative(a, 0.001, 10000);
            double expEV00 = 15.2964;
            double expEV11 = 4.3487;
            double expEV22 = 1.0523;

            //  Assert.AreEqual(expEV00, Math.Round(Math.Abs(ev[1][0][0]), 4));
            //  Assert.AreEqual(expEV11, Math.Round(Math.Abs(ev[1][1][1]), 4));
            //  Assert.AreEqual(expEV22, Math.Round(Math.Abs(ev[1][2][2]), 4));
        }

        private void TestMatricesProduct()
        {
            double[][] a = new double[][]
            {
                new double[] {1, -2},
                new double[] {2, 0}
            };
            double[][] b = new double[][]
            {
                new double[] {-3, -2},
                new double[] {2, -4}
            };
            double[][] c = MatricesProduct(a, b);
            double[][] c_res = new double[][]
            {
                new double[] {-7, 6},
                new double[] {-6, -4}
            };
        }

        public void test()
        {
            /*            List<double[]> _data = new List<double[]>()
                        {
                            new double[] {1, 2, 23},
                            new double[] {-3, 17, 5},
                            new double[] {13, -6, 7},
                            new double[] {7, 8, -9}
                        };*/
            List<double[]> _data = new List<double[]>()
            {
                new double[] {1, 5, 3},
                new double[] {2, 6, 9},
                new double[] {1, 1, 2},
                new double[] {4, 5, 6}
            };

            double[] _v1 = new double[] { 1, 2, 1, 4 };
            double[] _v2 = new double[] { 5, 6, 1, 5 };
            double[] _v3 = new double[] { 3, 9, 2, 6 };

            PCA _transformation = new PCA(_data, 0.0001, 1000, 2);

            double[] reduced1 = _transformation.Transform(_v1);
            double[] reconstructed1 = _transformation.Reconstruct(reduced1);

            double[] reduced2 = _transformation.Transform(_v2);
            double[] reconstructed2 = _transformation.Reconstruct(reduced2);

            double[] reduced3 = _transformation.Transform(_v3);
            double[] reconstructed3 = _transformation.Reconstruct(reduced3);
        }
    }
}
