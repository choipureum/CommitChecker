using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Gitalarmi
{
    class gitAPI
    {

        private static string path = @"../../key.txt";
        private static string[] apiKey = File.ReadAllLines(path);
        private static string accessToken = apiKey[3];
        private static string[] ids = { "choipureum" };
        public string today = DateTime.Now.ToString(@"dd'/'MM'/'yyyy");

        /// <summary>
        /// 레포지토리 리스트 출력
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public dynamic GetRepoList(string username)
        {
            Object obj = null;
            try
            {
                HttpWebRequest objWRequest = (HttpWebRequest)System.Net.WebRequest.Create("https://api.github.com/users/" + username + "/repos");
                objWRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                objWRequest.Headers.Add("Authorization", "Token:" + accessToken);
                HttpWebResponse objWResponse = (HttpWebResponse)objWRequest.GetResponse();
                Stream stream = objWResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();
                stream.Close();
                objWResponse.Close();
                //역직렬화
                obj = JsonConvert.DeserializeObject(result);
            }
            catch (Exception e) { }
            if (obj is string)

            {
                return obj as string;
            }
            else
            {
                return GetDynamicObject(obj as JToken);
            }
        }
        /// <summary>
        /// 모든 레포지토리 커밋 출력
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        public dynamic GetRepoCommit(string repo)
        {
            Object obj = null;
            try
            {
                HttpWebRequest objWRequest = (HttpWebRequest)System.Net.WebRequest.Create("https://api.github.com/repos/" + repo + "/commits");
                objWRequest.Headers.Add("Authorization", "Token:" + accessToken);
                objWRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                HttpWebResponse objWResponse = (HttpWebResponse)objWRequest.GetResponse();
                Stream stream = objWResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();
                stream.Close();
                objWResponse.Close();
                //역직렬화
                obj = JsonConvert.DeserializeObject(result);
            }
            catch (Exception) { }
            if (obj is string)

            {
                return obj as string;
            }
            else
            {
                return GetDynamicObject(obj as JToken);
            }

        }
        /// <summary>
        /// 모든 레포지토리 커밋 출력
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        public string GetRepoCommitStr(string repo)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest objWRequest = (HttpWebRequest)System.Net.WebRequest.Create("https://api.github.com/repos/" + repo + "/commits");
                objWRequest.Headers.Add("Authorization", "Token:" + accessToken);
                objWRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";
                HttpWebResponse objWResponse = (HttpWebResponse)objWRequest.GetResponse();
                Stream stream = objWResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                result = reader.ReadToEnd();
                stream.Close();
                objWResponse.Close();
            }
            catch (Exception) { }
            return result;
        }
        /// <summary>
        /// 개똥아 commit 했니 오늘?
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CommitYN(string username)
        {
            //ids에서 username 출력 -> 모든 레포 뽑기
            for (int i = 0; i < ids.Count(); i++)
            {
                List<ExpandoObject> repoList = GetRepoList(ids[i]);

                //모든 레포에서 반복해서 commit 리스트 출력 보여주기
                for (int j = 0; j < repoList.Count(); j++)
                {
                    try
                    {
                        foreach (KeyValuePair<string, object> k in repoList[j])
                        {
                            if (k.Key.Trim().Equals("full_name"))
                            {
                                Console.WriteLine(k.Value);
                                string repo = k.Value.ToString();
                                string repoStr = GetRepoCommitStr(repo);
                                var jarr = JArray.Parse(repoStr);
                                string jo = (string)jarr.First["commit"]["author"]["date"];
                                //오늘날짜 커밋일시
                                if (today.Equals(jo.Substring(0, 10).Trim()))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// dynamic JSON 받기
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static dynamic GetDynamicObject(JToken token)
        {
            if (token is JValue)
            {
                return (token as JValue).Value;
            }

            else if (token is JObject)
            {
                ExpandoObject expandoObject = new ExpandoObject();
                (from childToken in token where childToken is JProperty select childToken as JProperty).ToList().ForEach
                (
                    property =>
                    {
                        ((IDictionary<string, object>)expandoObject).Add(property.Name, GetDynamicObject(property.Value));
                    }
                );
                return expandoObject;

            }
            else if (token is JArray)
            {
                List<ExpandoObject> list = new List<ExpandoObject>();

                foreach (JToken item in token as JArray)
                {
                    list.Add(GetDynamicObject(item));
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// SMS 보내기
        /// </summary>
        /// <param name="username"></param>
        /// <param name="toPhone"></param>
        /// <returns></returns>
        public string SendSMS(string username, string toPhone)
        {
            string TWILIO_ACCOUNT_SID = apiKey[0];
            string TWILIO_AUTH_TOKEN = apiKey[1];
            string fromPhone = apiKey[2];

            string accountSid = TWILIO_ACCOUNT_SID;
            string authToken = TWILIO_AUTH_TOKEN;

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "제발 git 커밋하세요 \n -빅브라더 최푸름",
                from: new Twilio.Types.PhoneNumber(fromPhone),
                to: new Twilio.Types.PhoneNumber(toPhone)
            );

            return message.Status.ToString();
        }
    }
}
