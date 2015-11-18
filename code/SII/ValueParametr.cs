using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SII
{
    /*
     * Struct of parse file:
     * {x1;x2;x3;x4;...xn;~y0}, type of xi - i-parametr
     */
    public class ValueParametr
    {
        public int ID;
        public int ParametrID;
        public int SelectionID;
        public String Value;
        public int RowIndex;


        public ValueParametr(int idParametr, int idSelection, String value, int rowIndex)
        {
            ParametrID = idParametr;
            SelectionID = idSelection;
            Value = value;
            RowIndex = rowIndex;
        }

        public ValueParametr()
        {

        }

        static public List<ValueParametr> GetArrValuesFromFile(String namefile, List<Parametr> arrParams, int idSelection, bool withResult)
        {
            //withResult - файл содержит выборки с выходным параметором или нет
            List<ValueParametr> arr = null;

            try
            {
                arr = new List<ValueParametr>();
                int curCount = 0;
                using (StreamReader sr = new StreamReader(namefile))
                {
                    String line = sr.ReadLine();
                    while (line != null)
                    {
                        ValueParametr curValueParam;
                        while (line.Trim().Length > 0)
                        {
                            curCount++;

                            //int start = line.IndexOf('{');
                            //int end = line.IndexOf('}');

                            //string s = line.Substring(start + 1, end - (start + 1)).Trim();
                            if (curCount != 1)
                            {
                                string[] values = line.Split('\t');
                                values = values.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                                foreach (Parametr parametr in arrParams)
                                {
                                    if (parametr.Number != 0)
                                    {
                                        //то есть не выходной параметр
                                        String curValue = values[parametr.Number];
                                        curValueParam = new ValueParametr(parametr.ID, idSelection, curValue, curCount);
                                        arr.Add(curValueParam);
                                    }
                                    else if (withResult)
                                    {
                                        //выходной параметр
                                        String curValue = values[values.Length - 1];
                                        curValueParam = new ValueParametr(parametr.ID, idSelection, curValue, curCount);
                                        arr.Add(curValueParam);
                                    }
                                }
                            }                            
                            line = sr.ReadLine();
                        }
                    
                        //line = line.Remove(start, (end + 1) - start);
                        //curValue = new ValueParametr
                    }
                    Console.WriteLine(line);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return arr;
        }
    }
}
