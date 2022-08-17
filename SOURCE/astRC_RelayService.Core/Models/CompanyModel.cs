using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Models
{
    [JsonObject]
    public class CompanyModel : IEquatable<CompanyModel>
    {
        [JsonConstructor]
        public CompanyModel(string id, string password)
        {
            this.Id = id;
            this.Password = password;
        }

        [JsonProperty]
        public string Id { get; private set; }

        [JsonProperty]
        public string Password { get; private set; }

        #region IEquatable<Company>

        public override bool Equals(Object obj)
        {
            var target = obj as CompanyModel;
            if (target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;

            return this.Equals(target);
        }

        public bool Equals(CompanyModel target)
        {
            if (target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;

            if (this.Id != target.Id) return false;
            if (this.Password != target.Password) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            if (this.Id != null) hashCode ^= this.Id.GetHashCode();
            if (this.Password != null) hashCode ^= this.Password.GetHashCode();

            return hashCode;
        }

        #endregion
    }
}
