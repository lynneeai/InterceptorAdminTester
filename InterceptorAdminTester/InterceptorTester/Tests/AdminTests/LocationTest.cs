﻿using System;
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
using System.Net.Http;
using ConsoleApplication1;

namespace InterceptorTester.Tests.AdminTests
{
	[TestFixture()]
    public class LocationTest
    {
        public static KeyValuePair<JObject, string> locStore;

		public static string locIdCreated;
        public static string orgIdPassed;

        [TestFixtureSetUp()]
        public void setup()
        {
            TestGlobals.setup();
        }

		public static void createLoc(string orgId)
		{
			orgIdPassed = orgId;
			LocationJSON json = new LocationJSON(orgIdPassed, "suite", "street", "suddenValley", "um", "Murica", "A2A2A2");
			json.locDesc = "desc";
			json.locSubType = "subtype";
			json.locType = "type";
			Location newLoc = new Location(TestGlobals.adminServer, json);
			Test mTest = new Test(newLoc);
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken();
			AsyncContext.Run(async () => await new HTTPSCalls().runTest(mTest, HTTPOperation.POST, client));
			//Assert.AreEqual("201", HTTPSCalls.result.Value);
			Console.WriteLine(HTTPSCalls.result.Value);
			TestGlobals.locIdCreated = HTTPSCalls.result.Value.Substring(9, HTTPSCalls.result.Value.Length - 10);
			Console.WriteLine(HTTPSCalls.result.Value.Substring(9, HTTPSCalls.result.Value.Length - 10) + " Written to testGlobals");
		}

        [Test()]
        public static void createLocation()
        {
            orgIdPassed = OrganizationTest.getOrgId();
            LocationJSON json = new LocationJSON(orgIdPassed, "suite", "street", "suddenValley", "um", "Murica", "A2A2A2");
            json.locDesc = "desc";
            json.locSubType = "subtype";
            json.locType = "type";
			Location newLoc = new Location(TestGlobals.adminServer, json);
            Test mTest = new Test(newLoc);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken();
            AsyncContext.Run(async () => await new HTTPSCalls().runTest(mTest, HTTPOperation.POST, client));
            //Assert.AreEqual("201", HTTPSCalls.result.Value);
            Console.WriteLine(HTTPSCalls.result.Value);
            TestGlobals.locIdCreated = HTTPSCalls.result.Value.Substring(9, HTTPSCalls.result.Value.Length - 10);
            Console.WriteLine(HTTPSCalls.result.Value.Substring(9, HTTPSCalls.result.Value.Length - 10) + " Written to testGlobals");
        }

		[Test()]
		public static void invalidOrgID()
		{
			orgIdPassed = "invalid";
			LocationJSON json = new LocationJSON (orgIdPassed, "suite", "street", "suddenValley", "um", "Murica", "A2A2A2");
			json.locDesc = "desc";
			json.locSubType = "subtype";
			json.locType = "type";
			Location newLoc = new Location (TestGlobals.adminServer, json);
			Test mTest = new Test (newLoc);
			HttpClient client = new HttpClient ();
			client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken ();
			AsyncContext.Run (async () => await new HTTPSCalls().runTest (mTest, HTTPOperation.POST, client));
			string statusCode = HTTPSCalls.result.Key.Property ("StatusCode").Value.ToString ();
			Assert.AreEqual ("400", statusCode);
			locStore = HTTPSCalls.result;
		}

        [Test()]
        public void getSingleLocation()
        {
			string query = "/API/Location/" + TestGlobals.locIdCreated;
			GenericRequest getLoc = new GenericRequest(TestGlobals.adminServer, query, null);
            Test mTest = new Test(getLoc);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken();
            AsyncContext.Run(async () => await new HTTPSCalls().runTest(mTest, HTTPOperation.GET, client));
            string statusCode = HTTPSCalls.result.Key.GetValue("StatusCode").ToString();
            Assert.AreEqual("200", statusCode);
            locStore = HTTPSCalls.result;
        }

		[Test()]
		public void invalidLocID()
		{
			string query = "/API/Location/" + "invalid";
			GenericRequest getLoc = new GenericRequest (TestGlobals.adminServer, query, null);
			Test mTest = new Test (getLoc);
			HttpClient client = new HttpClient ();
			client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken ();
			AsyncContext.Run (async () => await new HTTPSCalls ().runTest (mTest, HTTPOperation.GET, client));
			string statusCode = HTTPSCalls.result.Key.GetValue ("StatusCode").ToString ();
			Assert.AreEqual ("404", statusCode);
			locStore = HTTPSCalls.result;
		}


		[Test()]
		public void getMultipleLocations()
		{
			string query = "/API/Location/?orgid=" + TestGlobals.orgIdCreated;
			GenericRequest getLoc = new GenericRequest(TestGlobals.adminServer, query, null);
			Test mTest = new Test(getLoc);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken();
            AsyncContext.Run(async () => await new HTTPSCalls().runTest(mTest, HTTPOperation.GET, client));
            string statusCode = HTTPSCalls.result.Key.GetValue("StatusCode").ToString();
            Assert.AreEqual("200", statusCode);
            locStore = HTTPSCalls.result;
		}

		[Test()]
		public void orgNotFound()
		{
			string query = "/API/Location??orgid=" + "invalid";
			GenericRequest getLoc = new GenericRequest (TestGlobals.adminServer, query, null);
			Test mTest = new Test(getLoc);
			HttpClient client = new HttpClient ();
			client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken ();
			AsyncContext.Run (async () => await new HTTPSCalls ().runTest (mTest, HTTPOperation.GET, client));
			string statusCode = HTTPSCalls.result.Key.GetValue ("StatusCode").ToString ();
			Assert.AreEqual ("400", statusCode);
			locStore = HTTPSCalls.result;
		}

		[Test()]
		public void removeLocation()
		{
			Console.WriteLine (TestGlobals.locIdCreated);
			string query = "/api/location/" + TestGlobals.locIdCreated;
			GenericRequest locReq = new GenericRequest(TestGlobals.adminServer, query, null);
			Test locTest = new Test(locReq);
			HttpClient client;

			client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken(); 
			AsyncContext.Run(async () => await new HTTPSCalls().runTest(locTest, HTTPOperation.DELETE, client));
			Console.WriteLine(HTTPSCalls.result.Value);
			createLocation ();
			Console.WriteLine (TestGlobals.locIdCreated);
		}

		[Test()]
		public void removingLocNotFound()
		{
			string query = "/api/location/" + "invalid";
			GenericRequest locReq = new GenericRequest (TestGlobals.adminServer, query, null);
			Test locTest = new Test (locReq);
			HttpClient client = new HttpClient ();
			client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken ();
			AsyncContext.Run (async () => await new HTTPSCalls ().runTest (locTest, HTTPOperation.DELETE, client));
			string statusCode = HTTPSCalls.result.Key.GetValue ("StatusCode").ToString ();
			Assert.AreEqual ("404", statusCode);
		}

		[Test()]
		public void intAssociated()
		{
			createLocation ();
			InterceptorTest.createInt (TestGlobals.locIdCreated);

			string query = "/api/location/" + TestGlobals.locIdCreated;
			GenericRequest locReq = new GenericRequest (TestGlobals.adminServer, query, null);
			Test locTest = new Test (locReq);
			HttpClient client = new HttpClient ();
			client.DefaultRequestHeaders.Authorization = AuthenticateTest.getSessionToken ();
			AsyncContext.Run (async () => await new HTTPSCalls ().runTest (locTest, HTTPOperation.DELETE, client));
			string statusCode = HTTPSCalls.result.Key.GetValue ("StatusCode").ToString ();
			Assert.AreEqual ("400", statusCode);
		}

        public static string getLocId()
        {
            if (TestGlobals.locIdCreated == null)
            {
                Console.WriteLine("Could not get loc ID from test globals");
                if (LocationTest.locIdCreated == null)
                {
                    Console.WriteLine("Could not get loc ID from loc test");
                    createLocation();
                    Console.WriteLine("Loc created");
                }
            }
            return TestGlobals.locIdCreated;
        }
    }
}