using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

using Newtonsoft.Json;
using System.Dynamic;
using System.Globalization;

namespace DataTools {
    public class ConvertFormat {
        public string dateFormat;
        public string decimalFormat;
        public string currencyFormat;

        public ConvertFormat() { }
        public ConvertFormat(string DateFormat, string DecimalFormat, string CurrencyFormat) {
            dateFormat = DateFormat;
            decimalFormat = DecimalFormat;
            currencyFormat = CurrencyFormat;
        }
    }

    public class DataConverter {

        public static bool IsNullable<T>(T t) { return false; }
        public static bool IsNullable<T>(T? t) where T : struct { return true; }

        public static Type GetDataType<T>(T val) {
            if (IsNullable(val)) {
                var propType = Nullable.GetUnderlyingType(val.GetType());
                return propType;
            } 

            return val.GetType();
        }

        public static string convertValue(string value, string format) {
            if (value == null)
                return "";

            return value;
        }

        public static string convertValue(DateTime value, string format) {
            if (value == null)
                return "";

            if (format != null) {
                return value.ToString(format);
                //return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
            }

            return "";
        }
        public static string convertValue(object value, string format) {
            if (value == null)
                return "";

            return value.ToString();
        }

        public static string ConvertDateFormat(string date, string dateFormat) {
            string _date = date;

            if ((date != null && date != "") && dateFormat != null) {
                DateTime dateTime = Convert.ToDateTime(date);
                _date = dateTime.ToString(dateFormat, new CultureInfo("en-US"));
            }

            return _date;
        }

        public static List<T> datasetTableToList<T>(DataTable dataTable) {
            string jsonStr = JsonConvert.SerializeObject(dataTable);
            List<T> tmpList = JsonConvert.DeserializeObject<List<T>>(jsonStr);

            return tmpList;
        }

        public static string ConvertListToHTMLTable<T>(List<T> listItems, Dictionary<string, string> headerMaps, ConvertFormat formatOptions = null) {

            /// Make a Header first
            /// 

            string htmlStr = "<table width='100%'>\n";

            Type hdType = listItems[0].GetType();
            PropertyInfo[] hdInfos = hdType.GetProperties();

            htmlStr += "\t<thead>\n";
            htmlStr += "\t\t<tr>\n";
            /// Get the Header field
            foreach (var hdInfo in hdInfos) {
                if (headerMaps != null) {
                    
                    if (headerMaps.ContainsKey(hdInfo.Name))
                        htmlStr += "\t\t\t<th>" + headerMaps[hdInfo.Name] + "</th>\n";

                }  else
                    htmlStr += "\t\t\t<th>" + hdInfo.Name + "</th>\n";
            }
            htmlStr += "\t\t</tr>\n";
            htmlStr += "\t</thead>\n";

            htmlStr += "\t<tbody>\n";
            foreach (var item in listItems) {
                Type type = item.GetType();
                PropertyInfo[] infos = type.GetProperties();

                /// Fill the table value
                htmlStr += "\t\t<tr>\n";
                foreach (var info in infos) {

                    if (headerMaps == null || headerMaps.ContainsKey(info.Name)) {
                        if (info.GetValue(item) != null) {
                            //if (typeof(DateTime).IsAssignableFrom(info.PropertyType))
                            if (info.GetValue(item) is DateTime && formatOptions.dateFormat != null)
                                htmlStr += "\t\t\t<td>" + convertValue((DateTime)info.GetValue(item), formatOptions.dateFormat) + "</td>\n";
                            else 
                                htmlStr += "\t\t\t<td>" + info.GetValue(item).ToString().TrimEnd() + "</td>\n";
                        } else
                            htmlStr += "\t\t\t<td>&nbsp;</td>\n";
                    }
                }
                htmlStr += "\t\t</tr>\n";
            }
            htmlStr += "\t</body>\n";
            htmlStr += "<table>\n";

            return htmlStr;
        }

        public static string ConvertDataTableToHTMLTable(DataTable dt, string cssClass = "") {
            string html = "<table " + ((cssClass == "") ? ">" : " class=\"" + cssClass + "\">\n");

            //add header row
            html += "<thead><tr>\n";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<th>" + dt.Columns[i].ColumnName + "</th>";
            html += "</tr></thead>\n";

            html += "<tbody>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++) {
                html += "<tr>\n";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>\n";
            }
            html += "</tbody>";
            html += "</table>";
            return html;
        }
    }
}
