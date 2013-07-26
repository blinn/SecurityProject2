using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using SecurityProj2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SecurityProj2.Controllers
{
    public class TestApiController : ApiController
    {
        private string tfsTeamProjectCollectionUrl = "https://code.allegient.com/tfs/crm";
        // GET api/testapi
        public List<ProjectItems> GetTest2()
        {
            string usernameToImpersonate = "blinn";


            // OPTION 1: no impersonation.
            // Get an instance to TFS using the current thread's identity.
            // NOTE: The current thread's identity needs to have the "" permision or else you will receive
            //       a runtime SOAP exception: "Access Denied: [username] needs the following permission(s) to perform this action: Make requests on behalf of others"
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(tfsTeamProjectCollectionUrl));
            IIdentityManagementService identityManagementService = tfs.GetService<IIdentityManagementService>();

            // OPTION 2: impersonation.  Remove the following two lines of code if you don't need to impersonate.
            // Get an instance to TFS impersonating the specified user.
            // NOTE: This is not needed if the current thread's identity is that of the user 
            //       needed to impersonate. Simple use the ablve TfsTeamProjectCollection instance
            TeamFoundationIdentity identity = identityManagementService.ReadIdentity(IdentitySearchFactor.AccountName, usernameToImpersonate, MembershipQuery.None, ReadIdentityOptions.None);
            tfs = new TfsTeamProjectCollection(tfs.Uri, identity.Descriptor);

            string wiqlQuery = "Select * from Issue where [Area Path] = 'New College Grad Project' and [Work Item Type] = 'Task' ";
            WorkItemStore store = tfs.GetService<WorkItemStore>();
            WorkItemCollection queryResults = store.Query(wiqlQuery);
            // Determine if we are creating a new WorkItem or loading an existing WorkItem.
            List<ProjectItems> Objects = new List<ProjectItems>();
            foreach (WorkItem item in queryResults)
            {
                ProjectItems temp = new ProjectItems();
                temp.Title = item.Title;
                Objects.Add(temp);
            }
            return Objects;
        }

        // GET api/testapi
        public IEnumerable<string> GetTest()
        {
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(tfsTeamProjectCollectionUrl));
            foreach (var p in tfs.GetService<VersionControlServer>().GetAllTeamProjects(false))
                yield return p.Name;
        }

        // GET api/testapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/testapi
        public void Post([FromBody]string value)
        {
        }

        // PUT api/testapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/testapi/5
        public void Delete(int id)
        {
        }
    }
}
