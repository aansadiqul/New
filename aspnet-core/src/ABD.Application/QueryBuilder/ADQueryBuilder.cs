using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Extensions;
using ABD.ADSearches.Dto;
using ABD.Domain.Dtos;

namespace ABD
{
    public static class ADQueryBuilder
    {
        private static ADSearchInput SearchInput;

        public static ADQueries GetADSearchQueries(ADSearchInput input)
        {
            SearchInput = input;
            var queries = new ADQueries();
            try
            {
                var agencyQuery = BuildSQL(true);
                queries.AgencyQuery = agencyQuery;
                queries.ADCountQuery = GetRecordCountQuery(agencyQuery);
                var sqlQuery = BuildSQL(false);
                queries.SQLQuery = sqlQuery.Trim().Replace("SELECT * FROM AgencyDB",
                    "SELECT * FROM ContactsDB INNER JOIN AgencyDB ON ContactsDB.ACCOUNTID = AgencyDB.ACCOUNTID");
                queries.ADContactsCountQuery = GetContactsCountQuery(sqlQuery,
                    "SELECT count(ContactsDB.Account) FROM ContactsDB INNER JOIN AgencyDB ON ContactsDB.ACCOUNTID = AgencyDB.ACCOUNTID");
                queries.ADEmailCountQuery = GetEmailContactsCountQuery(sqlQuery,
                    "SELECT count(ContactsDB.Account) FROM ContactsDB INNER JOIN AgencyDB ON ContactsDB.ACCOUNTID = AgencyDB.ACCOUNTID");

                return queries;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static string GetAdNamesQuery(string query)
        {

            query = query.Replace("SELECT * FROM AgencyDB", "SELECT AgencyDB.ACCOUNT,AgencyDB.State FROM AGENCYDB ");
            query = query.Replace("SELECT * FROM ContactsDB INNER JOIN AgencyDB ON ContactsDB.ACCOUNTID = AgencyDB.ACCOUNTID", "SELECT AgencyDB.ACCOUNT,AgencyDB.State FROM AGENCYDB ");
            query = query + " ORDER BY AgencyDB.ACCOUNT";
            return query;

        }

        public static string GetAdAnalyzeQuery(AnalyzeInput input)
        {
            string firstValue = input.FirstValue;
            string secondValue = input.SecondValue;
            var strQuery = input.AgencyQuery;
            string selectStatement = "";
            string strNewQuery = "";
            if (((firstValue != "") && (secondValue == "")))
            {
                selectStatement = firstValue;
            }

            if (((firstValue == "") && (secondValue != "")))
            {
                selectStatement = secondValue;
            }

            if (((firstValue != "") && (secondValue != "")))
            {
                selectStatement = (firstValue + ("," + secondValue));
            }

            string strSicColumn = "";
            if ((selectStatement != ""))
            {
                selectStatement = selectStatement.Replace("POSTALCODE", "POSTALCODE AS ZIPCODE");
                var groupStatement = selectStatement;
                groupStatement = groupStatement.Replace("POSTALCODE AS ZIPCODE", "POSTALCODE");
                if ((selectStatement.IndexOf("(SICCODE,1,2)") > -1))
                {
                    selectStatement = selectStatement.Replace("(SICCODE,1,2)", "(SICCODE,1,2) as [Two_Digit_SIC] ");
                    strSicColumn = "Two_Digit_SIC";
                }
                else if ((selectStatement.IndexOf("(SICCODE,1,4)") > -1))
                {
                    selectStatement = selectStatement.Replace("(SICCODE,1,4)", "(SICCODE,1,4) as [Four_Digit_SIC] ");
                    strSicColumn = "Four_Digit_SIC";
                }
                else if ((selectStatement.IndexOf("(SICCODE,1,6)") > -1))
                {
                    selectStatement = selectStatement.Replace("(SICCODE,1,6)", "(SICCODE,1,6) as [Six_Digit_SIC] ");
                    strSicColumn = "Six_Digit_SIC";
                }
                else if ((selectStatement.IndexOf("SICCODE") > -1))
                {
                    selectStatement = selectStatement.Replace("SICCODE", "SICCODE as [Eight_Digit_SIC] ");
                    strSicColumn = "Eight_Digit_SIC";
                }

                if ((selectStatement.IndexOf("SICCODE") > -1))
                {
                    var strReplaceby = (selectStatement + ",Count(*) as Records, Description FROM AgencyDB  INNER JOIN TargetSectorsDB  ON AgencyDB.ACCOUNTID = TargetSectorsDB.ACCOUNTID ");
                    strNewQuery = strQuery.Replace("* FROM AgencyDB ", strReplaceby);
                }
                else
                {
                    strNewQuery = strQuery.Replace("* ", (selectStatement + ",Count(*) as Records "));
                }

                if ((input.FirstValue.Contains("SIC") || input.SecondValue.Contains("SIC")))
                {
                    if (!string.IsNullOrEmpty(input.SicCodes))
                    {
                        strNewQuery = (strNewQuery + ("And"
                                    + (BuildSICQuery(input.SicCodes) + (" GROUP BY "
                                    + (groupStatement + ", Description" + (" ORDER BY " + groupStatement + ", Description"))))));
                    }
                    else
                    {
                        strNewQuery = (strNewQuery + (" GROUP BY "
                                      + (groupStatement + ", Description" + (" ORDER BY " + groupStatement + ", Description"))));
                    }

                }
                else
                {
                    strNewQuery = (strNewQuery + (" GROUP BY "
                                + (groupStatement + (" ORDER BY " + groupStatement))));
                }
            }
            return strNewQuery;
        }

        private static string GetEmailContactsCountQuery(string strQueryAgency, string strQuery)
        {
            strQueryAgency = strQueryAgency.Replace("*", "Count(Account)");
            strQuery = strQueryAgency.Replace("SELECT Count(Account) FROM AgencyDB", strQuery);
            strQuery = (strQuery + " AND NOT ContactsDB.CEMAIL IS NULL");
            return strQuery;
        }

        private static string GetContactsCountQuery(string strQueryAgency, string strQuery)
        {
            strQueryAgency = strQueryAgency.Replace("*", "Count(Account)");
            strQuery = strQueryAgency.Replace("SELECT Count(Account) FROM AgencyDB", strQuery);
            return strQuery;
        }

        private static string GetRecordCountQuery(string strQuery)
        {
            strQuery = strQuery.Replace("*", "Count(Account)");
            return strQuery;
        }

        private static string BuildSQL(bool Agency)
        {
            var test = SearchInput.QueryName;
            StringBuilder StMain = new StringBuilder();
            StringBuilder StQry = new StringBuilder();
            string Str;
            StMain.Append("SELECT * FROM AgencyDB ");
            // Get the Type Query (Agency,National Broker ect...)
            Str = "";
            Str = GetTypeQuery();
            AppendCondition(ref Str, ref StQry);

            // Get the Additional Query Premium Volume & Employee Size
            Str = GetAdditionalQuery();
            AppendCondition(ref Str, ref StQry);

            // Get the AgencyManagement Query
            Str = GetAgencyManagementQuery();
            AppendCondition(ref Str, ref StQry);

            //search by company name search
            Str = GetCompanySearchQuery();
            AppendCondition(ref Str, ref StQry);

            // Get the Country Query
            Str = GetCountryQuery();
            AppendCondition(ref Str, ref StQry);

            // Get the State Query
            Str = GetGeographicQuery();
            AppendCondition(ref Str, ref StQry);

            // Get the Contacts Title Search Query
            if (!Agency)
            {
                Str = GetTitleSearchQuery();
                AppendCondition(ref Str, ref StQry);

                // Get the Contacts Lines Search Query
                Str = GetLinesSearchQuery();
                AppendCondition(ref Str, ref StQry);
            }

            // Get the Companylines Query
            Str = GetCompaniesQuery();
            AppendCondition(ref Str, ref StQry);

            // Get the SpecialAffiliations Query
            Str = GetAffiliationsQuery();
            AppendCondition(ref Str, ref StQry);

            // Get the TargetSector(SICCODES) Query
            Str = BuildSICQuery(SearchInput.SICCodes);
            AppendCondition(ref Str, ref StQry);

            StMain.Append(StQry.ToString());
            return StMain.ToString();
        }

        private static void AppendCondition(ref string Str, ref StringBuilder StQry)
        {
            if ((Str.Trim() != ""))
            {
                if (string.IsNullOrEmpty(StQry.ToString()))
                {
                    StQry.Append((" WHERE " + Str));
                }
                else
                {
                    StQry.Append((" AND " + Str));
                }

                Str = "";
            }
        }

        private static string GetGeographicQuery()
        {
            StringBuilder stqry = new StringBuilder();
            string strStates = GetStatesQuery();
            string strExcludeStates = GetExcludeStatesQuery();
            if (((strStates != "") && (strExcludeStates != "")))
            {
                strStates = ("  ("
                            + (strStates + (" AND "
                            + (strExcludeStates + " ) "))));
            }
            else if (((strStates == "")
                        && (strExcludeStates != "")))
            {
                strStates = strExcludeStates;
            }

            string strCounties = GetCOUNTIESQuery();
            string strExcludeCounties = GetExcludeCountiesQuery();
            if (((strCounties != "") && (strExcludeCounties != "")))
            {
                if ((strStates == ""))
                {
                    strCounties = (" (("
                                + (strCounties + (" OR "
                                + (strExcludeCounties + " ) "))));
                }
                else
                {
                    strCounties = (" AND (("
                                + (strCounties + (" OR "
                                + (strExcludeCounties + " ) "))));
                }

            }
            else if (((strCounties == "")
                        && (strExcludeCounties != "")))
            {
                strCounties = (" AND (" + strExcludeCounties);
            }
            else if (((strCounties != "")
                        && (strExcludeCounties == "")))
            {
                if ((strStates == ""))
                {
                    strCounties = (" ( " + strCounties);
                }
                else
                {
                    strCounties = (" OR ( " + strCounties);
                }

            }

            string strZips = GetZipQuery();
            string strExcludeZips = GetExcludeZipQuery();
            if (((strZips != "") && (strExcludeZips != "")))
            {
                if ((strCounties != ""))
                {
                    strZips = (" OR ("
                                + (strZips + (" AND "
                                + (strExcludeZips + " ) "))));
                }
                else
                {
                    strZips = (" AND ( ("
                                + (strZips + (" AND "
                                + (strExcludeZips + " ) "))));
                }

            }
            else if (((strZips == "")
                        && (strExcludeZips != "")))
            {
                if ((strCounties != ""))
                {
                    strZips = (" AND " + strExcludeZips);
                }
                else
                {
                    strZips = (" AND ( " + strExcludeZips);
                }

            }
            else if (((strZips != "")
                        && (strExcludeZips == "")))
            {
                if ((strCounties != ""))
                {
                    strZips = (" OR " + strZips);
                }
                else
                {
                    strZips = (" And ( " + strZips);
                }

            }

            string strMSAs = GetMSAQuery();
            string strExcludeMSAs = GetExcludeMSAQuery();
            if (((strMSAs != "")
                        && (strExcludeMSAs != "")))
            {
                if (((strCounties == "")
                            && (strZips == "")))
                {
                    strMSAs = (" AND ( ("
                                + (strMSAs + (" AND "
                                + (strExcludeMSAs + " ) "))));
                }
                else
                {
                    strMSAs = (" OR ("
                                + (strMSAs + (" AND "
                                + (strExcludeMSAs + " ) "))));
                }

            }
            else if (((strMSAs == "")
                        && (strExcludeMSAs != "")))
            {
                if (((strCounties == "")
                            && (strZips == "")))
                {
                    strMSAs = (" AND ( " + strExcludeMSAs);
                }
                else
                {
                    strMSAs = (" AND " + strExcludeMSAs);
                }

            }
            else if (((strMSAs != "")
                        && (strExcludeMSAs == "")))
            {
                if (((strCounties == "")
                            && (strZips == "")))
                {
                    strMSAs = (" AND ( " + strMSAs);
                }
                else
                {
                    strMSAs = (" OR " + strMSAs);
                }

            }

            if ((strStates != ""))
            {
                stqry.Append(strStates);
            }

            if ((strCounties != ""))
            {
                stqry.Append(strCounties);
            }

            if ((strZips != ""))
            {
                stqry.Append(strZips);
            }

            if ((strMSAs != ""))
            {
                stqry.Append(strMSAs);
            }

            // Closing Parenthesis (Because of OR Conditions)
            if (((strCounties != "")
                        || ((strZips != "")
                        || (strMSAs != ""))))
            {
                stqry.Append(" )");
            }

            return stqry.ToString();
        }

        private static string GetExcludeMSAQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.ExcludeMSA))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.ExcludeMSA.Trim(), "AgencyDB.MSA", "AND", "<>", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("(" + (stb.ToString() + ")"));
                }
            }
            return "";
        }

        private static string GetMSAQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.MSA))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.MSA, "AgencyDB.MSA", "OR", "=", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("("
                            + (stb.ToString() + ")"));
                }
                else
                {
                    return "";
                }

            }
            else
            {
                return "";
            }

        }

        private static void BuildZipSubquery(string strList, string condition, string operators, ref StringBuilder stb)
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
                    if ((sValue.Length > 3))
                    {
                        stb.Append(("AGENCYDB.POSTALCODE " + operators + " like \'"
                                    + (sValue + "%\'")));
                    }
                    else if ((sValue.Length == 3))
                    {
                        stb.Append(("AGENCYDB.POSTALCODE " + operators + " like \'"
                                    + (sValue + "%\'")));
                    }

                }
                else if ((sValue.Length > 3))
                {
                    stb.Append((" " + condition + " AGENCYDB.POSTALCODE " + operators + " like \'"
                                + (sValue + "%\'")));
                }
                else if ((sValue.Length == 3))
                {
                    stb.Append((" " + condition + " AGENCYDB.POSTALCODE " + operators + " like \'"
                                + (sValue + "%\'")));
                }

            }
        }

        private static string GetExcludeZipQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.ExcludeZip))
            {
                StringBuilder stb = new StringBuilder();
                BuildZipSubquery(SearchInput.ExcludeZip.Trim(), "AND", "NOT", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("(" + (stb.ToString() + ")"));
                }
            }
            return "";
        }

        private static string GetZipQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Zip))
            {
                StringBuilder stb = new StringBuilder();
                BuildZipSubquery(SearchInput.Zip.Trim(), "OR", string.Empty, ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("(" + (stb.ToString() + ")"));
                }
            }
            return "";
        }

        private static string GetExcludeCountiesQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.ExcludeCountyIds))
            {
                if (!string.IsNullOrEmpty(SearchInput.CountyIDs))
                {
                    StringBuilder stb = new StringBuilder();
                    SubqueryBuilder(SearchInput.IncludeCountyIds, "AgencyDB.COUNTYCODE", "OR", "=", ref stb);
                    if ((stb.ToString().Trim() != ""))
                    {
                        return ("(" + (stb.ToString() + ")"));
                    }
                }
                else
                {
                    string strList = "";
                    strList = SearchInput.ExcludeCountyIds;
                    string[] strExcludeArray = strList.Split(",");
                    int i;
                    string sValue;
                    StringBuilder stb = new StringBuilder();
                    for (i = 0; (i <= (strExcludeArray.Length - 1)); i++)
                    {
                        sValue = "";
                        sValue = strExcludeArray[i];
                        if ((stb.ToString().Trim() == ""))
                        {
                            stb.Append(("AgencyDB.COUNTYCODE <>\'" + (sValue + "\'")));
                        }
                        else
                        {
                            stb.Append((" AND AgencyDB.COUNTYCODE <>\'" + (sValue + "\'")));
                        }

                    }

                    if ((stb.ToString().Trim() != ""))
                    {
                        return ("(" + (stb.ToString() + ")"));
                    }
                }

            }
            return "";
        }

        public static string GetAllExcludecounties(string excludeCountyIds)
        {
            StringBuilder stb = new StringBuilder();
            string ST2ExcludeCountyIDS = "";
            string strquery = "SELECT STRING_AGG(CountyCode, \', \')  FROM RS_Counties  WHERE (State IN ";
            if (!string.IsNullOrEmpty(excludeCountyIds))
            {
                string strListstate = "";
                strListstate = excludeCountyIds;
                string strvalue = "";
                string[] strExcludeArray = strListstate.Split(",");
                for (int i = 0; (i <= (strExcludeArray.Length - 1)); i++)
                {
                    strvalue = ("\'"
                                + (strExcludeArray[i].Substring(0, 2) + ("\'" + ("," + strvalue))));
                }

                stb.Append("(" + strvalue.Trim(',') + "" + "))");
                string strexcludecounty = "";
                strexcludecounty = excludeCountyIds;
                string[] strExcludeCounties = strexcludecounty.Split(",");
                for (int i = 0; (i <= (strExcludeCounties.Length - 1)); i++)
                {
                    if ((stb.ToString().Trim() == ""))
                    {
                        stb.Append((" AND (" + ("COUNTYCODE <>\'"
                                        + (strExcludeCounties[i] + ("\'" + ")")))));
                    }
                    else
                    {
                        stb.Append((" AND (" + ("COUNTYCODE <>\'"
                                        + (strExcludeCounties[i] + ("\'" + ")")))));
                    }

                }

                strquery = (strquery + stb.ToString());
                return strquery;
            }
            return "";
        }

        private static string GetCOUNTIESQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.CountyIDs))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.CountyIDs, "AgencyDB.COUNTYCODE", "OR", "=", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("("
                            + (stb.ToString() + ")"));
                }
            }
            return "";
        }

        private static string GetExcludeStatesQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.ExcludeState))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.ExcludeState, "AgencyDB.STATE", "AND", "<>", ref stb);
                if ((stb.ToString().Trim() != ""))
                {
                    return ("(" + (stb.ToString() + ")"));
                }
            }
            return "";
        }

        private static string GetStatesQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.State))
            {
                string strList = "";
                ArrayList arrLStates = new ArrayList();
                int i;
                if ((!string.IsNullOrEmpty(SearchInput.CountyIDs)) && (!string.IsNullOrEmpty(SearchInput.ExcludeCountyIds)))
                {
                    return "";
                }
                else
                {
                    if (!string.IsNullOrEmpty(SearchInput.CountyIDs))
                    {
                        strList = SearchInput.CountyIDs;
                        string[] strStatesSelected = strList.Split(",");
                        try
                        {
                            for (i = 0; (i <= (strStatesSelected.Length - 1)); i++)
                            {
                                if ((arrLStates.Contains(strStatesSelected[i].Substring(0, 2)) == false))
                                {
                                    arrLStates.Add(strStatesSelected[i].Substring(0, 2));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                    }

                    strList = SearchInput.State;
                    string[] strArray = strList.Split(",");
                    string sValue;
                    StringBuilder stb = new StringBuilder();
                    for (i = 0; (i <= (strArray.Length - 1)); i++)
                    {
                        sValue = "";
                        sValue = strArray[i];
                        if ((arrLStates.Contains(sValue) == false))
                        {
                            if ((stb.ToString().Trim() == ""))
                            {
                                stb.Append(("AgencyDB.STATE=\'"
                                                + (sValue + "\'")));
                            }
                            else
                            {
                                stb.Append((" OR AgencyDB.STATE=\'"
                                                + (sValue + "\'")));
                            }
                        }
                    }

                    if ((stb.ToString().Trim() != ""))
                    {
                        return ("(" + (stb.ToString() + ")"));
                    }
                }

            }
            return "";
        }

        private static string GetCountryQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Country))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.Country, "AgencyDB.COUNTRY", "OR", "=", ref stb);
                if ((stb.ToString().Trim() != ""))
                {
                    return ("(" + (stb.ToString() + ")"));
                }
            }

            return "";
        }

        private static string GetCompanySearchQuery()
        {
            StringBuilder strBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(SearchInput.CompanyName))
            {
                string strCompanyName = SearchInput.CompanyName;
                if ((SearchInput.CompanyNameType.ToUpper().Trim() == "CONTAINS"))
                {
                    if (strBuilder.ToString() == "")
                    {
                        strBuilder.Append(("AgencyDB.ACCOUNT LIKE \'%"
                                           + (strCompanyName + "%\'")));
                    }

                }
                else if (SearchInput.CompanyNameType.ToUpper().Trim() == "BEGINS")
                {
                    if ((strBuilder.ToString().Trim() == ""))
                    {
                        strBuilder.Append(("AgencyDB.ACCOUNT LIKE \'"
                                           + (strCompanyName + "%\'")));
                    }

                }

            }

            return strBuilder.ToString();
        }

        private static string GetAgencyManagementQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.AgencyManagement))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.AgencyManagement, "AgencyDB.AGENCYMANAGEMENT", "OR", "=", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    if (SearchInput.AgencyMgntCriteria.Trim() == "INCLUDE")
                    {
                        return ("(" + (stb.ToString() + ")"));
                    }
                    else if (SearchInput.AgencyMgntCriteria.Trim() == "EXCLUDE")
                    {
                        return ("(NOT (" + (stb.ToString() + "))"));
                    }

                }
            }
            return "";
        }


        private static string GetCompaniesQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.CompanyLines))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.CompanyLines.Trim(), "ClinesDB.COMPANYLINE", "OR", "=", ref stb);

                StringBuilder StFinal = new StringBuilder();
                if ((stb.ToString() != ""))
                {
                    StFinal.Append("(AgencyDB.ACCOUNTID IN (SELECT ACCOUNTID FROM ClinesDB WHERE ");

                    //StFinal.Append((stb.ToString() + "))"));

                    if (SearchInput.CarrierManageCrieteria.Trim() == "INCLUDE")
                    {
                        return StFinal.Append("(" + stb.ToString() + ")))").ToString();
                    }
                    else if (SearchInput.CarrierManageCrieteria.Trim() == "EXCLUDE")
                    {
                        return StFinal.Append("NOT (" + stb.ToString() + ")))").ToString();
                    }
                    return StFinal.ToString();
                }

                else
                {
                    return StFinal.ToString();
                    //done with me
                }
            }
            return "";
        }




        private static string GetAdditionalQuery()
        {
            StringBuilder St = new StringBuilder();
            string sValue1;
            string sValue2;
            string sValue3;
            string strCondition1;
            string strCondition2;
            strCondition1 = SearchInput.PEmpCriteria;
            strCondition2 = SearchInput.RevenueCriteria;
            sValue1 = GetVolumeQuery(SearchInput.PVolume, "AgencyDB.PREMIUMVOLUME");
            sValue2 = GetVolumeQuery(SearchInput.RevenueValue, "AgencyDB.REVENUE");
            sValue3 = GetEmployeeSizeQuery();
            if (((sValue1 != "") && ((sValue2 != "") && (sValue3 != ""))))
            {
                // 123
                St.Append(("(("
                                + (sValue1 + (") "
                                + (strCondition1 + (" ( "
                                + (sValue2 + (") "
                                + (strCondition2 + (" ("
                                + (sValue3 + "))")))))))))));
            }
            else if (((sValue1 != "")
                        && ((sValue2 != "")
                        && (sValue3 == ""))))
            {
                // 12
                St.Append(("(("
                                + (sValue1 + (")  "
                                + (strCondition1 + (" ( "
                                + (sValue2 + "))")))))));
            }
            else if ((sValue1 == "") && (sValue2 != "") && (sValue3 != ""))
            {
                // 23
                St.Append("((" + sValue2 + ") " + strCondition2 + " ( " + sValue3 + "))");
            }
            else if (((sValue1 != "")
                        && ((sValue2 == "")
                        && (sValue3 != ""))))
            {
                // 13
                St.Append("((" + sValue1 + ") " + strCondition1 + " ( " + sValue3 + "))");
            }
            else if (((sValue1 != "")
                        && ((sValue2 == "")
                        && (sValue3 == ""))))
            {
                // 1
                St.Append(("("
                                + (sValue1 + ")")));
            }
            else if (((sValue1 == "")
                        && ((sValue2 != "")
                        && (sValue3 == ""))))
            {
                // 2
                St.Append(("("
                                + (sValue2 + ")")));
            }
            else if (((sValue1 == "")
                        && ((sValue2 == "")
                        && (sValue3 != ""))))
            {
                // 3
                St.Append(("("
                                + (sValue3 + ")")));
            }

            return St.ToString();
        }

        private static string GetEmployeeSizeQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.EmpSize))
            {
                var st1 = '-';
                string strEMPSize = SearchInput.EmpSize;
                string[] strEMP = strEMPSize.Split(st1);
                string strEMPTO = "";
                string strEMPFROM = "";
                int i;
                for (i = 0; (i <= (strEMP.Length - 1)); i++)
                {
                    if ((i == 0))
                    {
                        strEMPFROM = strEMP[i];
                    }

                    if ((i == 1))
                    {
                        strEMPTO = strEMP[i];
                    }
                }
                StringBuilder St = new StringBuilder();
                St.Append("AgencyDB.EMPLOYEES ");
                if (((strEMPFROM.Trim() != "") && (strEMPTO.Trim() != "")))
                {
                    // From and To
                    St.Append("BETWEEN ");
                    St.Append(strEMPFROM);
                    St.Append(" AND ");
                    St.Append(strEMPTO);
                    return St.ToString();
                }
                else if (((strEMPTO.Trim() == "")
                          && (strEMPFROM.Trim() != "")))
                {
                    // >=From
                    St.Append((">=" + strEMPFROM));
                    return St.ToString();
                }
                else
                {
                    return "";
                }

            }
            else
            {
                return "";
            }
        }

        private static string GetTypeQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.TypeField))
            {
                string st1;
                st1 = ",";
                string strTypeField = SearchInput.TypeField;
                string[] strArray = strTypeField.Split(st1);
                int i;
                string sValue;
                StringBuilder St = new StringBuilder();
                for (i = 0; i <= strArray.Length - 1; i++)
                {
                    sValue = "";
                    sValue = strArray[i];

                    if (SearchInput.TypeCriteria == "CONTAINS")
                    {
                        if (string.IsNullOrEmpty(St.ToString()))
                        {
                            St.Append("AgencyDB.TYPE LIKE \'%" + sValue + "%\'");
                        }
                        else
                        {
                            St.Append(" OR AgencyDB.TYPE LIKE \'%" + sValue + "%\'");
                        }

                    }
                    else if ((SearchInput.TypeCriteria == "EXACT MATCH"))
                    {
                        if (string.IsNullOrEmpty(St.ToString()))
                        {
                            St.Append("AgencyDB.TYPE = \'" + sValue + "\'");
                        }
                        else
                        {
                            St.Append(" OR AgencyDB.TYPE = \'" + sValue + "\'");
                        }
                    }
                    else if ((SearchInput.TypeCriteria == "STARTS WITH"))
                    {
                        if (string.IsNullOrEmpty(St.ToString()))
                        {
                            St.Append("AgencyDB.TYPE LIKE \'" + sValue + "%\'");
                        }
                        else
                        {
                            St.Append(" OR AgencyDB.TYPE LIKE \'" + sValue + "%\'");
                        }
                    }
                }

                if (SearchInput.isRetail == true)
                {
                    St.Append(" and AgencyDB.TYPE not in (select Name from CompanyTypes where IsWholesale = 1 and(IsRetail = 0 or IsRetail is null))");
                }
                else if (SearchInput.isWholesale == true)
                {
                    St.Append(" and AgencyDB.TYPE not in (select Name from CompanyTypes where IsRetail = 1 and(IsWholesale = 0 or IsWholesale is null))");
                }

                string strTypeQuery = St.ToString();
                if ((strTypeQuery.Trim() != ""))
                {
                    return ("(" + (strTypeQuery + ")"));
                }
                else
                {
                    return "";
                }

            }
            else
            {
                return "";
            }
        }

        private static string GetVolumeQuery(string value, string table)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var st1 = '-';
                string strVolume = value;
                string[] strValues = strVolume.Split(st1);
                string strvTo = "";
                string strvFrom = "";
                int i;
                for (i = 0; (i <= (strValues.Length - 1)); i++)
                {
                    var amount = (double.Parse(strValues[i]) * 1000000).ToString();
                    if ((i == 0))
                    {
                        strvFrom = amount;
                    }

                    if ((i == 1))
                    {
                        strvTo = amount;
                    }

                }

                StringBuilder St = new StringBuilder();
                St.Append(table + " ");
                double vFrom = 0;
                double vTo = 0;
                if (((strvFrom.Trim() != "") && (strvTo.Trim() != "")))
                {
                    vFrom = double.Parse(strvFrom);
                    vTo = double.Parse(strvTo);
                    if (((vFrom > 0) && (vTo <= 0)))
                    {
                        St.Append((">=" + vFrom));
                        return St.ToString();
                    }
                    else if (((vTo > 0) && (vFrom <= 0)))
                    {
                        St.Append(("<=" + vTo));
                    }
                    else
                    {
                        St.Append("BETWEEN ");
                        St.Append(vFrom);
                        St.Append(" AND ");
                        St.Append(vTo);
                    }

                    return St.ToString();
                }
                else if (((strvTo.Trim() == "") && (strvFrom.Trim() != "")))
                {
                    // >=vFrom
                    vFrom = double.Parse(strvFrom);
                    St.Append((">=" + vFrom));
                    return St.ToString();
                }
                else
                {
                    return "";
                }

            }
            else
            {
                return "";
            }
        }

        private static string GetTitleSearchQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.TitleSearch))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.TitleSearch, "ContactsDB.TITLESEARCH", "OR", "=", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    if (SearchInput.TitleSearchCriteria.Trim() == "INCLUDE")
                    {
                        return ("(" + (stb.ToString() + ")"));
                    }
                    else if (SearchInput.TitleSearchCriteria.Trim() == "EXCLUDE")
                    {
                        return ("(NOT (" + (stb.ToString() + "))"));
                    }
                }
            }
            return "";
        }

        private static string GetLinesSearchQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.LinesSearch))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.LinesSearch, "ContactsDB.LINESEARCH", "OR", "=", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    if (SearchInput.LinesSearchCriteria.Trim() == "INCLUDE")
                    {
                        return ("(" + (stb.ToString() + ")"));
                    }
                    else if (SearchInput.LinesSearchCriteria.Trim() == "EXCLUDE")
                    {
                        return ("(NOT (" + (stb.ToString() + "))"));
                    }

                }
            }
            return "";
        }

       
        private static string GetAffiliationsQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Affiliations))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.Affiliations, "SPECIALAFFILIATION", "OR", "=", ref stb);

                StringBuilder StFinal = new StringBuilder();
                if ((stb.ToString() != ""))
                {
                    StFinal.Append("(AgencyDB.ACCOUNTID IN (SELECT ACCOUNTID FROM SplAffDB WHERE ");
                    StFinal.Append((stb.ToString() + "))"));
                    return StFinal.ToString();
                }
            }
            return "";
        }

        private static string BuildSICQuery(string sicCodes)
        {
            StringBuilder Sb = new StringBuilder();
            if (!string.IsNullOrEmpty(sicCodes))
            {
                string strList = sicCodes;
                string[] stArray = strList.Split(",");
                int i;
                string sValue = "";
                for (i = 0; (i <= (stArray.Length - 1)); i++)
                {
                    sValue = "";
                    sValue = stArray[i];
                    if ((sValue.Trim() != ""))
                    {
                        if ((Sb.ToString() == ""))
                        {
                            if (((sValue.Trim() == "A")
                                        || ((sValue.Trim() == "B")
                                        || ((sValue.Trim() == "C")
                                        || ((sValue.Trim() == "D")
                                        || ((sValue.Trim() == "E")
                                        || ((sValue.Trim() == "F")
                                        || ((sValue.Trim() == "G")
                                        || ((sValue.Trim() == "H")
                                        || ((sValue.Trim() == "I")
                                        || ((sValue.Trim() == "J")
                                        || (sValue.Trim() == "K"))))))))))))
                            {
                                Sb.Append(GetMainClassNumbers(sValue));
                            }
                            else
                            {
                                Sb.Append((" SICCODE LIKE \'" + (sValue.Trim() + "%\'")));
                            }

                        }
                        else if (((sValue.Trim() == "A")
                                    || ((sValue.Trim() == "B")
                                    || ((sValue.Trim() == "C")
                                    || ((sValue.Trim() == "D")
                                    || ((sValue.Trim() == "E")
                                    || ((sValue.Trim() == "F")
                                    || ((sValue.Trim() == "G")
                                    || ((sValue.Trim() == "H")
                                    || ((sValue.Trim() == "I")
                                    || ((sValue.Trim() == "J")
                                    || (sValue.Trim() == "K"))))))))))))
                        {
                            Sb.Append((" OR " + GetMainClassNumbers(sValue)));
                        }
                        else
                        {
                            Sb.Append((" OR SICCODE LIKE \'" + (sValue.Trim() + "%\'")));
                        }

                    }

                }

            }

            if ((Sb.ToString().Trim() != ""))
            {
                string strMain = "(AgencyDB.ACCOUNTID IN (SELECT ACCOUNTID FROM TargetSectorsDB WHERE ";
                strMain = (strMain
                            + (Sb.ToString() + "))"));
                return strMain;
            }
            else
            {
                return "";
            }

        }

        private static string GetMainClassNumbers(string strClass)
        {
            if ((strClass == "A"))
            {
                return " SICCODE LIKE \'01%\' OR SICCODE LIKE \'02%\' OR SICCODE LIKE \'07%\' OR SICCODE LIKE \'08%\' OR SICCODE LIKE" +
                " \'09%\'";
            }
            else if ((strClass == "B"))
            {
                return " SICCODE LIKE \'10%\' OR SICCODE LIKE \'12%\' OR SICCODE LIKE \'13%\' OR SICCODE LIKE \'14%\'";
            }
            else if ((strClass == "C"))
            {
                return " SICCODE LIKE \'15%\' OR SICCODE LIKE \'16%\' OR SICCODE LIKE \'17%\' OR SICCODE LIKE \'18%\' OR SICCODE LIKE" +
                " \'19%\'";
            }
            else if ((strClass == "D"))
            {
                return @" SICCODE LIKE '20%' OR SICCODE LIKE '21%' OR SICCODE LIKE '22%' OR SICCODE LIKE '23%' OR SICCODE LIKE '24%' OR SICCODE LIKE '25%' OR SICCODE LIKE '26%' OR SICCODE LIKE '27%' OR SICCODE LIKE '28%' OR SICCODE LIKE '29%' OR SICCODE LIKE'30%' OR SICCODE LIKE '31%' OR SICCODE LIKE '32%' OR SICCODE LIKE '33%' OR SICCODE LIKE '34%' OR SICCODE LIKE '35%' OR SICCODE LIKE '36%' OR SICCODE LIKE '37%' OR SICCODE LIKE '38%' OR SICCODE LIKE '39%'";
            }
            else if ((strClass == "E"))
            {
                return " SICCODE LIKE \'40%\' OR SICCODE LIKE \'41%\' OR SICCODE LIKE \'42%\' OR SICCODE LIKE \'43%\' OR SICCODE LIKE" +
                " \'44%\' OR SICCODE LIKE \'45%\' OR SICCODE LIKE \'46%\' OR SICCODE LIKE \'47%\' OR SICCODE LIKE \'48%\' OR SI" +
                "CCODE LIKE \'49%\'";
            }
            else if ((strClass == "F"))
            {
                return " SICCODE LIKE \'50%\' OR SICCODE LIKE \'51%\'";
            }
            else if ((strClass == "G"))
            {
                return " SICCODE LIKE \'52%\' OR SICCODE LIKE \'53%\' OR SICCODE LIKE \'54%\' OR SICCODE LIKE \'55%\' OR SICCODE LIKE" +
                " \'56%\' OR SICCODE LIKE \'57%\' OR SICCODE LIKE \'58%\' OR SICCODE LIKE \'59%\'";
            }
            else if ((strClass == "H"))
            {
                return " SICCODE LIKE \'60%\' OR SICCODE LIKE \'61%\' OR SICCODE LIKE \'62%\' OR SICCODE LIKE \'63%\' OR SICCODE LIKE" +
                " \'64%\' OR SICCODE LIKE \'65%\' OR SICCODE LIKE \'66%\' OR SICCODE LIKE \'67%\'";
            }
            else if ((strClass == "I"))
            {
                return @" SICCODE LIKE '70%' OR SICCODE LIKE '71%' OR SICCODE LIKE '72%' OR SICCODE LIKE '73%' OR SICCODE LIKE '74%' OR SICCODE LIKE '75%' OR SICCODE LIKE '76%' OR SICCODE LIKE '77%' OR SICCODE LIKE '78%' OR SICCODE LIKE '79%' OR SICCODE LIKE '80%' OR SICCODE LIKE '81%' OR SICCODE LIKE '82%' OR SICCODE LIKE '83%' OR SICCODE LIKE '84%' OR SICCODE LIKE '85%' OR SICCODE LIKE '86%' OR SICCODE LIKE '87%' OR SICCODE LIKE '88%' OR SICCODE LIKE '89%' OR SICCODE LIKE '90%'";
            }
            else if ((strClass == "J"))
            {
                return " SICCODE LIKE \'91%\' OR SICCODE LIKE \'92%\' OR SICCODE LIKE \'93%\' OR SICCODE LIKE \'94%\' OR SICCODE LIKE" +
                " \'95%\' OR SICCODE LIKE \'96%\' OR SICCODE LIKE \'97%\' OR SICCODE LIKE \'98%\'";
            }
            else if ((strClass == "K"))
            {
                return " SICCODE LIKE \'99%\'";
            }
            else
            {
                return "";
            }
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
