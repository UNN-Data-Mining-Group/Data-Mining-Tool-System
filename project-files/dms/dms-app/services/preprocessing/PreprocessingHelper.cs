using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;
using dms.services.preprocessing.normalization;

namespace dms.services.preprocessing
{
    class PreprocessingHelper
    {
        private static PreprocessingHelper preprocessing;
        public static PreprocessingHelper Preprocessing
        {
            get
            {
                if (preprocessing == null)
                {
                    preprocessing = new PreprocessingHelper();

                }
                return preprocessing;
            }
        }

        public PreprocessingHelper()
        { }

        public Object[][] getValues(int selectionId)
        {
            Selection selection = ((Selection)dms.services.DatabaseManager.SharedManager.entityById(selectionId, typeof(Selection)));
            int taskTemplateId = selection.ID;
            List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(dms.models.Parameter));
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));

            Object[][] values = new Object[selectionRows.Count][];
            
            int stepRow = 0;
            foreach (Entity selRow in selectionRows)
            {
                values[stepRow] = new Object[parameters.Count];
                int selectionRowId = selRow.ID;
                int stepParam = 0;
                foreach (Entity param in parameters)
                {
                    TypeParameter type = ((dms.models.Parameter)param).Type;

                    int paramId = param.ID;
                    List<Entity> value = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                        .addCondition("ParameterID", "=", paramId.ToString()).
                        addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));

                    switch (type)
                    {
                        case TypeParameter.Real:
                            values[stepRow][stepParam] = Convert.Tofloat((((ValueParameter)value[0]).Value).Replace(".", ","));
                            break;
                        case TypeParameter.Int:
                            values[stepRow][stepParam] = Convert.ToInt32(((ValueParameter)value[0]).Value);
                            break;
                        case TypeParameter.Enum:
                            values[stepRow][stepParam] = ((ValueParameter)value[0]).Value;
                            break;
                        default:
                            break;
                    }

                    stepParam++;
                }
                stepRow++;
            }

            return values;
        }

        public List<List<Object>> getListValues(int selectionId)
        {
            Selection selection = ((Selection)dms.services.DatabaseManager.SharedManager.entityById(selectionId, typeof(Selection)));
            int taskTemplateId = selection.ID;
            List<Entity> parameters = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(dms.models.Parameter));
            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));

            List<List<Object>> values = new List<List<Object>>();

            int stepRow = 0;
            foreach (Entity selRow in selectionRows)
            {
                int selectionRowId = selRow.ID;
                int stepParam = 0;
                foreach (Entity param in parameters)
                {
                    TypeParameter type = ((dms.models.Parameter)param).Type;
                    
                    int paramId = param.ID;
                    List<Entity> value = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                        .addCondition("ParameterID", "=", paramId.ToString()).
                        addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));

                    switch (type)
                    {
                        case TypeParameter.Real:
                            values[stepRow][stepParam] = Convert.Tofloat((((ValueParameter)value[0]).Value).Replace(".", ","));
                            break;
                        case TypeParameter.Int:
                            values[stepRow][stepParam] = Convert.ToInt32(((ValueParameter)value[0]).Value);
                            break;
                        case TypeParameter.Enum:
                            values[stepRow][stepParam] = ((ValueParameter)value[0]).Value;
                            break;
                        default:
                            break;
                    }

                    stepParam++;
                }
                stepRow++;
            }

            return values;
        }
    }
}
