using System;
using System.Collections.Generic;
using System.Text;
using ABD.BDOrders.Dto;
using ABD.BDSearches.Dto;
namespace ABD
{
    public static class BDQueryBuilder
    {
        private static BDSearchInput SearchInput;
        private static string XDatescondition;

        public static BDQueries GetBDSearchQueries(BDSearchInput input)
        {
            SearchInput = input;
            var queries = new BDQueries();
            try
            {
                string strSQL = BuildSQLQuery();
                queries.BusinessQuery = strSQL;
                queries.BDCountQuery = GetRecordCountQuery(strSQL);
                string strQuery = BuildXDatesSQLQuery();
                queries.BDXDateQuery = strQuery;
                queries.BDXDateCountQuery = strQuery.Trim().Replace("*", "Count(dunsNumber)");                
                queries.XDatescondition = XDatescondition;
                queries.BDBreakdownQuery = GetXDatesCounts();
                return queries;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private static string BuildSQLQuery()
        {
            StringBuilder strQuery = new StringBuilder();
            string strCompany = GetCompanyQuery();        // Company Section
            string strGeographic = GetGeographicQuery();  // Geographic Section
            string strIndustry = GetIndustryQuery();      // Industry (SIC) Section
            StringBuilder strXDatescondition = new StringBuilder();
            // Query SELECT Statement
            if (strCompany != "" || strGeographic != "" || strIndustry != "")
                strQuery.Append("SELECT  * FROM RECORDS WHERE ");
            else
                strQuery.Append("SELECT  * FROM RECORDS ");// No WHERE Conditions

            // Query WHERE Conditions
            if (strCompany != "")
            {
                strQuery.Append(strCompany);
                strXDatescondition.Append(strCompany);
                if (strGeographic != "" || strIndustry != "")
                {
                    strQuery.Append(" AND ");
                    strXDatescondition.Append(" AND ");
                }
            }

            if (strGeographic != "")
            {
                strQuery.Append(strGeographic);
                strXDatescondition.Append(strGeographic);
                if (strIndustry != "")
                {
                    strQuery.Append(" AND ");
                    strXDatescondition.Append(" AND ");
                }
            }

            if (strIndustry != "")
            {
                strQuery.Append(strIndustry);
                strXDatescondition.Append(strIndustry);
            }

            XDatescondition = strXDatescondition.ToString();
            return strQuery.ToString();
        }
        private static string BuildXDatesSQLQuery()
        {
            StringBuilder strQuery = new StringBuilder();
            string strCompany = GetCompanyQuery();        // Company Section
            string strGeographic = GetGeographicQuery();  // Geographic Section
            string strIndustry = GetIndustryQuery();      // Industry (SIC) Section
            string strXDates = ""; // GetXDatesQuery()      'XDates Section

            // Query SELECT Statement
            if (strCompany != "" || strGeographic != "" || strIndustry != "" || strXDates != "" || strXDates != "")
                strQuery.Append("SELECT * FROM RECORDS WHERE (NOT WORKERSCOMPMONTH IS NULL)  AND (");
            else
                strQuery.Append("SELECT * FROM RECORDS WHERE (NOT WORKERSCOMPMONTH IS NULL)");// No WHERE Conditions

            // Query WHERE Conditions
            if (strCompany != "")
            {
                strQuery.Append(strCompany);
                if (strGeographic != "" || strIndustry != "")
                    strQuery.Append(" AND ");
            }

            if (strGeographic != "")
            {
                strQuery.Append(strGeographic);
                if (strIndustry != "")
                    strQuery.Append(" AND ");
            }

            if (strIndustry != "")
            {
                strQuery.Append(strIndustry);
                if (strXDates != "")
                    strQuery.Append(" AND ");
            }

            if (strXDates != "")
                strQuery.Append(strXDates);
            if (strCompany != "" || strGeographic != "" || strIndustry != "" || strXDates != "" || strXDates != "")
                strQuery.Append(")");
            return strQuery.ToString();
        }
        private static string GetXDatesCounts()
        {
            //SqlConnection objCon = new SqlConnection(Settings.Connection.GetConnectionString(Settings.Connection.DBConnections.NMSDATA));
            string strSQLQuery;           
            string strSql;
            strSql = XDatescondition;

            if (strSql == "")
                strSql = "ONE";

            if (strSql != "ONE")
            {                    
                strSQLQuery = "SELECT WORKERSCOMPMONTH,count(DUNSNUMBER) as XDATECOUNT FROM RECORDS WHERE " + strSql;
                strSQLQuery = strSQLQuery + " GROUP BY WORKERSCOMPMONTH HAVING NOT WORKERSCOMPMONTH IS NULL ORDER BY WORKERSCOMPMONTH";
            }
            else
            {
                strSQLQuery = "SELECT WORKERSCOMPMONTH,count(DUNSNUMBER) as XDATECOUNT FROM RECORDS WHERE (NOT WORKERSCOMPXDATE IS NULL) GROUP BY WORKERSCOMPMONTH ORDER BY WORKERSCOMPMONTH";                   
            }           
            
            return strSQLQuery;
        }
        private static string GetRecordCountQuery(string strQuery)
        {
            strQuery = strQuery.Replace("*", "Count(dunsNumber)");
            return strQuery;            
        }

        private static string GetBreakDownPerCounty(string strQuery)
        {
            string strNewQuery = "";
            strNewQuery = strQuery.Replace("*", "STATE as State,CountyName as County,Count(dunsNumber) AS Records");
            strNewQuery = strNewQuery + " GROUP BY STATE,CountyName ORDER BY STATE,CountyName";
            return strNewQuery;
        }
        private static string GetCompanyNameQuery()
        {
            string IL_CompanyName = SearchInput.CompanyName;
            string IL_CompanyBeginsContains = SearchInput.CompanyBeginsContains;
            if (IL_CompanyName.Trim() != "")
            {
                string strCompany = IL_CompanyName.Trim();
                StringBuilder STB = new StringBuilder();

                if (IL_CompanyBeginsContains.Trim() == "Begins")
                    STB.Append("Company LIKE '" + strCompany + "%'");
                else
                    STB.Append("Company LIKE '%" + strCompany + "%'");

                return "(" + STB.ToString() + ")";
            }
            else
                return "";
        }

        private static string GetEmployeeCountQuery()
        {
            string IL_EmployeesFrom = SearchInput.EmployeesFrom.ToString();
            string IL_EmployeesTo = SearchInput.EmployeesTo.ToString();

            string strEmpFieldName = "";
            strEmpFieldName = "Employees";
            StringBuilder sb = new StringBuilder();
            if (IL_EmployeesFrom.Trim() != "" && IL_EmployeesTo.Trim() != "")
            {
                sb.Append("(" + strEmpFieldName + " ");
                sb.Append(" BETWEEN ");
                sb.Append(IL_EmployeesTo.Trim());
                sb.Append(" AND ");
                sb.Append(IL_EmployeesTo.Trim());
                sb.Append(")");
            }
            else if (IL_EmployeesFrom.Trim() != "" && IL_EmployeesTo == "")
            {
                sb.Append("(" + strEmpFieldName + " ");
                sb.Append(">=" + IL_EmployeesFrom.Trim() + ")");
            }
            else if (IL_EmployeesFrom.Trim() == "" && IL_EmployeesTo.Trim() != "")
            {
                sb.Append("(" + strEmpFieldName + " ");
                sb.Append("<=" + IL_EmployeesTo.Trim() + ")");
            }
            return sb.ToString();
        }

        private static string GetAnnualSalesQuery()
        {
            string IL_SalesFrom = SearchInput.SalesFrom.ToString();
            string IL_SalesTo = SearchInput.SalesTo.ToString();
            string strAnnualFieldName = "";
            strAnnualFieldName = "Sales";
            string annualSalesFrom, annualSalesTo;

            StringBuilder sb = new StringBuilder();
            if (IL_SalesFrom.Trim() != "" && IL_SalesTo.Trim() != "")
            {
                annualSalesFrom = (System.Convert.ToDouble(IL_SalesFrom) * 1000000).ToString();
                annualSalesTo = (System.Convert.ToDouble(IL_SalesTo) * 1000000).ToString();

                sb.Append("(" + strAnnualFieldName + " ");
                sb.Append(" BETWEEN ");
                sb.Append(annualSalesFrom);
                sb.Append(" AND ");
                sb.Append(annualSalesTo);
                sb.Append(")");
            }
            else if (IL_SalesFrom.Trim() != "" && IL_SalesTo.Trim() == "")
            {
                annualSalesFrom = (System.Convert.ToDouble(IL_SalesFrom) * 1000000).ToString();
                sb.Append("(" + strAnnualFieldName + " ");
                sb.Append(">=" + annualSalesFrom + ")");
            }
            else if (IL_SalesFrom.Trim() == "" && IL_SalesTo.Trim() != "")
            {
                annualSalesTo = (System.Convert.ToDouble(IL_SalesTo) * 1000000).ToString();
                sb.Append("(" + strAnnualFieldName + " ");
                sb.Append("<=" + annualSalesTo + ")");
            }
            return sb.ToString();
        }

        private static string GetLocationsQuery()
        {
            string IL_Locations = SearchInput.Locations;
            if (IL_Locations.Trim() != "")
            {
                string strList = "";
                strList = IL_Locations.Trim();
                string[] strArray = strList.Split(",");
                string sValue;
                string lValue;
                StringBuilder STB = new StringBuilder();
                for (int I = 0; I <= strArray.Length - 1; I++)
                {
                    lValue = "";
                    sValue = "";
                    sValue = strArray[I];

                    switch (sValue)
                    {
                        case "Single Location":
                            {
                                lValue = "0";
                                break;
                            }

                        case "Headquarters":
                            {
                                lValue = "1";
                                break;
                            }

                        case "Branch":
                            {
                                lValue = "2";
                                break;
                            }
                    }

                    if (STB.ToString().Trim() == "")
                        STB.Append("BranchIndicator='" + lValue + "'");
                    else
                        STB.Append(" OR BranchIndicator='" + lValue + "'");
                }
                if (STB.ToString().Trim() != "")
                    return "(" + STB.ToString() + ")";
                else
                    return "";
            }
            else
                return "";
        }

        private static string GetManufacturingQuery()
        {
            string IL_Manufacturing = SearchInput.Manufacturing;
            if (IL_Manufacturing.Trim() != "")
            {
                string strList = "";
                strList = IL_Manufacturing.Trim();
                string[] strArray = strList.Split(",");
                string sValue;
                string lValue;
                StringBuilder STB = new StringBuilder();
                for (int I = 0; I <= strArray.Length - 1; I++)
                {
                    lValue = "";
                    sValue = "";
                    sValue = strArray[I];

                    switch (sValue)
                    {
                        case "Manufacturing":
                            {
                                lValue = "0";
                                break;
                            }

                        case "Non-Manufacturing":
                            {
                                lValue = "1";
                                break;
                            }
                    }

                    if (STB.ToString().Trim() == "")
                        STB.Append("Manufacturing='" + lValue + "'");
                    else
                        STB.Append(" OR Manufacturing='" + lValue + "'");
                }
                if (STB.ToString().Trim() != "")
                    return "(" + STB.ToString() + ")";
                else
                    return "";
            }
            else
                return "";
        }
        private static string GetCompanyQuery()
        {
            StringBuilder stqry = new StringBuilder();
            string strCompany = GetCompanyNameQuery();        // Company Name Search
            string strEmployees = GetEmployeeCountQuery();    // Number of Employees
            string strAnnual = GetAnnualSalesQuery();         // Annual Sales Volume
            string strLocations = GetLocationsQuery();        // Location / Branch Type
            string strManufacturing = GetManufacturingQuery(); // Manufacturing Site

            if (strCompany.Trim() != "")
            {
                stqry.Append(strCompany);
                if (strLocations.Trim() != "" || strManufacturing.Trim() != "" || strEmployees.Trim() != "" || strAnnual.Trim() != "")
                    stqry.Append(" AND ");
            }

            if (strLocations.Trim() != "")
            {
                stqry.Append(strLocations);
                if (strManufacturing.Trim() != "" || strEmployees.Trim() != "" || strAnnual.Trim() != "")
                    stqry.Append(" AND ");
            }

            if (strManufacturing.Trim() != "")
            {
                stqry.Append(strManufacturing);
                if (strEmployees.Trim() != "" || strAnnual.Trim() != "")
                    stqry.Append(" AND ");
            }

            if (strEmployees.Trim() != "" && strAnnual.Trim() != "")
            {
                stqry.Append("(" + strEmployees);
                if (SearchInput.CompanyAndOr.Trim() == "AND")
                    stqry.Append(" AND " + strAnnual + ")");
                else
                    stqry.Append(" OR " + strAnnual + ")");
            }
            else if (strEmployees.Trim() != "")
                stqry.Append(strEmployees);
            else if (strAnnual.Trim() != "")
                stqry.Append(strAnnual);

            return stqry.ToString();
        }

        private static string GetGeographicQuery()
        {
            StringBuilder stqry = new StringBuilder();

            string strStates = GetStatesQuery();
            string strExcludeStates = GetExcludeStatesQuery();
            if (strStates != "" && strExcludeStates != "")
            {
               strStates = "  (" + strStates + " and " + strExcludeStates + " ) ";
            }
            else if (strStates == "" && strExcludeStates != "")
                strStates = strExcludeStates;


            string strCounties = GetCountiesQuery();
            string strExcludeCounties = GetExcludeCountiesQuery();
            if (strCounties != "" & strExcludeCounties != "")              
                strCounties = "  ((" + strCounties + " and " + strExcludeCounties + " ) ";
            else if (strCounties == "" & strExcludeCounties != "")              
                strCounties = "  (" + strExcludeCounties;
            else if (strCounties != "" & strExcludeCounties == "")              
                strCounties = " ( " + strCounties;


            string strZips = GetZipQuery();
            string strExcludeZips = GetExcludeZipQuery();
            if (strZips != "" && strExcludeZips != "")
            {
                if (strCounties != "")
                    strZips = " OR (" + strZips + " and " + strExcludeZips + " ) ";
                else
                    strZips = " OR ( (" + strZips + " and " + strExcludeZips + " ) ";
            }
            else if (strZips == "" && strExcludeZips != "")
            {
                if (strCounties != "")
                    strZips = " AND " + strExcludeZips;
                else
                    strZips = " AND ( " + strExcludeZips;
            }
            else if (strZips != "" && strExcludeZips == "")
            {
                if (strCounties != "")
                    strZips = " OR " + strZips;
                else
                    strZips = " OR ( " + strZips;
            }


            string strMSAs = GetMSAQuery();
            string strExcludeMSAs = GetExcludeMSAQuery();
            if (strMSAs != "" && strExcludeMSAs != "")
            {
                if (strCounties == "" && strZips == "")
                    strMSAs = " OR ( (" + strMSAs + " and " + strExcludeMSAs + " ) ";
                else
                    strMSAs = " OR (" + strMSAs + " and " + strExcludeMSAs + " ) ";
            }
            else if (strMSAs == "" && strExcludeMSAs != "")
            {
                if (strCounties == "" && strZips == "")
                    strMSAs = " AND ( " + strExcludeMSAs;
                else
                    strMSAs = " AND " + strExcludeMSAs;
            }
            else if (strMSAs != "" && strExcludeMSAs == "")
            {
                if (strCounties == "" && strZips == "")
                    strMSAs = " OR ( " + strMSAs;
                else
                    strMSAs = " OR " + strMSAs;
            }

            string strAreas = GetAreaQuery();
            string strExcludeAreas = GetExcludeAreaQuery();
            if (strAreas != "" && strExcludeAreas != "")
            {
                if (strCounties == "" && strZips == "" && strMSAs == "")
                    strAreas = " OR ( (" + strAreas + " and " + strExcludeAreas + " ) ";
                else
                    strAreas = " OR (" + strAreas + " and " + strExcludeAreas + " ) ";
            }
            else if (strAreas == "" && strExcludeAreas != "")
            {
                if (strCounties == "" && strZips == "" && strMSAs == "")
                    strAreas = " AND ( " + strExcludeAreas;
                else
                    strAreas = " AND " + strExcludeAreas;
            }
            else if (strAreas != "" && strExcludeAreas == "")
            {
                if (strCounties == "" && strZips == "" && strMSAs == "")
                    strAreas = " OR ( " + strAreas;
                else
                    strAreas = " OR " + strAreas;
            }

            string[] arrState;
            if (SearchInput.State == null && SearchInput.Exclude_State == null)
                SearchInput.State = "";
            if (SearchInput.Exclude_State == null)
                SearchInput.Exclude_State = "";
            arrState = Convert.ToString(SearchInput.State + "," + SearchInput.Exclude_State).Split(",");
            if (arrState.Length == 1)
            {
                if (strCounties != "")
                    strCounties = " AND " + strCounties.Substring(4, strCounties.Length - 4);
                if (strZips != "")
                    strZips = " AND " + strZips.Substring(4, strZips.Length - 4);
                if (strMSAs != "")
                    strMSAs = " AND " + strMSAs.Substring(4, strMSAs.Length - 4);
                if (strAreas != "")
                    strAreas = " AND " + strAreas.Substring(4, strAreas.Length - 4);
            }
           
            if (strCounties == "")
            {
                if (strStates != "")
                    stqry.Append(strStates);
                else
                {
                    // This section remove the condition if the previous condition is empty
                    if (strCounties != "")
                        strCounties = " " + strCounties.Substring(4, strCounties.Length - 4);
                    if (strCounties == "" & strZips != "")
                        strZips = " " + strZips.Substring(4, strZips.Length - 4);
                    if (strCounties != "" | strZips != "")
                    {
                    }
                    else if (strMSAs != "")
                        strMSAs = " " + strMSAs.Substring(4, strMSAs.Length - 4);
                    if (strCounties != "" | strZips != "" | strMSAs != "")
                    {
                    }
                    else if (strAreas != "")
                        strAreas = " " + strAreas.Substring(4, strAreas.Length - 4);
                }
            }
            if (strCounties != "")
                stqry.Append(strCounties);

            if (strZips != "")
                stqry.Append(strZips);

            if (strMSAs != "")
            {
                if (SearchInput.IsNewQuery)
                    stqry.Append(strMSAs);
                else
                {
                    strMSAs = strMSAs.Replace("SMSACode", "SMSAstateCode");
                    stqry.Append(strMSAs);
                }
            }

            if (strAreas != "")
                stqry.Append(strAreas);

            // Closing Parenthesis (Because of OR Conditions)
            if (strCounties != "" | strZips != "" | strMSAs != "" | strAreas != "")
                stqry.Append(" )");


            return stqry.ToString();
        }
        private static string GetIndustryQuery()
        {
            StringBuilder Sb = new StringBuilder();
            if (!string.IsNullOrEmpty(SearchInput.SIC.Trim()))
            {
                string strList = SearchInput.SIC.Trim();
                string[] stArray = strList.Split(",");

                string sValue = "";
                for (int I = 0; I <= stArray.Length - 1; I++)
                {
                    sValue = "";
                    sValue = stArray[I];
                    if (sValue.Trim() != "")
                    {
                        if (Sb.ToString() == "")
                        {
                            if (sValue.Trim() == "A" || sValue.Trim() == "B" || sValue.Trim() == "C" || sValue.Trim() == "D" || sValue.Trim() == "E" || sValue.Trim() == "F" || sValue.Trim() == "G" || sValue.Trim() == "H" || sValue.Trim() == "I" || sValue.Trim() == "J" || sValue.Trim() == "K")
                                Sb.Append(GetMainClassNumbers(sValue));
                            else
                                Sb.Append(" SIC8 LIKE '" + sValue.Trim() + "%'");
                        }
                        else if (sValue.Trim() == "A" || sValue.Trim() == "B" || sValue.Trim() == "C" || sValue.Trim() == "D" || sValue.Trim() == "E" || sValue.Trim() == "F" || sValue.Trim() == "G" || sValue.Trim() == "H" || sValue.Trim() == "I" || sValue.Trim() == "J" || sValue.Trim() == "K")
                            Sb.Append(" OR " + GetMainClassNumbers(sValue));
                        else
                            Sb.Append(" OR SIC8 LIKE '" + sValue.Trim() + "%'");
                    }
                }
            }

            if (Sb.ToString().Trim() != "")
                return "(" + Sb.ToString() + " )";
            else
                return "";
        }
        
        

        private static string GetStatesQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.State))
            {
                if (SearchInput.State != "")
                {
                    StringBuilder STB = new StringBuilder();
                    SubqueryBuilder(SearchInput.State, "STATE", "OR", "=", ref STB);

                    if (STB.ToString().Trim() != "")
                        return "(" + STB.ToString() + ")";
                }
            }
            return "";
            
        }

        private static string GetExcludeStatesQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Exclude_State))
            {
                if (SearchInput.Exclude_State != "")
                {         
                    StringBuilder STB = new StringBuilder();
                    SubqueryBuilder(SearchInput.Exclude_State, "STATE", "AND", "<>", ref STB);
                    
                    if (STB.ToString().Trim() != "")
                        return "(" + STB.ToString() + ")";                    
                }
            }
            return "";
        }
        
        private static string GetCountiesQuery()
        {            
            if (!string.IsNullOrEmpty(SearchInput.County.Trim()) || !string.IsNullOrEmpty(SearchInput.Exclude_County.Trim()))
            {
                string strcounty = "";
                if (!string.IsNullOrEmpty(SearchInput.State.Trim()) || !string.IsNullOrEmpty(SearchInput.Exclude_State.Trim()))
                {
                    strcounty = SearchInput.IncludeCountyIds;
                }
                string strList = "";
                strList = (SearchInput.County.Trim() + "," + strcounty.Trim()).Trim(',');
                string[] strArray = strList.Split(",");
                string sValue;
                StringBuilder STB = new StringBuilder();
                for (int I = 0; I <= strArray.Length - 1; I++)
                {
                    sValue = "";
                    sValue = strArray[I];
                    if (STB.ToString().Trim() == "")
                        STB.Append("(StateCounty='" + sValue + "')");
                    else
                        STB.Append(" OR (StateCounty='" + sValue + "')");
                }

                if (STB.ToString().Trim() != "")
                    return "(" + STB.ToString() + ")";
                else
                    return "";
            }
            else
                return "";
        }
        private static string GetExcludeCountiesQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Exclude_County.Trim()))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.Exclude_County.Trim(), "StateCounty", "AND", "<>", ref stb);


                if ((stb.ToString().Trim() != ""))
                {
                    return ("("
                            + (stb.ToString() + ")"));
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
        private static string GetExcludeZipQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Exclude_Zip))
            {
                StringBuilder stb = new StringBuilder();
                BuildZipSubquery(SearchInput.Exclude_Zip.Trim(), "AND", "NOT", ref stb);

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
                SubqueryBuilder(SearchInput.MSA, "SMSACode", "OR", "=", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("("
                            + (stb.ToString() + ")"));
                }

            }
            return "";           

        }
        private static string GetExcludeMSAQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Exclude_MSA))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.Exclude_MSA.Trim(), "SMSACode", "AND", "<>", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("(" + (stb.ToString() + ")"));
                }
            }
            return "";
        }
        private static string GetAreaQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.AreaCode))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.AreaCode, "AreaCode", "OR", "=", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("("
                            + (stb.ToString() + ")"));
                }

            }
            return "";

        }
        private static string GetExcludeAreaQuery()
        {
            if (!string.IsNullOrEmpty(SearchInput.Exclude_AreaCode))
            {
                StringBuilder stb = new StringBuilder();
                SubqueryBuilder(SearchInput.Exclude_AreaCode.Trim(), "AreaCode", "AND", "<>", ref stb);

                if ((stb.ToString().Trim() != ""))
                {
                    return ("(" + (stb.ToString() + ")"));
                }
            }
            return "";
        }
        

        private static string GetMainClassNumbers(string strClass)
        {
            if (strClass == "A")
                return " SIC8 LIKE '01%' OR SIC8 LIKE '02%' OR SIC8 LIKE '07%' OR SIC8 LIKE '08%' OR SIC8 LIKE '09%'";
            else if (strClass == "B")
                return " SIC8 LIKE '10%' OR SIC8 LIKE '12%' OR SIC8 LIKE '13%' OR SIC8 LIKE '14%'";
            else if (strClass == "C")
                return " SIC8 LIKE '15%' OR SIC8 LIKE '16%' OR SIC8 LIKE '17%' OR SIC8 LIKE '18%' OR SIC8 LIKE '19%'";
            else if (strClass == "D")
                return " SIC8 LIKE '20%' OR SIC8 LIKE '21%' OR SIC8 LIKE '22%' OR SIC8 LIKE '23%' OR SIC8 LIKE '24%' OR SIC8 LIKE '25%' OR SIC8 LIKE '26%' OR SIC8 LIKE '27%' OR SIC8 LIKE '28%' OR SIC8 LIKE '29%' OR SIC8 LIKE'30%' OR SIC8 LIKE '31%' OR SIC8 LIKE '32%' OR SIC8 LIKE '33%' OR SIC8 LIKE '34%' OR SIC8 LIKE '35%' OR SIC8 LIKE '36%' OR SIC8 LIKE '37%' OR SIC8 LIKE '38%' OR SIC8 LIKE '39%'";
            else if (strClass == "E")
                return " SIC8 LIKE '40%' OR SIC8 LIKE '41%' OR SIC8 LIKE '42%' OR SIC8 LIKE '43%' OR SIC8 LIKE '44%' OR SIC8 LIKE '45%' OR SIC8 LIKE '46%' OR SIC8 LIKE '47%' OR SIC8 LIKE '48%' OR SIC8 LIKE '49%'";
            else if (strClass == "F")
                return " SIC8 LIKE '50%' OR SIC8 LIKE '51%'";
            else if (strClass == "G")
                return " SIC8 LIKE '52%' OR SIC8 LIKE '53%' OR SIC8 LIKE '54%' OR SIC8 LIKE '55%' OR SIC8 LIKE '56%' OR SIC8 LIKE '57%' OR SIC8 LIKE '58%' OR SIC8 LIKE '59%'";
            else if (strClass == "H")
                return " SIC8 LIKE '60%' OR SIC8 LIKE '61%' OR SIC8 LIKE '62%' OR SIC8 LIKE '63%' OR SIC8 LIKE '64%' OR SIC8 LIKE '65%' OR SIC8 LIKE '66%' OR SIC8 LIKE '67%'";
            else if (strClass == "I")
                return " SIC8 LIKE '70%' OR SIC8 LIKE '71%' OR SIC8 LIKE '72%' OR SIC8 LIKE '73%' OR SIC8 LIKE '74%' OR SIC8 LIKE '75%' OR SIC8 LIKE '76%' OR SIC8 LIKE '77%' OR SIC8 LIKE '78%' OR SIC8 LIKE '79%' OR SIC8 LIKE '80%' OR SIC8 LIKE '81%' OR SIC8 LIKE '82%' OR SIC8 LIKE '83%' OR SIC8 LIKE '84%' OR SIC8 LIKE '85%' OR SIC8 LIKE '86%' OR SIC8 LIKE '87%' OR SIC8 LIKE '88%' OR SIC8 LIKE '89%' OR SIC8 LIKE '90%'";
            else if (strClass == "J")
                return " SIC8 LIKE '91%' OR SIC8 LIKE '92%' OR SIC8 LIKE '93%' OR SIC8 LIKE '94%' OR SIC8 LIKE '95%' OR SIC8 LIKE '96%' OR SIC8 LIKE '97%' OR SIC8 LIKE '98%'";
            else if (strClass == "K")
                return " SIC8 LIKE '99%'";
            else
                return "";
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
                        stb.Append(("Zip " + operators + " like \'"
                                    + (sValue + "%\'")));
                    }
                    else if ((sValue.Length == 3))
                    {
                        stb.Append(("Zip " + operators + " like \'"
                                    + (sValue + "%\'")));
                    }

                }
                else if ((sValue.Length > 3))
                {
                    stb.Append((" " + condition + " Zip " + operators + " like \'"
                                + (sValue + "%\'")));
                }
                else if ((sValue.Length == 3))
                {
                    stb.Append((" " + condition + " Zip " + operators + " like \'"
                                + (sValue + "%\'")));
                }

            }
        }

        public static string GetBDAnalyzeQuery(AnalyzeBDInput input)
        {
            string strNewQuery = "";
            string strSelected = input.FirstValue;
            string strSelected1 = input.SecondValue;
            string strSelected2 = input.ThirdValue;
            string strquery = input.BusinessQuery;

            string selectstatement = "";
            string groupstatement = "";
            string mselectstatement = "";


            if (strSelected != "" & strSelected1 == "" & strSelected2 == "")
                selectstatement = strSelected;
            if (strSelected == "" & strSelected1 != "" & strSelected2 == "")
                selectstatement = strSelected1;
            if (strSelected == "" & strSelected1 == "" & strSelected2 != "")
                selectstatement = strSelected2;
            if (strSelected != "" & strSelected1 != "" & strSelected2 == "")
                selectstatement = strSelected + "," + strSelected1;
            if (strSelected == "" & strSelected1 != "" & strSelected2 != "")
                selectstatement = strSelected1 + "," + strSelected2;
            if (strSelected != "" & strSelected1 == "" & strSelected2 != "")
                selectstatement = strSelected + "," + strSelected2;
            if (strSelected != "" & strSelected1 != "" & strSelected2 != "")
                selectstatement = strSelected + "," + strSelected1 + "," + strSelected2;

            mselectstatement = selectstatement;
            if (selectstatement != "")
            {
               
                groupstatement = selectstatement;
                string strSicColumn = "";
                if (selectstatement.IndexOf("(sic8,1,2)") > -1)
                {
                    selectstatement = selectstatement.Replace("(sic8,1,2)", "(sic8,1,2) as [Two_Digit_SIC] ");
                    mselectstatement = mselectstatement.Replace("substring(sic8,1,2)", "[Two_Digit_SIC]");
                    strSicColumn = "Two_Digit_SIC";
                }
                else if (selectstatement.IndexOf("(sic8,1,4)") > -1)
                {
                    selectstatement = selectstatement.Replace("(sic8,1,4)", "(sic8,1,4) as [Four_Digit_SIC] ");
                    mselectstatement = mselectstatement.Replace("substring(sic8,1,4)", "[Four_Digit_SIC]");
                    strSicColumn = "Four_Digit_SIC";
                }
                else if (selectstatement.IndexOf("(sic8,1,6)") > -1)
                {
                    selectstatement = selectstatement.Replace("(sic8,1,6)", "(sic8,1,6) as [Six_Digit_SIC] ");
                    mselectstatement = mselectstatement.Replace("substring(sic8,1,6)", "[Six_Digit_SIC]");
                    strSicColumn = "Six_Digit_SIC";
                }
                else if (selectstatement.IndexOf("sic8") > -1)
                {
                    selectstatement = selectstatement.Replace("sic8", "sic8 as [Eight_Digit_SIC] ");
                    mselectstatement = mselectstatement.Replace("sic8", "[Eight_Digit_SIC]");
                    strSicColumn = "Eight_Digit_SIC";
                }

                if (selectstatement.IndexOf("SMSACode") > -1)
                {
                    selectstatement = selectstatement.Replace("SMSACode", "SMSAStateCode as [Metro_Areas]");
                    mselectstatement = mselectstatement.Replace("SMSACode", "[Metro_Areas]");
                    groupstatement = groupstatement.Replace("SMSACode", "SMSAStateCode");
                }

                if (selectstatement.IndexOf("StateCounty") != -1)
                {
                    selectstatement = selectstatement.Replace("StateCounty", "StateCounty as County");
                    mselectstatement = mselectstatement.Replace("StateCounty", "County");
                }

                if (selectstatement.IndexOf("employees") != -1)
                {
                    selectstatement = selectstatement.Replace("employees", "employees as Employees");
                    mselectstatement = mselectstatement.Replace("employees", "Employees");
                }

                if (selectstatement.IndexOf("sales") != -1)
                {
                    selectstatement = selectstatement.Replace("sales", "convert(varchar,cast(sales as money),-1) as Revenue");
                    mselectstatement = mselectstatement.Replace("sales", "Revenue");
                }


                
                strNewQuery = strquery.Replace("*", selectstatement + ",Count(*) as Records");
                strNewQuery = strNewQuery + " GROUP BY " + groupstatement;

                string orderBystr = mselectstatement;
                //Dim strSql As String = "select SIC,DESCRIPTION from  RS_SICs WHERE RS_SICs.SIC IN (" & strSicCodes & ")"
                if ((selectstatement.IndexOf("sic") > -1) && (selectstatement.IndexOf("StateCounty") > -1))
                {
                    mselectstatement = mselectstatement.Replace("County", "(select top 1 county from  RS_DNB_FIPSCOUNTYCODES WHERE fipsstatecountycode = tb.county AND fipsstatecountycode is not null) as [County]");
                    strNewQuery = "Select " + mselectstatement + " , Records, (select top 1 DESCRIPTION from  RS_SICs WHERE SIC = tb.[" + strSicColumn + "] AND SIC is not null) as Description FROM (" + strNewQuery + ")  as tb";
                }
                else if ((selectstatement.IndexOf("sic") > -1))
                {
                   strNewQuery = "Select *, (select top 1 DESCRIPTION from  RS_SICs WHERE SIC = tb.[" + strSicColumn + "] AND SIC is not null) as Description FROM (" + strNewQuery + ")  as tb";
                }
                else if ((selectstatement.IndexOf("StateCounty") > -1))
                {
                    mselectstatement = mselectstatement.Replace("County", "(select top 1 county from  RS_DNB_FIPSCOUNTYCODES WHERE fipsstatecountycode = tb.county AND fipsstatecountycode is not null) as [County]");
                    strNewQuery = "Select " + mselectstatement + ", Records FROM (" + strNewQuery + ")  as tb";
                }

                strNewQuery = strNewQuery + " ORDER BY " + orderBystr;

            }
            
            return strNewQuery;
        }

        public static string GetRecordsQuery(PurchaseInput input)
        {
            var query = input.RecordQuery.Replace("*", "TOP " + input.RecordCount + " DUNSNUMBER");
            return query;
        }

        public static string GetXDatesQuery(PurchaseInput input)
        {
            var query = input.XDateQuery.Replace("*", "TOP " + input.XDateCount + " DUNSNUMBER");
            var subQuery = "";
            
            if (!string.IsNullOrEmpty(input.XDateMonths))
            {
               subQuery = GetXDatesMonthQuery(input.XDateMonths);
                if (query.Contains("WHERE"))
                {
                    query = query + " AND " + subQuery;
                }
            }

            return query;
        }

        private static string GetXDatesMonthQuery(string xDateMonths)
        {
            StringBuilder stb = new StringBuilder();
            string[] strArray = xDateMonths.Split(",");
            int i;
            string sValue;
            for (i = 0; (i <= (strArray.Length - 1)); i++)
            {
                sValue = strArray[i];
                if ((stb.ToString().Trim() == ""))
                {
                    stb.Append("WORKERSCOMPMONTH = " + sValue.Trim());
                }
                else
                {
                    stb.Append(" OR WORKERSCOMPMONTH = " + sValue.Trim());
                }
            }

            if (!string.IsNullOrEmpty(stb.ToString()))
            {
                return "(" + stb + ")";
            }
            else
            {
                return "";
            }
        }

        public static string GetCountyQuery(string states, string exStates)
        {
            string strcounties;
            strcounties = (states + "," + exStates).Trim(',');// Session("Exclude_IL_State").ToString()).Trim(",").Split(",");
            strcounties = strcounties.Replace(",", "','");
            strcounties = "'" + strcounties + "'";
            string strquerycounty = "SELECT STRING_AGG(FIPSStateCountyCode, \',\') FROM RS_DNB_FIPSCOUNTYCODES WHERE State IN (" + strcounties + ")";
            return strquerycounty;
        }
    }
}
