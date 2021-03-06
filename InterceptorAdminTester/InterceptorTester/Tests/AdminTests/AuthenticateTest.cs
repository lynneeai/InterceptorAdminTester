﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;
using System.Configuration;
using Nito.AsyncEx;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using ConsoleApplication1;

namespace InterceptorTester.Tests.AdminTests
{
	[TestFixture()]
	public class AuthenticateTest
    {
        public static JObject sessionToken;

        [TestFixtureSetUp()]
        public void testSetup()
        {
            TestGlobals.setup();
        }

		[Test()]
        public static void generateSessionToken()
		{
            AuthenticateJSON json = new AuthenticateJSON();
            //Set up JSON
            json.userID = TestGlobals.username;
            Console.WriteLine("username: " + TestGlobals.username);
            json.password = TestGlobals.password;
            Console.WriteLine("password: " + TestGlobals.password);
            Authenticate authCall = new Authenticate(TestGlobals.adminServer, json);
			Console.WriteLine (TestGlobals.adminServer);
            Test authTest = new Test(authCall);
            AsyncContext.Run(async () => await new HTTPSCalls().runTest(authTest, HTTPOperation.POST));
            string statusCode = HTTPSCalls.result.Key.GetValue("StatusCode").ToString();

            Console.WriteLine(statusCode);
            if (!statusCode.Equals("201"))
            {
                Console.WriteLine(HTTPSCalls.result.Key.ToString());
                Console.WriteLine(HTTPSCalls.result.Value);
            }
            sessionToken = JObject.Parse(HTTPSCalls.result.Value);
            Console.WriteLine(sessionToken.ToString());
            Assert.AreEqual("201", statusCode);

        }

        [Test()]
        public void closeSession()
        {
            if (sessionToken != null)
            {
                GenericRequest req = new GenericRequest(TestGlobals.adminServer, "/api/authenticate/", null);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = getSessionToken();
                Test closeTest = new Test(req);
                AsyncContext.Run(async () => await new HTTPSCalls().runTest(closeTest, HTTPOperation.DELETE, client));
            }
        }

		public static AuthenticationHeaderValue getSessionToken()   
		{
            if (sessionToken == null)
            {
                generateSessionToken();
            }
            if (sessionToken.GetValue("sessionToken").Equals(null))
            {
                generateSessionToken();
            }
            if (sessionToken.GetValue("sessionToken").ToString().Equals(null))
            {
                generateSessionToken();
            }
            string parse = "Token " + sessionToken.GetValue("sessionToken").ToString();
            Console.WriteLine(parse);
            AuthenticationHeaderValue ret = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(parse);
            return ret;
        }

	}
}