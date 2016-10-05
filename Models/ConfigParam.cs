using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crytex.GameServers.Enums;

namespace Crytex.GameServers.Models
{
    public class ConfigParam
    {
        public ConfigParamType Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ConfigParams : List<ConfigParam>
    {
        public ConfigParam Get(string name) => this.FirstOrDefault(o => o.Name.Equals(name));
        public string GetVal(string name)
        {
            var res = this.FirstOrDefault(o => o.Name.Equals(name));
            return res?.ToString() ?? "";
        }
    }
}
