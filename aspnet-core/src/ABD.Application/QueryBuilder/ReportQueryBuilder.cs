using System;
using System.Collections.Generic;
using System.Text;
using ABD.ADOrders.Dto;

namespace ABD.QueryBuilder
{
    public static class ReportQueryBuilder
    {
        private static ADOrderDto AdOrder;

        public static string GetAgecyReportQuery(ADOrderDto input)
        {
            AdOrder = input;
            var query = AdOrder.QueryCriteria;
            query = query.Trim().Replace(
                "SELECT * FROM ContactsDB INNER JOIN AgencyDB ON ContactsDB.ACCOUNTID = AgencyDB.ACCOUNTID",
                "SELECT Account,Address1,Address2,City,State,PostalCode,County,TimeZone,Country,Division,MainPhone,PhoneExtension,Fax,TollFree,WebAddress,Email,Type,Revenue,PercentComm,Employees,SpecialAffiliation,AgencyManagement,LastName,FirstName,Suffix,Title,Mobile,CEmail,AccountId,SpLines,TitleSearch,LineSearch,Msa,PremiumVolume,DunsNum,CEmail2,LinkedUrl,BranchIndicator,Num_Locations,TwitterUrl,FacebookUrl,GoogleplusUrl,YoutubeUrl,Linkedin,CountyCode FROM AgencyDB");

            query = CheckForContactQuery(query);
            query = query + " ORDER BY AgencyDB.ACCOUNT";
            return query;
        }

        public static string GetContactReportQuery(ADOrderDto input)
        {
            AdOrder = input;
            var query = AdOrder.QueryCriteria;
            query = query.Replace("*", "ContactsDB.*");
            query = query + " ORDER BY ContactsDB.ACCOUNT";
            return query;
        }

        public static string GetMapQuery(string query)
        {
            //query = query.Replace("*", "TOP (500) *");
            query = query + " AND AGENCYDB.Latitude is not null";
            return query;
        }

        public static string GetReportQuery(ADOrderDto input, ReportType reportType)
        {
            var query = input.QueryCriteria;
            query = GetModifiedQuery(input.IsCtPurchased, query);

            if (reportType == ReportType.CarriersReport)
            {
                query = "SELECT *  FROM ClinesDB WHERE ACCOUNTID IN (" + query + ") ORDER BY ACCOUNTID";
            }
            else if (reportType == ReportType.SicCodesReport)
            {
                query = "SELECT *  FROM TargetSectorsDB WHERE ACCOUNTID IN (" + query + ") ORDER BY ACCOUNTID";
            }
            else if (reportType == ReportType.AffliationsReport)
            {
                query = "SELECT *  FROM SplAffDB WHERE ACCOUNTID IN (" + query + ") ORDER BY ACCOUNTID";
            }

            return query;
        }

        public static string GetSicCodesReportQuery(ADOrderDto input)
        {
            var query = input.QueryCriteria;
            query = GetModifiedQuery(input.IsCtPurchased, query);
            var carriersQuery = "SELECT *  FROM ClinesDB WHERE ACCOUNTID IN (" + query + ") ORDER BY ACCOUNTID";

            return carriersQuery;
        }

        private static string GetModifiedQuery(bool isCtPurchased, string query)
        {
            if (isCtPurchased)
            {
                query = query.Replace("*", "ContactsDB.ACCOUNTID");
            }
            else
            {
                query = query.Replace("*", "ACCOUNTID");
            }

            if (query.IndexOf("ACCOUNTID FROM ContactsDB INNER JOIN AgencyDB", StringComparison.Ordinal) == 7)
            {
                query = query.Replace("SELECT ACCOUNTID FROM ContactsDB", "SELECT ContactsDB.ACCOUNTID FROM ContactsDB");
            }

            return query;
        }

        private static string CheckForContactQuery(string query)
        {
            if (!string.IsNullOrEmpty(AdOrder.TitleSearch))
            {
                string titleSearchQuery = GetTitleSearchQuery();
                query = query.Replace("AND " + titleSearchQuery, "");
            }

            if (!string.IsNullOrEmpty(AdOrder.LinesSearch))
            {
                string lineseSearchQuery = GetLinesSearchQuery();
                query = query.Replace("AND " + lineseSearchQuery, "");
            }

            return query;
        }

        private static string GetLinesSearchQuery()
        {
            StringBuilder stb = new StringBuilder();
            SubqueryBuilder(AdOrder.LinesSearch, "ContactsDB.LINESEARCH", "OR", "=", ref stb);

            if ((stb.ToString().Trim() != ""))
            {
                if (AdOrder.LinesSearchCriteria.Trim() == "INCLUDE")
                {
                    return ("(" + (stb.ToString() + ")"));
                }
                else if (AdOrder.LinesSearchCriteria.Trim() == "EXCLUDE")
                {
                    return ("(NOT (" + (stb.ToString() + "))"));
                }
            }
            return stb.ToString();
        }

        private static string GetTitleSearchQuery()
        {
            StringBuilder stb = new StringBuilder();
            SubqueryBuilder(AdOrder.TitleSearch, "ContactsDB.TITLESEARCH", "OR", "=", ref stb);

            if ((stb.ToString().Trim() != ""))
            {
                if (AdOrder.TitleSearchCriteria.Trim() == "INCLUDE")
                {
                    return ("(" + (stb.ToString() + ")"));
                }
                else if (AdOrder.TitleSearchCriteria.Trim() == "EXCLUDE")
                {
                    return ("(NOT (" + (stb.ToString() + "))"));
                }
            }

            return stb.ToString();
        }


        private static void SubqueryBuilder(string strList, string columnName, string condition, string operators, ref StringBuilder stb)
        {
            string[] strArray = strList.Split(",");
            int i;
            string sValue;
            for (i = 0; (i <= (strArray.Length - 1)); i++)
            {
                sValue = "";
                sValue = strArray[i];
                if ((stb.ToString().Trim() == ""))
                {
                    stb.Append(columnName + operators + "\'" + sValue.Trim() + "\'");
                }
                else
                {
                    stb.Append(" " + condition + " " + columnName + operators + "\'" + sValue.Trim() + "\'");
                }
            }
        }
    }
}
