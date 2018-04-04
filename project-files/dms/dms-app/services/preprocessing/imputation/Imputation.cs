using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.services;

namespace dms.services.preprocessing.imputation
{
    static class Imputation
    {
        public enum MethodsName { DeleteRows, ValueReplace, HotDeck };
        public static string reformedValue = "0.0";

        // dms.services.preprocessing.imputation.Imputation.startMethod(41, 43);// taskTemplateId, selectionId);
        public static void startMethod(int taskTemplateId, int selectionId, MethodsName type)
        {
            List<Entity> incorrectValues = getValuesForCheck(taskTemplateId, selectionId);
            switch (type)
            {
                case MethodsName.HotDeck:
                    executeHotDeck(taskTemplateId, selectionId, incorrectValues);
                    break;
                case MethodsName.DeleteRows:
                    deleteRows(taskTemplateId, selectionId, incorrectValues);
                    break;
                case MethodsName.ValueReplace:
                    updateValues(incorrectValues);
                    break;
            }
        }

        public static bool isWrongValue(string value)
        {
            if (value.Contains("NA ") && value.StartsWith("NA ") || value.Contains("?"))
            {
                return true;
            }

            return false;
        }

        public static List<Entity> getValuesForCheck(int taskTemplateId, int selectionId)
        {
            List<Entity> values = new List<Entity>();
            List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(models.Parameter));

            foreach (Entity parameter in parameters)
            {
                List<Entity> valueParameters = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                .addCondition("ParameterID", "=", parameter.ID.ToString())
                .addCondition("Value", "=", reformedValue), typeof(ValueParameter));

                values.AddRange(valueParameters);
            }
            return values;
        }

        private static void executeHotDeck(int taskTemplateId, int selectionId, List<Entity> emptyValues)
        {
            List<List<Entity>> matrix = new List<List<Entity>>();

            List<Entity> parameters = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(models.Parameter));
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));
          
            foreach (Entity parameter in parameters)
            {
                List<Entity> valueParam = Selection.getValueParametersBySelIdAndParamId(selectionId, parameter.ID, reformedValue).ToList();
                matrix.Add(valueParam);
            }

            int n = parameters.Count - 1; // input count
            int m = 1; // output count
            int i, j;
            Dictionary<int, double> d = new Dictionary<int, double>();
            Dictionary<int, double> d_end = new Dictionary<int, double>();
            int kSize = 4;
            int p = matrix[0].Count;
            for (int index = 0; index < emptyValues.Count; index++)
            {
                ValueParameter v = (ValueParameter)emptyValues[index];
                i = v.SelectionRowID;
                List<Entity> rows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("ID", "=", i.ToString()), typeof(SelectionRow));
                i = ((SelectionRow)rows.First()).Number - 1;

                j = v.ParameterID;
                List<Entity> params_ = models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("ID", "=", j.ToString()), typeof(Parameter));
                j = ((models.Parameter)params_.First()).Index;

                for (int k = 0; k < p; k++)
                {
                    if (k != i)
                    {
                        double d_k_i = 0;
                        for (int l = 0; l < n + m; l++)
                        {
                            if (l != j)
                            {
                                double value = Convert.ToDouble(((ValueParameter)matrix[l][k]).Value.Replace(".", ",")) - Convert.ToDouble(((ValueParameter)matrix[l][i]).Value.Replace(".", ","));
                                d_k_i += Math.Pow(value, 2);
                            }
                        }
                        d_k_i = Math.Sqrt(d_k_i);
                        d.Add(k, d_k_i);
                    }
                }
                //Sort, Delete
                d = d.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                for(int h=0; h < kSize; h++)
                {
                    d_end.Add(d.ElementAt(h).Key, d.ElementAt(h).Value);
                }

                double a_mult_c_l = 0;
                double c_l_sum = 0;
                foreach(var item in d_end)
                {
                    int l = item.Key;
                    double c_l = 1 / (1 + item.Value);
                    a_mult_c_l += Convert.ToDouble(((ValueParameter)matrix[j][l]).Value.Replace(".", ",")) * c_l;
                    c_l_sum += c_l;
                }
                double res = a_mult_c_l / c_l_sum;
                new DataHelper().updateValueParameter(v.ID, res.ToString());
            }
        }

        public static void updateValues(List<Entity> incorrectValues)
        {
            DataHelper helper = new DataHelper();
            foreach (var item in incorrectValues)
            {
                helper.updateValueParameter(item.ID, reformedValue);
            }
        }

        public static void deleteRows(int taskTemplateId, int selectionId, List<Entity> incorrectValues)
        {
            List<Entity> rows = new List<Entity>();
            DataHelper helper = new DataHelper();

            foreach (ValueParameter item in incorrectValues)
            {
                List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("ID", "=", item.SelectionRowID.ToString()), typeof(SelectionRow));
                rows.AddRange(selectionRows);
            }

            helper.deleteSelectionRowsWithValues(rows); ;
        }
    }
}
