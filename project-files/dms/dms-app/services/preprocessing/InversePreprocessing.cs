using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.services.preprocessing.normalization;
using dms.view_models;

namespace dms.services.preprocessing
{
    class InversePreprocessing
    {
        private static InversePreprocessing inversePreprocessing;
        public static InversePreprocessing InversePreprocessingObj
        {
            get
            {
                if (inversePreprocessing == null)
                {
                    inversePreprocessing = new InversePreprocessing();

                }
                return inversePreprocessing;
            }
        }

        public InversePreprocessing()
        { }

        public List<string> getAppropriateValues(List<string> obtainedValues, int selectionId, int parameterId)
        {
            PreprocessingViewModel.PreprocessingTemplate prepParameters = getPreprocessingParameters(selectionId);
            List<PreprocessingViewModel.SerializableList> inform = prepParameters.info;
            List<Entity> valuesForParameter = null;
            foreach(PreprocessingViewModel.SerializableList sel in inform)
            {
                if (selectionId.Equals(sel.selectionId))
                {
                    List<PreprocessingViewModel.ValuesForParameter> list = sel.parametersValues;
                    foreach(PreprocessingViewModel.ValuesForParameter elem in list)
                    {
                        if (parameterId.Equals(elem.parameterId))
                        {
                            valuesForParameter = elem.values;
                            break;
                        }
                    }
                    break;
                }
            }
            //       float valueDec = Convert.ToSingle(value.Replace(".", ","));
            //Формируем выборку для заданного параметра
            List<float> valuesForCurrParameter = new List<float>();
            foreach (Entity value in valuesForParameter)
            {
                string numberStr = ((ValueParameter)value).Value;
                float number = Convert.ToSingle(numberStr.Replace(".", ","));
                valuesForCurrParameter.Add(number);
            }
            /*List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));

            List<float> valuesForCurrParameter = new List<float>();
            foreach (Entity selRow in selectionRows)
            {
                int selectionRowId = selRow.ID;
                List<Entity> valueForParamFromRow = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                        .addCondition("ParameterID", "=", parameterId.ToString())
                        .addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));

                string numberStr = ((ValueParameter)valueForParamFromRow[0]).Value;
                float number = Convert.ToSingle(numberStr.Replace(".", ","));
                valuesForCurrParameter.Add(number);
            }*/
            valuesForCurrParameter.Sort();
            //находим в выборке соответсвующее значение для value (переданного аргумента) и присваиваем его appropriateValue
            float step = 0;
            List<string> appropriateValues = new List<string>();

            for (int j = 0; j < obtainedValues.Count; j++)
            {
                float obtainedValue = Convert.ToSingle(obtainedValues[j].Replace(".", ","));

                /*float prev = valuesForCurrParameter[0];
                for (int i = 1; i < valuesForCurrParameter.Count; i++)
                {
                    float next = valuesForCurrParameter[i];
                    step = Math.Abs(next - prev);
                    if ((obtainedValue - prev) <= (step / 2))
                    {
                        appropriateValues.Add(prev.ToString());
                        break;
                    }
                    prev = next;
                }*/
                float min = Math.Abs( valuesForCurrParameter[0] - obtainedValue);
                float tmpMin;
                int obtainedValueID = 0;
                for (int i = 1; i < valuesForCurrParameter.Count; i++)
                {
                    tmpMin = Math.Abs(valuesForCurrParameter[i] - obtainedValue);
                    if (tmpMin < min)
                    {
                        min = tmpMin;
                        obtainedValueID = i;
                    }
                }
                appropriateValues.Add(valuesForCurrParameter[obtainedValueID].ToString());
                //проверка на выод за границу диапозона значений в выборке ???
                if (appropriateValues.Count <= j)
                {
                    float firstVal = valuesForCurrParameter[0];
                    float lastVal = valuesForCurrParameter[valuesForCurrParameter.Count - 1];
                    if (obtainedValue >= lastVal)
                    {
                        appropriateValues.Add(lastVal.ToString());
                    }
                    else if (obtainedValue <= firstVal)
                    {
                        appropriateValues.Add(firstVal.ToString());
                    }
                }
            }
            return appropriateValues;
        }

        public PreprocessingViewModel.PreprocessingTemplate getPreprocessingParameters(int selectionId)
        {
            //оперделяем шаблон для выборки и достаем из него PreprocessingParameters 
            Selection selection = ((Selection)services.DatabaseManager.SharedManager.entityById(selectionId, typeof(Selection)));
            int templateId = selection.TaskTemplateID;
            TaskTemplate template = ((TaskTemplate)services.DatabaseManager.SharedManager.entityById(templateId, typeof(TaskTemplate)));
            PreprocessingViewModel.PreprocessingTemplate prepParameters = (PreprocessingViewModel.PreprocessingTemplate)template.PreprocessingParameters;
            return prepParameters;
        }

        public List<bool> getComparisonResults(int selectionId, int parameterId, List<string> appropriateValues, List<string> expectedValues)
        {
            PreprocessingViewModel.PreprocessingTemplate prepParameters = getPreprocessingParameters(selectionId);
            List<PreprocessingViewModel.SerializableList> info = prepParameters.info;
            List<view_models.Parameter> parametersWithPrepType = prepParameters.parameters;
            //находим нужный preprocessing list и нужное преобразование
            foreach (PreprocessingViewModel.SerializableList elem in info)
            {
                if (selectionId.Equals(elem.selectionId))
                {
                    List<int> parameterIdList = elem.parameterIds;
                    int index = 0;
                    foreach (int paramId in parameterIdList)
                    {
                        if (parameterId.Equals(paramId))
                        {
                            IParameter p = elem.prepParameters[index];
                            foreach (view_models.Parameter prepParam in parametersWithPrepType)
                            {
                                if (parameterId.Equals(prepParam.Id))
                                {
                                    string prepType = prepParam.Type;
                                    switch (prepType)
                                    {
                                        case "Линейная нормализация 1 (к float)":
                                            return getValuesFromLinearNormalized(appropriateValues, expectedValues, p);
                                        case "Нелинейная нормализация 2 (к float)":
                                            return getValuesFromNonlinearNormalized(appropriateValues, expectedValues, p);
                                        case "нормализация 3 (к int)":
                                            return getValuesFromNormalized(appropriateValues, expectedValues, p);
                                        case "бинаризация":
                                            //добавить бинаризацию!!!!
                                            break;
                                        case "без предобработки":
                                            return getValuesWithoutPreprocessing(appropriateValues, expectedValues);
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                        index++;
                    }
                    break;
                }
            }
            return null;
        }

        public List<bool> getValuesFromLinearNormalized(List<string> appropriateValues, List<string> expectedValues, IParameter p)
        {
            List<bool> results = new List<bool>();
            for (int i = 0; i < appropriateValues.Count; i++)
            {
                string apVal = p.GetFromLinearNormalized(Convert.ToSingle(appropriateValues[i].Replace(".", ",")));
                string exVal = p.GetFromLinearNormalized(Convert.ToSingle(expectedValues[i].Replace(".", ",")));

                if (!exVal.Equals("") && !apVal.Equals("") && exVal.Equals(apVal))
                {
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            return results;
        }

        public List<bool> getValuesFromNonlinearNormalized(List<string> appropriateValues, List<string> expectedValues, IParameter p)
        {
            List<bool> results = new List<bool>();
            for (int i = 0; i < appropriateValues.Count; i++)
            {
                string apVal = p.GetFromNonlinearNormalized(Convert.ToSingle(appropriateValues[i].Replace(".", ",")));
                string exVal = p.GetFromNonlinearNormalized(Convert.ToSingle(expectedValues[i].Replace(".", ",")));

                if (!exVal.Equals("") && !apVal.Equals("") && exVal.Equals(apVal))
                {
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            return results;
        }

        public List<bool> getValuesFromNormalized(List<string> appropriateValues, List<string> expectedValues, IParameter p)
        {
            List<bool> results = new List<bool>();
            for (int i = 0; i < appropriateValues.Count; i++)
            {
                string apVal = p.GetFromNormalized(Convert.ToInt32(appropriateValues[i]));
                string exVal = p.GetFromNormalized(Convert.ToInt32(expectedValues[i]));

                if (!exVal.Equals("") && !apVal.Equals("") && exVal.Equals(apVal))
                {
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            return results;
        }

        public List<bool> getValuesWithoutPreprocessing(List<string> appropriateValues, List<string> expectedValues)
        {
            List<bool> results = new List<bool>();
            for (int i = 0; i < appropriateValues.Count; i++)
            {
                string apVal = appropriateValues[i];
                string exVal = expectedValues[i];

                if (!exVal.Equals("") && !apVal.Equals("") && exVal.Equals(apVal))
                {
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            return results;
        }

        public List<string> getAppropriateValuesAfterInversePreprocessing(int selectionId, int parameterId, List<string> appropriateValues)
        {
            PreprocessingViewModel.PreprocessingTemplate prepParameters = getPreprocessingParameters(selectionId);
            List<PreprocessingViewModel.SerializableList> info = prepParameters.info;
            List<view_models.Parameter> parametersWithPrepType = prepParameters.parameters;
            //находим нужный preprocessing list и нужное преобразование
            foreach (PreprocessingViewModel.SerializableList elem in info)
            {
                if (selectionId.Equals(elem.selectionId))
                {
                    List<int> parameterIdList = elem.parameterIds;
                    int index = 0;
                    foreach (int paramId in parameterIdList)
                    {
                        if (parameterId.Equals(paramId))
                        {
                            IParameter p = elem.prepParameters[index];
                            foreach (view_models.Parameter prepParam in parametersWithPrepType)
                            {
                                if (parameterId.Equals(prepParam.Id))
                                {
                                    string prepType = prepParam.Type;
                                    List<string> results = new List<string>();
                                    switch (prepType)
                                    {
                                        case "Линейная нормализация 1 (к float)":
                                            for (int i = 0; i < appropriateValues.Count; i++)
                                            {
                                                string apVal = p.GetFromLinearNormalized(Convert.ToSingle(appropriateValues[i].Replace(".", ",")));
                                                results.Add(apVal);
                                            }
                                            return results;
                                        case "Нелинейная нормализация 2 (к float)":
                                            for (int i = 0; i < appropriateValues.Count; i++)
                                            {
                                                string apVal = p.GetFromNonlinearNormalized(Convert.ToSingle(appropriateValues[i].Replace(".", ",")));
                                                results.Add(apVal);
                                            }
                                            return results;
                                        case "нормализация 3 (к int)":
                                            for (int i = 0; i < appropriateValues.Count; i++)
                                            {
                                                string apVal = p.GetFromNormalized(Convert.ToInt32(appropriateValues[i]));
                                                results.Add(apVal);
                                            }
                                            return results;
                                        case "бинаризация":
                                            //добавить бинаризацию!!!!
                                            break;
                                        case "без предобработки":
                                            return appropriateValues;
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                        index++;
                    }
                    break;
                }
            }
            return null;
        }
    }
}
