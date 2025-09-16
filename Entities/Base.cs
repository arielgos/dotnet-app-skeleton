using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities
{
    [Serializable]
    public abstract class Base
    {
        [NotMapped,JsonIgnore]
        public Action Action { get; set; } = Action.None;

        public Base Clone()
        {
            return (Base)this.MemberwiseClone();
        }
    }

    public enum Action
    {
        None, Insert, Update, Delete
    }
}