using System;
using System.Net;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Util;

namespace FubuMVC.Core.Http
{
    public interface ICurrentRequest
    {
        /// <summary>
        ///   Full url of the request, never contains a trailing /
        /// </summary>
        /// <returns></returns>
        string RawUrl();

        /// <summary>
        ///   Url relative to the application
        /// </summary>
        /// <returns></returns>
        string RelativeUrl();

        /// <summary>
        ///   Base root of the application
        /// </summary>
        /// <returns></returns>
        string ApplicationRoot();

        string HttpMethod();
    }

    public interface IRequestHeaders
    {
        /// <summary>
        ///   Retrieve the value of a single Header property.  
        ///   The callback action will only be called if the Header
        ///   value exists
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "header"></param>
        /// <param name = "callback"></param>
        void Value<T>(string header, Action<T> callback);

        /// <summary>
        ///   Bind an object of type T to the data in the Headers
        ///   collection
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
        T BindToHeaders<T>();
    }

    public class RequestHeaders : IRequestHeaders
    {
        private readonly IObjectConverter _converter;
        private readonly AggregateDictionary _dictionary;
        private readonly IObjectResolver _resolver;

        public RequestHeaders(IObjectConverter converter, IObjectResolver resolver, AggregateDictionary dictionary)
        {
            _converter = converter;
            _resolver = resolver;
            _dictionary = dictionary;
        }

        public void Value<T>(string header, Action<T> callback)
        {
            _dictionary.Value(RequestDataSource.Header.ToString(), header, (name, value) =>
            {
                if (value == null)
                {
                    callback(default(T));
                }
                else
                {
                    var converted = _converter.FromString<T>(value.ToString());
                    callback(converted);
                }
            });
        }

        public T BindToHeaders<T>()
        {
            var data = _dictionary.DataFor(RequestDataSource.Header.ToString());
            var bindResult = _resolver.BindModel(typeof (T), data);

            bindResult.AssertNoProblems(typeof (T));

            return (T) bindResult.Value;
        }
    }

    public static class HttpHeaderNameExtensions
    {
        public static void Value<T>(this IRequestHeaders headers, HttpRequestHeader header, Action<T> callback)
        {
            headers.Value(header.ToName(), callback);
        }

        public static string ToName(this HttpRequestHeader header)
        {
            return HttpRequestHeaders.HeaderNameFor(header);
        }

        public static string ToName(this HttpResponseHeader header)
        {
            return HttpResponseHeaders.HeaderNameFor(header);
        }
    }


    public class HttpGeneralHeaders
    {
        public static readonly string CacheControl = "Cache-Control";
        public static readonly string Connection = "Connection";
        public static readonly string Date = "Date";
        public static readonly string Pragma = "Pragma";
        public static readonly string Trailer = "Trailer";
        public static readonly string TransferEncoding = "Transfer-Encoding";
        public static readonly string Upgrade = "Upgrade";
        public static readonly string Via = "Via";
        public static readonly string Warning = "Warning";


        // Entity fields
        public static readonly string Allow = "Allow";
        public static readonly string ContentEncoding = "Content-Encoding";
        public static readonly string ContentLanguage = "Content-Language";
        public static readonly string ContentLocation = "Content-Location";
        public static readonly string ContentRange = "Content-Range";
        public static readonly string Expires = "Expires";
        public static readonly string LastModified = "Last-Modified";

        protected HttpGeneralHeaders()
        {
        }
    }

    public class HttpResponseHeaders : HttpGeneralHeaders
    {
        public static readonly string AcceptRanges = "AcceptRanges";
        public static readonly string Age = "Age";
        public static readonly string ContentLength = "ContentLength";
        public static readonly string ContentMd5 = "ContentMd5";
        public static readonly string ContentType = "ContentType";
        public static readonly string ETag = "ETag";
        public static readonly string KeepAlive = "KeepAlive";
        public static readonly string Location = "Location";
        public static readonly string ProxyAuthenticate = "ProxyAuthenticate";
        public static readonly string RetryAfter = "RetryAfter";
        public static readonly string Server = "Server";
        public static readonly string SetCookie = "SetCookie";
        public static readonly string Vary = "Vary";
        public static readonly string WwwAuthenticate = "WwwAuthenticate";

        private static readonly Cache<HttpResponseHeader, string> _headerNames = new Cache<HttpResponseHeader, string>();

        static HttpResponseHeaders()
        {
            _headerNames[HttpResponseHeader.AcceptRanges] = AcceptRanges;
            _headerNames[HttpResponseHeader.Age] = Age;
            _headerNames[HttpResponseHeader.Allow] = Allow;
            _headerNames[HttpResponseHeader.CacheControl] = CacheControl;
            _headerNames[HttpResponseHeader.Connection] = Connection;
            _headerNames[HttpResponseHeader.ContentEncoding] = ContentEncoding;
            _headerNames[HttpResponseHeader.ContentLanguage] = ContentLanguage;
            _headerNames[HttpResponseHeader.ContentLength] = ContentLength;
            _headerNames[HttpResponseHeader.ContentLocation] = ContentLocation;
            _headerNames[HttpResponseHeader.ContentMd5] = ContentMd5;
            _headerNames[HttpResponseHeader.ContentRange] = ContentRange;
            _headerNames[HttpResponseHeader.ContentType] = ContentType;
            _headerNames[HttpResponseHeader.Date] = Date;
            _headerNames[HttpResponseHeader.ETag] = ETag;
            _headerNames[HttpResponseHeader.Expires] = Expires;
            _headerNames[HttpResponseHeader.KeepAlive] = KeepAlive;
            _headerNames[HttpResponseHeader.LastModified] = LastModified;
            _headerNames[HttpResponseHeader.Location] = Location;
            _headerNames[HttpResponseHeader.Pragma] = Pragma;
            _headerNames[HttpResponseHeader.ProxyAuthenticate] = ProxyAuthenticate;
            _headerNames[HttpResponseHeader.RetryAfter] = RetryAfter;
            _headerNames[HttpResponseHeader.Server] = Server;
            _headerNames[HttpResponseHeader.SetCookie] = SetCookie;
            _headerNames[HttpResponseHeader.Trailer] = Trailer;
            _headerNames[HttpResponseHeader.TransferEncoding] = TransferEncoding;
            _headerNames[HttpResponseHeader.Upgrade] = Upgrade;
            _headerNames[HttpResponseHeader.Vary] = Vary;
            _headerNames[HttpResponseHeader.Via] = Via;
            _headerNames[HttpResponseHeader.Warning] = Warning;
            _headerNames[HttpResponseHeader.WwwAuthenticate] = WwwAuthenticate;
        }

        protected HttpResponseHeaders()
        {
        }

        public static string HeaderNameFor(HttpResponseHeader header)
        {
            return _headerNames[header];
        }
    }


    public class HttpRequestHeaders : HttpGeneralHeaders
    {
        public static readonly string Accept = "Accept";
        public static readonly string AcceptCharset = "Accept-Charset";
        public static readonly string AcceptEncoding = "Accept-Encoding";
        public static readonly string AcceptLanguage = "Accept-Language";
        public static readonly string Authorization = "Authorization";
        public static readonly string Cookie = "Cookie";
        public static readonly string ContentLength = "Content-Length";
        public static readonly string ContentMd5 = "Content-MD5";
        public static readonly string ContentType = "Content-Type";
        public static readonly string Expect = "Expect";
        public static readonly string From = "From";
        public static readonly string Host = "Host";
        public static readonly string IfMatch = "If-Match";
        public static readonly string IfModifiedSince = "If-Modified-Since";
        public static readonly string IfNoneMatch = "If-None-Match";
        public static readonly string IfRange = "If-Range";
        public static readonly string IfUnmodifiedSince = "If-Unmodified-Since";
        public static readonly string MaxForwards = "Max-Forwards";
        public static readonly string ProxyAuthorization = "Proxy-Authorization";
        public static readonly string Range = "Range";
        public static readonly string Referer = "Referer";
        public static readonly string Te = "TE";
        public static readonly string UserAgent = "User-Agent";


        // http 1.0
        public static readonly string KeepAlive = "keep-alive";


        // Not sure this is still used
        public static readonly string Translate = "Translate";


        private static readonly Cache<HttpRequestHeader, string> _headerNames = new Cache<HttpRequestHeader, string>();

        private static readonly Cache<string, string> _propertyAliases = new Cache<string, string>(name =>
        {
            var prop = typeof (HttpRequestHeaders).GetField(name, BindingFlags.Static | BindingFlags.Public);
            return prop == null ? name : (string) prop.GetValue(null);
        });

        static HttpRequestHeaders()
        {
            _headerNames[HttpRequestHeader.CacheControl] = CacheControl;
            _headerNames[HttpRequestHeader.Connection] = Connection;
            _headerNames[HttpRequestHeader.Date] = Date;
            _headerNames[HttpRequestHeader.KeepAlive] = KeepAlive;
            _headerNames[HttpRequestHeader.Pragma] = Pragma;
            _headerNames[HttpRequestHeader.Trailer] = Trailer;
            _headerNames[HttpRequestHeader.TransferEncoding] = TransferEncoding;
            _headerNames[HttpRequestHeader.Upgrade] = Upgrade;
            _headerNames[HttpRequestHeader.Via] = Via;
            _headerNames[HttpRequestHeader.Warning] = Warning;
            _headerNames[HttpRequestHeader.Allow] = Allow;
            _headerNames[HttpRequestHeader.ContentLength] = ContentLength;
            _headerNames[HttpRequestHeader.ContentType] = ContentType;
            _headerNames[HttpRequestHeader.ContentEncoding] = ContentEncoding;
            _headerNames[HttpRequestHeader.ContentLanguage] = ContentLanguage;
            _headerNames[HttpRequestHeader.ContentLocation] = ContentLocation;
            _headerNames[HttpRequestHeader.ContentMd5] = ContentMd5;
            _headerNames[HttpRequestHeader.ContentRange] = ContentRange;
            _headerNames[HttpRequestHeader.Expires] = Expires;
            _headerNames[HttpRequestHeader.LastModified] = LastModified;
            _headerNames[HttpRequestHeader.Accept] = Accept;
            _headerNames[HttpRequestHeader.AcceptCharset] = AcceptCharset;
            _headerNames[HttpRequestHeader.AcceptEncoding] = AcceptEncoding;
            _headerNames[HttpRequestHeader.AcceptLanguage] = AcceptLanguage;
            _headerNames[HttpRequestHeader.Authorization] = Authorization;
            _headerNames[HttpRequestHeader.Cookie] = Cookie;
            _headerNames[HttpRequestHeader.Expect] = Expect;
            _headerNames[HttpRequestHeader.From] = From;
            _headerNames[HttpRequestHeader.Host] = Host;
            _headerNames[HttpRequestHeader.IfMatch] = IfMatch;
            _headerNames[HttpRequestHeader.IfModifiedSince] = IfModifiedSince;
            _headerNames[HttpRequestHeader.IfNoneMatch] = IfNoneMatch;
            _headerNames[HttpRequestHeader.IfRange] = IfRange;
            _headerNames[HttpRequestHeader.IfUnmodifiedSince] = IfUnmodifiedSince;
            _headerNames[HttpRequestHeader.MaxForwards] = MaxForwards;
            _headerNames[HttpRequestHeader.ProxyAuthorization] = ProxyAuthorization;
            _headerNames[HttpRequestHeader.Referer] = Referer;
            _headerNames[HttpRequestHeader.Range] = Range;
            _headerNames[HttpRequestHeader.Te] = Te;
            _headerNames[HttpRequestHeader.Translate] = Translate;
            _headerNames[HttpRequestHeader.UserAgent] = UserAgent;
        }

        protected HttpRequestHeaders()
        {
        }

        public static string HeaderNameFor(HttpRequestHeader header)
        {
            return _headerNames[header];
        }

        public static string HeaderDictionaryNameForProperty(string propertyName)
        {
            return _propertyAliases[propertyName];
        }
    }
}