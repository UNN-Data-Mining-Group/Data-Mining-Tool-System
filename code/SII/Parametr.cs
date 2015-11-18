using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SII
{
    public enum TypeParametr {
        Int,
        Real,
        Bool,
        Enum
    }

    class ShowTypeParametr
    {
        private TypeParametr type;
        private string name;

        public ShowTypeParametr(TypeParametr _type, string _name)
        {
            this.type = _type;
            this.name = _name;
        }

        public TypeParametr Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public override string ToString()
        {
            return this.Type.ToString() + " - " + this.Name;
        }
    }

    public class Parametr
    {
        public int ID;
        public int TaskID;
        public String Name;
        public TypeParametr Type;
        public String Range;
        public int Number;
    }
}
