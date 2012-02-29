using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    public class Person
    {
        private int _id;
        public int Id
        {
            get{
                return this._id;
        }
        }
        private String _name;
        public String Name
        {
            get
            {
                return this._name;
            }
        }
        public Person(int id, String name)
        {
            this._id = id;
            this._name = name;
        }
    }
}
