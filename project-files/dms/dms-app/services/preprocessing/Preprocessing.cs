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
    class Preprocessing
    {
        private static Preprocessing preprocessing;
        public static Preprocessing PreprocessingObj
        {
            get
            {
                if (preprocessing == null)
                {
                    preprocessing = new Preprocessing();

                }
                return preprocessing;
            }
        }

        public Preprocessing()
        { }

        public int addNewEntitiesForPreprocessing(string selectionName, int countRows, int taskTemplateId)
        {
            DataHelper helper = new DataHelper();

            string type = "develop";
            int selectionId = helper.addSelection(selectionName, taskTemplateId, countRows, type);

            List<Entity> listSelRow = new List<Entity>(countRows);
            for (int i = 0; i < countRows; i++)
            {
                SelectionRow entity = helper.addSelectionRow(selectionId, i + 1);
                listSelRow.Add(entity);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(listSelRow);

            return selectionId;
        }
        
        private List<int> pars = new List<int>();
        public void executePreprocessing(int taskTemplateId, int newSelectionId, int selectionId, 
            int paramId, string prepType, int paramCount, int parameterPosition, bool canAdd)
        {
            dms.models.Parameter param = ((dms.models.Parameter)dms.services.DatabaseManager.SharedManager
                .entityById(paramId, typeof(dms.models.Parameter)));
            List<Entity> oldSelectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));
            TypeParameter type;
            bool flag = false;
            switch (prepType)
            {
                case "Линейная нормализация 1 (к float)":
                    type = TypeParameter.Real;
                    break;
                case "Нелинейная нормализация 2 (к float)":
                    type = TypeParameter.Real;
                    break;
                case "нормализация 3 (к int)":
                    type = TypeParameter.Int;
                    break;
                case "бинаризация":
                    flag = true;
                    type = TypeParameter.Int;// Enum;
                    break;
                case "без предобработки":
                    type = param.Type;
                    break;
                default:
                    type = TypeParameter.Real;
                    break;
            }
            int newParamId;
            if (canAdd)
            {
                if (flag)
                {
                    newParamId = new DataHelper().addOneParameter(param.Name, param.Comment,
               taskTemplateId, param.Index, param.IsOutput + 1, type);
                }
               else
                {
                    newParamId = new DataHelper().addOneParameter(param.Name, param.Comment,
               taskTemplateId, param.Index, param.IsOutput, type);
                }
                pars.Add(newParamId);
            }
            else
            {

                newParamId = pars[parameterPosition - 1];

            }
            
            
            List<string> values = new List<string>();
            List<Entity> valueParam = new List<Entity>();
            int index = 0;
            foreach (Entity entity in oldSelectionRows)
            {
                int selectionRowId = entity.ID;
                List<Entity> list = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                .addCondition("ParameterID", "=", paramId.ToString()).
                addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));
                valueParam = valueParam.Concat(list).ToList();
                values.Add(((ValueParameter)valueParam[index]).Value);
                index++;
            }

            IParameter p;
            switch (prepType)
            {
                case "Линейная нормализация 1 (к float)":
                case "Нелинейная нормализация 2 (к float)":
                case "нормализация 3 (к int)":
                    if (param.Type == TypeParameter.Real)
                    {
                        p = new RealParameter(values);
                        normalizeValues(valueParam, p, newParamId, newSelectionId, prepType);
                    }
                    else if (param.Type == TypeParameter.Int)
                    {
                        p = new IntegerParameter(values);
                        normalizeValues(valueParam, p, newParamId, newSelectionId, prepType);
                    }
                    else if (param.Type == TypeParameter.Enum)
                    {
                        p = new EnumeratedParameter(values);
                        normalizeValues(valueParam, p, newParamId, newSelectionId, prepType);
                    }
                    break;
                case "бинаризация":
                    binarizationValues(valueParam, newParamId, newSelectionId, paramCount, parameterPosition);
                    break;
                case "без предобработки":
                    processWithoutPreprocessing(valueParam, newParamId, newSelectionId);
                    break;
            }
        }
        
        private void processWithoutPreprocessing(List<Entity> values, int paramId, int newSelectionId)
        {
            DataHelper helper = new DataHelper();
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", newSelectionId.ToString()), typeof(SelectionRow));

            int index = 0;
            List<Entity> listValues = new List<Entity>();
            foreach (Entity value in values)
            {
                string val = withoutPreprocessing(value);
                listValues.Add(helper.addValueParameter(selectionRows[index].ID, paramId, val));
                index++;
            }
            DatabaseManager.SharedManager.insertMultipleEntities(listValues);
        }

        private void binarizationValues(List<Entity> values, int paramId, int newSelectionId, int paramCount, int parameterPosition)
        {
            DataHelper helper = new DataHelper();
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", newSelectionId.ToString()), typeof(SelectionRow));

            int index = 0;
            List<Entity> listValues = new List<Entity>();

            List<string> valueStr = new List<string>();
            foreach (Entity value in values)
            {
                valueStr.Add(((ValueParameter)value).Value);
            }
            EnumeratedParameter p = new EnumeratedParameter(valueStr);
            
            foreach (Entity value in values)
            {
                int i = p.GetInt(((ValueParameter)value).Value);
                string val = binarization(value, i, parameterPosition);
                listValues.Add(helper.addValueParameter(selectionRows[index].ID, paramId, val));
                index++;
            }
            DatabaseManager.SharedManager.insertMultipleEntities(listValues);
        }

        private void normalizeValues(List<Entity> values, IParameter p, int paramId, int newSelectionId, string prepType)
        {
            List<Entity> listValues = new List<Entity>();
            DataHelper helper = new DataHelper();
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", newSelectionId.ToString()), typeof(SelectionRow));

            int index = 0;
            foreach (Entity value in values)
            {
                string val;
                switch (prepType)
                {
                    case "Линейная нормализация 1 (к float)":
                        val = normalize(1, value, p);
                        break;
                    case "Нелинейная нормализация 2 (к float)":
                        val = normalize(2, value, p);
                        break;
                    case "нормализация 3 (к int)":
                        val = normalize(3, value, p);
                        break;
                    default:
                        val = "";
                        break;
                }
                listValues.Add(helper.addValueParameter(selectionRows[index].ID, paramId, val));
                index++;
            }
            DatabaseManager.SharedManager.insertMultipleEntities(listValues);
        }

        private string normalize(int type, Entity value, IParameter p)
        {
            if (type == 1)
            {
                return p.GetLinearNormalizedFloat(((ValueParameter)value).Value).ToString();
            } else if (type == 2)
            {
                return p.GetNonlinearNormalizedFloat(((ValueParameter)value).Value).ToString();
            } else
            {
                return p.GetNormalizedInt(((ValueParameter)value).Value).ToString();
            }
        }

        private string binarization(Entity value, int index, int parameterPosition)
        {
          //  string vec = "";
            string val = ((ValueParameter)value).Value; 
            if (index.Equals(parameterPosition))
            {
                return "1";
            }else
            {
                return "0";
            }
            
       /*     for (int i = 1; i <= parameterCount; i++)
            {
                if (i == parameterPosition)
                {
                    if (parameterPosition == 1)
                    {
                        vec = "1";// val;
                        continue;
                    }
                    vec = vec + "," + "1";// val;
                    continue;
                }
                if (i == 1)
                {
                    vec = "0";
                    continue;
                }
                vec = vec + ", 0";
            }
            return vec;
*/        }

        private string withoutPreprocessing(Entity value)
        {
            return ((ValueParameter)value).Value;
        }
    }
}
