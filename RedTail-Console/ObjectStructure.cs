using System.Collections.Generic;

namespace RedTail_Console
{
    class ObjectStructure
    {
        private readonly List<Element> _elements = new List<Element>();

        public void Attach(Element element)
        {
            _elements.Add(element);
        }

        public void Detatch(Element element)
        {
            _elements.Remove(element);
        }

        public void DetachAll()
        {
            _elements.Clear();
        }

        public void Accept(Visitor visitor)
        {
            foreach (var element in _elements)
            {
                element.Accept(visitor);
            }
        }
             
    }
}
