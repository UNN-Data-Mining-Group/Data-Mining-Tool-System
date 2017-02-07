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

        public void executePreprocessing(int taskTemplateId, int newSelectionId, int selectionId, int paramId, string prepType)
        {
            dms.models.Parameter param = ((dms.models.Parameter)dms.services.DatabaseManager.SharedManager
                .entityById(paramId, typeof(dms.models.Parameter)));
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));
            
            dms.models.Parameter newParam = new DataHelper().addParameter(param.Name, param.Comment, 
                taskTemplateId, param.Index, param.IsOutput, TypeParameter.Real);
            newParam.save();
            
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
                    if (param.Type == TypeParameter.Real)
                    {
                        p = new RealParameter(values);
                        preprocessValues(valueParam, p, newParam.ID, newSelectionId, prepType);
                    }
                    else if (param.Type == TypeParameter.Int)
                    {
                        p = new IntegerParameter(values);
                        preprocessValues(valueParam, p, newParam.ID, newSelectionId, prepType);
                    }
                    else if (param.Type == TypeParameter.Enum)
                    {
                        p = new EnumeratedParameter(values);
                        preprocessValues(valueParam, p, newParam.ID, newSelectionId, prepType);
                    }
                    break;
                case "бинаризация":

                    break;
                case "без предобработки":
                    preprocessValues(valueParam, null, newParam.ID, newSelectionId, prepType);
                    break;
            }
        }

        private void preprocessValues(List<Entity> values, IParameter p, int paramId, int newSelectionId, string prepType)
        {
            DataHelper helper = new DataHelper();
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", newSelectionId.ToString()), typeof(SelectionRow));
            int index = 0;
            List<Entity> listValues = new List<Entity>();
            foreach (Entity value in values)
            {
                string val;
                switch (prepType)
                {
                    case "нормализация":
                        val = p.GetNormalizedDouble(((ValueParameter)value).Value).ToString();
                        break;
                    case "бинаризация":
                        val = ((ValueParameter)value).Value;//phphp
                        break;
                    case "без предобработки":
                        val = ((ValueParameter)value).Value;
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

        public int addNewTemplateForPreprocessingSelection(string selectionName, int countRows, int taskId, int taskTemplateId)
        {
            DataHelper helper = new DataHelper();

            string type = "develop";
            int selectionId = helper.addSelection(selectionName, taskTemplateId, countRows, type);
            
            List<Entity> listSelRow = new List<Entity>(countRows);
            for (int i = 0; i < countRows; i++)
            {
                SelectionRow entity = helper.addSelectionRow(selectionId, i+1);
                listSelRow.Add(entity);
            }
            DatabaseManager.SharedManager.insertMultipleEntities(listSelRow);

            return selectionId;
        }
    }
}
