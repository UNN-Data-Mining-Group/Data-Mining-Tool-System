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
        private static Parser parserObj;
        public static Parser ParserObj
        {
            get
            {
                if (parserObj == null)
                {
                    parserObj = new Parser();

                }
                return parserObj;
            }
        }

        private int rowCount;

        public Parser()
        { }

        public void parse(string taskTemplateName, string filePath, char delimiter, int taskId, string selectionName, ParameterCreationViewModel[] parameters)//, hasHeader
        {
            int taskTemplateId = addTaskTemplate(taskTemplateName, taskId, null);
            int selectionId = addSelection(selectionName, taskTemplateId);
            addParameters(filePath, delimiter, parameters, selectionId, taskTemplateId);
        }

        private int addTaskTemplate(string name, int taskId, IPreprocessingParameters ppParameters)
        {
            //ppParameters = null для главного шаблона
            DataHelper helper = new DataHelper();
            int taskTemplateId = helper.addTaskTemplate(name, taskId, ppParameters);
            return taskTemplateId;
        }
        private int addSelection(string name, int taskTemplateId)
        {
            string type = "develop";
            DataHelper helper = new DataHelper();
            int selectionId = helper.addSelection(name, taskTemplateId, rowCount, type);
            return selectionId;
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
        private int getRowCount(string path, char delimiter)
        {
            int size = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    line = sr.ReadLine();
                    size++;
                }
            }
            return size;
        }

        private void addParameters(string path, char delimiter, ParameterCreationViewModel[] parameters, int selectionId, int taskTemplateId)
        {
            int size = getRowCount(path, delimiter);
            using (StreamReader sr = new StreamReader(path))
            {
                DataHelper helper = new DataHelper();
                int rowNumber = 0;
                string line = sr.ReadLine();
                string[] values = line.Split(delimiter);
                int count = values.Length;
                List<Entity> listSelRow = new List<Entity>(size);
                List<Entity> listParams = new List<Entity>(count*size);
                List<ValueParameter> listValParams = new List<ValueParameter>(count*size);

                while (line != null)
                {
                    rowNumber++;
                    SelectionRow entity = helper.addSelectionRow(selectionId, rowNumber);
                    listSelRow.Add(entity);
                    values = line.Split(delimiter);
                    
                    int i = -1;
                    foreach (string value in values)
                    {
                        i++;
                        string parameterName = parameters[i].Name;
                        string comment = parameters[i].Comment;
                        if (comment == null)
                        {
                            comment = "";
                        }
                        int isOutput = getIsOutput(parameters[i].KindOfParameter);
                        TypeParameter type = getTypeParameter(parameters[i].Type);
                    
                        dms.models.Parameter parameter = helper.addParameter(parameterName, comment, taskTemplateId, i, isOutput, type);
                        listParams.Add(parameter);
                        listValParams.Add(helper.addValueParameter(entity.ID, parameter.ID, value));
                    }
                    
                    line = sr.ReadLine();
                }

                DatabaseManager.SharedManager.insertMultipleEntities(listSelRow);
                DatabaseManager.SharedManager.insertMultipleEntities(listParams);
                List<Entity> list = new List<Entity>(size*count);
                int selRowId = 0;
                for (int i = 0; i < count * size; i++)
                {
                    if (i % count == 0) {
                        int index;
                        if (i == 0)
                            index = 0;
                        else
                            index = i / count - 1;
                        selRowId = listSelRow[index].ID;
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
            if ("Входной" == isOutput)
            {
                return 0;
            }
            else
            {
                return 1;
            }
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
