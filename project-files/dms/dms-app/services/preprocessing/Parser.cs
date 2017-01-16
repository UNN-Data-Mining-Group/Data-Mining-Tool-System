using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dms.models;
using dms.view_models;

namespace dms.services.preprocessing
{
    class Parser
    {
        private static Parser selectionParser;
        public static Parser SelectionParser
        {
            get
            {
                if (selectionParser == null)
                {
                    selectionParser = new Parser();

                }
                return selectionParser;
            }
        }

        private int rowCount;

        public Parser()
        { }

        public void parse(string taskTemplateName, string filePath, char delimiter, int taskId, string selectionName, 
            ParameterCreationViewModel[] parameters)//, hasHeader
        {
            DataHelper helper = new DataHelper();

            //ppParameters = null для главного шаблона
            IPreprocessingParameters ppParameters = null;
            int taskTemplateId = helper.addTaskTemplate(taskTemplateName, taskId, ppParameters);

            string type = "develop";
            int selectionId = helper.addSelection(selectionName, taskTemplateId, rowCount, type);
            
            addParameters(filePath, delimiter, parameters, selectionId, taskTemplateId);
        }

        public string[] getParameterTypes(string filePath, char delimiter)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                int iter = -1;
                string line = sr.ReadLine();
                string[] values = line.Split(delimiter);
                string[] types = new string[values.Length];
                while (line != null)
                {
                    iter++;
                    values = line.Split(delimiter);
                    int index = 0;
                    foreach (string value in values)
                    {
                        Type type = value.GetType();
                        int intValue = 0;
                        float doubleValue = 0;
                        if (Int32.TryParse(value.Replace('.', ','), out intValue))
                        {
                            type = intValue.GetType();
                        }
                        else if (float.TryParse(value.Replace('.', ','), out doubleValue))
                        {
                            type = doubleValue.GetType();
                        }
                        if (iter == 0)
                        {
                            types[index] = getType(type.Name);
                        }
                        else
                        {
                            if (types[index] != getType(type.Name))
                            {
                                switch (types[index])
                                {
                                    case "enum":
                                        types[index] = getType(type.Name);
                                        break;
                                    case "int":
                                        if ("float".Equals(getType(type.Name)))
                                        {
                                            types[index] = getType(type.Name);
                                        }
                                        break;
                                }
                            }
                        }
                        index++;
                    }
                    line = sr.ReadLine();
                }

                rowCount = iter + 1;
                return types;
            }
        }

        private string getType(string typeStr)
        {
            string type;
            switch (typeStr)
            {
                case "String":
                    type = "enum";
                    break;
                case "Int32":
                    type = "int";
                    break;
                default:
                    //  case "float":
                    type = "float";
                    break;
            }
            return type;
        }

        private void addParameters(string filePath, char delimiter, ParameterCreationViewModel[] parameters, int selectionId, int taskTemplateId)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                DataHelper helper = new DataHelper();
                int rowStep = 0;
                string line = sr.ReadLine();
                int paramCount = parameters.Length;

                List<Entity> listSelRow = new List<Entity>(rowCount);
                List<Entity> listParams = new List<Entity>(paramCount * rowCount);
                List<ValueParameter> listValParams = new List<ValueParameter>(paramCount * rowCount);
                while (line != null)
                {
                    rowStep++;
                    SelectionRow entity = helper.addSelectionRow(selectionId, rowStep);
                    listSelRow.Add(entity);

                    string[] values = line.Split(delimiter);
                    
                    int index = -1;
                    foreach (string value in values)
                    {
                        index++;
                        string parameterName = parameters[index].Name;
                        string comment = parameters[index].Comment == null ? "" : parameters[index].Comment;
                        int isOutput = getIsOutput(parameters[index].KindOfParameter);
                        TypeParameter type = getTypeParameter(parameters[index].Type);
                    
                        dms.models.Parameter parameter = helper.addParameter(parameterName, comment, taskTemplateId, index, isOutput, type);
                        listParams.Add(parameter);
                        listValParams.Add(helper.addValueParameter(entity.ID, parameter.ID, value));
                    }
                    
                    line = sr.ReadLine();
                }

                DatabaseManager.SharedManager.insertMultipleEntities(listSelRow);
                DatabaseManager.SharedManager.insertMultipleEntities(listParams);
                List<Entity> list = new List<Entity>(rowCount * paramCount);
                int selRowId = 0;
                for (int i = 0; i < paramCount * rowCount; i++)
                {
                    if (i % paramCount == 0) {
                        selRowId = i == 0 ? listSelRow[0].ID : listSelRow[i / paramCount - 1].ID;
                    }
                    int paramId = listParams[i].ID;

                    ValueParameter param = listValParams[i];
                    param.ParameterID = paramId;
                    param.SelectionRowID = selRowId;
                    list.Add(param);
                }
                DatabaseManager.SharedManager.insertMultipleEntities(list);
            }
        }
        private int getIsOutput(string isOutput)
        {
            return "Входной" == isOutput ? 0 : 1;
        }
        private TypeParameter getTypeParameter(string typeStr)
        {
            switch (typeStr)
            {
                case "enum":
                    return TypeParameter.Enum;
                case "int":
                    return TypeParameter.Int;
                default:
                    //  case "float":
                    return TypeParameter.Real;
            }
        }

    }
}
