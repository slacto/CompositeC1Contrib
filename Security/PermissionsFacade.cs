﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;

using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data.Types;
using CompositeC1Contrib.Security.Web;

namespace CompositeC1Contrib.Security
{
    public static class PermissionsFacade
    {
        private static SiteMapNode _loginSiteMapNode;

        public static SiteMapNode LoginSiteMapNode
        {
            get
            {
                if (_loginSiteMapNode == null)
                {
                    var loginPage = FormsAuthentication.LoginUrl;
                    if (loginPage.StartsWith("/"))
                    {
                        loginPage = loginPage.Remove(0, 1);
                    }

                    _loginSiteMapNode = SiteMap.Provider.FindSiteMapNodeFromKey(loginPage);
                }

                return _loginSiteMapNode;
            }
        }

        public static Uri GetLoginUri()
        {
            var ctx = HttpContext.Current;
            var returnUrl = EnsureHttps(ctx.Request.Url).AbsolutePath;
            var loginPage = EnsureHttps(new Uri(ctx.Request.Url, LoginSiteMapNode.Url));

            return new Uri(loginPage + "?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl));
        }

        public static bool HasAccess(SiteMapNode node)
        {
            var page = PageManager.GetPageById(new Guid(node.Key));

            return HasAccess(page);
        }

        public static bool HasAccess(IPage page)
        {
            return EvaluatedPagePermissions.HasAccess(page);
        }

        public static bool HasAccess(IMediaFile media)
        {
            using (var data = new DataConnection())
            {
                var folder = data.Get<IMediaFileFolder>().Single(f => f.StoreId == media.StoreId && f.Path == media.FolderPath);
                var folderPermission = data.Get<IMediaFolderPermissions>().SingleOrDefault(f => f.KeyPath == folder.KeyPath);
                if (folderPermission != null)
                {
                    var fep = EvaluatePermissions(folderPermission);

                    return HasAccess(fep);
                }

                var mediaPermissions = data.Get<IMediaFilePermissions>().SingleOrDefault(m => m.KeyPath == media.KeyPath);
                if (mediaPermissions != null)
                {
                var mep = EvaluatePermissions(mediaPermissions);

                return HasAccess(mep);
            }
        }
        
            return true;
        }

        public static EvaluatedPermissions EvaluatePermissions(IDataPermissions permissions)
        {
            if (permissions == null)
            {
                return null;
            }

            var evaluatedPermissions = new EvaluatedPermissions
            {
                ExplicitAllowedRoles = Split(permissions.AllowedRoles).ToArray(),
                ExplicitDeniedRoled = Split(permissions.DeniedRoles).ToArray(),
            };

            return evaluatedPermissions;
        }

        public static IEnumerable<string> Split(string roles)
        {
            return roles == null ? new string[0] : roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool HasAccess(EvaluatedPermissions permissions)
        {
            if (permissions == null)
            {
                return true;
            }

            if (permissions.DeniedRoled.Length <= 0 && permissions.AllowedRoles.Length <= 0)
            {
                return true;
            }

            var principal = Thread.CurrentPrincipal;
            var userRoles = new List<string>();

            if (principal.Identity.IsAuthenticated)
            {
                userRoles.AddRange(Roles.GetRolesForUser());

                userRoles.Add(CompositeC1RoleProvider.AuthenticatedRole);
            }
            else
            {
                userRoles.Add(CompositeC1RoleProvider.AnonymousdRole);
            }

            if (permissions.DeniedRoled.Intersect(userRoles).Any())
            {
                return false;
            }

            if (!permissions.AllowedRoles.Intersect(userRoles).Any())
            {
                return false;
            }

            return true;
        }

        public static Uri EnsureHttps(Uri uri)
        {
            if (!FormsAuthentication.RequireSSL)
            {
                return uri;
            }

            var uriBuilder = new UriBuilder(uri)
            {
                Scheme = "https",
                Port = 443
            };

            return uriBuilder.Uri;
        }
    }
}