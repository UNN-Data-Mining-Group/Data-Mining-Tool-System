using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dms.models;

namespace dms.services.Preprocessing
{
    class DataHelper
    {
        public int setSelection(string name, int taskTemplateId, int count, string type)
        {
            Selection entity = new Selection();
            entity.Name = name;
            entity.TaskTemplateID = taskTemplateId;
            entity.RowCount = count;
            entity.Type = type;
            entity.save();

            return entity.ID;
        }
        public int setSelectionRow(int selectionId, int rowNumber)
        {
            SelectionRow entity = new SelectionRow();
            entity.SelectionID = selectionId;
            entity.Number = rowNumber;
            entity.save();

            return entity.ID;
        }
        public void setValueParameter(int selectionRowID, int parameterID, string value)
        {
            ValueParameter entity = new ValueParameter();
            entity.SelectionRowID = selectionRowID;
            entity.ParameterID = parameterID;
            entity.Value = value;
            entity.save();
        }
        public int setParameter(string name, string comment, int taskTemplateID, int index, int isOutput, TypeParameter type)
        {
            Parameter entity = new Parameter();
            entity.Name = name;
            entity.Comment = comment;
            entity.TaskTemplateID = taskTemplateID;
            entity.Index = index;
            entity.IsOutput = isOutput;
            entity.Type = type;
            entity.save();

            return entity.ID;
        }
        public int setTaskTemplate(string name, int taskId, IPreprocessingParameters ppParameters)
        {
            TaskTemplate entity = new TaskTemplate();
            entity.Name = name;
            entity.TaskID = taskId;
            entity.PreprocessingParameters = ppParameters;

            return entity.ID;
        }
    }
}
