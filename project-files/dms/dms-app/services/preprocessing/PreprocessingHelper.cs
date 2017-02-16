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
            List<Entity> inputParams = new List<Entity>();
            List<Entity> outputParams = new List<Entity>();
            foreach (Entity param in parameters)
            {
                models.Parameter p = (models.Parameter)param;
                if (p.IsOutput == 0)
                {
                    inputParams.Add(param);
                }
                else
                {
                    outputParams.Add(param);
                }
            }

            List<Entity> selectionRows = SelectionRow.where(new Query("SelectionRow").addTypeQuery(TypeQuery.select)
                .addCondition("SelectionID", "=", selectionId.ToString()), typeof(SelectionRow));

            Object[][] valuesX = new Object[selectionRows.Count][];
            Object[][] valuesY = new Object[selectionRows.Count][];

            int stepRow = 0;
            foreach (Entity selRow in selectionRows)
            {
                for (int i = 0; i < 2; i++) {
                    if (i == 0)
                    {
                        valuesX[stepRow] = new Object[inputParams.Count];
                        parameters = inputParams;
                    }
                    else
                    {
                        valuesY[stepRow] = new Object[outputParams.Count];
                        parameters = outputParams;
                    }
                    
                    int selectionRowId = selRow.ID;
                    int stepParam = 0;
                    foreach (Entity param in parameters)
                    {
                        TypeParameter type = ((dms.models.Parameter)param).Type;

                        int paramId = param.ID;
                        List<Entity> value = ValueParameter.where(new Query("ValueParameter").addTypeQuery(TypeQuery.select)
                            .addCondition("ParameterID", "=", paramId.ToString()).
                            addCondition("SelectionRowID", "=", selectionRowId.ToString()), typeof(ValueParameter));
                        if (i == 0)
                        {
                            switch (type)
                            {
                                case TypeParameter.Real:
                                    valuesX[stepRow][stepParam] = Convert.ToDouble((((ValueParameter)value[0]).Value).Replace(".", ","));
                                    break;
                                case TypeParameter.Int:
                                    valuesX[stepRow][stepParam] = Convert.ToInt32(((ValueParameter)value[0]).Value);
                                    break;
                                case TypeParameter.Enum:
                                    valuesX[stepRow][stepParam] = ((ValueParameter)value[0]).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (type)
                            {
                                case TypeParameter.Real:
                                    valuesY[stepRow][stepParam] = Convert.ToDouble((((ValueParameter)value[0]).Value).Replace(".", ","));
                                    break;
                                case TypeParameter.Int:
                                    valuesY[stepRow][stepParam] = Convert.ToInt32(((ValueParameter)value[0]).Value);
                                    break;
                                case TypeParameter.Enum:
                                    valuesY[stepRow][stepParam] = ((ValueParameter)value[0]).Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                        stepParam++;
                    }
                }
                stepRow++;
            }

            return null;//values;
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
                            values[stepRow][stepParam] = Convert.ToDouble((((ValueParameter)value[0]).Value).Replace(".", ","));
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
