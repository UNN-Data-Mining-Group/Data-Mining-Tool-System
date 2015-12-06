using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SII
{
    public class Selection
    {
        public int ID;
        public int TaskID;
        public String Name;
        public int CountRows;

        public bool WithRes; //обучающая выборка или нет(результат известен или нет)

        private List<ValueParametr> ArrValueParameters;

        public List<ValueParametr> GetArrValueParameters()
        {
            return ArrValueParameters;
        }

        public void LoadArrValueParametersFromFile(String namefile, List<Parametr> arrParameters)
        {
            //test withResult selection
            ArrValueParameters = ValueParametr.GetArrValuesFromFile(namefile, arrParameters, ID, WithRes);
            AddValuesToDB();
            CountRows = ArrValueParameters.Count / arrParameters.Count;
        }

        private void AddValuesToDB()
        {
            SQLManager sqlManager = SQLManager.MainSQLManager;
            String sqlReqStr;
            foreach (ValueParametr value in ArrValueParameters)
            {
                sqlReqStr = "INSERT INTO VALUE_PARAM (PARAM_ID, SELECTION_ID, VALUE, ROW_INDEX) " +
                "VALUES('" + value.ParametrID + "','" + value.SelectionID + "','" + value.Value + "','" + value.RowIndex + "');";
                int state = sqlManager.SendInsertRequest(sqlReqStr);
                value.ID = state;
            }            
        }
    }
}
