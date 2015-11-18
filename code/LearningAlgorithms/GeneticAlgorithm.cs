using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace LearningAlgorithms
{
    class GeneticAlgorithm : LearningAlgorithm
    {
        double eps = 1E-008, coef_mutation = 1E-004,persent_train = 0.1,selection_pesent = 0.1;
        int count_person = 100,step_train,max_learning_step = 10000;
        int step,train, best_population_num;

        public override string Name
        {
            get { return "Генетический алгоритм"; }
        }
        bool is_rep_2;
        double min_err;
        person[] population;
        struct person
        {
            public INeuroNetLearning pers;
            public long[] population_weight;
            public double err, average_error;
            public ulong live_time;
            public int good_live;
        }
        bool[,] topologi;
        double[][] training_X;
        double[] training_Y;
        Random rand = new Random();
        public int get_step()//сколько шагов потребовалось
        {
            return step;
        }
        public double get_min_err()//с какой точностью завершился
        {
            return population[best_population_num].average_error;
        }
        public int get_good_live()//соклько прожил хорошим
        {
            return population[best_population_num].good_live;
        }
        public void set_max_step(int max_step)//число повторений для обучения
        {
            max_learning_step = max_step;
        }
        public void set_eps(double e_)//точность
        {
            eps = e_;
        }
        public void set_selection_persent(double selec_)//какой процент скрещивать
        {
            selection_pesent = selec_;
        }
        public void set_coef_mut(double mut)//мутация
        {
            coef_mutation = mut;
        }
        public void set_count_popul(int popul)//количество особей
        {
            count_person = popul;
        }
        public void set_persent_train(double persent_train_)//какой процент выборки использовать до скрещивания
        {
            persent_train = persent_train_;
        }
        private double[,] long_to_doubl(int num_person)
        {
            int num_hromosom = 0;
            double[,] res = new double[topologi.GetLength(0), topologi.GetLength(1)];
            for (int i = 0; i < topologi.GetLength(0); i++)
            {
                for (int j = 0; j < topologi.GetLength(1); j++)
                {
                    if (topologi[i, j])
                    {
                        res[i, j] = BitConverter.Int64BitsToDouble(population[num_person].population_weight[num_hromosom]);
                        num_hromosom++;
                    }
                    else
                    {
                        res[i, j] = 0;
                    }
                }
            }
            return res;
        }
        private void sort_person()
        {
            person tmp;
            for (int i = 0; i < population.Length; i++)
            {
                for (int j = i; j < population.Length; j++)
                {
                    if (population[i].average_error - population[j].average_error > 1E-030)
                    {
                        tmp = population[i];
                        population[i] = population[j];
                        population[j] = tmp;
                    }
                }
            }
        }
        System.Windows.Forms.ProgressBar pb;
        public void save_result()
        {
            using (StreamWriter stream = new StreamWriter("data\\matrix.txt"))
            {
                double[,] tmp = long_to_doubl(best_population_num);
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    for (int j = 0; j < tmp.GetLength(1); j++)
                    {                        
                        stream.Write("{0} ", tmp[i, j]);
                    }
                    stream.WriteLine();
                }
            }

        }
        public void genom(INeuroNetLearning solver, double[,] training_set_,bool is_rep_2_)//начало работы генетического алгоритма
        {
            is_rep_2 = is_rep_2_;
            best_population_num = 0;
            training_X = new double[training_set_.GetLength(0)][];
            training_Y = new double[training_set_.GetLength(0)];
            train = Convert.ToInt32(selection_pesent * count_person);
            for (int i = 0; i < training_set_.GetLength(0); i++)
            {
                training_X[i] = new double[training_set_.GetLength(1) - 1];
                training_Y[i] = training_set_[i, training_set_.GetLength(1) - 1];
                for (int j = 0; j < training_set_.GetLength(1) - 1; j++)
                {
                    training_X[i][j] = training_set_[i, j];
                }
            }
            step_train = Convert.ToInt32(training_X.GetLength(0) * persent_train);
            topologi = solver.get_bool_links();//получение топологии
            population = new person[count_person];
            for (int i = 0; i < count_person; i++)
            {
                population[i].pers = solver.copy();//копируем решатели
            }
            int count_hromosom = 0;
            for (int i = 0; i < topologi.GetLength(0); i++)
            {
                for (int j = 0; j < topologi.GetLength(1); j++)
                {
                    if (topologi[i, j])
                    {
                        count_hromosom++;
                    }
                }
            }

            for (int i = 0; i < count_person; i++)
            {
                population[i].population_weight = new long[count_hromosom];
                population[i].err = 0;
                population[i].live_time = 0;
                population[i].good_live = 0;
                for (int j = 0; j < count_hromosom; j++)
                {
//                     long tmp = rand.Next();
//                     tmp <<= 33;
//                     tmp |= rand.Next();
                    population[i].population_weight[j] = BitConverter.DoubleToInt64Bits(2*rand.NextDouble() - 1);
                    while (System.Double.IsNaN(BitConverter.Int64BitsToDouble(population[i].population_weight[j])))
                    {
                        long mut = rand.Next();
                        mut <<= 33;
                        mut |= rand.Next();
                        population[i].population_weight[j] ^= mut;
                    }
                }
            }
            for (int i = 0; i < count_person; i++)
            {
                population[i].pers.set_links(long_to_doubl(i));//задаём новые веса, полная матрица весов
            }
            start_genom();
            solver.set_links(long_to_doubl(best_population_num));
        }
        delegate void MyDel(Stopwatch st, int step);
        MyDel del;
        Label label2;
        TimeSpan ts;
        private void start_genom()
        {

            step = 0;             
            Progres pg = new Progres();
            
            Thread t1 = new Thread(new ThreadStart(delegate
                {                    
                    pb = pg.get_pg();
                    pb.Maximum = max_learning_step;
                    del = new MyDel(pg.start);
                    label2 = pg.get_lb();
                    pg.ShowDialog();
                    
                }));
            Thread t = new Thread(new ThreadStart(delegate
            {
                Stopwatch stopWatch = new Stopwatch();
                

                
                do
                {   stopWatch.Start();
                    for (int i = 0; i < training_Y.Length / step_train; i++)
                    {
                        for (int j = i * step_train; j < step_train * (i + 1); j++)
                        {
                            for (int k = 0; k < count_person; k++)
                            {
                                population[k].err += Math.Pow(training_Y[j] - population[k].pers.get_res(training_X[j]), 2);
                            }
                        }
                        selection(step_train);
                    }
                    if (step_train * (Convert.ToInt32(training_Y.Length / step_train )) < training_Y.Length)
                    {
                        for (int i = step_train * (Convert.ToInt32(training_Y.Length / step_train)); i < training_Y.Length; i++)
                        {
                            for (int k = 0; k < count_person; k++)
                            {
                                population[k].err += Math.Pow(training_Y[i] - population[k].pers.get_res(training_X[i]), 2);
                            }
                        }
                        selection(training_Y.Length - Convert.ToInt32(persent_train * 100 * step_train));
                    }
                    step++;
                    stopWatch.Stop();
                    //del.Invoke(stopWatch, max_learning_step - step);
                    ts = new TimeSpan(stopWatch.Elapsed.Ticks * (max_learning_step - step));
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                           ts.Hours, ts.Minutes, ts.Seconds);
                    if (label2.InvokeRequired)
                    {
                        pb.BeginInvoke(new MethodInvoker(delegate
                        {
                            label2.Text = elapsedTime.ToString();
                        }));
                    }
                    else
                    {
                        label2.Text = elapsedTime.ToString();
                    }
                    stopWatch.Reset();
                    if (!pg.Visible)
                    {
                        break;
                    }

                    if (pb.InvokeRequired)
                        pb.BeginInvoke(new MethodInvoker(delegate
                        {
                            pb.Value = step;
                        }));
                   else
                        pb.Value = step;

                   // pb.Value++;
                } while ((step < max_learning_step) && (is_not_stop()));
                if (pb.InvokeRequired)
                    pb.BeginInvoke(new MethodInvoker(delegate
                    {
                        pb.Dispose();
                    }));
                else
                    pb.Dispose();
            }));
            t1.Start();
            t.Start();
            t.Join();
            
            if (pg.InvokeRequired)
                pg.BeginInvoke(new MethodInvoker(delegate
                {
                    pg.Close();
                }));
            else
                pg.Close();
            t1.Join();
           
            

        }
       
        private bool is_not_stop()
        {
            int i = 0;
            while ((i < count_person))
            {
                if (population[i].average_error < eps)
                {

                    if (population[i].live_time >= Convert.ToUInt64(training_Y.Length * 2))
                    {
                        best_population_num = i;
                        return false;
                    }
                    i++;
                }
                else
                    break;
            }
            return true;
        }
        private void selection(int live_time)
        {
            for (int i = 0; i < population.Length; i++)
            {
                population[i].live_time += Convert.ToUInt64(live_time);
                population[i].average_error = population[i].err / population[i].live_time;
            }
            //Array.Sort(population, new Comparison<person>((a, b) => a.average_error.CompareTo(b.average_error)));
            sort_person();
            if (!is_rep_2)
            { 
                Reproduction();
            } 
            else
            {
                Reproduction_2();
            }
           
            for (int i = 0; i < train; i++)
            {
                population[i].good_live++;

            }
            for (int i = train; i < population.Length - train - 1; i++)
            {
                population[i].good_live = 0;
            }
            for (int i = population.Length - train - 1; i < population.Length; i++)
            {
                population[i].good_live = 0;
                population[i].live_time = 0;
                population[i].err = 0;
            }
        }
        private void Reproduction()
        {
            long tmp, tmp_;
            for (int i = 0; i < train; i += 2)
            {
                for (int j = 0; j < population[i].population_weight.Length; j++)
                {
                    tmp = rand.Next();
                    tmp <<= 33;
                    tmp |= rand.Next();
                    tmp_ = tmp;
                    tmp = population[i].population_weight[j] & tmp_;
                    tmp |= (population[i + 1].population_weight[j] & (~tmp_));
                    population[population.Length - 1 - i].population_weight[j] = tmp;
                    if (rand.NextDouble() < coef_mutation)
                    {
                        long mut = rand.Next();
                        mut <<= 33;
                        mut |= rand.Next();
                        population[population.Length - 1 - i].population_weight[j] ^= mut;
                    }
                    tmp = population[i].population_weight[j] & (~tmp_);
                    tmp |= (population[i + 1].population_weight[j] & tmp_);
                    population[population.Length - 2 - i].population_weight[j] = tmp;
                    if (rand.NextDouble() < coef_mutation)
                    {
                        long mut = rand.Next();
                        mut <<= 33;
                        mut |= rand.Next();
                        population[population.Length - 2 - i].population_weight[j] ^= mut;
                    }
                    while (System.Double.IsNaN(BitConverter.Int64BitsToDouble(population[population.Length - 1 - i].population_weight[j])))
                    {
                        long mut = rand.Next();
                        mut <<= 33;
                        mut |= rand.Next();
                        population[population.Length - 1 - i].population_weight[j] ^= mut;
                    }
                    while (System.Double.IsNaN(BitConverter.Int64BitsToDouble(population[population.Length - 2 - i].population_weight[j])))
                    {
                        long mut = rand.Next();
                        mut <<= 33;
                        mut |= rand.Next();
                        population[population.Length - 2 - i].population_weight[j] ^= mut;
                    }
                }
                population[population.Length - 1 - i].pers.set_links(long_to_doubl(population.Length - 1 - i));
                population[population.Length - 2 - i].pers.set_links(long_to_doubl(population.Length - 2 - i));
            }
        }
        private void Reproduction_2()
        {
            double tmp, tmp_,r;
            for (int i = 0; i < train; i += 2)
            {
                for (int j = 0; j < population[i].population_weight.Length; j++)
                {
                    r = (2 * rand.NextDouble() - 1);
                    tmp = BitConverter.Int64BitsToDouble(population[i].population_weight[j]);
                    tmp_ = BitConverter.Int64BitsToDouble(population[i + 1].population_weight[j]); 
                    population[population.Length - 1 - i].population_weight[j] =BitConverter.DoubleToInt64Bits(tmp
                        + r*(tmp - tmp_));
                    if (rand.NextDouble() < coef_mutation)
                    {
                        population[population.Length - 1 - i].population_weight[j] = BitConverter.DoubleToInt64Bits(2 * rand.NextDouble() - 1);
                    }
                    population[population.Length - 2 - i].population_weight[j] = BitConverter.DoubleToInt64Bits(tmp_
                        + r * (tmp_ - tmp)); 
                    if (rand.NextDouble() < coef_mutation)
                    {
                        population[population.Length - 2 - i].population_weight[j] = BitConverter.DoubleToInt64Bits(2 * rand.NextDouble() - 1);
                    }                   
                }
                population[population.Length - 1 - i].pers.set_links(long_to_doubl(population.Length - 1 - i));
                population[population.Length - 2 - i].pers.set_links(long_to_doubl(population.Length - 2 - i));
            }
        }
    }
}
