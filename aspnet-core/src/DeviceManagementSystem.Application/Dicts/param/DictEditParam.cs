using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dicts.param
{
    /// <summary>
    /// 字典编辑参数
    /// </summary>
    public class DictEditParam : DictAddParam   
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
    }
}
