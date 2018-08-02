﻿using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.XmlRpc.MetaWeblog
{
    [XmlRpcMissingMappingAttribute(MappingAction.Ignore)]
    public struct Source
    {
        public string name;

        public string url;
    }
}
