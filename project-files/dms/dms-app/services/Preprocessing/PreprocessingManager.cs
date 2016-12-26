using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dms.models;
using dms.view_models;

namespace dms.services.Preprocessing
{
    class PreprocessingManager
    {
        
        private static PreprocessingManager manager;
        public static PreprocessingManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = new PreprocessingManager();
                    
                }
                return manager;
            }
        }

        int rowCount;
        int taskTemplateId;
        int selectionId;

        public PreprocessingManager()
        {
        }

        public string[] getParameterTypes(String path, char delimiter)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                int iter = -1;
                String line = sr.ReadLine();
                string[] values = line.Split('|');
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
                            types[index] = type.Name;
                        }
                        else
                        {
                            if (types[index] != type.Name)
                            {
                                switch (types[index])
                                {
                                    case "String":
                                        types[index] = type.Name;
                                        break;
                                    case "Int32":
                                        if ("Single".Equals(type.Name))
                                        {
                                            types[index] = type.Name;
                                        }
                                        break;
                                }
                            }
                        }
                        index++;
                    }
                    line = sr.ReadLine();
                }
                for (int i = 0; i < types.Length; i++)
                {
                    switch (types[i])
                    {
                        case "String":
                            types[i] = "enum";
                            break;
                        case "Int32":
                            types[i] = "int";
                            break;
                        default:
                            types[i] = "float";
                            break;
                    }
                }
                rowCount = iter + 1;
                return types;
            }
        }

        public void setTaskTemplate(string name, int taskId, IPreprocessingParameters ppParameters)
        {//ppParameters
            DataHelper helper = new DataHelper();
            taskTemplateId = helper.setTaskTemplate(name, taskId, ppParameters);
        }

        public void setSelection(string name, string type)
        {//type
            DataHelper helper = new DataHelper();
            selectionId = helper.setSelection(name, taskTemplateId, rowCount, type);
        }

        public void setParameterTables(string path, char delimiter, ParameterCreationViewModel[] parameters, string[] types)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                DataHelper helper = new DataHelper();
                int rowNumber = 0;
                String line = sr.ReadLine();
                while (line != null)
                {
                    rowNumber++;
                    int selectionRowId = helper.setSelectionRow(selectionId, rowNumber);
                    string[] values = line.Split(delimiter);
                    int i = -1;
                    foreach (string value in values)
                    {
                        string parameterName = parameters[i].Name;
                        string comment = parameters[i].Comment;
                        string isOutputFlag = parameters[i].KindOfParameter;
                        int isOutput = 1;//типы не совпадают
                        TypeParameter type = TypeParameter.Real; //типы не совпадают
                        i++;//i ???
                        int parameterId = helper.setParameter(parameterName, comment, taskTemplateId, i, isOutput, type);
                        helper.setValueParameter(selectionRowId, parameterId, value);
                    }
                }
            }
        }
    }
}
