using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Linq;
using GazRouter.DTO.Authorization.User;
using GazRouter.Log;
using SearchScope = System.DirectoryServices.SearchScope;
namespace GazRouter.DataServices.Infrastructure.Authorization
{
    public static class AdManager
    {
#region variables
        private static readonly MyLogger Logger = new MyLogger("mainLogger");
        private const string Samaccountname     = "samaccountname";
        private const string Displayname        = "displayname";
        private const string Description        = "description";
        private const string UserFilter         = "objectClass=user";
        private const string GlobalCatalog      = "GC://";
        private const string Ldap               = "LDAP://";
#endregion
        // 
        public static List<AdUserDTO> GetAdUsers()
        {
            var searchRoot = new DirectoryEntry($"LDAP://{AppSettingsManager.ActiveDirectory}");
            var seach = new DirectorySearcher(searchRoot) { Filter = "(&(objectClass=user))" };
            const string samaccountname = "samaccountname";
            const string displayname = "displayname";

            seach.PropertiesToLoad.Add(samaccountname);
            seach.PropertiesToLoad.Add(displayname);

            var coll = seach.FindAll();
            var result = new List<AdUserDTO>();
            foreach (SearchResult z in coll)
            {
                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[samaccountname][0]}";
                if (z.Properties[displayname].Count == 0)
                    continue;
                var displayName = z.Properties[displayname][0].ToString();
                result.Add(new AdUserDTO(login, displayName));
            }
            return result;
        }
        public static List<AdUserDTO> GetAdUsers(AdUserFilterParameterSet filterSet)
        {
            if (string.IsNullOrEmpty(filterSet?.SearchString)) return new List<AdUserDTO>();
            var searchString = filterSet.SearchString.Replace("*", "").Trim(); 
            if (searchString.Length == 0) return new List<AdUserDTO>();
            //             
            var filterAttribute = filterSet.SearchByAccount ? Samaccountname : Displayname;
            var filter = $"(&({UserFilter})({filterAttribute}=*{searchString}*))";
            var searchRoot = new DirectoryEntry($"{Ldap}{AppSettingsManager.ActiveDirectory}");
            //
            var searcher = new DirectorySearcher(searchRoot) { Filter = filter };
            searcher.PropertiesToLoad.Add(Samaccountname);
            searcher.PropertiesToLoad.Add(Displayname);
            var findAll = searcher.FindAll();
            var result = new List<AdUserDTO>();
            foreach (SearchResult z in findAll)
            {
                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[Samaccountname][0]}";
                if (z.Properties[Displayname].Count == 0) continue;
                //
                var displayName = z.Properties[Displayname][0].ToString();
                result.Add(new AdUserDTO(login, displayName));
            }
            return result;
        }
        public static List<AdUserDTO> AdForestUsers(AdUserFilterParameterSet filterSet)
        {            
            if (string.IsNullOrEmpty(filterSet?.SearchString)) return new List<AdUserDTO>();
            var searchString = filterSet.SearchString.Replace("*", "").Trim();
            if (searchString.Length == 0) return new List<AdUserDTO>();
            using (var searcher = new DirectorySearcher(new DirectoryEntry($"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}"))){
                var attributeFilter = filterSet.GetSearchAttribute(); 
                // 
                var filter = $"(&({UserFilter})({attributeFilter}=*{searchString}*))";          
                searcher.Filter = filter;                                                       
                searcher.SearchScope = SearchScope.Subtree;                                     
                searcher.ReferralChasing = ReferralChasingOption.None;                          
                var results = searcher.FindAll();
                // 
                var list = new List<AdUserDTO>();
                foreach (SearchResult result in results)
                {
                    Trace("=============================== ad trace ===============================");
                    if (!filterSet.IsEnterprise)
                    {
                        Trace("===== !filterSet.IsEnterprise =====");
                        TracePropertyNames(result.Properties);
                        if (!result.Path.Contains($"DC={AppSettingsManager.ActiveDirectory.ToLower()},")) continue;
                    }
                    //
                    var login = $"{GetSubDomainNameByPath(result.Path)}\\{result.Properties[Samaccountname][0]}";
                    var displayName = result.Properties[Displayname].Count == 0 ? login :
                                                                                  result.Properties[Displayname][0].ToString();
                    var description = result.Properties[Description].Count == 0 ? string.Empty :
                                                                                  result.Properties[Description][0].ToString();
                    // log
                    TracePropertyNames(result.Properties);
                    list.Add(new AdUserDTO(login, displayName, description));
                }
                return list;
            }
        }
        /// <summary>
        /// path ex: "GC://DEV/CN=MII,CN=Computers,DC=dev,DC=ga,DC=loc"
        /// 
        /// выбирается последний поддомен "dev"
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetSubDomainNameByPath(string path)
        {
            var startIndex = path.IndexOf("DC=", StringComparison.InvariantCultureIgnoreCase);
            var endIndex = path.IndexOf(",", startIndex, StringComparison.InvariantCultureIgnoreCase);
            return path.Substring(startIndex + 3, endIndex - startIndex - 3);
        }
#region helpers
        private static void TracePropertyNames(ResultPropertyCollection myResultPropColl)
        {
            if (myResultPropColl?.PropertyNames == null) return;
            //
            foreach (string myKey in myResultPropColl.PropertyNames)
            {
                foreach (Object myCollection in myResultPropColl[myKey])
                    Trace($"result.Properties[{myKey}]", $"{myCollection}");
            }
        }
        private static void Trace(string msg, string msg2 = "")
        {
            Logger.Info($"- Trace:> {msg} = {msg2}");
        }
        private static List<AdUserDTO> GetDomainUsers(AdUserFilterParameterSet filterSet)
        {
#region trash            
            //            var seach = new DirectorySearcher(domain) { Filter = "(&(objectClass=user))" };
            //            const string samaccountname = "samaccountname";
            //            const string displayname = "displayname";            
            //            seach.SearchScope = SearchScope.Subtree;
            //            seach.PropertiesToLoad.Add(samaccountname);
            //            seach.PropertiesToLoad.Add(displayname);
            //            var coll = seach.FindAll();
            //            var result = new List<AdUserDTO>();
            //            foreach (SearchResult z in coll)
            //            {
            //                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[samaccountname][0]}";
            //                if (z.Properties[displayname].Count == 0)
            //                {
            //                    continue;
            //                }
            //                string displayName = z.Properties[displayname][0].ToString();
            //                result.Add(new AdUserDTO(login, displayName));
            //            }
            //            return result;
#endregion
            if (string.IsNullOrEmpty(filterSet?.SearchString)) return new List<AdUserDTO>();
            var searchString = filterSet.SearchString.Replace("*", "").Trim();
            if (searchString.Length == 0) return new List<AdUserDTO>();
            //              
            var filterAttribute = filterSet.SearchByAccount ? Samaccountname : Displayname;
            var filter = $"(&({UserFilter})({filterAttribute}=*{searchString}*))";
            var domain = $"{Ldap}{AppSettingsManager.ActiveDirectory}";
            var searchRoot = new DirectoryEntry(domain);
            PrintTree(searchRoot);
            //
            var searcher = new DirectorySearcher(searchRoot)
            {
                Filter = filter,
                SearchScope = SearchScope.Subtree
            };
            searcher.PropertiesToLoad.Add(Samaccountname);
            searcher.PropertiesToLoad.Add(Displayname);
            var coll = searcher.FindAll();
            var result = new List<AdUserDTO>();
            foreach (SearchResult z in coll)
            {
                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[Samaccountname][0]}";
                if (z.Properties[Displayname].Count == 0) continue;
                //
                var displayName = z.Properties[Displayname][0].ToString();
                result.Add(new AdUserDTO(login, displayName));
            }
            return result;
        }
        public static List<string> AdOu(string domain)
        {
            var searchRoot = new DirectoryEntry($"LDAP://{domain}");
            var searcher = new DirectorySearcher(searchRoot);
            searcher.Filter = "(objectCategory=organizationalUnit)";
            var list = new List<string>();
            foreach (SearchResult v in searcher.FindAll())
                list.Add(v.Path);
            return list;
        }
        private static List<AdUserDTO> GetAllDomainUsers(string domain)
        {
            Logger.Info($"AdManager.GetAllDomainUsers()?domain={domain}");
            var searchRoot = new DirectoryEntry($"LDAP://{domain}");
            var seach = new DirectorySearcher(searchRoot) { Filter = "(&(objectClass=user))" };
            const string samaccountname = "samaccountname";
            const string displayname    = "displayname";
            //
            seach.PropertiesToLoad.Add(samaccountname);
            seach.PropertiesToLoad.Add(displayname);

            var coll = seach.FindAll();
            var result = new List<AdUserDTO>();
            foreach (SearchResult z in coll)
            {
                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[samaccountname][0]}";
                if (z.Properties[displayname].Count == 0)
                {
                    continue;
                }
                string displayName = z.Properties[displayname][0].ToString();
                result.Add(new AdUserDTO(login, displayName));
            }
            return result;
        }
        public static List<string> SubDomainList(string domain)
        {
//            Logger.Info($"AdManager.SubDomainList():");
            // 
            string sRootDomain;
            DirectoryEntry deRootDSE;
            DirectoryEntry deSearchRoot;
            DirectorySearcher dsFindDomains;
            SearchResultCollection srcResults;
            deRootDSE = new DirectoryEntry("GC://RootDSE");
            sRootDomain = "GC://" + deRootDSE.Properties["rootDomainNamingContext"].Value;
            //
            deSearchRoot = new DirectoryEntry(sRootDomain);
            dsFindDomains = new DirectorySearcher(deSearchRoot)
            {
                Filter = "(objectCategory=domainDNS)",
                SearchScope = SearchScope.Subtree
            };
            //
            srcResults = dsFindDomains.FindAll();

            var list = (from SearchResult srDomain in srcResults
                        select srDomain.Properties["name"][0]
                            .ToString()).ToList();
            list.ForEach(e => Logger.Info($"AdManager.SubDomainList():{e}"));
            return list;
        }
        private static void PrintTree(DirectoryEntry entry)
        {
            Action<DirectoryEntry> action = e =>
            {
                Logger.Info("//// SchemaClassName = " + e.SchemaClassName + "name = " + e.Name + "; Username = " + e.Username + "; Path = " + e.Path); //   /*e.Parent.Name + ", " + */
            };
            action.Invoke(entry);
            foreach (DirectoryEntry e in entry.Children) PrintTree(e);
        }
        private static List<AdUserDTO> Search()
        {
            const string samaccountname = "samaccountname";
            const string displayname    = "displayname";
            const string filter         = "objectClass=user";
            var r = new List<AdUserDTO>();
            ////
            DirectoryContext context = new DirectoryContext(DirectoryContextType.Forest, AppSettingsManager.ActiveDirectory);
            Forest forest = Forest.GetForest(context);
            GlobalCatalog gc = null;
            //
            Action<SearchResult> action = z =>
            {
                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[samaccountname][0]}";
                if (z.Properties[displayname].Count != 0)
                {
                    var displayName = z.Properties[displayname][0].ToString();
                    r.Add(new AdUserDTO(login, displayName));
                }
            };
            //
            try
            {
                gc = forest.FindGlobalCatalog();
            }
            catch (ActiveDirectoryObjectNotFoundException)
            {
                Logger.Info("No GC found in this forest");
            }
//            if (gc != null)
//            {
//                Logger.Info("gc != null");
//                DirectorySearcher searcher = gc.GetDirectorySearcher();
//                searcher.Filter = filter;
//                foreach (SearchResult z in searcher.FindAll()) action(z);
//                return r;
//            }
            foreach (Domain dd in forest.Domains)
            {
                Logger.Info("gc == null");
                DirectorySearcher searcher = new DirectorySearcher(dd.GetDirectoryEntry(), filter);
                foreach (SearchResult z in searcher.FindAll()) action(z);
            }
            return r;
        }
#endregion
#region test
        public static void AdTest()
        {
            // 1
//            AdGlobalCatalogUsers().ForEach(e=>Logger.Info(e));
        }
        public static List<AdUserDTO> ForestTest()
        {
            Logger.Info("+++++++++++++++++++++++++++++ forest ++++++++++++++++++++++++++++++++");
            var a = Search();
            a.ForEach(e => Logger.Info($"Login = {e.Login}; DisplayName = {e.DisplayName}"));
            return a;
        }
        public static DomainCollection GetForestDomains()
        {
            Logger.Info("AdManager.GetForestDomains()");
            DirectoryContext context = new DirectoryContext(DirectoryContextType.Forest, 
                                                            AppSettingsManager.ActiveDirectory);
            Forest forest = Forest.GetForest(context);
            GlobalCatalog gc = null;
            try
            {
                gc = forest.FindGlobalCatalog();
                foreach (var dom in gc.Forest.Domains)
                    Logger.Info($"dom = {dom.ToString()}");
                return gc.Forest.Domains;
            }
            catch (ActiveDirectoryObjectNotFoundException)
            {
                Logger.Info("No GC found in this forest");
            }
            return null;
        }
        public static List<AdUserDTO> AdForestUsersTest(AdUserFilterParameterSet filterSet)
        {
            //            G3(filterSet);
            Trace("=== +++ AdForestUsers +++ ===");
            Trace("SearchString", $"{filterSet.SearchString}");
            Trace("SearchString", $"{filterSet.SearchString}");
            Trace("IsEnterprise", $"{filterSet.IsEnterprise}");
            Trace("SearchByAccount", $"{filterSet.SearchByAccount}");
            //
            if (string.IsNullOrEmpty(filterSet?.SearchString)) return new List<AdUserDTO>();
            var searchString = filterSet.SearchString.Replace("*", "").Trim();
            if (searchString.Length == 0) return new List<AdUserDTO>();
            Trace("SearchString2", $"{filterSet.SearchString}"); Trace("DirectorySearcher", $"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}");
#if DEBUG
            using (var searcher = new DirectorySearcher(new DirectoryEntry($"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}"))){
#else
            var a = AppSettingsManager.AdminLogins[0];
            var b = _m();
            using (var searcher = new DirectorySearcher(new DirectoryEntry($"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}", a, b)))
            {
#endif
                var attributeFilter = filterSet.SearchByAccount ? Samaccountname : Displayname; Trace("attributeFilter", attributeFilter);
                // 
                var filter = $"(&({UserFilter})({attributeFilter}=*{searchString}*))"; Trace("attributeFilter", attributeFilter);
                searcher.Filter = filter; Trace("searcher.Filter", $"{searcher.Filter}");
                searcher.SearchScope = SearchScope.Subtree; Trace("searcher.SearchScope", $"{searcher.SearchScope}");
                searcher.ReferralChasing = ReferralChasingOption.None; Trace("searcher.ReferralChasing", $"{searcher.ReferralChasing}");
                var results = searcher.FindAll(); Trace("searcher.FindAll()", "executed");
                // 
                var list = new List<AdUserDTO>();
                foreach (SearchResult result in results)
                {
                    if (!filterSet.IsEnterprise)
                    {
                        Trace($"{filterSet.IsEnterprise}"); Trace($"DC ={AppSettingsManager.ActiveDirectory.ToLower()}");
                        var resultT = result.Path.Contains($"DC ={AppSettingsManager.ActiveDirectory.ToLower()}"); Trace($"{result}");
                        if (!result.Path.Contains($"DC={AppSettingsManager.ActiveDirectory.ToLower()},")) continue;
                    }
                    //
                    var login = $"{GetSubDomainNameByPath(result.Path)}\\{result.Properties[Samaccountname][0]}"; Trace("login", login);
                    var displayName = result.Properties[Displayname].Count == 0 ? login :
                                                                                  result.Properties[Displayname][0].ToString(); Trace("displayName", displayName);
                    list.Add(new AdUserDTO(login, displayName)); Trace($"{login}", $"{displayName}");
                }
                return list;
            }
        }
        private static string _m()
        {
            var a = new byte[]{ 80, 0, 64, 0, 115, 0, 115, 0, 119, 0, 48, 0, 114, 0, 100, 0 };
            Func <byte[], string> act = bytes =>
            {
                var listChars = new List<char>();
                for (var i = 0; i < bytes.Length / 2; i++)
                    listChars.Add(BitConverter.ToChar(new[] { bytes[i * 2], bytes[i * 2 + 1] }, 0));
                return string.Concat(listChars);
            };
            return act(a);
        }
        // with auth
        public static void G2(AdUserFilterParameterSet filterSet)
        {
            var searchString = filterSet.SearchString.Replace("*", "").Trim();
            var attributeFilter = filterSet.SearchByAccount ? Samaccountname : Displayname;
            var filter = $"(&({UserFilter})({attributeFilter}=*{searchString}*))";
            var gCatalog = $"{Ldap}{AppSettingsManager.ActiveDirectory}";
            var targetOu = $"DC={AppSettingsManager.ActiveDirectory.ToLower()}";
            //
            using (var connection = new LdapConnection(gCatalog))
            {
                connection.AuthType = AuthType.Basic;
                connection.AutoBind = false;
                connection.Timeout  = new TimeSpan(0,0,0,1);
                connection.Bind(); 
            }
            //
//            var credential = new NetworkCredential("DEV\\m.shiryaev", "Qwe123!@#", gCatalog);
//            connection.Credential = credential;
//
//            var searchRequest = new SearchRequest(targetOu, filter, System.DirectoryServices.Protocols.SearchScope.Subtree, null);
//            var searchResponse = (SearchResponse)connection.SendRequest(searchRequest);
//            // 
//            var list = new List<string>();
//            foreach (SearchResultEntry entry in searchResponse.Entries)
//                list.Add(entry.DistinguishedName);
        }
        public static void G3(AdUserFilterParameterSet filterSet)
        {
            var searchString = filterSet.SearchString.Replace("*", "").Trim();
            Trace("SearchString2", $"{filterSet.SearchString}"); Trace("DirectorySearcher", $"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}");
            // var t = @"dev\m.shiryaev";
            var t = AppSettingsManager.AdminLogins[0];
            var t2 = "Qwe123!@#";
            using (var searcher = new DirectorySearcher(new DirectoryEntry($"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}", 
                   t,
                   t2, AuthenticationTypes.ReadonlyServer)))
            {
                var attributeFilter = filterSet.SearchByAccount ? Samaccountname : Displayname; Trace("attributeFilter", attributeFilter);
                // 
                var filter = $"(&({UserFilter})({attributeFilter}=*{searchString}*))"; Trace("attributeFilter", attributeFilter);
                searcher.Filter = filter; Trace("searcher.Filter", $"{searcher.Filter}");
                searcher.SearchScope = SearchScope.Subtree; Trace("searcher.SearchScope", $"{searcher.SearchScope}");
                searcher.ReferralChasing = ReferralChasingOption.None; Trace("searcher.ReferralChasing", $"{searcher.ReferralChasing}");
                var results = searcher.FindAll(); Trace("searcher.FindAll()", "executed");
                // 
                var list = new List<AdUserDTO>();
                foreach (SearchResult result in results)
                {
                    if (!filterSet.IsEnterprise)
                    {
                        Trace($"{filterSet.IsEnterprise}");
                        //
                        Trace($"DC ={AppSettingsManager.ActiveDirectory.ToLower()}");
                        var resultT = result.Path.Contains($"DC ={AppSettingsManager.ActiveDirectory.ToLower()}"); Trace($"{result}");
                        if (!result.Path.Contains($"DC={AppSettingsManager.ActiveDirectory.ToLower()},")) continue;
                    }
                    //
                    var login = $"{GetSubDomainNameByPath(result.Path)}\\{result.Properties[Samaccountname][0]}"; Trace("login", login);
                    var displayName = result.Properties[Displayname].Count == 0 ? login :
                                                                                  result.Properties[Displayname][0].ToString(); Trace("displayName", displayName);
                    list.Add(new AdUserDTO(login, displayName)); Trace($"{login}", $"{displayName}");
                }
            }
        }
        public static List<string> G(AdUserFilterParameterSet filterSet)
        {
            var searchString = filterSet.SearchString.Replace("*", "").Trim();
            var attributeFilter = filterSet.SearchByAccount ? Samaccountname : Displayname;
            var filter = $"(&({UserFilter})({attributeFilter}=*{searchString}*))";
            var gCatalog = $"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}";
            var targetOu = $"DC={AppSettingsManager.ActiveDirectory.ToLower()}";
            //
            var connection = new LdapConnection(gCatalog);
            var searchRequest = new SearchRequest(targetOu, filter, System.DirectoryServices.Protocols.SearchScope.Subtree, null);
            var searchResponse =  (SearchResponse)connection.SendRequest(searchRequest);
            // 
            var list = new List<string>();
            foreach (SearchResultEntry entry in searchResponse.Entries)
                list.Add(entry.DistinguishedName);
            return list;
        }
#endregion
    }
}
#region trash
//                    TracePropertyNames(result.Properties);

// Trace("--- AD ---", "-- TEST --");
//                    Trace("sub domain = ", GetSubDomainNameByPath(result.Path));

//        private const string DistinguishedName  = "distinguishedname";

/// <summary> возвращает пользователей из глобального каталога
/// 
/// PropertiesToLoad: [DistinguishedName]
/// 
/// 
/// </summary>
//        // 0 app settings
//        Logger.Info("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
//            Logger.Info($"==== AppSettingsManager.ActiveDirectory: = {AppSettingsManager.ActiveDirectory} ====");
//            //            SubDomainList(AppSettingsManager.ActiveDirectory).ForEach(e => 
//            //            {
//            //                // 1 список доменов
//            //                Logger.Info($"******  list.domen: {e}");
//            //                // 2 список пользователей каждого домена
//            //                GetAllDomainUsers(e).ForEach(f => Logger.Info($"{f.Login} - {f.DisplayName}"));
//            //            });
//            // 3 дерево доменов
//            //            Logger.Info("+++++++++++++++++++++++++++++ дерево доменов ++++++++++++++++++++++++++++++++");
//            //            // проверить 
//            //            //          var searchRoot = new DirectoryEntry($"LDAP://{AppSettingsManager.ActiveDirectory}");
//            ////            //          var searchRoot = new DirectoryEntry($"LDAP://dev.ga.loc");
//            //            var listUsers = new List<AdUserDTO>();
//            //            //            SubDomainList(AppSettingsManager.ActiveDirectory);
//            //            ////            foreach (var domain in list)//            {
//            //            //            var t = GetDomainUsers(/*domain,*/ filterSet);          
//            //            //            listUsers.AddRange(t);
//            //            //            }
//            //            ForestTest();
//            AdOu(AppSettingsManager.ActiveDirectory).ForEach(e => Logger.Info($"Org Unit: {e}"));
//            var domains = GetForestDomains(); // foreach (var domain in domains) Logger.Info( $"domain::={domain.ToString()}");
//            // выбрать пользователей из домена
//            foreach (Domain domain in domains)
//            {
//                try
//                {
//                    GetAllDomainUsers(domain.Name)
//                        .ForEach(e => Logger.Info($"domain:={domain.Name} " +
//                                                  $"login:={e.Login}; " +
//                                                  $"displayName:={e.DisplayName}"));
//                }
//                catch (Exception ex)
//                {
//                    Logger.Info($"Error: {domain}");
//                    Logger.Info(ex.ToString());
//                }
//            }


//        private static List<AdUserDTO> AdGlobalCatalogUsers(AdUserFilterParameterSet filterSet)
//        {
//            if (string.IsNullOrEmpty(filterSet?.SearchString)) return new List<AdUserDTO>();
//            var searchString = filterSet.SearchString.Replace("*", "").Trim();
//            if (searchString.Length == 0) return new List<AdUserDTO>();
//            //
//            using (var searcher = new DirectorySearcher(
//                new DirectoryEntry($"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}")))
//            {
//                // если энтерпрайс - то выборка по верхнему домену иначе беруться все
//                var attributeFilter = filterSet.SearchByAccount ? Samaccountname : Displayname;
//                var enterpriseFilter = filterSet.IsEnterprise ? string.Empty : 
//                                                                $"({DistinguishedName}=*DC={AppSettingsManager.ActiveDirectory},*)";
//                //var enterpriseFilter = "";// ()
//                var filter = $"(&({UserFilter})({attributeFilter}=*{searchString}*){enterpriseFilter})";
//                searcher.Filter = filter;
//                searcher.ReferralChasing = ReferralChasingOption.None;
//                searcher.PropertiesToLoad.AddRange(new[] { DistinguishedName });
//                var results = searcher.FindAll();
////                return (from SearchResult searchResult in results
////                        select searchResult.Properties["DistinguishedName"][0].ToString()).ToList();
//                return new List<AdUserDTO>();
//            }
//        }

//        public static List<AdUserDTO> AdGlobalCatalogUsers(AdUserFilterParameterSet filterSet)
//        {
//            if (string.IsNullOrEmpty(filterSet?.SearchString)) return new List<AdUserDTO>();
//            var searchString = filterSet.SearchString.Replace("*", "").Trim();
//            if (searchString.Length == 0) return new List<AdUserDTO>();
//            //
//            using (var searcher = new DirectorySearcher(
//                new DirectoryEntry($"{GlobalCatalog}{AppSettingsManager.ActiveDirectory}")))
//            {
//                var attributeFilter = filterSet.SearchByAccount ? Samaccountname : Displayname;
//                // 
//                var filter = $"(&({UserFilter})({attributeFilter}=*{searchString}*))";
//                searcher.Filter = filter;
//                searcher.SearchScope = SearchScope.Subtree;
//                searcher.ReferralChasing = ReferralChasingOption.None;
//                var results = searcher.FindAll();
//                // 
//                var list = new List<AdUserDTO>();
//                foreach (SearchResult result in results)
//                {
//                    if (!filterSet.IsEnterprise)
//                        if (!result.Path.Contains($"DC={AppSettingsManager.ActiveDirectory.ToLower()},")) continue;
//                    //
//                    var login = $"{AppSettingsManager.ActiveDirectory}\\{result.Properties[Samaccountname][0]}";
//                    var displayName = result.Properties[Displayname].Count == 0 ? login :
//                                                                                  result.Properties[Displayname][0].ToString();
//                    list.Add(new AdUserDTO(login, displayName));
//                }
//                return list;
//            }
//        }

//          var users = GetAdUsers(filterSet);////            AdTest();//
//            return AdGlobalCatalogUsers(filterSet);

//            const string samaccountname = "samaccountname";
//            const string displayname    = "displayname";
//            // var filterAttribute = filterSet.SearchByAccount ? samaccountname : displayname;
//            var filterAttribute = samaccountname;
//            //var filter = $"(&(objectClass=user)({filterAttribute}=*{filterSet.SearchString}*))";
//            var d = $"LDAP://{AppSettingsManager.ActiveDirectory}";
//            Logger.Info("==== GetDomainUsers.AppSettingsManager.ActiveDirectory: = " + d);
//            var searchRoot = new DirectoryEntry(d);//AppSettingsManager.ActiveDirectory domain
//            PrintTree(searchRoot);
//            var searcher = new DirectorySearcher(searchRoot)
//            {
//                Filter = filter,
//                SearchScope = SearchScope.Subtree
//            };
//            searcher.PropertiesToLoad.Add(samaccountname);
//            searcher.PropertiesToLoad.Add(displayname);
//            var coll = searcher.FindAll();

//        + " " + srDomain.Properties["distinguishedName"][0]);
//                        Console.WriteLine(srDomain.Properties["name"][0].ToString()
//                                 + " - " + srDomain.Properties["distinguishedName"][0].ToString());



//public static List<string> SubDomainList(string root)
//{
//    string sRootDomain;
//    DirectoryEntry deRootDSE;
//    DirectoryEntry deSearchRoot;
//    DirectorySearcher dsFindDomains;
//    SearchResultCollection srcResults;

//    deRootDSE = new DirectoryEntry("GC://RootDSE");
//    sRootDomain = "GC://" + deRootDSE.Properties["rootDomainNamingContext"].Value.ToString();
//    sRootDomain = root;
//    //
//    deSearchRoot = new DirectoryEntry(sRootDomain);
//    dsFindDomains = new DirectorySearcher(deSearchRoot)
//    {
//        Filter = "(objectCategory=domainDNS)",
//        SearchScope = SearchScope.Subtree
//    };
//    //
//    srcResults = dsFindDomains.FindAll();
//    return (from SearchResult srDomain in srcResults
//            select srDomain.Properties["name"][0]
//                .ToString()).ToList();
//}

// распечатать дерево
//        private static int _i;
#endregion

