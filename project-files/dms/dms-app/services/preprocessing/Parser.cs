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

        public Parser()
        { }

        private int countRows;
        public int CountRows { get; set; }
        public int CountParameters { get; set; }
        public bool HasHeader { get; set; }
        public string[] ParametersName { get; set; }

        public int parse(int taskTemplateId, string filePath, char delimiter, int taskId, string selectionName, 
            ParameterCreationViewModel[] parameters, bool hasHeader, bool isUsingExitingTemplate)
        {
            DataHelper helper = new DataHelper();
            HasHeader = hasHeader;
            string type = "develop";
            int selectionId = helper.addSelection(selectionName, taskTemplateId, countRows, type);
            
            addParameters(filePath, delimiter, parameters, selectionId, taskTemplateId, isUsingExitingTemplate);

            return selectionId;
        }

        private string convertToType(string typeStr)
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

        public string[] getParametersTypes(string filePath, char delimiter, bool hasHeader)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                int iter = -1;
                string line = sr.ReadLine();
                if (line != "" && hasHeader)
                {
                    ParametersName = line.Split(delimiter);
                    line = sr.ReadLine();
                }
                string[] values = line.Split(delimiter);
                CountParameters = values.Length;
                string[] types = new string[values.Length];
                while (line != "" && line != null)
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
                            types[index] = convertToType(type.Name);
                        }
                        else
                        {
                            if (types[index] != convertToType(type.Name))
                            {
                                switch (types[index])
                                {
                                    case "enum":
                                        types[index] = convertToType(type.Name);
                                        break;
                                    case "int":
                                        if ("float".Equals(convertToType(type.Name)))
                                        {
                                            types[index] = convertToType(type.Name);
                                        }
                                        break;
                                }
                            }
                        }
                        index++;
                    }
                    line = sr.ReadLine();
                }

                countRows = iter + 1;
                CountRows = countRows;
                return types;
            }
        }

        private int getIsOutput(string isOutput)
        {
            return "Входной" == isOutput ? 0 : 1;
        }

        public TypeParameter getTypeParameter(string typeStr)
        {
            if (typeStr.Contains("enum"))
            {
                return TypeParameter.Enum;
            }
            else if (typeStr.Contains("int"))
            {
                return TypeParameter.Int;
            }
            else if (typeStr.Contains("float"))
            {
                return TypeParameter.Real;
            }
            return TypeParameter.Real;
        }

        private void addParameters(string filePath, char delimiter, ParameterCreationViewModel[] parameters, 
            int selectionId, int taskTemplateId, bool isUsingExitingTemplate)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                DataHelper helper = new DataHelper();
                int rowStep = 0;
                string line = sr.ReadLine();
                if (line != "" && HasHeader)
                {
                    line = sr.ReadLine();
                }
                int paramCount = parameters.Length;

                List<Entity> listSelRow = new List<Entity>(countRows);
                List<Entity> listParams = new List<Entity>(paramCount * countRows);
                List<ValueParameter> listValParams = new List<ValueParameter>(paramCount * countRows);
                while (line != "" && line != null)
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

                        if (rowStep == 1)
                        {
                            dms.models.Parameter parameter = helper.addParameter(parameterName, comment, taskTemplateId, index, isOutput, type);
                            listParams.Add(parameter);
                        }
                        listValParams.Add(helper.addValueParameter(entity.ID, -1/*parameter.ID*/, value));
                    }
                    
                    line = sr.ReadLine();
                }

                DatabaseManager.SharedManager.insertMultipleEntities(listSelRow);
                if (!isUsingExitingTemplate)
                {
                    DatabaseManager.SharedManager.insertMultipleEntities(listParams);
                }
                else
                {
                    listParams = dms.models.Parameter.where(new Query("Parameter").addTypeQuery(TypeQuery.select)
                        .addCondition("TaskTemplateID", "=", taskTemplateId.ToString()), typeof(dms.models.Parameter));
                }

                List<Entity> list = new List<Entity>(countRows * paramCount);
                int selRowId = 0;
                for (int i = 0; i < paramCount * countRows; i++)
                {
                    if (i % paramCount == 0)
                    {
                        selRowId = i == 0 ? listSelRow[0].ID : listSelRow[i / paramCount].ID;
                    }
                    int paramId = listParams[i % paramCount].ID;

                    ValueParameter param = listValParams[i];
                    param.ParameterID = paramId;
                    param.SelectionRowID = selRowId;
                    list.Add(param);
                }
                DatabaseManager.SharedManager.insertMultipleEntities(list);
            }
        }
    }
}
