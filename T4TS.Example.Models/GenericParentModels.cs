using System;
using System.Runtime.Serialization;

namespace T4TS.Example.Models
{
    public interface IEntityObject
    {
    }
    public interface IEntityObjectId : IEntityObject
    {
        string Id { get; set; }

        string ApiKey { get; set; }

        string UserId { get; set; }
    }

    [DataContract(IsReference = true)]
    [Serializable]
    public abstract class EntityObject<TEntityObject> : IEntityObject where TEntityObject : EntityObject<TEntityObject>, new()
    {
    }

    [DataContract(IsReference = true)]
    public abstract class EntityObjectId<TEntityObject> : EntityObject<TEntityObject>, IEntityObjectId where TEntityObject : EntityObject<TEntityObject>, new()
    {
        [DataMember]
        public string ApiKey { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string Id { get; set; }
    }


    [DataContract(IsReference = true)]
    public class Device : EntityObjectId<Device>
    {
        [DataMember]
        public string Name { get; set; }
    }
}
