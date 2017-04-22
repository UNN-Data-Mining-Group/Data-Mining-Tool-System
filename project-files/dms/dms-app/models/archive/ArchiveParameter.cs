using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.models.archive
{
    [Serializable]
    public class ArchiveParameter : ArchiveModel
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

        private string comment;
        public string Comment
        {
            get
            {
                return comment;
            }

            set
            {
                comment = value;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        private int isOutput;
        public int IsOutput
        {
            get
            {
                return isOutput;
            }

            set
            {
                isOutput = value;
            }
        }

        private TypeParameter type;
        public TypeParameter Type
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

        public ArchiveParameter(models.Parameter par)
        {
            this.ID = par.ID;
            this.Name = par.Name;
            this.Index = par.Index;
            this.Type = par.Type;
            this.Comment = par.Comment;
            this.IsOutput = par.IsOutput;
        }
    }
}
