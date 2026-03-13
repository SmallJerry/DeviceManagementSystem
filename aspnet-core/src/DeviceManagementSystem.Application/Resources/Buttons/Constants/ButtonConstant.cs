using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Buttons.Constants
{
    /// <summary>
    /// 按钮常量
    /// </summary>
    public static class ButtonConstant
    {


        #region 生成按钮常量

        /// <summary>
        /// 新增按钮标题模板
        /// </summary>
        public const string BUTTON_ADD_TITLE = "新增{0}";

        /// <summary>
        /// 编辑按钮标题模板
        /// </summary>
        public const string BUTTON_EDIT_TITLE = "编辑{0}";

        /// <summary>
        /// 删除按钮标题模板
        /// </summary>
        public const string BUTTON_DELETE_TITLE = "删除{0}";

        /// <summary>
        /// 批量删除按钮标题
        /// </summary>
        public const string BUTTON_BATCH_DELETE_TITLE = "批量删除";

        /// <summary>
        /// 导入按钮标题模板
        /// </summary>
        public const string BUTTON_IMPORT_TITLE = "导入{0}";

        /// <summary>
        /// 导出按钮标题模板
        /// </summary>
        public const string BUTTON_EXPORT_TITLE = "导出{0}";

        /// <summary>
        /// 新增按钮编码模板
        /// </summary>
        public const string BUTTON_ADD_CODE = "{0}Add";

        /// <summary>
        /// 编辑按钮编码模板
        /// </summary>
        public const string BUTTON_EDIT_CODE = "{0}Edit";

        /// <summary>
        /// 删除按钮编码模板
        /// </summary>
        public const string BUTTON_DELETE_CODE = "{0}Delete";

        /// <summary>
        /// 批量删除按钮编码模板
        /// </summary>
        public const string BUTTON_BATCH_DELETE_CODE = "{0}BatchDelete";

        /// <summary>
        /// 导入按钮编码模板
        /// </summary>
        public const string BUTTON_IMPORT_CODE = "{0}Import";

        /// <summary>
        /// 导出按钮编码模板
        /// </summary>
        public const string BUTTON_EXPORT_CODE = "{0}Export";

        /// <summary>
        /// 默认按钮排序码
        /// </summary>
        public static readonly Dictionary<string, int> DefaultButtonSortCodes = new Dictionary<string, int>
        {
            { "Add", 1 },
            { "Edit", 2 },
            { "Delete", 3 },
            { "BatchDelete", 4 },
            { "Import", 5 },
            { "Export", 6 }
        };

        #endregion

        #region 关系扩展JSON键名

        /// <summary>
        /// 按钮信息键名
        /// </summary>
        public const string RELATION_EXT_JSON_BUTTON_INFO = "buttonInfo";

        #endregion


    }
}
