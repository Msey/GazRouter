using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using GazRouter.DTO.Authorization.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DataServices.InfrastructureTest
{
    [TestClass]
    public class AdUsersTest
    {
        [TestMethod]public void GetAdUsersTest()
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
                {
                    continue;
                }
                string displayName = z.Properties[displayname][0].ToString();
                result.Add(new AdUserDTO(login, displayName));
            }
        }
        [TestMethod]public void GetAdUsersTest2()
        {


            
        }
        public List<AdUserDTO> GetAdUsers(AdUserFilterParameterSet filterSet)
        {
            if (string.IsNullOrEmpty(filterSet?.SearchString)) return new List<AdUserDTO>();

            var searchString = filterSet.SearchString.Replace("*", "").Trim();
            if (searchString.Length == 0) return new List<AdUserDTO>();
            //              
            const string samaccountname = "samaccountname";
            const string displayname    = "displayname";
            var filterAttribute = filterSet.SearchByAccount ? samaccountname : displayname;
            var filter = $"(&(objectClass=user)({filterAttribute}=*{searchString}*))";
            var searchRoot = new DirectoryEntry($"LDAP://{AppSettingsManager.ActiveDirectory}");
            //
            var seach = new DirectorySearcher(searchRoot) { Filter = filter };
            seach.PropertiesToLoad.Add(samaccountname);
            seach.PropertiesToLoad.Add(displayname);
            var coll = seach.FindAll();
            var result = new List<AdUserDTO>();
            foreach (SearchResult z in coll)
            {
                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[samaccountname][0]}";
                if (z.Properties[displayname].Count == 0) continue;
                //
                var displayName = z.Properties[displayname][0].ToString();
                result.Add(new AdUserDTO(login, displayName));
            }
            return result;
        }
        [TestMethod]public void GetAdUsersFullTree()
        {
            // var searchRoot = new DirectoryEntry($"LDAP://{AppSettingsManager.ActiveDirectory}");
            var searchRoot = new DirectoryEntry($"LDAP://DEV");
//            var defaultContext = searchRoot.Properties["defaultNamingContext"][0].ToString();
            
            PrintTree(searchRoot);

            var seach = new DirectorySearcher(searchRoot) { Filter = "(&(objectClass=user))" };
            const string samaccountname = "samaccountname";
            const string displayname = "displayname";

            seach.PropertiesToLoad.Add(samaccountname);
            seach.PropertiesToLoad.Add(displayname);
            seach.SearchScope = SearchScope.Subtree;

            var coll = seach.FindAll();
            var result = new List<AdUserDTO>();
            foreach (SearchResult z in coll)
            {
                string login = $"{AppSettingsManager.ActiveDirectory}\\{z.Properties[samaccountname][0]}";
                if (z.Properties[displayname].Count == 0) continue;
                // 
                var displayName = z.Properties[displayname][0].ToString();
                result.Add(new AdUserDTO(login, displayName));
            }
//            return result;
        }
        private static void PrintTree(DirectoryEntry entry)
        {
            var count = 0;
            Action<DirectoryEntry> getCount = e => { count += e.Children.Cast<object>().Count(); };            
            getCount.Invoke(entry);
            Action<DirectoryEntry> action = e =>
            {
                if (count > 0) Debug.WriteLine("==****=== count = " + count + " " + e.Name + "; " + /*e.Parent.Name + ", " + */ e.Username + "; " + e.Path); 
                else Debug.WriteLine( e.Name + "; " + /*e.Parent.Name + ", " + */ e.Username + "; " + e.Path);
            };
            action.Invoke(entry);
            foreach (DirectoryEntry e in entry.Children) PrintTree(e);
        }
    }
    internal class AppSettingsManager
    {
        public const string ActiveDirectory = "DEV";



    }
}
