using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.view_models;
using dms.services.preprocessing.normalization;

namespace dms.services.preprocessing
{
    class PreprocessingManager
    {
        private static PreprocessingManager prepManager;
        public static PreprocessingManager PrepManager
        {
            get
            {
                if (prepManager == null)
                {
                    prepManager = new PreprocessingManager();

                }
                return prepManager;
            }
        }

        public PreprocessingManager()
        {
        }
  
        public void updateTask(int taskId, int parameterCount, int selectionCount)
        {
            DataHelper helper = new DataHelper();
            helper.updateTask(taskId, parameterCount, selectionCount);
        }

        public string[] getParameterTypes(string filePath, char delimiter)
        {
            return Parser.SelectionParser.getParameterTypes(filePath, delimiter);
        }

        public int parseSelection(string taskTemplateName, string filePath, char delimiter, int taskId, string selectionName, ParameterCreationViewModel[] parameters)
        {
            return Parser.SelectionParser.parse(taskTemplateName, filePath, delimiter, taskId, selectionName, parameters);
        }

        public int getCountRows()
        {
            return Parser.SelectionParser.CountRows;
        }

        public int getCountParameters()
        {
            return Parser.SelectionParser.CountParameters;
        }

        /* --------------------------------------- */

        public void executePreprocessing(int selectionId, int paramId, string prepType)
        {
            dms.models.Parameter param = ((dms.models.Parameter)dms.services.DatabaseManager.SharedManager.entityById(paramId, typeof(dms.models.Parameter)));
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));

            List<string> values = new List<string>();
            List<Entity> valueParam = new List<Entity>();
            int index = 0;
            foreach (Entity entity in selectionRows)
            {
                int selectionRowId = entity.ID;
                List<Entity> list = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                .addCondition("ParameterID", "=", paramId.ToString()).
                addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));
                if (index == 0)
                {
                    valueParam = list;
                }
                else
                {
                    valueParam = valueParam.Concat(list).ToList();
                }
                values.Add(((ValueParameter)valueParam[index]).Value);
                index++;
            }
            IParameter p;

            switch (prepType)
            {
                case "нормализация":
                   /* if (paramId == 1)
                    {
                        p = new IntegerParameter(values);
                        normalizeValues(valueParam, p);
                    } else if (paramId == 5)
                    {
                        p = new EnumeratedParameter(values);
                        normalizeValues(valueParam, p);
                    } else
                    {
                        p = new RealParameter(values);
                        normalizeValues(valueParam, p);
                    }*/
                      if (param.Type == TypeParameter.Real)
                      {
                          p = new RealParameter(values);
                          normalizeValues(valueParam, p);
                      } else if (param.Type == TypeParameter.Int)
                      {
                          p = new IntegerParameter(values);
                          normalizeValues(valueParam, p);
                      } else if (param.Type == TypeParameter.Enum)
                      {
                          p = new EnumeratedParameter(values);
                          normalizeValues(valueParam, p);
                      }
                    break;
                case "бинаризация":

                    break;
                case "без предобработки":
                    break;
            }
        }

        private void normalizeValues(List<Entity> values, IParameter p)
        {
            List<Entity> listValues = new List<Entity>();
            foreach (Entity value in values)
            {
                Entity val = value;
                ((ValueParameter)val).Value = p.GetNormalizedDouble(((ValueParameter)value).Value).ToString();
                listValues.Add(val);
            }
            //добавить создание нового шаблона -> новой выборки -> строк выборки -> параметров - > значений параметров
           // DatabaseManager.SharedManager.insertMultipleEntities(listValues);
        }
    }
}
