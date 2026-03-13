using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dicts.param
{
    /// <summary>
    /// 字典添加参数
    /// </summary>
    public class DictAddParam
    {

        /// <summary>
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; } = Guid.Empty;

        /// <summary>
        /// 字典名称
        /// </summary>
        [Required(ErrorMessage ="字典名称不能为空")]
        public string DictLabel { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        [Required(ErrorMessage = "字典值不能为空")]
        public string DictValue { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }


        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }

    }
}
