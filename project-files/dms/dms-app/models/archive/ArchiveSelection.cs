using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveSelection : ArchiveModel
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        private int rowCount;
        public int RowCount
        {
            get
            {
                return rowCount;
            }

            set
            {
                rowCount = value;
            }
        }

        private string type;
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public ArchiveSelection(models.Selection selection)
        {
            this.ID = selection.ID;
            this.Name = selection.Name;
            this.RowCount = selection.RowCount;
            this.Type = selection.Type;
        }

        override public bool equalsEntity(models.Entity entity)
        {
            if (entity.GetType() != typeof(models.Selection))
            {
                return false;
            }
            return this.Name == ((models.Selection)entity).Name;
        }
    }
}
