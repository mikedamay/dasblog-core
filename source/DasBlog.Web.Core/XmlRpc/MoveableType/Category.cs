using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.XmlRpc.MoveableType
{
    public struct Category
    {
        public string categoryId;

        [XmlRpcMissingMappingAttribute(MappingAction.Ignore)]
        public string categoryName;
        [XmlRpcMissingMappingAttribute(MappingAction.Ignore)]
        public bool isPrimary;
    }
}
