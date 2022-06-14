using System;

namespace ModulDelivery.Infrastructure
{
    public class LabelAttribute : Attribute
    {
        public LabelAttribute(string name) => this.name = name;
        public string name;
    }
}