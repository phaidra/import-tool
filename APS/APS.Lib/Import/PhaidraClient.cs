using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using APS.Lib.Helper;
using System.Reflection;

namespace APS.Lib.Import
{
    public class PhaidraClient
    {
        private HttpClient _client;
        private string _username;
        private string _password;
        private string _url;
        private string _searchEngineUrl;
        private const string CONST_Rest_Languages = "languages";
        private const string CONST_Rest_picture_create = "picture/create";
        private const string CONST_Rest_create = "/create";
        private const string CONST_Rest_collection_create = "collection/create";
        private const string CONST_Rest_object_delete = "object/{PID}/delete";
        private const string CONST_Rest_collection_members_add = "collection/{PID}/members/add";
        private const string CONST_Rest_signin = "signin";
        private const string CONST_Rest_signout = "signout";
        private const string CONST_Header_X_XSRF_TOKEN = "X-XSRF-TOKEN";
        private const string CONST_Header_XSRF_TOKEN = "XSRF-TOKEN";
        private const string CONST_SearchSystemTag = "select?indent=on&q=systemtag:{SYSTEMTAG}&wt=json";
        private const string CONST_Placeholder_PID = "{PID}";
        private const string CONST_Placeholder_SYSTEMTAG = "{SYSTEMTAG}";

        private Dictionary<string, string> _collectionCache;

        public PhaidraClient(string url, string searchEngineUrl, string username, string password)
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromHours(6);
            _url = (url ?? "").TrimEnd('/') + '/';
            _searchEngineUrl = (searchEngineUrl ?? "").TrimEnd('/') + '/';
            _username = username;
            _password = password;
            _collectionCache = new Dictionary<string, string>();
        }
        public async Task<List<string>> GetLanguages()
        {
            List<string> languages = new List<string>();
            try
            {
                var client = GetClient(false);
                JObject obj = await GetRequest(false, CONST_Rest_Languages);

                var token = obj.SelectToken("languages");

                foreach (JProperty child in token.Children())
                {
                    languages.Add(child.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in get languages: {ex.ToString()}");
                Debug.WriteLine(ex.ToString());
            }

            return languages;
        }

        //public async Task<bool> UploadImageFileWithJsonFileMetadata(string filePath, string metadataPath)
        //{
        //    bool success = false;
        //    try
        //    {
        //        //var httpClientHandler = new HttpClientHandler()
        //        //{
        //        //    Proxy = new WebProxy("proxyAddress", "proxyPort")
        //        //    {
        //        //        Credentials = CredentialCache.DefaultCredentials
        //        //    },
        //        //    PreAuthenticate = true,
        //        //    UseDefaultCredentials = true
        //        //};

        //        FileInfo fileFI = new FileInfo(filePath);
        //        FileInfo metadataFI = new FileInfo(metadataPath);

        //        var fileContent = new StreamContent(fileFI.OpenRead())
        //        {
        //            Headers = { ContentType = new MediaTypeHeaderValue("application/octet-stream") }
        //        };
        //        fileContent.Headers.Add("Content-Disposition", string.Format("form-data; name=\"file\"; filename=\"file{0}\"", System.IO.Path.GetExtension(fileFI.Name)));

        //        var metadataContent = new StringContent(System.IO.File.ReadAllText(metadataFI.FullName));
        //        metadataContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //        metadataContent.Headers.Add("Content-Disposition", "form-data; name=\"metadata\"; filename=\"metadata.json\"");


        //        //var metadataContent = new StreamContent(metadataFI.OpenRead())
        //        //{
        //        //    Headers = { ContentType = new MediaTypeHeaderValue("application/octet-stream") },
        //        //};

        //        //metadataContent.Headers.Add("Content-Disposition", "form-data; name=\"metadata\"; filename=\"metadata.json\"");

        //        //var metadataContent = new StreamContent(metadataFI.OpenRead())
        //        //{
        //        //    Headers = { ContentLength = metadataFI.Length, ContentType = new MediaTypeHeaderValue("application/octet-stream") }
        //        //};

        //        var formDataContent = new MultipartFormDataContent();
        //        formDataContent.Add(fileContent, "file", fileFI.Name);
        //        formDataContent.Add(metadataContent, "metadata", metadataFI.Name);

        //        var client = GetClient(auth: true);

        //        client.DefaultRequestHeaders.Add("Expect", "100-continue");

        //        using (var res = await client.PostAsync(GetRequestUrl(CONST_Rest_picture_create), formDataContent))
        //        {
        //            string responseJson = await res.Content.ReadAsStringAsync();
        //            success = res.IsSuccessStatusCode;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogE($"Error in get UploadImageWithJsonFileMEsa: {ex.ToString())}");
        //        Debug.WriteLine(ex.ToString());
        //        return (success = false);
        //    }
        //    return success;
        //}

        public async Task<bool> SigninTest()
        {
            var signinResponse = await GetRequest(true, CONST_Rest_signin);
            if (signinResponse != null)
            {
                string token = signinResponse[CONST_Header_XSRF_TOKEN];
                if (!string.IsNullOrEmpty(token))
                {
                    await SignOut(token);
                    return true;
                }
            }
            return false;
        }

        private async Task SignOut(string token)
        {
            var additionalHeaders = new Dictionary<string, string> { { CONST_Header_X_XSRF_TOKEN, token } };
            await GetRequest(false, CONST_Rest_signout, additionalHeaders);
        }

        public async Task<UploadFileResult> UploadFileWithJsonMetadata(string filePath, string metadataJson, string fileMimeType, string phaidraType)
        {
            UploadFileResult result = new UploadFileResult();
            result.Success = false;
            string restMethod = phaidraType + CONST_Rest_create;
            //string fileMimeType = "image/png";
            //string metadataMimeType = "application/octet-stream";
            string multipartMimeType = "application/octet-stream";
            try
            {
                FileInfo fileFI = new FileInfo(filePath);

                string md5Checksum = MD5Helper.CreateMD5(filePath);

                var fileContent = new StreamContent(fileFI.OpenRead())
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(multipartMimeType) }
                };
                fileContent.Headers.Add("Content-Disposition", string.Format("form-data; name=\"file\"; filename=\"file{0}\"", System.IO.Path.GetExtension(fileFI.Name)));

                var formDataContent = new MultipartFormDataContent();
                formDataContent.Add(fileContent, "file", fileFI.Name);

                AddStringContent(formDataContent, "metadata", metadataJson);
                AddStringContent(formDataContent, "mimetype", fileMimeType);
                AddStringContent(formDataContent, "checksumtype", "MD5");
                AddStringContent(formDataContent, "checksum", md5Checksum);

                var client = GetClient(auth: true);

                client.DefaultRequestHeaders.Add("Expect", "100-continue");

                using (var res = await client.PostAsync(GetRequestUrl(restMethod), formDataContent))
                {
                    string responseJson = await res.Content.ReadAsStringAsync();
                    result.Success = res.IsSuccessStatusCode;

                    JObject response = (JObject)JsonConvert.DeserializeObject(responseJson);
                    result.Status = (long)((JValue)response.SelectToken("status")).Value;
                    if (result.Status == 200)
                    {
                        string pid = (string)((JValue)response.SelectToken("pid")).Value;
                        result.CreatedPid = pid;
                    }
                    else
                    {
                        Logger.LogE($"Success is false in UploadFileWithJsonMetadata.");
                        Logger.LogE($"Request: {metadataJson}");
                        Logger.LogE($"Response: {responseJson}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in UploadFileWithJsonMetadata: {ex.ToString()}");
                Debug.WriteLine(ex.ToString());
                result.Success = false;
                return result;
            }
            return result;
        }

        public async Task<CreateCollectionResult> CreateCollectionWithMetadata(string metadataJson)
        {
            CreateCollectionResult result = new CreateCollectionResult();

            result.Success = false;
            string restMethod = CONST_Rest_collection_create;
            //string metadataMimeType = "application/octet-stream";
            try
            {
                //var metadataContent = new StringContent(metadataJson);
                //metadataContent.Headers.ContentType = new MediaTypeHeaderValue(metadataMimeType);
                //metadataContent.Headers.Add("Content-Disposition", "form-data; name=\"metadata\"; filename=\"metadata.json\"");

                var formDataContent = new MultipartFormDataContent();
                //formDataContent.Add(metadataContent, "metadata", "metadata.json");

                AddStringContent(formDataContent, "metadata", metadataJson);

                var client = GetClient(auth: true);

                client.DefaultRequestHeaders.Add("Expect", "100-continue");

                using (var res = await client.PostAsync(GetRequestUrl(restMethod), formDataContent))
                {
                    string responseJson = await res.Content.ReadAsStringAsync();
                    result.Success = res.IsSuccessStatusCode;
                    if (result.Success)
                    {
                        JObject response = (JObject)JsonConvert.DeserializeObject(responseJson);
                        result.Status = (long)((JValue)response.SelectToken("status")).Value;
                        if (result.Status == 200)
                        {
                            string pid = (string)((JValue)response.SelectToken("pid")).Value;
                            result.CreatedPid = pid;
                        }
                    }
                    else
                    {
                        Logger.LogE($"Success is false in CreateCollectionWithMetadata.");
                        Logger.LogE($"Request: {metadataJson}");
                        Logger.LogE($"Response: {responseJson}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in CreateCollectionWithMetadata: {ex.ToString()}");
                Debug.WriteLine(ex.ToString());
                result.Success = false;
                return result;
            }
            return result;
        }

        public async Task<CreateCollectionResult> CreateCollection(string collectionName, string collectionPath)
        {
            string collectionMetadataJsonBaseFilename = System.IO.Path.Combine(new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName,
                "PhaidraAttributeFiles", "create-collection-base.json");
            string collectionMetadataJson = System.IO.File.ReadAllText(collectionMetadataJsonBaseFilename);

            JObject collectionMetadataObj = (JObject)JsonConvert.DeserializeObject(collectionMetadataJson);

            var collectionNameToken = collectionMetadataObj.SelectToken("metadata.json-ld.dce:title[0].bf:mainTitle[0].@value") as JValue;

            if (collectionNameToken != null)
            {
                collectionNameToken.Value = collectionName;
            }

            var jsonLdToken = collectionMetadataObj.SelectToken("metadata.json-ld") as JObject;

            if (jsonLdToken != null)
            {
                jsonLdToken.Add("phaidra:systemTag", new JArray(new object[] { collectionPath }));
            }

            collectionMetadataJson = JsonConvert.SerializeObject(collectionMetadataObj, Formatting.Indented);

            var result = await CreateCollectionWithMetadata(collectionMetadataJson);

            return result;
        }

        public async Task<CreateCollectionPathResult> CreateCollectionPath(string collectionPath)
        {
            CreateCollectionPathResult result = new CreateCollectionPathResult();

            string[] collectionPathParts = collectionPath.Trim('/').Split('/');
            string parentPid = "";
            for (int i = 0; i < collectionPathParts.Length; i++)
            {
                string path = "/" + string.Join("/", collectionPathParts.Take(i + 1));
                path.ToString();
                string pid = "";

                if (_collectionCache.ContainsKey(path))
                {
                    pid = _collectionCache[path];
                }
                else
                {
                    var findCollectionResult = await FindCollectionBySystemTag(path);
                    if (findCollectionResult.FoundPids.Count > 0)
                    {
                        _collectionCache[path] = findCollectionResult.FoundPids[0];
                        pid = _collectionCache[path];
                    }
                    else
                    {
                        var createCollectionResult = await CreateCollection(collectionPathParts[i], path);
                        _collectionCache[path] = createCollectionResult.CreatedPid;
                        pid = _collectionCache[path];
                        result.CreatedCollections[path] = pid;
                        if (!string.IsNullOrEmpty(parentPid))
                        {
                            var addMemberResult = await AddCollectionMembers(parentPid, new List<string> { pid });
                        }
                    }
                }

                result.Collections[path] = pid;
                result.CollectionPathPid = pid;
                parentPid = pid;
            }

            return result;
        }

        public async Task<AddCollectionMembersResult> AddCollectionMembers(string pid, List<string> memberPidsToAdd)
        {
            AddCollectionMembersResult result = new AddCollectionMembersResult();
            string restMethod = CONST_Rest_collection_members_add;
            try
            {
                restMethod = restMethod.Replace(CONST_Placeholder_PID, pid);
                string collectionMetadataJsonBaseFilename = System.IO.Path.Combine(new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName,
                    "PhaidraAttributeFiles", "collection-members-base.json");

                string collectionMembersMetadataJson = System.IO.File.ReadAllText(collectionMetadataJsonBaseFilename);
                JObject collectionMetadataObj = (JObject)JsonConvert.DeserializeObject(collectionMembersMetadataJson);

                JArray membersArray = (JArray)collectionMetadataObj.SelectToken("metadata.members");
                foreach (var memberPid in memberPidsToAdd)
                {
                    membersArray.Add(new JObject() { new JProperty("pid", memberPid) });
                }

                collectionMembersMetadataJson = JsonConvert.SerializeObject(collectionMetadataObj, Formatting.Indented);

                var formDataContent = new MultipartFormDataContent();

                AddStringContent(formDataContent, "metadata", collectionMembersMetadataJson);

                var client = GetClient(auth: true);

                client.DefaultRequestHeaders.Add("Expect", "100-continue");

                using (var res = await client.PostAsync(GetRequestUrl(restMethod), formDataContent))
                {
                    string responseJson = await res.Content.ReadAsStringAsync();
                    result.Success = res.IsSuccessStatusCode;
                    if (result.Success)
                    {
                        JObject response = (JObject)JsonConvert.DeserializeObject(responseJson);
                        result.Status = (long)((JValue)response.SelectToken("status")).Value;
                    }
                    else
                    {
                        Logger.LogE($"Success is false in AddCollectionMembers.");
                        Logger.LogE($"Request: {collectionMembersMetadataJson}");
                        Logger.LogE($"Response: {responseJson}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in AddCollectionMembers: {ex.ToString()}");
                Debug.WriteLine(ex.ToString());
                result.Success = false;
                return result;
            }

            return result;
        }

        //public async Task<RemoveCollectionMembersResult> RemoveCollectionMembers(string pid, List<string> memberPidsToRemove)
        //{
        //    RemoveCollectionMembersResult result = new RemoveCollectionMembersResult();

        //    return result;
        //}

        //public async Task<GetCollectionMembersResult> GetCollectionMembers(string pid)
        //{
        //    GetCollectionMembersResult result = new GetCollectionMembersResult();

        //    return result;
        //}


        //public async Task<bool> DeleteObject(string pidToDelete)
        //{
        //    bool success = false;
        //    string restMethod = CONST_Rest_object_delete;

        //    try
        //    {
        //        restMethod = restMethod.Replace(CONST_Placeholder_PID, pidToDelete);

        //        var client = GetClient(auth: true);

        //        client.DefaultRequestHeaders.Add("Expect", "100-continue");

        //        using (var res = await client.PostAsync(GetRequestUrl(restMethod), null))
        //        {
        //            string responseJson = await res.Content.ReadAsStringAsync();
        //            success = res.IsSuccessStatusCode;
        //            if (success)
        //            {
        //                JObject response = (JObject)JsonConvert.DeserializeObject(responseJson);
        //                long statusCode = (long)((JValue)response.SelectToken("status")).Value;
        //                if (statusCode == 200)
        //                {
        //                    success = true;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //        success = false;
        //        return success;
        //    }
        //    return success;
        //}

        public async Task<FindCollectionResult> FindCollectionBySystemTag(string systemTag)
        {
            //https://app01.cc.univie.ac.at:8983/solr/phaidra_sandbox/select?indent=on&q=systemtag:\/test1\/test2\/*&wt=json


            FindCollectionResult result = new FindCollectionResult();

            try
            {

                var client = GetClient(auth: true);

                client.DefaultRequestHeaders.Add("Expect", "100-continue");

                string findCollectionBySystemTagUrl = GetRequestUrlForFindCollectionBySystemTag(systemTag);

                using (var res = await client.GetAsync(findCollectionBySystemTagUrl))
                {
                    string responseJson = await res.Content.ReadAsStringAsync();
                    result.Success = res.IsSuccessStatusCode;
                    if (result.Success)
                    {
                        JObject response = (JObject)JsonConvert.DeserializeObject(responseJson);
                        long statusCode = (long)((JValue)response.SelectToken("responseHeader.status")).Value;
                        if (statusCode == 0)
                        {
                            result.Success = true;
                            result.NumFound = (long)((JValue)response.SelectToken("response.numFound")).Value;
                            JArray docs = (JArray)response.SelectToken("response.docs");
                            foreach (JObject doc in docs)
                            {
                                result.FoundPids.Add((string)((JValue)doc.SelectToken("pid")).Value);
                            }
                        }
                        else
                        {
                            Logger.LogE($"Success is false in FindCollectionBySystemTag.");
                            Logger.LogE($"Request url: {findCollectionBySystemTagUrl}");
                            Logger.LogE($"Response: {responseJson}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in FindCollectionBySystemTag: {ex.ToString()}");
                Debug.WriteLine(ex.ToString());
                result.Success = false;
                return result;
            }
            return result;
        }

        //public async Task<bool> UploadImageFile2(string filePath)
        //{
        //    bool success = false;
        //    try
        //    {
        //        FileInfo fileFI = new FileInfo(filePath);
        //        //FileInfo metadataFI = new FileInfo(metadataPath);

        //        var fileContent = new StreamContent(fileFI.OpenRead())
        //        {
        //            Headers = { ContentType = new MediaTypeHeaderValue("application/octet-stream") }
        //        };
        //        fileContent.Headers.Add("Content-Disposition", string.Format("form-data; name=\"file\"; filename=\"file{0}\"", System.IO.Path.GetExtension(fileFI.Name)));


        //        string metadataJson = "";

        //        //var ms = new MemoryStream();
        //        //TextWriter textWriter = new StreamWriter(ms);
        //        //JsonTextWriter writer = new JsonTextWriter(textWriter);
        //        //writer.

        //        //JObject obj = new JObject();

        //        var obj = CreateMetaData();

        //        string objStr = JsonConvert.SerializeObject(obj, Formatting.Indented);


        //        var metadataContent = new StringContent(metadataJson);
        //        metadataContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        //        metadataContent.Headers.Add("Content-Disposition", "form-data; name=\"metadata\"; filename=\"metadata.json\"");

        //        var formDataContent = new MultipartFormDataContent();
        //        formDataContent.Add(fileContent, "file", fileFI.Name);
        //        formDataContent.Add(metadataContent, "metadata", "metadata.json");


        //        var client = GetClient(auth: true);

        //        client.DefaultRequestHeaders.Add("Expect", "100-continue");

        //        using (var res = await client.PostAsync(GetRequestUrl(CONST_Rest_picture_create), formDataContent))
        //        {
        //            string responseJson = await res.Content.ReadAsStringAsync();
        //            success = res.IsSuccessStatusCode;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //        return (success = false);
        //    }
        //    return success;
        //}

        private async Task<dynamic> GetRequest(bool auth, string restMethod, Dictionary<string, string> additionalHeaders = null)
        {
            try
            {
                var client = GetClient(auth, additionalHeaders);
                string responseJson = await client.GetStringAsync(GetRequestUrl(restMethod));
                JObject obj = (JObject)JsonConvert.DeserializeObject(responseJson);
                return obj;
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in GetRequest: {ex.ToString()}");
                Debug.WriteLine(ex.ToString());
            }

            return null;
        }

        private HttpClient GetClient(bool auth, Dictionary<string, string> additionalHeaders = null)
        {
            _client.DefaultRequestHeaders.Clear();

            _client.DefaultRequestHeaders.Add("Accept", "*/*");
            _client.DefaultRequestHeaders.Add("User-Agent", "curl/7.55.1");

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            if (auth)
            {
                var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _username, _password));
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            else
            {
                _client.DefaultRequestHeaders.Authorization = null;
            }
            return _client;
        }

        private void AddStringContent(MultipartFormDataContent formDataContent, string name, string value)
        {
            var checksumContent = new StringContent(value);
            checksumContent.Headers.ContentType = null;
            checksumContent.Headers.ContentLength = null;
            checksumContent.Headers.Add("Content-Disposition", $"form-data; name=\"{name}\"");
            formDataContent.Add(checksumContent, name);
        }

        private string GetRequestUrl(string restMethod)
        {
            return string.Format("{0}{1}", _url, restMethod);
        }

        private string GetRequestUrlForFindCollectionBySystemTag(string systemTag)
        {
            string restMethod = CONST_SearchSystemTag.Replace(CONST_Placeholder_SYSTEMTAG, NormalizeSearchParameter(systemTag));
            return string.Format("{0}{1}", _searchEngineUrl, restMethod);
        }

        private string NormalizeSearchParameter(string systemTag)
        {
            return (systemTag ?? "").Replace("/", "\\/").Replace(" ", "\\ ");
        }
    }
}
