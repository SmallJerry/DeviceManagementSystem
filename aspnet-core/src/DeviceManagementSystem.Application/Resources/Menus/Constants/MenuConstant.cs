using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Menus.Constants
{
    /// <summary>
    /// 菜单常量
    /// </summary>
    public static class MenuConstant
    {


        #region 菜单类型常量

        /// <summary>
        /// 目录类型
        /// </summary>
        public const string MENU_TYPE_CATALOG = "CATALOG";

        /// <summary>
        /// 菜单类型
        /// </summary>
        public const string MENU_TYPE_MENU = "MENU";

        /// <summary>
        /// 内链类型
        /// </summary>
        public const string MENU_TYPE_IFRAME = "IFRAME";

        /// <summary>
        /// 外链类型
        /// </summary>
        public const string MENU_TYPE_LINK = "LINK";

        #endregion

        #region 是否常量

        /// <summary>
        /// 是
        /// </summary>
        public const string WHETHER_YES = "YES";

        /// <summary>
        /// 否
        /// </summary>
        public const string WHETHER_NO = "NO";

        #endregion

        #region 特殊字符常量


        /// <summary>
        /// 斜杠
        /// </summary>
        public const char SLASH = '/';

        /// <summary>
        /// 横杠
        /// </summary>
        public const char DASHED = '-';

        /// <summary>
        /// 索引页面名称
        /// </summary>
        public const string INDEX_PAGE = "index";

        #endregion

        #region 默认值常量

        /// <summary>
        /// 默认图标
        /// </summary>
        public const string DEFAULT_ICON = "appstore-outlined";

        /// <summary>
        /// 默认排序码
        /// </summary>
        public const int DEFAULT_SORT_CODE = 99;

        #endregion

        #region 验证方法

        /// <summary>
        /// 验证菜单类型是否有效
        /// </summary>
        /// <param name="menuType">菜单类型</param>
        /// <returns>是否有效</returns>
        public static bool IsValidMenuType(string menuType)
        {
            return menuType == MENU_TYPE_CATALOG
                || menuType == MENU_TYPE_MENU
                || menuType == MENU_TYPE_IFRAME
                || menuType == MENU_TYPE_LINK;
        }

        /// <summary>
        /// 验证标题是否包含特殊字符
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns>是否包含特殊字符</returns>
        public static bool IsTitleContainsSpecialChars(string title)
        {
            return !string.IsNullOrEmpty(title) && title.Contains(DASHED);
        }

        #endregion



    }
}
