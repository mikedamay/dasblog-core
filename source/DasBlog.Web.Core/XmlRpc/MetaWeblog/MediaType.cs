using CookComputing.XmlRpc;

namespace DasBlog.Core.XmlRpc.MetaWeblog
{
    public struct MediaType
    {
        public string name;

        [XmlRpcMissingMappingAttribute(MappingAction.Ignore)]
        public string type;

        public byte[] bits;
    }
}
