namespace WebApplication.Helpers
{
    public static class ApiEndpointsHelper
    {
        /// <summary>
        /// Format route with path parameters, like "/api/employee/{0}"
        /// </summary>
        public static string Format(string template, params object[] args)
        {
            return string.Format(template, args);
        }

        /// <summary>
        /// Format route and append query string like "?key=value&..."
        /// </summary>
        public static string FormatWithQuery(string template, object[]? routeParams = null, Dictionary<string, string>? queryParams = null)
        {
            string basePath = routeParams != null && routeParams.Length > 0
                ? string.Format(template, routeParams)
                : template;

            if (queryParams == null || queryParams.Count == 0)
                return basePath;

            var query = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            return $"{basePath}?{query}";
        }
    }
}
