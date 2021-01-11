using Newtonsoft.Json;

namespace Framework.Base.ModelEntity
{
    public abstract class ModelEntityBase : IModelEntityBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
