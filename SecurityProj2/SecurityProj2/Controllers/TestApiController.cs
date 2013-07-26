using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
