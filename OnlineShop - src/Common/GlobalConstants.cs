using System;

namespace Common
{
    public static class GlobalConstants
    {
        public const string WWWROOT = "wwwroot";

        public const string CANNOT_DELETE_CATEGORY_IF_ANY_PRODUCTS = "Може да изтриете категория само ако не съдържа продукти!";

        public const string SUB_CATEGORY_PATH_TEMPLATE = "wwwroot/images/SubCategories/image{0}.png";

        public const string SUB_CATEGORY_SRC_ROOT_TEMPLATE = "/images/SubCategories/image{0}.png";

        public const string PRODUCT_PATH_TEMPLATE = "wwwroot/images/Products/image{0}.jpg";

        public const string PRODUCT_SRC_ROOT_TEMPLATE = "/images/Products/image{0}.jpg";

        public const string URL_TEMPLATE_AUTOCOMPLETE = "https://localhost:5001/Product/Details/{0}";
    }
}
